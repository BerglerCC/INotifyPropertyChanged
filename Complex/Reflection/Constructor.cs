#region ...   [Header]   ...

// Solution    : Bergler.CC.InotifyPropertyChanged.Complex
// Project     : Bergler.CC.InotifyPropertyChanged.Complex.Reflection
// File        : Constructor.cs
//  
// Created on  : 11-11-2016 11:40
// Altered on  : 11-11-2016 11:44
// Altered by  : Ace aka Arjan Crielaard

#endregion

#region ...   [Usings]   ...

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

#endregion

namespace Bergler.CC.INotifyPropertyChanged.Complex.Reflection
{
    /// <summary>
    ///     Reflective object construction helper.
    ///     All methods are thread safe.
    /// </summary>
    public static class Constructor
    {
        /// <summary>
        ///     Searches an instanceType constructor with delegateType-matching signature and constructs delegate of delegateType
        ///     creating new instance of instanceType.
        ///     Instance is casted to delegateTypes's return type.
        ///     Delegate's return type must be assignable from instanceType.
        /// </summary>
        /// <param name="delegateType">Type of delegate, with constructor-corresponding signature to be constructed.</param>
        /// <param name="instanceType">Type of instance to be constructed.</param>
        /// <returns>
        ///     Delegate of delegateType wich constructs instance of instanceType by calling corresponding instanceType
        ///     constructor.
        /// </returns>
        public static Delegate Compile(Type delegateType, Type instanceType)
        {
            if (!typeof (Delegate).IsAssignableFrom(delegateType))
            {
                throw new ArgumentException(string.Format("{0} is not a Delegate type.", delegateType.FullName),
                    "delegateType");
            }
            var invoke = delegateType.GetMethod("Invoke");
            var parameterTypes = invoke.GetParameters().Select(pi => pi.ParameterType).ToArray();
            var resultType = invoke.ReturnType;
            if (!resultType.IsAssignableFrom(instanceType))
            {
                throw new ArgumentException(string.Format("Delegate's return type ({0}) is not assignable from {1}.",
                    resultType.FullName, instanceType.FullName));
            }
            var ctor = instanceType.GetConstructor(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, parameterTypes, null);
            if (ctor == null)
            {
                throw new ArgumentException("Can't find constructor with delegate's signature", "instanceType");
            }
            var parameters = parameterTypes.Select(Expression.Parameter).ToArray();

            var newExpression = Expression.Lambda(delegateType,
                Expression.Convert(Expression.New(ctor, parameters), resultType),
                parameters);
            var @delegate = newExpression.Compile();
            return @delegate;
        }

        public static TDelegate Compile<TDelegate>(Type instanceType)
        {
            return (TDelegate) (object) Compile(typeof (TDelegate), instanceType);
        }
    }
}