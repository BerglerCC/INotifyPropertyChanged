#region ...   [Header]   ...

// Solution    : Bergler.CC.InotifyPropertyChanged.Simple
// Project     : Bergler.CC.InotifyPropertyChanged.Simple
// File        : Bindable.cs
//  
// Created on  : 11-11-2016 11:36
// Altered on  : 11-11-2016 11:39
// Altered by  : Ace aka Arjan Crielaard

#endregion

#region ...   [Usings]   ...

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Bergler.CC.INotifyPropertyChanged.Simple.Flags;
using Bergler.CC.INotifyPropertyChanged.Simple.Interfaces;
using Bergler.CC.INotifyPropertyChanged.Simple.Property;

#endregion

namespace Bergler.CC.INotifyPropertyChanged.Simple
{
    public class Bindable : IBindable, IObjectWithIdentity
    {
        private readonly bool isNew;

        protected Bindable(Guid identity)
        {
            Identity = identity;
            PropertyHistory = new PropertyHistory();
            State = State.Unchanged;
        }

        protected Bindable()
            : this(Guid.NewGuid())
        {
            State = State.New;
            isNew = true;
        }

        public State State { get; private set; }
        public PropertyHistory PropertyHistory { get; }


        public T Get<T>([CallerMemberName] string name = null)
        {
            PropertyHistory.Add(name);

            object value = PropertyHistory[name].GetLastValue();
            return value == null ? default(T) : (T) value;
        }

        public void Set<T>(T value, [CallerMemberName] string name = null)
        {
            PropertyHistory.Add(name);

            if (Equals(value, Get<T>(name))) return;
            try
            {
                PropertyHistory[name].Add(this, value);
                if (State != State.New)
                    State = State.Modified;
            }
            catch (Exception e)
            {
                //log errror
            }
            finally
            {
                OnPropertyChanged(name);
            }
        }

        public void OnPropertyChanged([CallerMemberName] string property = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void Delete()
        {
            State = State.Deleted;
        }

        public virtual void StartInitialize()
        {
        }

        public virtual void EndInitialize()
        {
            State = !isNew ? State.Unchanged : State.New;
        }

        public virtual Guid Identity { get; set; }
    }
}