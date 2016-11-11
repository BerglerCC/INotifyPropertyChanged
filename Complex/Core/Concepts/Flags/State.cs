#region ...   [Header]   ...

// Solution    : Bergler.CC.InotifyPropertyChanged.Complex
// Project     : Bergler.CC.InotifyPropertyChanged.Complex.Core
// File        : State.cs
//  
// Created on  : 11-11-2016 11:40
// Altered on  : 11-11-2016 11:43
// Altered by  : Ace aka Arjan Crielaard

#endregion

namespace Bergler.CC.INotifyPropertyChanged.Complex.Core.Concepts.Flags
{
    public enum State
    {
        New,
        Modified,
        Deleted,
        Unchanged,
        Unknown
    }
}