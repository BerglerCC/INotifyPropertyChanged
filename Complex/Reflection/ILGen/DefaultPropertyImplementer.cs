#region ...   [Header]   ...

// Solution    : Bergler.CC.InotifyPropertyChanged.Complex
// Project     : Bergler.CC.InotifyPropertyChanged.Complex.Reflection
// File        : DefaultPropertyImplementer.cs
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
    public class DefaultPropertyImplementer : IPropertyImplementer
    {
        #region Implementation of IPropertyGetterImplementer

        public void ImplementGetter(MethodBuilder methodBuilder, PropertyInfo property)
        {
            var il = methodBuilder.GetILGenerator();
            var local = il.DeclareLocal(property.PropertyType);
            il.Emit(OpCodes.Ldloc, local);
            il.Emit(OpCodes.Ret);
        }

        #endregion

        #region Implementation of IPropertySetterImplementer

        public void ImplementSetter(MethodBuilder methodBuilder, PropertyInfo property)
        {
            var il = methodBuilder.GetILGenerator();
            il.Emit(OpCodes.Ret);
        }

        #endregion

        #region Implementation of IPropertyImplementer

        public void BeginEmit(TypeBuilder builder)
        {
        }

        public void EndEmit()
        {
        }

        #endregion
    }
}