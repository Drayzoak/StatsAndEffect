using System.Collections.Generic;
using StatAndEffects.Modifiers;
using StatAndEffects.Stat;
using UnityEditor;
using UnityEngine.UIElements;

namespace StatAndEffects.Editor.Stat
{
    public class LayerCollectionTabView : TabView
    {
        private LayeredModifierCollection _layeredModifiers;
        private SerializedProperty _property;

        private readonly Dictionary<StatLayer, ModifierCollectionTabView> _views = new();

        public void Initialize(LayeredModifierCollection layeredModifiers, SerializedProperty property)
        {
            Cleanup();

            _layeredModifiers = layeredModifiers;
            _property = property;

            if (_layeredModifiers == null || _property == null)
                return;

            BuildTabs();
        }

        private void Cleanup()
        {
            // Unsubscribe + clear existing views
            foreach (var view in _views.Values)
            {
                view.onEdit -= OnEdit;
            }

            _views.Clear();
            this.Clear();

            _layeredModifiers = null;
            _property = null;
        }

        private void BuildTabs()
        {
            foreach (var kvp in _layeredModifiers)
            {
                CreateLayerTab(kvp.Key, kvp.Value);
            }
        }

        private void CreateLayerTab(StatLayer layer, ModifierCollection collection)
        {
            var tab = new Tab(layer.ToString())
            {
                dataSource = layer
            };

            var view = new ModifierCollectionTabView
            {
                Collection = collection,
                StatLayer = layer
            };

            view.onEdit += OnEdit;
            view.Initialize(_property);

            _views[layer] = view;

            tab.Add(view);
            this.Add(tab);
        }

        private void OnEdit()
        {
            _layeredModifiers?.MarkDirty();
        }
    }
}