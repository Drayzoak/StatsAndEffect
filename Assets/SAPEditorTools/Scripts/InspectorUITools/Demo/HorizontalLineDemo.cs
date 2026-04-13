using UnityEngine;
using SAPUnityEditorTools.Attributes;

using SAPUnityEditorTools;
public class HorizontalLineDemo : SerializableMonoBehaviour
{
    [HorizontalLine]
    public int Speed;
    public int JumpSpeed;
    [HorizontalLine]
    public int score;

    [HorizontalLine]
    [SerializableDictionaryProperty("#732fff")]
    public SerializableDictionary<string, Player> myPlayerDictonary;
    [SerializableDictionaryProperty("#ff0000")]
    public SerializableDictionary<int, Enemy> allEnemiesDictionary;
    public GameObject[,] gridObjects = new GameObject[10,10];
}

[System.Serializable]
public class Player
{
    public int playerId;
    public int playerHealth;
}

[System.Serializable]
public class Enemy
{
    public int enemyId;
    public int enemyHealth;
}