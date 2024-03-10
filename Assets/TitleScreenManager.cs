using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TitleScreenManager : MonoBehaviour
{
    public Transform gridTransform;
    public GameObject prefabPlayerContainer;

    public List<PlayerContainer> playerContainers = new List<PlayerContainer>();

    public PlayerContainer hoveredPlayerContainer;
    public PlayerContainer selectedPlayerContainer;
    // Start is called before the first frame update 
    void Start()
    {
        FillClasses();
        GlobalHelper.getCamMovement().ShakeCamera(2);
        GlobalHelper.UI().SetBottomText("") ;
    }

    public void FillClasses()
    {
        List<PlayerData> classes = GlobalHelper.playerDataList.OrderBy(x=> x.difficulty).ToList();
        foreach (PlayerData playerData in classes)
        {
            GameObject go = GameObject.Instantiate(prefabPlayerContainer, gridTransform);
            PlayerContainer pc = go.GetComponent<PlayerContainer>();
            EventTrigger et = pc.GetComponent<EventTrigger>();
            // Add PointerEnter event listener
            EventTrigger.Entry entry2 = new EventTrigger.Entry();
            entry2.eventID = EventTriggerType.PointerClick;
            entry2.callback.AddListener((data) => OnSelectPlayerContainer((PointerEventData)data, pc));
            et.triggers.Add(entry2);

            //EventTrigger.Entry entryDeselect = new EventTrigger.Entry();
            //entryDeselect.eventID = EventTriggerType.Deselect;
            //entryDeselect.callback.AddListener((data) => OnDeselectPlayerContainer((PointerEventData)data));
            //et.triggers.Add(entryDeselect);

            EventTrigger.Entry entryHover = new EventTrigger.Entry();
            entryHover.eventID = EventTriggerType.PointerEnter;
            entryHover.callback.AddListener((data) => OnHoverEnter((PointerEventData)data,pc));
            et.triggers.Add(entryHover);

            EventTrigger.Entry entryExitHover = new EventTrigger.Entry();
            entryExitHover.eventID = EventTriggerType.PointerExit;
            entryExitHover.callback.AddListener((data) => OnHoverExit((PointerEventData)data));
            et.triggers.Add(entryExitHover);
            pc.AddUnits(playerData.startingUnits);
            pc.FillPlayer(playerData);
            playerContainers.Add(pc);
        }
    }

    public void OnValidatePlayerContainer()
    {
        //When clicking on the button in the bottom right ? 
        if(selectedPlayerContainer == null)
        {
            GlobalHelper.getCamMovement().ShakeCamera(2);
            GlobalHelper.UI().SetBottomText("Select a starting class to continue", 5);
        }
        else
        {
            GlobalHelper.GlobalVariables.player.LoadPlayerData(selectedPlayerContainer.playerData);
            GlobalHelper.getCamMovement().ShakeCamera(2);
            GlobalHelper.UI().HideTitleScreen();
            GlobalHelper.UI().HideHoverInfos();
            GlobalHelper.UI().SetBottomText("Come rescue the White King at the bottom of the Black king Lair!", 3);
            GlobalHelper.GetGameManager().StartGame();


        }
    }
    public void OnHoverEnter(PointerEventData data, PlayerContainer container)
    {
        if(selectedPlayerContainer == null)
        {
            hoveredPlayerContainer = container;

            GlobalHelper.UI().ShowHoverInfos(hoveredPlayerContainer);
        }
    }
    public void OnHoverExit(PointerEventData data)
    {
        hoveredPlayerContainer = null;
        if (selectedPlayerContainer == null)
        {
            GlobalHelper.UI().HideHoverInfos();
        }
    }
    public void OnSelectPlayerContainer(PointerEventData data, PlayerContainer container)
    {
        GlobalHelper.getCamMovement().ShakeCamera(0.5f, 0.2f);
        selectedPlayerContainer = container;
        GlobalHelper.UI().ShowHoverInfos(selectedPlayerContainer);
    }

    public void OnDeselectPlayerContainer(PointerEventData data)
    {
        selectedPlayerContainer = null;
        hoveredPlayerContainer = null;
        GlobalHelper.UI().HideHoverInfos();
    }

    public void OnDiveButtonHoverEnter()
    {
        GlobalHelper.getCamMovement().ShakeCamera(0.5f,0.2f);
        GlobalHelper.UI().SetBottomText("Are you ready to defeat the King?", 3);
    }
    public void OnDiveButtonHoverExit()
    {
        GlobalHelper.getCamMovement().ShakeCamera(0.5f, 0.2f);
        GlobalHelper.UI().SetBottomText("Need a bit more time?", 3);
    }
}
