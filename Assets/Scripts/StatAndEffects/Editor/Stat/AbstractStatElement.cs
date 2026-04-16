using System;
using System.Collections.Generic;
using Common;
using Common.Helper;
using StatAndEffects.Modifiers;
using StatAndEffects.Stat;
using Unity.Properties;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace StatAndEffects.Editor.Stat
{
    public abstract class AbstractStatElement : VisualElement
    {
        public Action<string> changeStatType;

        private DropdownField _statTypeDropdown;
        private ObjectField _statObjectField;
        private FloatField _baseValueField;
        private FloatField _modifiedValueField;
        private Label _valueField;
        private Foldout _foldout;

        private readonly LayerCollectionTabView _layerCollectionTabView = new LayerCollectionTabView();

        protected SerializedProperty _property;
        protected VisualElement _tabField;
        public string Name
        {
            set => this._foldout.text = value;            
        }

        public void Initialize()
        {
            this.LoadUxml();
            this.style.marginTop = 2;
            this.style.borderBottomColor = Color.black;
            this.style.borderBottomWidth = 1;
            this.CacheElements();
            this.CacheChildElement();
            this.BindCommonFields();
            this.BindElements();
            this.CreateFields();
        }

        protected virtual void CreateFields()
        {
            this._layerCollectionTabView.Initialize((dataSource as AbstractStat)?.ModifiersCollection,this._property);
            Foldout f = new Foldout()
            {
                text = "Base Value Modifiers",
                value = false,
            };
            f.Add(this._layerCollectionTabView);
            this._tabField.Add(f);

        }
        protected virtual void CacheChildElement()
        {
            
        }
        protected virtual void BindElements()
        {
            
        }
        protected virtual void LoadUxml()
        {
            var uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Assets/Scripts/StatAndEffects/Editor/Uxml/AbstractStatUxml.uxml");
            uxml.CloneTree(this);
        }

        private void CacheElements()
        {
            this._statTypeDropdown   = this.Q<DropdownField>("StatType");
            this._statObjectField    = this.Q<ObjectField>("StatObject");

            this._baseValueField     = this.Q<FloatField>("BaseValue");
            this._modifiedValueField = this.Q<FloatField>("ModifiedValue");
            this._valueField         = this.Q<Label>("Value");
            this ._tabField          = this.Q<VisualElement>("TabField");

            this._foldout = this.Q<Foldout>("Foldout");

            this._statTypeDropdown.RegisterValueChangedCallback(this.OnDropdownValueChanged);
            this._statObjectField.RegisterValueChangedCallback(this.OnStatObjectChange);
        }

        public void SetStat(string statType, int index)
        {
            this.ResetElement();
            
            if (this._statTypeDropdown.choices.Contains(dataSource.GetType().Name))
            {
                this._statTypeDropdown.SetValueWithoutNotify(dataSource.GetType().Name);
            }
        }

        private void BindCommonFields()
        {
            AbstractStat stat = (AbstractStat)dataSource;
            
            this._baseValueField.SetValueWithoutNotify(stat.BaseValue);
            PropertySetterHelper.BindElement(
                this._baseValueField,
                () => stat.BaseValue,
                v => stat.BaseValue = v,
                this._property
            );

            this._modifiedValueField.SetValueWithoutNotify(stat.ModifiedValue);
            DataBindingHelper.BindOneWay(this._modifiedValueField,
                new PropertyPath(nameof(AbstractStat.ModifiedValue)));
            
            DataBindingHelper.BindOneWay(this._valueField,
                new PropertyPath(nameof(AbstractStat.Value)));

            this._statObjectField.SetValueWithoutNotify(stat.StatDefinition);
            PropertySetterHelper.BindElement(
                this._statObjectField,
                () => stat.StatDefinition,
                v => stat.StatDefinition = (StatDefinition)v,
                this._property,
                this.OnStatObjectChange
            );
            
            this._foldout.text = stat.StatDefinition ? stat.StatDefinition.DisplayName : "Stat";
        }

        private void ResetElement()
        {
            this.Unbind();
            
        }

        private void OnDropdownValueChanged(ChangeEvent<string> evt)
        {
            if (evt.newValue == evt.previousValue)
                return;

            this.changeStatType?.Invoke(evt.newValue);
        }

        private void OnStatObjectChange(ChangeEvent<Object> evt)
        {
            this._foldout.text = evt.newValue is StatDefinition def
                ? def.DisplayName
                : "None";
        }
        
        public void SetSerializedProperty(SerializedProperty property)
        {
            this._property = property;
        }

        public void SetTypeChangeEnabled(bool isEnabled)
        {
            this._statTypeDropdown.SetEnabled(isEnabled);
        }
    }

}
