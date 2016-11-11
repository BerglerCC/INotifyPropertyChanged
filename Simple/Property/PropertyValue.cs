#region ...   [Header]   ...

// Solution    : Bergler.CC.InotifyPropertyChanged.Simple
// Project     : Bergler.CC.InotifyPropertyChanged.Simple
// File        : PropertyValue.cs
//  
// Created on  : 11-11-2016 11:36
// Altered on  : 11-11-2016 11:39
// Altered by  : Ace aka Arjan Crielaard

#endregion

#region ...   [Usings]   ...

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#endregion

namespace Bergler.CC.INotifyPropertyChanged.Simple.Property
{
    public class PropertyValue : IPropertyAction<object>
    {
        public PropertyValue(string name)
        {
            Values = new Dictionary<PropertyId, object>();
            Name = name;
        }

        public string Name { get; set; }
        public Dictionary<PropertyId, object> Values { get; set; }
        public MethodInfo MethodInfo { get; set; }
        public Type ReturnType { get; set; }

        public object this[int index]
        {
            get
            {
                int counter = 0;
                foreach (var value in Values)
                {
                    if (counter == index)
                        return value.Value;
                    counter++;
                }
                return null;
            }
        }

        public object GetPreviousValue()
        {
            int index = Values.Values.ToList().IndexOf(GetLastValue());
            if (index < 0) index = 0;
            return GetValueAtIndex(index - 1);
        }

        public object GetLastValue()
        {
            DateTime latest = new DateTime(1973, 3, 1);
            var result = default(object);
            foreach (var value in Values)
            {
                if (DateTime.Compare(latest, value.Key.DateTime) <= 0)
                {
                    latest = value.Key.DateTime;
                    result = value.Value;
                }
            }

            return result;
        }

        public object GetValueAtIndex(int index)
        {
            return this[index];
        }

        public List<object> GetValuesBefore(DateTime dateTime)
        {
            return
                (from value in Values where DateTime.Compare(dateTime, value.Key.DateTime) > 0 select value.Value)
                    .ToList();
        }

        public List<object> GetValuesOnOrBefore(DateTime dateTime)
        {
            return
                (from value in Values where DateTime.Compare(dateTime, value.Key.DateTime) >= 0 select value.Value)
                    .ToList();
        }

        public List<object> GetValuesAfter(DateTime dateTime)
        {
            return
                (from value in Values where DateTime.Compare(dateTime, value.Key.DateTime) < 0 select value.Value)
                    .ToList();
        }

        public List<object> GetValuesOnOrAfter(DateTime dateTime)
        {
            return
                (from value in Values where DateTime.Compare(dateTime, value.Key.DateTime) <= 0 select value.Value)
                    .ToList();
        }

        public void Add(object source, object value)
        {
            if (MethodInfo == null)
            {
                MethodInfo = source.GetType().GetMethod("get_" + Name);
                ReturnType = MethodInfo.ReturnParameter?.ParameterType;
            }

            PropertyId id = new PropertyId
            {
                Id = Guid.NewGuid().ToString("N"),
                DateTime = DateTime.Now
            };

            Values.Add(id, value ?? MethodInfo.Invoke(source, null));
        }
    }
}