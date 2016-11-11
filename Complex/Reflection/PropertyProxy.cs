#region ...   [Header]   ...

// Solution    : Bergler.CC.InotifyPropertyChanged.Complex
// Project     : Bergler.CC.InotifyPropertyChanged.Complex.Reflection
// File        : PropertyProxy.cs
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
using Bergler.CC.INotifyPropertyChanged.Complex.Reflection.Delegates;
using Bergler.CC.INotifyPropertyChanged.Complex.Reflection.ILGen;

#endregion

namespace Bergler.CC.INotifyPropertyChanged.Complex.Reflection
{
    /// <summary>
    ///     Factory for constructing virtual|interface property implementations.
    /// </summary>
    public static class PropertyProxy
    {
        private static AssemblyBuilder builder;
        private static readonly object syncRoot = new object();

        /// <summary>
        ///     Dynamic assembly for storing constructed Proxy types.
        /// </summary>
        public static AssemblyBuilder AssemblyBuilder
        {
            get
            {
                if (builder == null)
                {
                    lock (syncRoot)
                    {
                        if (builder == null)
                        {
                            var name = new AssemblyName("Property.Proxy");
                            builder = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run,
                                null, true, null);
                        }
                    }
                }
                return builder;
            }
        }

        /// <summary>
        ///     Constructs type derived from TBaseType, implementing interfaceType, and overriding TBaseType virtual members if
        ///     overrideVirtual is true.
        /// </summary>
        /// <typeparam name="TBaseType">Base class to derive from.</typeparam>
        /// <typeparam name="TImplementer">Delegate based implementation provider for TBaseType</typeparam>
        /// <param name="interfaceType">Type of interface to implement.</param>
        /// <param name="overrideVirtual">If true, additionally overrides virtual members.</param>
        /// <returns>Constructed type.</returns>
        public static Type ConstructType<TBaseType, TImplementer>(Type interfaceType, bool overrideVirtual = false)
            where TImplementer : class, IImplementationMap<TBaseType>, new()
        {
            return ConstructType(new PropertyImplementer<TBaseType, TImplementer>(), typeof (TBaseType),
                interfaceType, overrideVirtual);
        }

        /// <summary>
        ///     Constructs type derived from TBaseType, implementing interfaceTypes, and overriding TBaseType virtual members if
        ///     overrideVirtual is true.
        /// </summary>
        /// <typeparam name="TBaseType">Base class to derive from.</typeparam>
        /// <typeparam name="TImplementer">Delegate based implementation provider for TBaseType</typeparam>
        /// <param name="interfaceTypes">Types of interface to implement.</param>
        /// <param name="overrideVirtual">If true, additionally overrides virtual members.</param>
        /// <returns>Constructed type.</returns>
        public static Type ConstructType<TBaseType, TImplementer>(Type[] interfaceTypes, bool overrideVirtual = false)
            where TImplementer : class, IImplementationMap<TBaseType>, new()
        {
            return ConstructType(new PropertyImplementer<TBaseType, TImplementer>(), typeof (TBaseType),
                interfaceTypes, overrideVirtual);
        }

        /// <summary>
        ///     Constructs type derived from baseType, implementing interfaceType, and overriding TBaseType virtual members if
        ///     overrideVirtual is true.
        /// </summary>
        /// <param name="implementer">Implementation provider for implementation.</param>
        /// <param name="baseType">Base type to derive from.</param>
        /// <param name="interfaceType">Type of interface to implement.</param>
        /// <param name="overrideVirtual">If true, additionally overrides virtual members.</param>
        /// <returns>Constructed type.</returns>
        public static Type ConstructType(IPropertyImplementer implementer, Type baseType, Type interfaceType,
            bool overrideVirtual = false)
        {
            return ConstructType(implementer, baseType, new[] {interfaceType}, overrideVirtual);
        }

