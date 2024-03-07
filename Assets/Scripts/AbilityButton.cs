using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AbilityButton : MonoBehaviour
{

    private BaseAbility _ability;
    public TextMeshProUGUI ButtonText;
    public Vector3 startPosition;
    public BaseAbility ability { get
        {
            if (_ability == null)
            {
                RefreshAbility();
            }
            return _ability;
        }
        set
        {
            _ability = value;
        }
    }

    public Button button;
    // Start is called before the first frame update
    void Start()
    {
        GlobalHelper.GlobalVariables.player.OnAbilityChange += RefreshAbility;
        startPosition = transform.position;
        button = GetComponent<Button>(); 
    }
    public bool selected = false;

    public void RefreshAbility()
    {
        _ability = GlobalHelper.GlobalVariables.player.ability;
        ButtonText.text = _ability.abilityData.abilityName.ToUpper();
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

        GlobalHelper.UI().HideHoverInfos();
        GlobalHelper.UI().ShowHoverInfos(ability);
    }

    public void OnDeselect(BaseEventData data)
    {

        GlobalHelper.UI().HideHoverInfos();
    }
    Tween incorrectTween;
    public void UseAbility()
    {
        if(GlobalHelper.GlobalVariables.gameManager.currentState is FightState && GlobalHelper.isPlayerTurn())
        {
            
            if (ability.isCharged())
            {
                ability.ExecuteAbility(null);
            }
            else
            {
                IncorrectAnimation();
            }
        }
        else if(GlobalHelper.GlobalVariables.gameManager.currentState is FightState && !GlobalHelper.isPlayerTurn())
        {

            GlobalHelper.UI().SetBottomText("Can only use abilities during your turn!", 3);
            IncorrectAnimation();
        }
        else
        {
            GlobalHelper.UI().SetBottomText("Can only use abilities while fighting !",3);
            IncorrectAnimation();
        }

    }
    public void IncorrectAnimation()
    {

        if (incorrectTween != null)
        {
            incorrectTween = null;
        }
        incorrectTween = transform.DOShakePosition(0.8f, new Vector3(8f, 0, 0), 8).SetEase(Ease.OutBounce);
        incorrectTween.onComplete += () =>
        {
            transform.DOMove(startPosition, 0.2f);
        };
    }
}
