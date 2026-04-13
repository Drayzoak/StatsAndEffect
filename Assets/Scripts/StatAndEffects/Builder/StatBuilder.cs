using System;
using StatAndEffects.Modifiers;
using StatAndEffects.Stat;
namespace StatAndEffects.Builder
{
    public class StatBuilder
    {
        private LayerCreationContext layerCreationContext = new LayerCreationContext();

        private StatLayer currentLayer;
        private OperationCreationContext _currentCreationContext;

        public StatBuilder SetLayer(StatLayer layer)
        {
            currentLayer = layer;
            return this;
        }
        
        public StatBuilder AddLayer(StatLayer layer)
        {
            currentLayer = layer;

            if (!layerCreationContext.layers.TryGetValue(layer, out this._currentCreationContext))
            {
                this._currentCreationContext = new OperationCreationContext();
                layerCreationContext.layers.TryAdd(layer, this._currentCreationContext);
            }

            return this;
        }
        
        public StatBuilder AddOperation(StatModifierType type, int capacity)
        {
            if (this._currentCreationContext == null)
                throw new Exception("AddLayer must be called before adding operations.");

            this._currentCreationContext.AddOperation(type, capacity);
            return this;
        }

        public StatBuilder AddDefaultOperation(int capacity)
        {
            if (this._currentCreationContext == null)
            {
                throw new Exception("AddDefaultOperation must be called before adding operations.");
            }
            
            this._currentCreationContext.AddOperation(StatModifierType.Flat , capacity);
            this._currentCreationContext.AddOperation(StatModifierType.Additive, capacity);
            this._currentCreationContext.AddOperation(StatModifierType.Multiplicative, capacity);
            return this;
        }

        
        public LayerCreationContext Build()
        {
            return layerCreationContext;
        }
        
    }

}