        /// <summary>
        ///     Constructs type derived from baseType, implementing interfaceTypes, and overriding TBaseType virtual members if
        ///     overrideVirtual is true.
        /// </summary>
        /// <param name="implementer">Implementation provider for implementation.</param>
        /// <param name="baseType">Base type to derive from.</param>
        /// <param name="interfaceTypes">Type of interfaces to implement.</param>
        /// <param name="overrideVirtual">If true, additionally overrides virtual members.</param>
        /// <returns>Constructed type.</returns>
        public static Type ConstructType(IPropertyImplementer implementer, Type baseType, Type[] interfaceTypes,
            bool overrideVirtual = false)
        {
            return ConstructType(implementer, baseType, interfaceTypes, new Type[] {}, overrideVirtual);
        }

        /// <summary>
        ///     Constructs type derived from baseType, implementing interfaceTypes, and overriding TBaseType virtual members if
        ///     overrideVirtual is true.
        /// </summary>
        /// <param name="implementer">Implementation provider for implementation.</param>
        /// <param name="baseType">Base type to derive from.</param>
        /// <param name="interfaceTypes">Type of interfaces to implement.</param>
        /// <param name="excludeInterfaces">Will exclude implementation of this interfaces.</param>
        /// <param name="overrideVirtual">If true, additionally overrides virtual members.</param>
        /// <returns>Constructed type.</returns>
        public static Type ConstructType(IPropertyImplementer implementer, Type baseType, Type[] interfaceTypes,
            Type[] excludeInterfaces, bool overrideVirtual = false)
        {
            lock (syncRoot)
            {
                return ConstructType(implementer, baseType, interfaceTypes, (IEnumerable<Type>) excludeInterfaces,
                    overrideVirtual);
            }
        }

        private static Type ConstructType(IPropertyImplementer implementer, Type baseType, Type[] implementInterfaces,
            IEnumerable<Type> excludeTypes, bool overrideVirtual = false)
        {
            var interfaceNames = string.Join("+", implementInterfaces.Select(t => t.Name).ToArray());
            var moduleBuilder =
                AssemblyBuilder.DefineDynamicModule(string.Format("{0}DerivedFrom{1}ImplenentedBy{2}",
                    interfaceNames, baseType.Name,
                    implementer.GetType().Name));
            // Set the class to be public and sealed
            TypeAttributes typeAttributes =
                TypeAttributes.Class | TypeAttributes.Public;


            var typeBuilder = moduleBuilder.DefineType(string.Format("{1}${0}", interfaceNames, baseType.Name),
                typeAttributes, baseType, implementInterfaces);

            implementer.BeginEmit(typeBuilder);


            foreach (
                var constructor in
                    baseType.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (!constructor.IsPrivate)
                {
                    var constructorBuilder = typeBuilder.DefineConstructor(constructor.Attributes,
                        constructor.CallingConvention,
                        constructor.GetParameters().Select(p => p.ParameterType).ToArray());

                    var ilGen = constructorBuilder.GetILGenerator();


                    ilGen.Emit(OpCodes.Jmp, constructor);
                }
            }

            var implementedTypes = new HashSet<Type>(excludeTypes);

            foreach (var type in implementInterfaces)
            {
                if (!type.IsInterface)
                {
                    throw new ArgumentException("Cant implement Type other then interface.");
                }
                GenerateInterfaceMethod(implementer, type, typeBuilder, implementedTypes);
            }
            if (overrideVirtual)
            {
                OverrideVirtualMethod(implementer, baseType, typeBuilder, implementedTypes);
            }
            implementer.EndEmit();

            return typeBuilder.CreateType();
        }


