#region ...   [Header]   ...

// Solution    : Bergler.CC.InotifyPropertyChanged.Simple
// Project     : Bergler.CC.InotifyPropertyChanged.Simple
// File        : State.cs
//  
// Created on  : 11-11-2016 11:36
// Altered on  : 11-11-2016 11:39
// Altered by  : Ace aka Arjan Crielaard

#endregion

namespace Bergler.CC.INotifyPropertyChanged.Simple.Flags
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