using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// All effects are on a Unit. 
/// </summary>
[CreateAssetMenu(fileName = "EffectData", menuName = "ScriptableObjects/EffectData", order = 1)]
public class EffectData: ScriptableObject
{
    public string effectName;
    public Sprite sprite;
    public bool stackable = false;
    public string effectDescription;

    public EFFECT_ACTIVATION_TIME effectActivationTime;
    public int defaultValue = 1;
    public bool transferOnMega = true;
}
