#region ...   [Header]   ...

// Solution    : Bergler.CC.InotifyPropertyChanged.Complex
// Project     : Bergler.CC.InotifyPropertyChanged.Complex.Reflection
// File        : PropertyImplementation.cs
//  
// Created on  : 11-11-2016 11:40
// Altered on  : 11-11-2016 11:44
// Altered by  : Ace aka Arjan Crielaard

#endregion

#region ...   [Usings]   ...

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

#endregion

namespace Bergler.CC.INotifyPropertyChanged.Complex.Reflection
{
    /// <summary>
    ///     Provides fast access to virtual property implementations by declaring type, implementing type and property name.
    ///     All calls to virtual methods done as call (not callvirt) giving access to exact implementation of property.
    ///     All operations in this class is O(1) except first access wich takes O(N) where N is number of properties in TType.
    ///     All methods a thread safe.
    ///     All accesses (except first one) are lock-free.
    /// </summary>
    /// <typeparam name="TImplementation">Type implementing properties.</typeparam>
    /// <typeparam name="TDeclaration">Type declaring properties.</typeparam>
    public static class PropertyImplementation<TImplementation, TDeclaration> where TImplementation : TDeclaration
    {
        private static Dictionary<string, Delegate> getters;
        private static Dictionary<string, Delegate> setters;
        private static readonly object syncRoot = new object();

        private static Dictionary<string, Delegate> Getters
        {
            get
            {
                if (getters == null)
                {
                    lock (syncRoot)
                    {
                        if (getters == null)
                        {
                            getters = CompileGetters();
                        }
                    }
                }
                return getters;
            }
        }

        private static Dictionary<string, Delegate> Setters
        {
            get
            {
                if (setters == null)
                {
                    lock (syncRoot)
                    {
                        if (setters == null)
                        {
                            setters = CompileSetters();
                        }
                    }
                }
                return setters;
            }
        }

        /// <summary>
        ///     Returns delegate calling an implementation of property propertyName getter declared in TDeclaration, defined in
        ///     TImplementation.
        /// </summary>
        /// <typeparam name="TValue">Property type.</typeparam>
        /// <param name="propertyName">Name of property.</param>
        /// <returns>Delegate, calling specified function on given instance.</returns>
        public static Func<TImplementation, TValue> GetGetter<TValue>(string propertyName)
        {
            return (Func<TImplementation, TValue>) Getters[propertyName];
        }

        /// <summary>
        ///     Returns delegate calling an implementation of property propertyName setter declared in TDeclaration, defined in
        ///     TImplementation.
        /// </summary>
        /// <typeparam name="TValue">Property type.</typeparam>
        /// <param name="propertyName">Name of property.</param>
        /// <returns>Delegate, calling specified function on given instance.</returns>
        public static Action<TImplementation, TValue> GetSetter<TValue>(string propertyName)
        {
            return (Action<TImplementation, TValue>) Setters[propertyName];
        }

        public static TValue Get<TValue>(TImplementation instance, string propertyName)
        {
            return GetGetter<TValue>(propertyName)(instance);
        }

        public static void Set<TValue>(TImplementation instance, string propertyName, TValue value)
        {
            GetSetter<TValue>(propertyName)(instance, value);
        }

        private static MethodInfo GetBaseDefinition(MethodInfo method)
        {
            MethodInfo baseDefinition;
            do
            {
                baseDefinition = method.GetBaseDefinition();
            } while (baseDefinition.GetBaseDefinition() != baseDefinition);
            return baseDefinition;
        }


        private static Dictionary<string, Delegate> CompileGetters()
        {
            var implementationType = typeof (TImplementation);
            var declarationType = typeof (TDeclaration);

            if (implementationType.IsInterface)
            {
                throw new ArgumentException("TImplementation cannot be an interface.");
            }

            if (!declarationType.IsAssignableFrom(implementationType))
            {
                throw new ArgumentException("ImplementingType is not assignable from InstanceType");
            }

            var propertys =
                declarationType.GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance);
            var getters = new Dictionary<string, Delegate>(propertys.Length);

