#region ...   [Header]   ...

// Solution    : Bergler.CC.InotifyPropertyChanged.Complex
// Project     : Bergler.CC.InotifyPropertyChanged.Complex.Reflection
// File        : Property.cs
//  
// Created on  : 11-11-2016 11:40
// Altered on  : 11-11-2016 11:44
// Altered by  : Ace aka Arjan Crielaard

#endregion

#region ...   [Usings]   ...

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

#endregion

namespace Bergler.CC.INotifyPropertyChanged.Complex.Reflection
{
    /// <summary>
    ///     Provides fast acces to TType's properties by its name.
    ///     All acceses done as is e.g. callvirt to virtual properties and call to others.
    ///     All operations in this class is O(1) except first access wich takes O(N) where N is number of properties in TType.
    ///     All methods a thread safe.
    ///     All accesses (except first one) are lock-free.
    /// </summary>
    /// <typeparam name="TType">Type, to construct property accessors.</typeparam>
    public static class Property<TType>
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

        public static Func<TType, TValue> GetGetter<TValue>(string propertyName)
        {
            return (Func<TType, TValue>) Getters[propertyName];
        }

        public static Action<TType, TValue> GetSetter<TValue>(string propertyName)
        {
            return (Action<TType, TValue>) Setters[propertyName];
        }

        public static TValue Get<TValue>(TType instance, string propertyName)
        {
            return GetGetter<TValue>(propertyName)(instance);
        }

        public static void Set<TValue>(TType instance, string propertyName, TValue value)
        {
            GetSetter<TValue>(propertyName)(instance, value);
        }

        private static Dictionary<string, Delegate> CompileGetters()
        {
            var type = typeof (TType);
            var propertys = type.GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance);
            var compileGetters = new Dictionary<string, Delegate>(propertys.Length);

            foreach (var property in propertys)
            {
                if (property.CanRead)
                {
                    var parameter = Expression.Parameter(type, "instance");
                    var lambda = Expression.Lambda(Expression.Call(parameter, property.GetGetMethod()), parameter);
                    compileGetters.Add(property.Name, lambda.Compile());
                }
            }
            return compileGetters;
        }

        private static Dictionary<string, Delegate> CompileSetters()
        {
            var type = typeof (TType);
            var propertys = type.GetProperties(BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.Instance);
            var compileSetters = new Dictionary<string, Delegate>(propertys.Length);

            foreach (var property in propertys)
            {
                if (property.CanWrite)
                {
                    var instance = Expression.Parameter(type, "instance");
                    var value = Expression.Parameter(property.PropertyType, "value");
                    var lambda = Expression.Lambda(Expression.Call(instance, property.GetSetMethod(), value), instance,
                        value);
                    compileSetters.Add(property.Name, lambda.Compile());
                }
            }
            return compileSetters;
        }
    }
}