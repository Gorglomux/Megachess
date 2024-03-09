using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "ScriptableObjects/UnitData", order = 1)]
public class UnitData : ScriptableObject
{
    public string unitName;
    public Sprite sprite;
    public Sprite sprite_NB;
    public int baseCost = 1;
    public bool isBoss = false;
    public bool moveAfterAttack = true;
    public bool canTargetAllies = false;


    public List<EffectData> effectDatas;
    public int paletteIndex = 4;
    public int attackOrder = 5;
}
