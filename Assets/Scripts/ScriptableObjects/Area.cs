using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Area", menuName = "ScriptableObjects/Area", order = 1)]
public class Area : ScriptableObject
{
    public string areaName;
    public int paletteIndex;

    //If set to true, change the color of all text to black, or vice versa.
    public bool contrastText = false;
    //Or have a color text (unused)
    public Color colorText;

    public List<Room> roomList;
    public bool shuffleArea = true;

    private int currentRoomIndex;
    private bool areaShuffled = false;
    
    public List<Room> GetRooms() { 
        if (shuffleArea)
        {
            //Shuffle according to the Random 
            roomList = roomList.OrderBy( x => GlobalHelper.rand.Next()).ToList();
            areaShuffled = true;
        }
        else
        {
            roomList = roomList.OrderBy(x => int.Parse(x.name.Split('R')[1])).ToList();
        }

        return roomList;
    }
}
