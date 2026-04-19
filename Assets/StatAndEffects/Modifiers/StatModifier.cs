using System;
using System.Runtime.CompilerServices;
using StatAndEffects.Stat;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace StatAndEffects.Modifiers
{
    [Serializable]
    public class StatModifier : IDataSourceViewHashProvider , INotifyBindablePropertyChanged
    {
        [SerializeField,DontCreateProperty] private StatModifierType type;
        [SerializeField,DontCreateProperty] private StatLayer layer;
        [SerializeField,DontCreateProperty] private float value;
        
        [CreateProperty]
        public float Value
        {
            get { return value; }
            set
            {
                if (Mathf.Approximately(value , this.value)) return;
                this.value = value;
                this.NotifyPropertyChanged();
            }
        }
        
        [CreateProperty]
        public StatModifierType Type
        {
            get { return type; }
            set
            {
                if (Equals(value,this.type)) return;
                this.type = value;
                this.NotifyPropertyChanged();
            }
        }

        [CreateProperty]
        public StatLayer Layer
        {
            get { return layer; }
            set
            {
                if (Equals(value,this.layer)) return;
                this.layer = value;
                this.NotifyPropertyChanged();
            }
        }
        
        public static implicit operator float(StatModifier modifier) => modifier.Value;

        public StatModifier(StatModifierType type, float value, StatLayer layer = StatLayer.Base)
        {
            this.value = value;
            this.Type = type;
            this.layer = layer;
        }
        
        public StatModifier()
        {
            this.value = 1;
            this.Type = StatModifierType.Flat;
            this.layer = StatLayer.Base;
        }
        private long viewHash;
        
        public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;

        public long GetViewHashCode() => viewHash;
        
        void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            viewHash++;
            propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs(propertyName));
        }
    }
}
