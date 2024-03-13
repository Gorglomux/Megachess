using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//Keeping for reference, to remove
public enum GAME_STATE
{
    ROOM_TRANSITION,SHOP,FIGHT,TUTORIAL,BOSS,PAUSED, UNIT_PLACEMENT
}
[CreateAssetMenu(fileName = "GameInfos", menuName = "ScriptableObjects/GameInfos", order = 1)]
public class GameInfos : ScriptableObject
{
    public Area currentArea;
    public RoomView currentRoom;
    //public Player currentPlayer;

    public int currentGlobalPaletteIndex;

    public IState gameState;


    public ISelectable selected;

    public IHoverable hovered;

    public int currentTurn = 0;
    public int areaSize = 0;
    public bool shouldClearPlayerPrefs = false;
    public int shopPassiveChoiceAmount = 3;
    public int shopUnitChoiceAmount = 6;
    public int roomToClearBaseAmount = 5;

    public float minAnimationSpeed = 0.5f;
    public float maxAnimationSpeed = 3f;


    public float minScreenShake = 0f;
    public float maxScreenShake = 3f;
    public int baseResetCost = 1;

    public bool test = false;


    public int classesUnlockAmount = 2;
    public List<PlayerData> PlayerDatasUnlockedAtStart;
}
