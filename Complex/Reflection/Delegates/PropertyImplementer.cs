#region ...   [Header]   ...

// Solution    : Bergler.CC.InotifyPropertyChanged.Complex
// Project     : Bergler.CC.InotifyPropertyChanged.Complex.Reflection
// File        : PropertyImplementer.cs
//  
// Created on  : 11-11-2016 11:40
// Altered on  : 11-11-2016 11:44
// Altered by  : Ace aka Arjan Crielaard

#endregion

#region ...   [Usings]   ...

using System;
using System.Reflection;
using System.Reflection.Emit;
using Bergler.CC.INotifyPropertyChanged.Complex.Reflection.ILGen;

#endregion

namespace Bergler.CC.INotifyPropertyChanged.Complex.Reflection.Delegates
{
    /// <summary>
    ///     Provides IL level implementation of new type's properties, based on TImplementation-povided delegates.
    /// </summary>
    /// <typeparam name="TBaseType"></typeparam>
    /// <typeparam name="TImplementation"></typeparam>
    public class PropertyImplementer<TBaseType, TImplementation> : IPropertyImplementer
        where TImplementation : class, IImplementationMap<TBaseType>, new()
    {
        private static readonly MethodInfo ImplementGetterMethod = typeof (IImplementationMap<TBaseType>)
            .GetMethod("ImplementGetter", BindingFlags.Public | BindingFlags.Instance);

        private static readonly MethodInfo ImplementSetterMethod = typeof (IImplementationMap<TBaseType>)
            .GetMethod("ImplementSetter", BindingFlags.Public | BindingFlags.Instance);

        private static readonly MethodInfo OverrideGetterMethod = typeof (IImplementationMap<TBaseType>)
            .GetMethod("OverrideGetter", BindingFlags.Public | BindingFlags.Instance);

        private static readonly MethodInfo OverrideSetterMethod = typeof (IImplementationMap<TBaseType>)
            .GetMethod("OverrideSetter", BindingFlags.Public | BindingFlags.Instance);


        private static readonly MethodInfo GetTypeFromHandle = typeof (Type).GetMethod("GetTypeFromHandle");
        private static readonly MethodInfo GetProperty = typeof (Type).GetMethod("GetProperty", new[] {typeof (string)});


        private ConstructorBuilder cctor;
        private ILGenerator cctorIl;
        private int counter;
        private LocalBuilder mapLocal;

        protected TypeBuilder TypeBuilder { get; set; }

        #region Implementation of IPropertyGetterImplementer

        public void ImplementGetter(MethodBuilder methodBuilder, PropertyInfo property)
        {
            #region cctor emit

            var delegateType = typeof (Func<,>).MakeGenericType(typeof (TBaseType), property.PropertyType);

            var delegateField = TypeBuilder.DefineField(
                string.Format("get_{2}.{0}#{3}", property.Name, property.PropertyType.Name,
                    property.DeclaringType.FullName, counter++)
                , delegateType
                , FieldAttributes.Private | FieldAttributes.Static | FieldAttributes.InitOnly);

            //load pointer to Implementation
            cctorIl.Emit(OpCodes.Ldloc, mapLocal);

            //_cctorIL.Emit(OpCodes.Ldstr, property.Name);

            //emit runtime type handle
            cctorIl.Emit(OpCodes.Ldtoken, property.DeclaringType);

            cctorIl.Emit(OpCodes.Call, GetTypeFromHandle);

            cctorIl.Emit(OpCodes.Ldstr, property.Name);

            cctorIl.Emit(OpCodes.Call, GetProperty);

            var genericMethod = property.DeclaringType.IsAssignableFrom(TypeBuilder.BaseType)
                ? OverrideGetterMethod
                : ImplementGetterMethod;

            var metod = genericMethod.MakeGenericMethod(TypeBuilder.BaseType, property.DeclaringType, TypeBuilder,
                property.PropertyType);

            cctorIl.Emit(OpCodes.Callvirt, metod);

            cctorIl.Emit(OpCodes.Stsfld, delegateField);

            #endregion

            #region  Getter Emit

            var il = methodBuilder.GetILGenerator();

            il.Emit(OpCodes.Ldsfld, delegateField);

            il.Emit(OpCodes.Ldarg_0);

            if (typeof (TBaseType).IsInterface)
            {
                il.Emit(OpCodes.Castclass, typeof (TBaseType));
            }

            var invokeMethod = delegateType.GetMethod("Invoke");

            il.Emit(OpCodes.Tailcall);

            il.Emit(OpCodes.Callvirt, invokeMethod);

            il.Emit(OpCodes.Ret);

            #endregion
        }

        #endregion

        #region Implementation of IPropertySetterImplementer

        public void ImplementSetter(MethodBuilder methodBuilder, PropertyInfo property)
        {
            #region cctor emit

            var delegateType = typeof (Action<,>).MakeGenericType(typeof (TBaseType), property.PropertyType);

            var delegateField = TypeBuilder.DefineField(
                string.Format("set_{2}.{0}#{3}", property.Name, property.PropertyType.Name,
                    property.DeclaringType.FullName, counter++)
                , delegateType
                , FieldAttributes.Private | FieldAttributes.Static | FieldAttributes.InitOnly);

            //load pointer to Implementation
            cctorIl.Emit(OpCodes.Ldloc, mapLocal);

            //_cctorIL.Emit(OpCodes.Ldstr,property.Name);

            //emit runtime type handle
            cctorIl.Emit(OpCodes.Ldtoken, property.DeclaringType);


            cctorIl.Emit(OpCodes.Call, GetTypeFromHandle);

            cctorIl.Emit(OpCodes.Ldstr, property.Name);

            cctorIl.Emit(OpCodes.Call, GetProperty);

            var genericMethod = property.DeclaringType.IsAssignableFrom(TypeBuilder.BaseType)
                ? OverrideSetterMethod
                : ImplementSetterMethod;

            var metod = genericMethod.MakeGenericMethod(TypeBuilder.BaseType, property.DeclaringType, TypeBuilder,
                property.PropertyType);

            //getting implementation
            cctorIl.Emit(OpCodes.Callvirt, metod);

            cctorIl.Emit(OpCodes.Stsfld, delegateField);

            #endregion

            #region  Setter Emit

            var il = methodBuilder.GetILGenerator();

            il.Emit(OpCodes.Ldsfld, delegateField);

            il.Emit(OpCodes.Ldarg_0);

            if (typeof (TBaseType).IsInterface)
            {
                il.Emit(OpCodes.Castclass, typeof (TBaseType));
            }

            il.Emit(OpCodes.Ldarg_1);

            var invokeMethod = delegateType.GetMethod("Invoke");

            il.Emit(OpCodes.Tailcall);

            il.Emit(OpCodes.Callvirt, invokeMethod);

            il.Emit(OpCodes.Ret);

            #endregion
        }

        #endregion

        #region Implementation of IPropertyImplementer

        public virtual void BeginEmit(TypeBuilder builder)
        {
            if (TypeBuilder != null)
            {
                throw new InvalidOperationException("Emit is already initialized.");
            }

            TypeBuilder = builder;

            var baseType = typeof (TBaseType);
            if (!baseType.IsAssignableFrom(TypeBuilder.BaseType))
            {
                throw new ArgumentException("TBaseType is not assignable from TypeBuilder.BaseType", "_inType");
            }


            //define static constructor
            cctor = TypeBuilder.DefineConstructor(MethodAttributes.Static, CallingConventions.Standard, new Type[0]);
            cctorIl = cctor.GetILGenerator();

            mapLocal = cctorIl.DeclareLocal(typeof (IImplementationMap<TBaseType>));

            var ctor = typeof (TImplementation).GetConstructor(new Type[0]);

            //emit construction of implementation instance
            cctorIl.Emit(OpCodes.Newobj, ctor);

            cctorIl.Emit(OpCodes.Castclass, typeof (IImplementationMap<TBaseType>));

            //set field
            cctorIl.Emit(OpCodes.Stloc, mapLocal);
        }

        public virtual void EndEmit()
        {
            if (TypeBuilder == null)
            {
                throw new InvalidOperationException();
            }

            cctorIl.Emit(OpCodes.Ret);

            TypeBuilder = null;
        }

        #endregion
    }
}