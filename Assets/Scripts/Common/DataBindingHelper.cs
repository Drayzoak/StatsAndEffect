using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace Common
{
    public static class DataBindingHelper
    {
        static string GetDefaultTargetProperty<T>() where T : VisualElement
        {
            return typeof(T) == typeof(Label) ? "text" : "value";
        }

        #region Core Binding

        static void Bind(
            VisualElement element,
            PropertyPath propertyPath,
            BindingMode bindingMode,
            string targetProperty,
            BindingUpdateTrigger updateTrigger)
        {
            if (element == null)
            {
                Debug.LogWarning($"Cannot bind to null element for property: {propertyPath}");
                return;
            }

            if (element is not BindableElement bindableElement)
            {
                Debug.LogWarning($"Element type {element.GetType().Name} does not support binding.");
                return;
            }

            bindableElement.bindingPath = propertyPath.ToString();

            var binding = new DataBinding
            {
                dataSourcePath = propertyPath,
                bindingMode = bindingMode,
                updateTrigger = updateTrigger
            };

            element.SetBinding(targetProperty, binding);
        }

        #endregion

        #region Generic Bind

        public static void Bind<T>(
            T element,
            PropertyPath propertyPath,
            BindingMode bindingMode,
            BindingUpdateTrigger updateTrigger = BindingUpdateTrigger.OnSourceChanged)
            where T : VisualElement
        {
            string targetProperty = GetDefaultTargetProperty<T>();
            Bind(element, propertyPath, bindingMode, targetProperty, updateTrigger);
        }

        public static void Bind<T>(
            T element,
            PropertyPath propertyPath,
            BindingMode bindingMode,
            string targetProperty,
            BindingUpdateTrigger updateTrigger = BindingUpdateTrigger.OnSourceChanged)
            where T : VisualElement
        {
            Bind((VisualElement)element, propertyPath, bindingMode, targetProperty, updateTrigger);
        }

        #endregion

        #region One Way (Data → UI)

        public static void BindOneWay<T>(
            T element,
            PropertyPath propertyPath,
            BindingUpdateTrigger updateTrigger = BindingUpdateTrigger.OnSourceChanged)
            where T : VisualElement
        {
            Bind(element, propertyPath, BindingMode.ToTarget, updateTrigger);
        }

        #endregion

        #region One Way (UI → Data)

        public static void BindToSource<T>(
            T element,
            PropertyPath propertyPath,
            BindingUpdateTrigger updateTrigger = BindingUpdateTrigger.OnSourceChanged)
            where T : VisualElement
        {
            Bind(element, propertyPath, BindingMode.ToSource, updateTrigger);
        }

        #endregion

        #region Two Way

        public static void BindTwoWay<T>(
            T element,
            PropertyPath propertyPath,
            BindingUpdateTrigger updateTrigger = BindingUpdateTrigger.OnSourceChanged)
            where T : VisualElement
        {
            Bind(element, propertyPath, BindingMode.TwoWay, updateTrigger);
        }

        #endregion

        #region One Time

        public static void BindToTargetOnce<T>(
            T element,
            PropertyPath propertyPath)
            where T : VisualElement
        {
            Bind(element, propertyPath, BindingMode.ToTargetOnce, BindingUpdateTrigger.OnSourceChanged);
        }

        #endregion
    }
}