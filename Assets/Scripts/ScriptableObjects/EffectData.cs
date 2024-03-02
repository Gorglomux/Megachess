using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectData", menuName = "ScriptableObjects/EffectData", order = 1)]
public class EffectData: ScriptableObject
{
    public string effectName;
    public Sprite sprite;
    public bool stackable = false;
    public string effectDescription;
}
