using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SAPUnityEditorTools
{
    public class TooltipHandler : MonoBehaviour
    {
        /// <summary>
        /// Static instance of the class.
        /// </summary>
        private static TooltipHandler Instance;

        [HideInInspector]
        public RectTransform tooltipTransform, tooltipTextRect;
        [HideInInspector]
        public TMPro.TextMeshProUGUI tooltipText;
        [HideInInspector]
        public bool shouldUpdate = false;
        [HideInInspector]
        public PointerEventData data;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        public static void ShowToolTip(string str, PointerEventData d)
        {
            if (Instance == null)
                CreateTooltipHandler();

            Instance.tooltipText.text = str;
            Instance.shouldUpdate = true;
            Instance.data = d;
            Instance.StartCoroutine(nameof(DisplayTooltipRoutine));
        }

        private IEnumerator DisplayTooltipRoutine()
        {
            tooltipTransform.gameObject.SetActive(true);
            yield return null;
            var pos = data.position;
            pos.y -= 20f;
            pos.x += 20f;
            tooltipTransform.position = pos;
            var sizeDelta = tooltipText.GetComponent<RectTransform>().sizeDelta;
            sizeDelta.y += 10;
            sizeDelta.x += 10;
            if (pos.x + sizeDelta.x > Screen.width)
            {
                var diff = pos.x + sizeDelta.x - Screen.width;
                tooltipTransform.position -= new Vector3(diff, 0, 0);
            }
            tooltipTransform.sizeDelta = sizeDelta;
            yield return null;
            shouldUpdate = false;
        }

        public static void HideToolTip()
        {
            Instance.tooltipTransform.gameObject.SetActive(false);
        }

        public static void CreateTooltipHandler()
        {
            //Create UI panel with screen size to place tool tip UI object under.
            var go = new GameObject("Panel - TooltipHandler");
            var rectTrans = go.AddComponent<RectTransform>();
            rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width);
            rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height);
            rectTrans.pivot = new Vector2(0.5f, 0.5f);
            rectTrans.SetParent(FindObjectOfType<Canvas>().transform);
            var handler = go.AddComponent<TooltipHandler>();

            //Create parent for tool tip text UI gameobject.
            var tooltiptextTransParent = new GameObject("Image - Tooltip Container");
            var toolTipTrans = tooltiptextTransParent.AddComponent<RectTransform>();
            toolTipTrans.pivot = new Vector2(0, 1);
            toolTipTrans.anchorMin = new Vector2(0, 1);
            toolTipTrans.anchorMax = new Vector2(0, 1);
            var img = tooltiptextTransParent.AddComponent<Image>();
            img.raycastTarget = false;
            img.color = new Color(0, 0, 0, 0.8f);
            toolTipTrans.SetParent(rectTrans);

            //Create tool tip text UI gameobject.
            var tooltipText = new GameObject("Text - Tooltip");
            tooltipText.transform.SetParent(toolTipTrans);
            var tm_rect = tooltipText.AddComponent<RectTransform>();
            tm_rect.pivot = new Vector2(0, 1);
            tm_rect.anchorMin = new Vector2(0, 1);
            tm_rect.anchorMax = new Vector2(0, 1);
            tm_rect.anchoredPosition = new Vector2(5, -5);
            var tm_text = tooltipText.AddComponent<TMPro.TextMeshProUGUI>();
            var asset = Resources.Load<ToolTipSetting>("TooltipSettings");
            if (asset != null)
            {
                if (asset.tmp_font_asset != null)
                {
                    tm_text.font = asset.tmp_font_asset;
                    tm_text.UpdateFontAsset();
                }
                tm_text.fontSize = asset.fontSize;
                tm_text.alignment = asset.alignment;
                tm_text.fontStyle = asset.fontStyles;
            }
            var textFitter = tooltipText.AddComponent<ContentSizeFitter>();
            textFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            textFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            //Show tool tip on the UI
            handler.tooltipTransform = toolTipTrans;
            handler.tooltipText = tm_text;
            handler.tooltipTextRect = tm_rect;
        }
    }
}