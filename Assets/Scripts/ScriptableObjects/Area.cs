using System.Collections;
using System.Collections.Generic;
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
}
