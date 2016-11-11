#region ...   [Header]   ...

// Solution    : Bergler.CC.InotifyPropertyChanged.Simple
// Project     : Bergler.CC.InotifyPropertyChanged.Simple
// File        : IPropertyAction.cs
//  
// Created on  : 11-11-2016 11:36
// Altered on  : 11-11-2016 11:39
// Altered by  : Ace aka Arjan Crielaard

#endregion

#region ...   [Usings]   ...

using System;
using System.Collections.Generic;

#endregion

namespace Bergler.CC.INotifyPropertyChanged.Simple.Property
{
    public interface IPropertyAction<TObject>
    {
        TObject GetPreviousValue();

        TObject GetLastValue();

        TObject GetValueAtIndex(int index);

        List<TObject> GetValuesBefore(DateTime dateTime);

        List<TObject> GetValuesOnOrBefore(DateTime dateTime);

        List<TObject> GetValuesAfter(DateTime dateTime);

        List<TObject> GetValuesOnOrAfter(DateTime dateTime);
    }
}