using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TARGETTING_TYPE
{
    RESERVE, // Clone a unit
    UNIT, // Gain Bloodlust, shield, kill
    GLOBAL //Increase par count by one, Get money equal to megas on board,  +1 capture this combat
}
public enum COOLDOWN_TYPE
{
    FIGHT_AMOUNT,
    KILL_AMOUNT,
    AREA_CLEARED,
    UNIT_LOST_AMOUNT,
    TURN_AMOUNT
}

[CreateAssetMenu(fileName = "AbilityData", menuName = "ScriptableObjects/AbilityData", order = 1)]
public class AbilityData : ScriptableObject
{
    public string abilityName;
    public TARGETTING_TYPE targettingType;
    public COOLDOWN_TYPE cooldownType;
    public int cooldownDuration= 1;
    public string description;
}
