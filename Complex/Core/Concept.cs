#region ...   [Header]   ...

// Solution    : Bergler.CC.InotifyPropertyChanged.Complex
// Project     : Bergler.CC.InotifyPropertyChanged.Complex.Core
// File        : Concept.cs
//  
// Created on  : 11-11-2016 11:42
// Altered on  : 11-11-2016 11:43
// Altered by  : Ace aka Arjan Crielaard

#endregion

#region ...   [Usings]   ...

using System;
using System.ComponentModel;
using Bergler.CC.INotifyPropertyChanged.Complex.Core.Concepts;
using Bergler.CC.INotifyPropertyChanged.Complex.Core.Concepts.Flags;
using Bergler.CC.INotifyPropertyChanged.Complex.Core.Concepts.Property;
using Bergler.CC.INotifyPropertyChanged.Complex.Reflection;

#endregion

namespace Bergler.CC.INotifyPropertyChanged.Complex.Core
{
    public class Concept : System.ComponentModel.INotifyPropertyChanged, IObjectWithIdentity
    {
        protected Concept(Guid identity)
        {
            PropertyHistory = new PropertyHistory();
            State = State.Unchanged;
            Identity = identity;
        }

        protected Concept()
            : this(Guid.NewGuid())
        {
            State = State.New;
        }

        public State State { get; private set; }

        public PropertyHistory PropertyHistory { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual Guid Identity { get; set; }

        public void Delete()
        {
            State = State.Deleted;
        }

        protected internal void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyHistory[e.PropertyName].Add(this);
            State = State.Modified;

            PropertyChanged?.Invoke(this, e);
        }

        public static class Create<TConcept> where TConcept : Concept
        {
            //Construct derived Type
            public static readonly Type Type = PropertyProxy.ConstructType<TConcept, Implementation<TConcept>>(new Type[0], true);

            //Create constructing delegate
            public static Func<TConcept> New = Constructor.Compile<Func<TConcept>>(Type);
        }
    }
}