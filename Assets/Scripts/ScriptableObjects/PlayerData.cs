using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData", order = 1)]
public class PlayerData : ScriptableObject
{
    public string playerName;
    public Sprite sprite;
    public string description;
    public List<UnitData> startingUnits;
    public AbilityData startingAbilityData;

    public int difficulty;
}