            foreach (var property in propertys)
            {
                if (property.CanRead)
                {
                    MethodInfo method = null;
                    if (!declarationType.IsInterface)
                    {
                        method = property.GetGetMethod();

                        if (method.IsVirtual && !method.IsFinal)
                        {
                            var baseDefinition = GetBaseDefinition(method);


                            foreach (
                                var methodInfo in
                                    implementationType.GetMethods(BindingFlags.GetProperty | BindingFlags.Public |
                                                                  BindingFlags.Instance))
                            {
                                var foundBaseDefinition = GetBaseDefinition(methodInfo);
                                if (baseDefinition == foundBaseDefinition)
                                {
                                    method = methodInfo;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        var mapping = implementationType.GetInterfaceMap(declarationType);
                        for (int i = 0; i < mapping.InterfaceMethods.Length; i++)
                        {
                            if (mapping.InterfaceMethods[i] == property.GetGetMethod())
                            {
                                method = mapping.TargetMethods[i];
                                break;
                            }
                        }
                    }
                    var parameterTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();

                    var dynamicmethod = new DynamicMethod(method.Name, method.ReturnType, new[] {implementationType},
                        implementationType, true);

                    var il = dynamicmethod.GetILGenerator();

                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Tailcall);
                    il.Emit(OpCodes.Call, method);
                    il.Emit(OpCodes.Ret);

                    var @delegate =
                        dynamicmethod.CreateDelegate(typeof (Func<,>).MakeGenericType(implementationType,
                            method.ReturnType));
                    getters.Add(property.Name, @delegate);
                }
            }
            return getters;
        }

        private static Dictionary<string, Delegate> CompileSetters()
        {
            var implementationType = typeof (TImplementation);
            var declaringType = typeof (TDeclaration);

            if (!declaringType.IsAssignableFrom(implementationType))
            {
                throw new ArgumentException("ImplementingType is not assignable from InstanceType");
            }

            var propertys =
                declaringType.GetProperties(BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.Instance);
            var setters = new Dictionary<string, Delegate>(propertys.Length);

            foreach (var property in propertys)
            {
                if (property.CanWrite)
                {
                    MethodInfo method = null;
                    if (!declaringType.IsInterface)
                    {
                        method = property.GetSetMethod();

                        if (method.IsVirtual && !method.IsFinal)
                        {
                            var baseDefinition = GetBaseDefinition(method);


                            foreach (
                                var methodInfo in
                                    implementationType.GetMethods(BindingFlags.SetProperty | BindingFlags.Public |
                                                                  BindingFlags.Instance))
                            {
                                var foundBaseDefinition = GetBaseDefinition(methodInfo);

                                if (baseDefinition == foundBaseDefinition)
                                {
                                    method = methodInfo;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        var mapping = implementationType.GetInterfaceMap(declaringType);
                        for (int i = 0; i < mapping.InterfaceMethods.Length; i++)
                        {
                            if (mapping.InterfaceMethods[i] == property.GetSetMethod())
                            {
                                method = mapping.TargetMethods[i];
                                break;
                            }
                        }
                    }
                    var parameterTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();

                    var dynamicmethod = new DynamicMethod(method.Name, typeof (void),
                        new[] {implementationType, parameterTypes[0]}, implementationType, true);

                    var il = dynamicmethod.GetILGenerator();


                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Tailcall);
                    il.Emit(OpCodes.Call, method);
                    il.Emit(OpCodes.Ret);

                    var @delegate =
                        dynamicmethod.CreateDelegate(typeof (Action<,>).MakeGenericType(implementationType,
                            parameterTypes[0]));
                    setters.Add(property.Name, @delegate);
                }
            }
            return setters;
        }
    }
}