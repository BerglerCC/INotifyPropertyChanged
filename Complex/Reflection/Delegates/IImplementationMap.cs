#region ...   [Header]   ...

// Solution    : Bergler.CC.InotifyPropertyChanged.Complex
// Project     : Bergler.CC.InotifyPropertyChanged.Complex.Reflection
// File        : IImplementationMap.cs
//  
// Created on  : 11-11-2016 11:40
// Altered on  : 11-11-2016 11:44
// Altered by  : Ace aka Arjan Crielaard

#endregion

#region ...   [Usings]   ...

using System;
using System.Reflection;

#endregion

namespace Bergler.CC.INotifyPropertyChanged.Complex.Reflection.Delegates
{
    /// <summary>
    ///     Used by PropertyImplementer. Provides implementations for property getters and setters as delegates.
    /// </summary>
    public interface IImplementationMap<in TBase>
    {
        /// <summary>
        ///     Provides implementation for property getter.
        /// </summary>
        /// <typeparam name="TBaseType">Base type, implementation will derive from.</typeparam>
        /// <typeparam name="TResult">Type of implementing property.</typeparam>
        /// <typeparam name="TDeclaringType">Type declating property/</typeparam>
        /// <typeparam name="TConstructedType">
        ///     Implemented type.
        ///     Never create instances of this type inside implementations.
        ///     Methods of this interface will be called in TConstructedType's static constructor.
        ///     TConstructedType is not initialized yet on call.
        /// </typeparam>
        /// <param name="property">PropertyInfo of property to implement getter.</param>
        /// <returns>Delegate, that will be used as property getter implementation.</returns>
        Func<TBaseType, TResult> ImplementGetter<TBaseType, TDeclaringType, TConstructedType, TResult>(
            PropertyInfo property)
            where TConstructedType : TBaseType, TDeclaringType
            where TBaseType : TBase;

        /// <summary>
        ///     Provides implementation for property setter.
        /// </summary>
        /// <typeparam name="TBaseType">Base type, implementation will derive from.</typeparam>
        /// <typeparam name="TResult">Type of implementing property.</typeparam>
        /// <typeparam name="TDeclaringType">Type declating property/</typeparam>
        /// <typeparam name="TConstructedType">
        ///     Implemented type.
        ///     Never create instances of this type inside implementations.
        ///     Methods of this interface will be called in TConstructedType's static constructor.
        ///     TConstructedType is not initialized yet on call.
        /// </typeparam>
        /// <param name="property">PropertyInfo of property to implement setter.</param>
        /// <returns>Delegate, that will be used as property setter implementation.</returns>
        Action<TBaseType, TResult> ImplementSetter<TBaseType, TDeclaringType, TConstructedType, TResult>(
            PropertyInfo property)
            where TConstructedType : TBaseType, TDeclaringType
            where TBaseType : TBase;

        /// <summary>
        ///     Provides implementation for property getter.
        /// </summary>
        /// <typeparam name="TBaseType">Base type, implementation will derive from.</typeparam>
        /// <typeparam name="TResult">Type of implementing property.</typeparam>
        /// <typeparam name="TDeclaringType">Type declating property/</typeparam>
        /// <typeparam name="TConstructedType">
        ///     Implemented type.
        ///     Never create instances of this type inside implementations.
        ///     Methods of this interface will be called in TConstructedType's static constructor.
        ///     TConstructedType is not initialized yet on call.
        /// </typeparam>
        /// <param name="property">PropertyInfo of property to implement getter.</param>
        /// <returns>Delegate, that will be used as property getter implementation.</returns>
        Func<TBaseType, TResult> OverrideGetter<TBaseType, TDeclaringType, TConstructedType, TResult>(
            PropertyInfo property)
            where TConstructedType : TBaseType, TDeclaringType
            where TBaseType : TBase, TDeclaringType;

        /// <summary>
        ///     Provides implementation for property setter.
        /// </summary>
        /// <typeparam name="TBaseType">Base type, implementation will derive from.</typeparam>
        /// <typeparam name="TResult">Type of implementing property.</typeparam>
        /// <typeparam name="TDeclaringType">Type declating property/</typeparam>
        /// <typeparam name="TConstructedType">
        ///     Implemented type.
        ///     Never create instances of this type inside implementations.
        ///     Methods of this interface will be called in TConstructedType's static constructor.
        ///     TConstructedType is not initialized yet on call.
        /// </typeparam>
        /// <param name="property">PropertyInfo of property to implement setter.</param>
        /// <returns>Delegate, that will be used as property setter implementation.</returns>
        Action<TBaseType, TResult> OverrideSetter<TBaseType, TDeclaringType, TConstructedType, TResult>(
            PropertyInfo property)
            where TConstructedType : TBaseType, TDeclaringType
            where TBaseType : TBase, TDeclaringType;
    }
}