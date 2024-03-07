using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

}
