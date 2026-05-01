using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Common.MyUiToolkit
{
    [UxmlElement]
    public partial class IconFoldout : VisualElement
    {
        // ================= UI =================

        private VisualElement _header;
        private Label _arrow;
        private VisualElement _icon;
        private Label _label;
        private Button _actionButton;
        private VisualElement _content;

        // ================= STATE =================

        private bool _value;
        private bool _showButton;

        public override VisualElement contentContainer => _content;

        public Action<bool> OnValueChanged;
        public Action OnButtonClicked;

        // ================= PROPERTIES =================

        [UxmlAttribute]
        public bool Value
        {
            get => _value;
            set
            {
                if (_value == value) return;

                _value = value;
                UpdateState();
                OnValueChanged?.Invoke(_value);
            }
        }

        [UxmlAttribute]
        public bool ShowButton
        {
            get => _showButton;
            set
            {
                _showButton = value;
                UpdateButtonVisibility();
            }
        }

        [UxmlAttribute]
        public string ButtonText
        {
            get => _actionButton?.text;
            set
            {
                if (_actionButton != null)
                    _actionButton.text = value;
            }
        }

        public string Text
        {
            get => _label.text;
            set
            {
                if (!string.IsNullOrEmpty(value))
                    _label.text = value;
            }
        }

        public StyleBackground Icon
        {
            get => _icon.style.backgroundImage;
            set => _icon.style.backgroundImage = value;
        }

        public VisualElement Content => _content;

        // ================= INIT =================

        public IconFoldout()
        {
            BuildUI();
            SetValueWithoutNotify(true);
            ShowButton = false; // default
        }

        // ================= UI BUILD =================

        private void BuildUI()
        {
            style.flexDirection = FlexDirection.Column;

            // ===== HEADER =====
            _header = new VisualElement();
            _header.style.flexDirection = FlexDirection.Row;
            _header.style.alignItems = Align.Center;
            _header.style.height = 20;
            _header.style.unityFontStyleAndWeight = FontStyle.Bold;

            // Arrow
            _arrow = new Label("▼");
            _arrow.style.width = 16;
            _arrow.style.unityTextAlign = TextAnchor.MiddleCenter;

            // Icon
            _icon = new VisualElement();
            _icon.style.width = 16;
            _icon.style.height = 16;
            _icon.style.marginRight = 4;

            // Label
            _label = new Label("Foldout");
            _label.style.flexGrow = 1;

            // Button
            _actionButton = new Button();
            _actionButton.text = "+";
            _actionButton.style.marginLeft = 4;
            _actionButton.style.height = 18;

            // Button click
            _actionButton.clicked += () =>
            {
                OnButtonClicked?.Invoke();
            };

            // 🚀 IMPORTANT: prevent foldout toggle when clicking button
            _actionButton.RegisterCallback<ClickEvent>(evt =>
            {
                evt.StopPropagation();
            });

            // Header click (toggle)
            _header.RegisterCallback<ClickEvent>(_ => Toggle());

            // Add elements in correct order
            _header.Add(_arrow);
            _header.Add(_icon);
            _header.Add(_label);
            _header.Add(_actionButton);

            // ===== CONTENT =====
            _content = new VisualElement();
            _content.style.marginLeft = 18;

            // Root
            hierarchy.Add(_header);
            hierarchy.Add(_content);

            // Initial button state
            UpdateButtonVisibility();
        }

        // ================= LOGIC =================

        private void Toggle()
        {
            Value = !Value;
        }

        private void UpdateState()
        {
            if (_value)
            {
                _arrow.text = "▼";
                _content.style.display = DisplayStyle.Flex;
            }
            else
            {
                _arrow.text = "▶";
                _content.style.display = DisplayStyle.None;
            }
        }

        public void SetValueWithoutNotify(bool value)
        {
            _value = value;
            UpdateState();
        }

        private void UpdateButtonVisibility()
        {
            if (_actionButton == null) return;

            _actionButton.style.display =
                _showButton ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}