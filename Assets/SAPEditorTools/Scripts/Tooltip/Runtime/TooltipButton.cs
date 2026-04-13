using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SAPUnityEditorTools
{
    public class TooltipButton : Button
    {
        [Header("Tooltip")]
        public string tooltipStr;

        private float _currentWait = 0f;
        private bool _isOverButton, _isTipShown;
        private PointerEventData _currentData;

        private void Update()
        {
            if (_isOverButton || _isTipShown)
                return;
            _currentWait -= Time.deltaTime;
            if (_currentWait > 0)
            {
                return;
            }
            _isTipShown = true;
            TooltipHandler.ShowToolTip(tooltipStr, _currentData);
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            _currentData = eventData;
            _currentWait = 1f;
            _isOverButton = true;
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            _isOverButton = false;
            _isTipShown = false;
            if (_currentWait <= 0)
            {
                TooltipHandler.HideToolTip();
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (_isTipShown)
            {
                _isOverButton = false;
                if (_isTipShown)
                {
                    _isTipShown = false;
                    TooltipHandler.HideToolTip();
                }
            }
        }
    }
}
