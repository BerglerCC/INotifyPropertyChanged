#region ...   [Header]   ...

// Solution    : Bergler.CC.InotifyPropertyChanged.Complex
// Project     : Bergler.CC.InotifyPropertyChanged.Complex.Core
// File        : Implementation.cs
//  
// Created on  : 11-11-2016 11:40
// Altered on  : 11-11-2016 11:43
// Altered by  : Ace aka Arjan Crielaard

#endregion

#region ...   [Usings]   ...

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Bergler.CC.INotifyPropertyChanged.Complex.Reflection;
using Bergler.CC.INotifyPropertyChanged.Complex.Reflection.Delegates;

#endregion

namespace Bergler.CC.INotifyPropertyChanged.Complex.Core.Concepts
{
    public class Implementation<TConcept> : DefaultImplementation<TConcept> where TConcept : Concept
    {
        public override Func<TBaseType, TResult> ImplementGetter<TBaseType, TDeclaringType, TConstructedType, TResult>(
            PropertyInfo property)
        {
            throw new ArgumentException("Can't implement new property. Just override.");
        }

        public override Action<TBaseType, TResult> ImplementSetter<TBaseType, TDeclaringType, TConstructedType, TResult>
            (PropertyInfo property)
        {
            throw new ArgumentException("Can't implement new property. Just override.");
        }

        public override Func<TBaseType, TResult> OverrideGetter<TBaseType, TDeclaringType, TConstructedType, TResult>(
            PropertyInfo property)
        {
            //this code will run once per implementing property
            return PropertyImplementation<TBaseType, TDeclaringType>.GetGetter<TResult>(property.Name);
        }

        public override Action<TBaseType, TResult> OverrideSetter<TBaseType, TDeclaringType, TConstructedType, TResult>(
            PropertyInfo property)
        {
            //this code will run once per implementing property
            var propertyName = property.Name;
            var setter = PropertyImplementation<TBaseType, TDeclaringType>.GetSetter<TResult>(propertyName);
            var getter = PropertyImplementation<TBaseType, TDeclaringType>.GetGetter<TResult>(propertyName);
            var comparer = EqualityComparer<TResult>.Default;

            return (pthis, value) =>
            {
//constructed delegate will run every time accessing property.
                if (!comparer.Equals(value, getter(pthis)))
                {
                    setter(pthis, value);
                    pthis.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
                }
            };
        }
    }
}