using System;
using System.Collections.Generic;
using StatAndEffects.Modifiers;
using StatAndEffects.Stat;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace StatAndEffects.Editor.Stat
{
    public class ModifierCollectionTabView : TabView
    {
        private ListView _listView;

        private StatLayer _statLayer;
        private ModifierCollection _collection;
        private List<StatModifier> _currentModifiers;
        private Dictionary<StatModifierType, Tab> _tabs;
        private SerializedProperty _property;

        public Action onEdit;

        private void Reset()
        {
            _listView?.Unbind();
            _listView = null;

            _currentModifiers?.Clear();
            _currentModifiers = null;

            _tabs?.Clear();
            _tabs = null;

            _property = null;
        }
        public ModifierCollection Collection
        {
            get => this._collection;
            set => this._collection = value;
        }

        public StatLayer StatLayer
        {
            get => this._statLayer;
            set => this._statLayer = value;
        }
        public void Initialize(SerializedProperty property)
        {
            Reset();

            _property = property;

            if (_collection == null)
                return; 

            _currentModifiers = new List<StatModifier>();

            BuildTabs();
        }
        private void BuildTabs()
        {
            this.Clear();
            this._tabs = new Dictionary<StatModifierType, Tab>();
            this.activeTabChanged -= OnActiveChanged;
            this.activeTabChanged += OnActiveChanged;

            this.Add(new Tab("All"));

            foreach (var type in this._collection.Getkey)
            {
                Tab tab = new Tab(type.ToString() + 
                        $" {this._collection[type].Count}/{this._collection[type].Capacity}");
                tab.dataSource = type;
                this._tabs.TryAdd(type, tab);
                this.Add(tab);
            }

            RefreshTab(this.activeTab);
        }

        private void OnActiveChanged(Tab previous, Tab current)
        {
            previous?.Clear();
            RefreshTab(current);
        }

        private void RefreshTab(Tab tab)
        {
            _listView?.Unbind();
            _listView = null;
            tab.Clear();

            _currentModifiers = tab.label == "All"
                ? this._collection.GetModifiersCopy()
                : this._collection.GetModifiersCopy((StatModifierType)tab.dataSource);

            _listView = new ListView
            {
                itemsSource = _currentModifiers,
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
                makeItem = MakeItem,
                bindItem = BindItem,
                unbindItem = UnbindItem,
                showAddRemoveFooter = true,
                onAdd = OnAdd,
                onRemove = OnRemove
            };

            tab.Add(_listView);
        }

        private VisualElement MakeItem() => new StatModifierElement();

        private void BindItem(VisualElement element, int index)
        {
            var item = (StatModifierElement)element;
            item.SetStatModifier(_currentModifiers[index]);
            item.OnTypeChanged = RelocateModifier;
        }

        private void UnbindItem(VisualElement element, int index)
        {
            var item = (StatModifierElement)element;
            item.UnBind();
            item.OnTypeChanged = null;
        }

        private void OnAdd(BaseListView view)
        {
            var type = this.activeTab?.dataSource is StatModifierType t
                ? t
                : StatModifierType.Flat;

            var modifier = new StatModifier(type, 1,this._statLayer);

            if (!this._collection.TryAddModifier(modifier))
                return;

            UpdateTabLabels(type);
            _currentModifiers.Add(modifier);
            _listView.RefreshItems();
            
            Apply();
        }

        private void OnRemove(BaseListView view)
        {
            StatModifier modifier = null;

            if (_listView.selectedItem is StatModifier selected)
                modifier = selected;
            else if (_currentModifiers.Count > 0)
                modifier = _currentModifiers[^1];

            if (modifier == null)
                return;

            if (!this._collection.TryRemoveModifier(modifier))
                return;

            UpdateTabLabels(modifier.Type);
            _currentModifiers.Remove(modifier);
            _listView.RefreshItems();

            Apply();
        }

        private void RelocateModifier(StatModifier modifier, StatModifierType newType)
        {
            if (!this._collection.TryRemoveModifier(modifier))
                return;

            UpdateTabLabels(modifier.Type);
            modifier.Type = newType;
            this._collection.TryAddModifier(modifier);
            UpdateTabLabels(newType);
            RefreshTab(this.activeTab);
            
            Apply();
        }
        
        private void UpdateTabLabels(StatModifierType type)
        {
            var op = _collection[type];
            this._tabs[type].label = $"{type} {op.Count}/{op.Capacity}";
        }
        
        private void Apply()
        {
            var stat = this._property.managedReferenceValue as AbstractStat;
            this.onEdit?.Invoke();
            if (stat != null)
                stat.IsDirty = true;
            _property.serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(_property.serializedObject.targetObject);
        }
    }

}