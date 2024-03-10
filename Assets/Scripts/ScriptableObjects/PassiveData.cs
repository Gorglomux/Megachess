using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "PassiveData", menuName = "ScriptableObjects/PassiveData", order = 1)]
public class PassiveData : ScriptableObject
{
    public string passiveName;
    public string passiveDescription;
    public Sprite passiveSprite;
    public EFFECT_ACTIVATION_TIME passiveActivationTime;

    public bool unique = true;
    public int passiveCost = 5; 
}
