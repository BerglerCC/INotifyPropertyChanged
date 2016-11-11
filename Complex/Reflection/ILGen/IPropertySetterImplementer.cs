#region ...   [Header]   ...

// Solution    : Bergler.CC.InotifyPropertyChanged.Complex
// Project     : Bergler.CC.InotifyPropertyChanged.Complex.Reflection
// File        : IPropertySetterImplementer.cs
//  
// Created on  : 11-11-2016 11:40
// Altered on  : 11-11-2016 11:44
// Altered by  : Ace aka Arjan Crielaard

#endregion

#region ...   [Usings]   ...

using System.Reflection;
using System.Reflection.Emit;

#endregion

namespace Bergler.CC.INotifyPropertyChanged.Complex.Reflection.ILGen
{
    public interface IPropertySetterImplementer
    {
        void ImplementSetter(MethodBuilder methodBuilder, PropertyInfo property);
    }
}