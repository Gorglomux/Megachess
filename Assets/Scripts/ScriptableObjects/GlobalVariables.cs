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
    public Material unitMaterial;
    //public PlayerInventory player;


    public Player player;

    public IndicatorManager indicatorManager;
    public InputManager inputManager;
    public GameObject unitPrefab;
    public string TILE_GROUND = "RuleFloor";
    public string TILE_WALL= "RuleWall";
    public string TILE_EMPTY= "RuleEmpty";
    public TileBase TileEmpty;

    public UIManager UIManager;
    public CameraMovementManager cameraMovement;
    public GameManager gameManager;
    public BloodSplatManager bloodSplatManager;
}
