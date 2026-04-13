using UnityEngine;

namespace SAPUnityEditorTools
{
    public class SerializableDictionaryProperty : PropertyAttribute
    {
        public Color boxColor;
        public SerializableDictionaryProperty(string colorHex)
        {
            if (ColorUtility.TryParseHtmlString(colorHex, out var c))
            {
                this.boxColor = c;
            }
            else
            {
                Debug.Log("Parse Failed : " + colorHex);
                this.boxColor = Color.green;
            }
        }
    }
}
