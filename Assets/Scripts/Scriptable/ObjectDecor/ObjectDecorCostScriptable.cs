using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectDecorCostScriptable", menuName = "ScriptableData/CostDataScriptable/MapHouse/ObjectDecorCostScriptable")]
public class ObjectDecorCostScriptable : ScriptableObject
{
    public List<ObjectDecorCostData> objectDecorCostDatas;
}

[System.Serializable]
public class ObjectDecorCostData
{
    public StaticKeyEnum.ObjectDecorId id;
    public string objectDecorName;
    public int priceUnlock;
}
