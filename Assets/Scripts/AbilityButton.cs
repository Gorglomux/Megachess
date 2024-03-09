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
    public bool shouldLookForTarget = false;
    public object target;
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

        StartCoroutine(FindTarget());

    }
    public IEnumerator FindTarget()
    {
        yield return null;
        if (shouldLookForTarget)
        {
            ISelectable selectable = GlobalHelper.GlobalVariables.gameInfos.selected;
            print(selectable is ReserveContainer);
            if (GlobalHelper.GlobalVariables.gameInfos.selected != null)
            {
                if (selectable is ReserveContainer && ability.abilityData.targettingType == TARGETTING_TYPE.RESERVE)
                {
                    print("Found reserve targetr");
                    ability.ExecuteAbility(GlobalHelper.GlobalVariables.gameInfos.selected);
                    selectable.onDeselect(Vector3.negativeInfinity);
                }
                else if (selectable is Unit && ability.abilityData.targettingType == TARGETTING_TYPE.UNIT)
                {

                    print("Found unit targetr");
                    ability.ExecuteAbility(GlobalHelper.GlobalVariables.gameInfos.selected);

                }

            }

            print("Looking for target;");

            shouldLookForTarget = false;
            ToggleBlink(false);

            GlobalHelper.UI().SetBottomText("", 0);
        }
    }
    Tween incorrectTween;
    public void UseAbility()
    {
        if (shouldLookForTarget)
        {
            shouldLookForTarget = false;
            ToggleBlink(false);
            return;
        }
        if(GlobalHelper.GlobalVariables.gameManager.currentState is FightState && GlobalHelper.isPlayerTurn())
        {
            if (ability.isCharged() && !shouldLookForTarget)
            {
            
                //Enter targetting mode 
                switch (ability.abilityData.targettingType)
                {
                    case TARGETTING_TYPE.RESERVE:
                        shouldLookForTarget = true;
                        GlobalHelper.UI().SetBottomText("Select a unit in the reserve",999);
                        ToggleBlink(true);

                        break;
                    case TARGETTING_TYPE.UNIT:
                        shouldLookForTarget = true;
                        GlobalHelper.UI().SetBottomText("Select a unit in the room.",999);
                        ToggleBlink(true);

                        break;
                    case TARGETTING_TYPE.GLOBAL:
                        ability.ExecuteAbility(null);

                        break;
                }


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


    public bool blinking = false;
    public Coroutine tweenBlink;
    public void ToggleBlink(bool enabled, float blinkspeed = 1)
    {
        if (blinking && !enabled)
        {
            blinking = false;
            button.image.color = new Color(1, 1, 1, 1);
            StopCoroutine(tweenBlink);
        }
        else if (!blinking && enabled)
        {
            tweenBlink = StartCoroutine(corBlink(0.17f / blinkspeed,0.44f/ blinkspeed));
            //tweenBlink = spriteRenderer.DOColor(new Color(1, 1, 1, 0.7f), 0.5f).SetEase(Ease.Flash,20,0).SetLoops(-1, LoopType.Restart);
            blinking = true;
        }

    }

    public IEnumerator corBlink(float outDelay, float inDelay)
    {

        while (true)
        {
            button.image.color = new Color(1, 1, 1, 1);
            yield return new WaitForSeconds(inDelay);
            button.image.color = new Color(1, 1, 1, 0);
            yield return new WaitForSeconds(outDelay);

        }


    }
     

}
