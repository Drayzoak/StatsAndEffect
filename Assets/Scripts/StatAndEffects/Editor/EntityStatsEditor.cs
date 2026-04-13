using System.Collections;
using System.Collections.Generic;
using StatAndEffects.Stat;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
namespace StatAndEffects.Editor
{
    //[CustomEditor(typeof(EntityStats))]
    public class EntityStatsEditor : UnityEditor.Editor
    {
        [SerializeField]private StyleSheet styleSheet;
        private EntityStats entityStats;
        private ListView _listView;

        public override VisualElement CreateInspectorGUI()
        {
            return new VisualElement();
        }
    }
}