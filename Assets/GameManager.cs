using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public Room roomToDebug;

    public RoomView currentRoom;

    public Queue<Room> roomQueue = new Queue<Room>();

    private Coroutine paletteCoroutine;

    public GameObject roomRoot;
    // Start is called before the first frame update
    void Start()
    {
        if (roomToDebug != null)
        {
            print("Debug Initiative launched");
            LoadRoom(roomToDebug);
        }
        else
        {
            LoadArea(GlobalHelper.areaList[0]);

        }
    }

    public void LoadArea(Area a)
    {
        print("Loading area " + a.name);
        a.GetRooms().ForEach(r => roomQueue.Enqueue(r));

        if (paletteCoroutine != null)
        {
            StopCoroutine(paletteCoroutine);
        }
        paletteCoroutine = StartCoroutine(LoadPalette(a.paletteIndex, a.colorText));
        LoadRoom(roomQueue.Dequeue());
    }
    public IEnumerator LoadPalette(int paletteIndex, Color textColor)
    {
        Material globalMaterial = GlobalHelper.GlobalVariables.paletteMaterial; ;
        GlobalHelper.GlobalVariables.areaMaterial = new Material(globalMaterial);
        GlobalHelper.GlobalVariables.areaMaterial.SetFloat("_PaletteIndex", paletteIndex);
        GlobalHelper.GlobalVariables.areaMaterial.SetFloat("_IndexToLerpTo", 0);
        print("Setting Palette ! ");
        //Set the palette to the room
        globalMaterial.SetFloat("_IndexToLerpTo", paletteIndex);
        float lerpValue = 0f;
        Tween t = DOTween.To(() => lerpValue, x => lerpValue = x, 1, 3);
        t.onUpdate += () =>
        {
            globalMaterial.SetFloat("_Lerp",lerpValue);
        };
        yield return t.WaitForCompletion();

        globalMaterial.SetFloat("_Lerp", 0);
        globalMaterial.SetFloat("_IndexToLerpTo", 0);
        globalMaterial.SetFloat("_PaletteIndex", paletteIndex);
        //Set the material to the room and the floor in the room prefab
        paletteCoroutine = null;
    }
    public void LoadRoom(Room r)
    {
        currentRoom = GameObject.Instantiate(r.roomPrefab,roomRoot.transform).GetComponent<RoomView>();
        currentRoom.LoadRoom();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