        private static void OverrideVirtualMethod(IPropertyImplementer implementer, Type type, TypeBuilder typeBuilder,
            HashSet<Type> implemented)
        {
            if (implemented.Contains(type) | type == typeof (object))
            {
                return;
            }

            foreach (
                var propertyInfo in
                    type.GetProperties(BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.Public |
                                       BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                if (propertyInfo.CanRead)
                {
                    MethodInfo getMethod = propertyInfo.GetGetMethod();
                    if (getMethod.IsVirtual && !getMethod.IsFinal)
                    {
                        //exclude overridden methods
                        if (getMethod.GetBaseDefinition() == getMethod)
                        {
                            ImplementGetter(implementer, typeBuilder, propertyInfo, getMethod);
                        }
                    }
                }
                if (propertyInfo.CanWrite)
                {
                    MethodInfo setMethod = propertyInfo.GetSetMethod();
                    if (setMethod.IsVirtual && !setMethod.IsFinal)
                    {
                        //exclude overridden method
                        if (setMethod.GetBaseDefinition() == setMethod)
                        {
                            ImplementSetter(implementer, typeBuilder, propertyInfo, setMethod);
                        }
                    }
                }
            }
            OverrideVirtualMethod(implementer, type.BaseType, typeBuilder, implemented);
        }


        private static void GenerateInterfaceMethod(IPropertyImplementer implementer, Type interfaceType,
            TypeBuilder typeBuilder, HashSet<Type> implemented)
        {
            if (implemented.Contains(interfaceType))
            {
                return;
            }
            foreach (var propertyInfo in interfaceType.GetProperties())
            {
                //if(propertyInfo.DeclaringType == propertyInfo.ReflectedType)
                //{
                if (propertyInfo.CanRead)
                {
                    MethodInfo getMethod = propertyInfo.GetGetMethod();
                    if (getMethod.IsVirtual && !getMethod.IsFinal)
                    {
                        // create a new builder for the method in the interface
                        ImplementGetter(implementer, typeBuilder, propertyInfo, getMethod);
                    }
                }
                if (propertyInfo.CanWrite)
                {
                    MethodInfo setMethod = propertyInfo.GetSetMethod();
                    if (setMethod.IsVirtual && !setMethod.IsFinal)
                    {
                        // create a new builder for the method in the interface
                        ImplementSetter(implementer, typeBuilder, propertyInfo, setMethod);
                    }
                }
                //}
            }

            foreach (Type parentType in interfaceType.GetInterfaces())
            {
                GenerateInterfaceMethod(implementer, parentType, typeBuilder, implemented);
            }

            implemented.Add(interfaceType);
        }

        private static void ImplementGetter(IPropertyImplementer implementer, TypeBuilder typeBuilder,
            PropertyInfo propertyInfo, MethodInfo getMethod)
        {
            // create a new builder for the method in the interface
            MethodBuilder methodBuilder = typeBuilder.DefineMethod(
                string.Format("{0}.{1}", getMethod.DeclaringType.Name, getMethod.Name),
                MethodAttributes.Private | MethodAttributes.NewSlot | MethodAttributes.HideBySig |
                MethodAttributes.Virtual | MethodAttributes.Final,
                CallingConventions.Standard);

            methodBuilder.SetParameters();
            methodBuilder.SetReturnType(getMethod.ReturnType);

            #region( "Implementing Get Method IL Code" )

            implementer.ImplementGetter(methodBuilder, propertyInfo);

            typeBuilder.DefineMethodOverride(methodBuilder, getMethod);

            #endregion
        }

        private static void ImplementSetter(IPropertyImplementer implementer, TypeBuilder typeBuilder,
            PropertyInfo propertyInfo, MethodInfo setMethod)
        {
            // create a new builder for the method in the interface
            MethodBuilder methodBuilder = typeBuilder.DefineMethod(
                string.Format("{0}.{1}", setMethod.DeclaringType.Name, setMethod.Name),
                MethodAttributes.Private | MethodAttributes.NewSlot | MethodAttributes.HideBySig |
                MethodAttributes.Virtual | MethodAttributes.Final,
                CallingConventions.Standard);

            methodBuilder.SetParameters(propertyInfo.PropertyType);
            methodBuilder.SetReturnType(typeof (void));

            #region( "Implementing Get Method IL Code" )

            implementer.ImplementSetter(methodBuilder, propertyInfo);

            typeBuilder.DefineMethodOverride(methodBuilder, setMethod);

            #endregion
        }
    }
}