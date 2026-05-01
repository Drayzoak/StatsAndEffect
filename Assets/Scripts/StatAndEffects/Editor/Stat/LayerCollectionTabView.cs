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

            foreach (var view in _views.Values)
            {
                view?.Clear();
            }

            _views.Clear();
            this.Clear();

            _layeredModifiers = null;
            _property = null;
        }

        private void BuildTabs()
        {
            this.Clear();
            _views.Clear();

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
                StatLayer = layer,
                LayeredModifiers = _layeredModifiers,
            };

            view.Initialize(_property);

            _views[layer] = view;

            tab.Add(view);
            this.Add(tab);
        }

        // ================= 🔥 REFRESH =================

        private void OnLayerDirty()
        {
            RefreshAll();
        }

        public void RefreshAll()
        {
            if (_layeredModifiers == null)
                return;

            foreach (var kvp in _views)
            {
                kvp.Value?.Refresh(); // 🔥 call child refresh
            }
        }

        // 🔥 Optional: full rebuild (if structure changed)
        public void RebuildAll()
        {
            BuildTabs();
        }
    }
}