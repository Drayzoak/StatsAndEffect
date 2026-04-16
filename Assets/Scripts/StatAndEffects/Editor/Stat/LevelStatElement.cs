using Common;
using StatAndEffects.Stat;
using Unity.Properties;
using UnityEditor;
using UnityEngine.UIElements;
namespace StatAndEffects.Editor.Stat
{
    public class LevelStatElement : AbstractStatElement
    {
        private IntegerField _levelField;
        private IntegerField _maxLevelField;
        private FloatField _baseXpField;
        private FloatField _growthFactorField;

        private Slider _slider;
        
        private Label _maxValueField;
        private Label _currentValueLabel;
        
        private readonly LayerCollectionTabView _layerCollectionTabView = new LayerCollectionTabView();

        protected override void LoadUxml()
        {
            var uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Assets/Scripts/StatAndEffects/Editor/Uxml/LevelStatUxml.uxml");
            uxml.CloneTree(this);
        }

        protected override void CreateFields()
        {
            base.CreateFields();
            this._layerCollectionTabView.Initialize((dataSource as LevelStat)?.XPModifiersCollection,_property);
            Foldout fold = new Foldout()
            {
                text = "Xp Boost Modifiers",
                value = false,
            };
            fold.Add(this._layerCollectionTabView);
            this._tabField.Add(fold);
        }
        protected override void CacheChildElement()
        {
            _levelField         = this.Q<IntegerField>("Level");
            _maxLevelField      = this.Q<IntegerField>("MaxLevel");
            _baseXpField        = this.Q<FloatField>("BaseXp");
            _growthFactorField  = this.Q<FloatField>("Growth");
            
            _maxValueField      = this.Q<Label>("MaxValue");
            _currentValueLabel  = this.Q<Label>("CurrentValue");
            
            _slider             = this.Q<Slider>("Slider");
        }

        protected override void BindElements()
        {
            LevelStat stat = (LevelStat)dataSource;
            
            this._levelField.SetValueWithoutNotify(stat.Level);
            DataBindingHelper.BindTwoWay(this._levelField,
                new PropertyPath(nameof(LevelStat.Level)));
            
            this._maxLevelField.SetValueWithoutNotify(stat.MaxLevel);
            DataBindingHelper.BindTwoWay(this._maxLevelField,
                new PropertyPath(nameof(LevelStat.MaxLevel)));
            
            this._baseXpField.SetValueWithoutNotify(stat.BaseXp);
            DataBindingHelper.BindTwoWay(this._baseXpField,
                new PropertyPath(nameof(LevelStat.BaseXp)));
 
            this._growthFactorField.SetValueWithoutNotify(stat.GrowthFactor);
            DataBindingHelper.BindTwoWay(this._growthFactorField,
                new PropertyPath(nameof(LevelStat.GrowthFactor)));
            
            this._maxValueField.text = stat.Value.ToString();
            DataBindingHelper.BindOneWay(this._maxValueField,
                new PropertyPath(nameof(LevelStat.Value)));
            
            this._currentValueLabel.text = stat.XpRequiredToLevelUp.ToString();
            DataBindingHelper.BindOneWay(this._currentValueLabel,
                new PropertyPath(nameof(LevelStat.CurrentXp)));
            
            this._slider.lowValue     = 0;
            this._slider.SetValueWithoutNotify(stat.XpRequiredToLevelUp);
            DataBindingHelper.BindTwoWay(this._slider,
                new PropertyPath(nameof(LevelStat.CurrentXp)));
            
            DataBindingHelper.Bind(this._slider,
                new PropertyPath(nameof(LevelStat.XpRequiredToLevelUp)),
                BindingMode.ToTarget,
                "highValue");

            
        }
    }
}