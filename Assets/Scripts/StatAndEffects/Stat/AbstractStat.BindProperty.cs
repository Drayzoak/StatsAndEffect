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
        
        public long GetViewHashCode() => viewHash;

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            viewHash++;
            propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs(propertyName));
        }
    }
}