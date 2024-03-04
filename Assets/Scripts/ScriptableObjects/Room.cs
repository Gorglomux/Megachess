using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Room", menuName = "ScriptableObjects/Room", order = 1)]
public class Room : ScriptableObject
{
    public string roomName;
    public GameObject roomPrefab;
    public int par;
    public bool isTutorial = false;
    public bool isBoss = false;
    public int maxUnits = 3;

}
