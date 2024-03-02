using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "ScriptableObjects/UnitData", order = 1)]
public class UnitData : ScriptableObject
{
    public string unitName;
    public Sprite sprite;
    public int baseCost = 1;
    public bool isBoss = false;

    public List<EffectData> effectDatas;
}
