using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Globals", menuName = "ScriptableObjects/Globals", order = 1)]
public class GlobalVariables : ScriptableObject
{
    public GameInfos gameInfos;
    public bool GenerateSeed = true;
    public int seed = -1;

    public int paletteCount = 17;
    public Material paletteMaterial;
    public Material areaMaterial;
    //public PlayerInventory player;


    public GameObject unitPrefab;
    public string TILE_GROUND = "RuleFloor";
}
