using System.Collections.Generic;
using UnityEngine;
namespace StatAndEffects.Stat
{
    [CreateAssetMenu(fileName = "Attribute Stat Definition", menuName = "StatAndEffects/Attribute Stat Definition")]
    public class AttributeStatDefinition : StatDefinition
    {
        public  List<AttributeData> attributesDatas = new List<AttributeData>();

    }
}