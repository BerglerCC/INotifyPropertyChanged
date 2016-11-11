#region ...   [Header]   ...

// Solution    : Bergler.CC.InotifyPropertyChanged.Complex
// Project     : Bergler.CC.InotifyPropertyChanged.Complex.Core
// File        : IObjectWithIdentity.cs
//  
// Created on  : 11-11-2016 11:40
// Altered on  : 11-11-2016 11:43
// Altered by  : Ace aka Arjan Crielaard

#endregion

#region ...   [Usings]   ...

using System;

#endregion

namespace Bergler.CC.INotifyPropertyChanged.Complex.Core.Concepts
{
    public interface IObjectWithIdentity
    {
        Guid Identity { get; set; }
    }
}