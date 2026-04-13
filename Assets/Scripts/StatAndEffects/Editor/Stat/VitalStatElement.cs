using Common;
using StatAndEffects.Stat;
using Unity.Properties;
using UnityEditor;
using UnityEngine.UIElements;
namespace StatAndEffects.Editor.Stat
{
    public class VitalStatElement : AbstractStatElement
    {
        private VisualElement _vitalElement;
        private FloatField _minValueField;
        private Label _maxValueField;
        private Label _currentValueLabel;
        private Slider _slider;

        protected override void CacheChildElement()
        {
            _vitalElement       = this.Q<VisualElement>("VitalStat");
            _minValueField      = this.Q<FloatField>("MinValue");
            _maxValueField      = this.Q<Label>("MaxValue");
            _currentValueLabel  = this.Q<Label>("CurrentValue");
            this._slider             = this.Q<Slider>("Slider");
            
        }
        protected override void LoadUxml()
        {
            var uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Assets/Scripts/StatAndEffects/Editor/Uxml/VitalStatUxml.uxml");

            uxml.CloneTree(this);
        }
        
        protected override void BindElements()
        {
            VitalStat stat = (VitalStat)dataSource;

            this._vitalElement.style.display = DisplayStyle.Flex;
            this._slider.SetValueWithoutNotify(stat.CurrentValue);
            DataBindingHelper.BindTwoWay(this._slider,
                new PropertyPath(nameof(VitalStat.CurrentValue)));

            DataBindingHelper.Bind(this._slider,
                new PropertyPath(nameof(VitalStat.MinValue)),
                BindingMode.ToTarget,
                "lowValue");

            DataBindingHelper.Bind(this._slider,
                new PropertyPath(nameof(VitalStat.Value)),
                BindingMode.ToTarget,
                "highValue");
            
            this._minValueField.SetValueWithoutNotify(stat.MinValue);
            DataBindingHelper.BindTwoWay(_minValueField,
                new PropertyPath(nameof(VitalStat.MinValue)));
            
            this._maxValueField.text = stat.Value.ToString();
            DataBindingHelper.BindOneWay(this._maxValueField,new PropertyPath(nameof(VitalStat.Value)));
            this._currentValueLabel.text = stat.CurrentValue.ToString();
            DataBindingHelper.BindOneWay(this._currentValueLabel,new PropertyPath(nameof(VitalStat.CurrentValue)));
        }
    }
}