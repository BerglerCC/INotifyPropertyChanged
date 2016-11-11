#region ...   [Header]   ...

// Solution    : Bergler.CC.InotifyPropertyChanged.Complex
// Project     : Bergler.CC.InotifyPropertyChanged.Complex.Core
// File        : IPropertyAction.cs
//  
// Created on  : 11-11-2016 11:40
// Altered on  : 11-11-2016 11:43
// Altered by  : Ace aka Arjan Crielaard

#endregion

#region ...   [Usings]   ...

using System;
using System.Collections.Generic;

#endregion

namespace Bergler.CC.INotifyPropertyChanged.Complex.Core.Concepts.Property
{
    public interface IPropertyAction<TObject>
    {
        TObject GetLastValue();

        TObject GetValueAtIndex(int index);

        List<TObject> GetValuesBefore(DateTime dateTime);

        List<TObject> GetValuesOnOrBefore(DateTime dateTime);

        List<TObject> GetValuesAfter(DateTime dateTime);

        List<TObject> GetValuesOnOrAfter(DateTime dateTime);
    }
}