using System.Linq;
using Common;
using Common.Helper;
using StatAndEffects.Modifiers;
using StatAndEffects.Stat;
using Unity.Properties;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using ObjectField = UnityEditor.UIElements.ObjectField;

namespace StatAndEffects.Editor.Stat
{
    public class AttributeStatElement : AbstractStatElement
    {
        private MultiColumnListView multiColumnListView;
        private AttributeStat stat;

        protected override void LoadUxml()
        {
            var uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Assets/Scripts/StatAndEffects/Editor/Uxml/AttributeStatUxml.uxml");

            uxml.CloneTree(this);
        }

        protected override void CacheChildElement()
        {
            this.multiColumnListView = this.Q<MultiColumnListView>("MultiColumnListView");
           
        }
        private void ButtonClicked()
        {
            AttributeStat attributeStat = dataSource as AttributeStat;
            attributeStat?.Refresh();
        }

        protected override void BindElements()
        {
            stat = dataSource as AttributeStat;

            // Bind StatDefinition (top field)
            this._statObjectField.ClearBindings();
            this._statObjectField.SetValueWithoutNotify(stat.StatDefinition);
            PropertySetterHelper.BindElement(
                this._statObjectField,
                () => stat.StatDefinition,
                v => stat.StatDefinition = (AttributeStatDefinition)v,
                this._property,
                this.OnStatObjectChange
            );

            _foldout.ShowButton = true;
            _foldout.ButtonText = "Refresh";
            _foldout.OnButtonClicked = ButtonClicked;
            
            // 🔥 Use index list as itemsSource
            this.multiColumnListView.itemsSource =
                Enumerable.Range(0, stat.AttributeModifiers.Count).ToList();

            this.multiColumnListView.fixedItemHeight = 22;
            this.multiColumnListView.reorderable = false;

            SetupColumns();
        }

        // ================= COLUMNS =================

        private void SetupColumns()
        {
            // 🔹 Column 0: StatDefinition (Key)
            var column0 = this.multiColumnListView.columns[0];
            column0.makeCell = MakeKeyCell;
            column0.bindCell = BindKeyCell;

            // 🔹 Column 1: Value
            var column1 = this.multiColumnListView.columns[1];
            column1.makeCell = MakeValueCell;
            column1.bindCell = BindValueCell;

            // 🔹 Optional Column 2: Type (if exists)
            if (this.multiColumnListView.columns.Count > 2)
            {
                var column2 = this.multiColumnListView.columns[2];
                column2.makeCell = () => new Label();
                column2.bindCell = (element, index) =>
                {
                    var mod = stat.AttributeModifiers.Values.ElementAt(index);
                    ((Label)element).text = mod.Type.ToString();
                };
            }
        }

        // ================= COLUMN 0 (KEY) =================

        private VisualElement MakeKeyCell()
        {
            return new ObjectField()
            {
                enabledSelf = false // readonly
            };
        }

        private void BindKeyCell(VisualElement element, int index)
        {
            var field = element as ObjectField;

            field.objectType = typeof(StatDefinition);
            field.SetValueWithoutNotify(
                this.stat.AttributeModifiers.Keys.ElementAt(index)
            );
        }
        
        private VisualElement MakeValueCell()
        {
            FloatField field = new FloatField();
            DataBindingHelper.BindOneWay(field, new PropertyPath(nameof(StatModifier.Value)));
            return field;
        }

        private void BindValueCell(VisualElement element, int index)
        {
            var field = element as FloatField;
            field.dataSource = this.stat.AttributeModifiers.Values.ElementAt(index);
        }


        public void Refresh()
        {
            if (multiColumnListView == null || stat == null)
                return;

            multiColumnListView.itemsSource =
                Enumerable.Range(0, stat.AttributeModifiers.Count).ToList();

            multiColumnListView.Rebuild();
        }
    }
}