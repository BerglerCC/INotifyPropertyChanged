#region ...   [Header]   ...

// Solution    : Bergler.CC.InotifyPropertyChanged.Complex
// Project     : Bergler.CC.InotifyPropertyChanged.Complex.Core
// File        : PropertyHistory.cs
//  
// Created on  : 11-11-2016 11:40
// Altered on  : 11-11-2016 11:43
// Altered by  : Ace aka Arjan Crielaard

#endregion

#region ...   [Usings]   ...

using System.Collections;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace Bergler.CC.INotifyPropertyChanged.Complex.Core.Concepts.Property
{
    public class PropertyHistory : IList<PropertyValue>
    {
        private readonly List<PropertyValue> values;

        public PropertyHistory()
        {
            values = new List<PropertyValue>();
        }

        public PropertyValue this[string name]
        {
            get
            {
                var propertyValue = values.SingleOrDefault(pv => pv.Name == name);
                if (propertyValue == default(PropertyValue))
                    return Add(name);

                return propertyValue;
            }
        }

        public IEnumerator<PropertyValue> GetEnumerator()
        {
            return values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(PropertyValue item)
        {
            if (!Contains(item))
                values.Add(item);
        }

        public void Clear()
        {
            values.Clear();
        }

        public bool Contains(PropertyValue item)
        {
            return values.Any(pv => pv.Name == item.Name);
        }

        public void CopyTo(PropertyValue[] array, int arrayIndex)
        {
            values.CopyTo(array, arrayIndex);
        }

        public bool Remove(PropertyValue item)
        {
            return values.Remove(item);
        }

        public int Count => values.Count;

        public bool IsReadOnly => false;

        public int IndexOf(PropertyValue item)
        {
            return values.IndexOf(item);
        }

        public void Insert(int index, PropertyValue item)
        {
            values.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            values.RemoveAt(index);
        }

        public PropertyValue this[int index]
        {
            get { return values[index]; }
            set { values[index] = value; }
        }

        public PropertyValue Add(string name)
        {
            var result = new PropertyValue(name);
            values.Add(result);
            return result;
        }
    }
}