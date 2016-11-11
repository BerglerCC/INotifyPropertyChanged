#region ...   [Header]   ...

// Solution    : Bergler.CC.InotifyPropertyChanged.Complex
// Project     : Bergler.CC.InotifyPropertyChanged.Complex.Reflection
// File        : Singleton.cs
//  
// Created on  : 11-11-2016 11:40
// Altered on  : 11-11-2016 11:44
// Altered by  : Ace aka Arjan Crielaard

#endregion

#region ...   [Usings]   ...

using System;

#endregion

namespace Bergler.CC.INotifyPropertyChanged.Complex.Reflection.Common
{
    public class Singleton<TInstanceType>
    {
        private readonly object syncRoot = new object();
        private Func<TInstanceType> construction;
        private TInstanceType instance;

        public Singleton(Func<TInstanceType> construction)
        {
            if (construction == null)
            {
                throw new ArgumentException("Null delegate is not allowed.");
            }
            this.construction = construction;
        }

        public TInstanceType Instance
        {
            get
            {
                if (ReferenceEquals(instance, null))
                {
                    lock (syncRoot)
                    {
                        if (ReferenceEquals(instance, null))
                        {
                            instance = construction();
                            construction = null;
                            if (ReferenceEquals(instance, null))
                            {
                                throw new InvalidOperationException("Constructing delegate returned null reference.");
                            }
                        }
                    }
                }
                return instance;
            }
        }

        public static implicit operator TInstanceType(Singleton<TInstanceType> singletone)
        {
            return singletone.Instance;
        }
    }
}