using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AbilityButton : MonoBehaviour
{
    public BaseAbility ability;

    public Button button;
    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>(); 
        RefreshAbility();
    }
    public bool selected = false;

    public void RefreshAbility()
    {
        ability = GlobalHelper.GlobalVariables.player.ability;
    }

    public bool canSelect()
    {
        BaseAbility ability = GlobalHelper.GlobalVariables.player.ability;
        return ability.isCharged();
    }

    public void OnPointerEnter(BaseEventData data)
    {

        GlobalHelper.UI().ShowHoverInfos(ability);
    }

    public void OnPointerExit(BaseEventData data)
    {
        GlobalHelper.UI().HideHoverInfos();

    }

    public void OnBeginDrag(BaseEventData data)
    {
        if (canSelect())
        {
            EventSystem.current.SetSelectedGameObject(gameObject);

        }
        //Dequeue unit 

        //Put it as the current selected ? 
    }

    public void OnEndDrag(BaseEventData data)
    {
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void OnPointerDown(BaseEventData data)
    {
        if (canSelect())
        {
            EventSystem.current.SetSelectedGameObject(gameObject);

        }
    }
    public void OnSelect(BaseEventData data)
    {

        GlobalHelper.UI().ShowHoverInfos(ability);
    }

    public void OnDeselect(BaseEventData data)
    {

        GlobalHelper.UI().HideHoverInfos();
    }
    public void UseAbility()
    {
        print("Kapow");

    }
}
