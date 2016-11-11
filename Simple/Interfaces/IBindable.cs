#region ...   [Header]   ...

// Solution    : Bergler.CC.InotifyPropertyChanged.Simple
// Project     : Bergler.CC.InotifyPropertyChanged.Simple
// File        : IBindable.cs
//  
// Created on  : 11-11-2016 11:36
// Altered on  : 11-11-2016 11:39
// Altered by  : Ace aka Arjan Crielaard

#endregion

#region ...   [Usings]   ...

using System.Runtime.CompilerServices;

#endregion

namespace Bergler.CC.INotifyPropertyChanged.Simple.Interfaces
{
    public interface IBindable : System.ComponentModel.INotifyPropertyChanged
    {
        T Get<T>([CallerMemberName] string name = null);
        void Set<T>(T value, [CallerMemberName] string name = null);
        void OnPropertyChanged([CallerMemberName] string property = null);
    }
}