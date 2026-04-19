using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;
namespace StatAndEffects.Stat
{
    public abstract partial class AbstractStat : IDataSourceViewHashProvider , INotifyBindablePropertyChanged
    {
        protected long viewHash;
        public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;
        
        public long GetViewHashCode() => this.viewHash;
        
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            this.viewHash++;
            
            if (this.IsDirty)
            {
                this.ModifiedValue = this.CalculateModifiedValue(this.m_DigitAccuracy);
            }
            
            propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs(propertyName));
        }
    }
}