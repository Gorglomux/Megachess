using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GAME_STATE
{
    ROOM_TRANSITION,SHOP,FIGHT,TUTORIAL,BOSS,PAUSED
}
[CreateAssetMenu(fileName = "GameInfos", menuName = "ScriptableObjects/GameInfos", order = 1)]
public class GameInfos : ScriptableObject
{
    public Area currentArea;
    public RoomView currentRoom;
    //public Player currentPlayer;

    public int currentGlobalPaletteIndex;

    public GAME_STATE gameState;


    public ISelectable selected;

    public IHoverable hovered;
}
