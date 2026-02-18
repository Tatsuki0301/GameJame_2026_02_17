using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectDataStorage", menuName = "Custom/ObjectDataStorage", order = 1)]
public class ObjectDataStorage : ScriptableObject
{
    /// <summary>
    /// オブジェクトのデータ&設置のオフセット
    /// </summary>
    [System.Serializable]
    public struct ObjectData
    {
        public GameObject obj;
        public Quaternion qua;
        public int value;
    }

    public List<ObjectData> objDatas = new List<ObjectData>();
}