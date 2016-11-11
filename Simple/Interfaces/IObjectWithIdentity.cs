#region ...   [Header]   ...

// Solution    : Bergler.CC.InotifyPropertyChanged.Simple
// Project     : Bergler.CC.InotifyPropertyChanged.Simple
// File        : IObjectWithIdentity.cs
//  
// Created on  : 11-11-2016 11:36
// Altered on  : 11-11-2016 11:39
// Altered by  : Ace aka Arjan Crielaard

#endregion

#region ...   [Usings]   ...

using System;

#endregion

namespace Bergler.CC.INotifyPropertyChanged.Simple.Interfaces
{
    public interface IObjectWithIdentity
    {
        Guid Identity { get; set; }
        void Delete();
        void StartInitialize();
        void EndInitialize();
    }
}