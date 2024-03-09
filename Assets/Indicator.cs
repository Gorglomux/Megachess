using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum INDICATOR_STATE { INACTIVE, MOVE, TARGETED,ATTACK ,PLACE_FROM_RESERVE}
public class Indicator : MonoBehaviour
{
    public Sprite spriteMove;
    public Sprite spriteTargeted;
    public Sprite spriteAttack;
    public Sprite spritePlaceFromReserve;
    public INDICATOR_STATE currentState;

    public SpriteRenderer spriteRenderer;
    public SpriteRenderer spriteRendererActive;

    public Color colorAlly;
    public Color colorEnemy;

    public bool isEnemy;
    public bool isSelected = false;
    private Coroutine desactivateCoroutine;
    // Start is called before the first frame update
    void Start()
    {
    }


    public void SetState(INDICATOR_STATE state, bool isEnemy, bool isActive )
    {
        if(currentState != state)
        {
            if(state != INDICATOR_STATE.INACTIVE)
            {
                if(desactivateCoroutine != null)
                {
                    StopCoroutine(desactivateCoroutine);
                }
            }
            currentState = state;
            switch (currentState)
            {
                case INDICATOR_STATE.MOVE:
                    spriteRenderer.sprite = spriteMove;
                    break;
                case INDICATOR_STATE.TARGETED:
                    spriteRenderer.sprite = spriteTargeted;
                    break;
                case INDICATOR_STATE.INACTIVE:
                    desactivateCoroutine = StartCoroutine(corDesactivate());
                    break;
                case INDICATOR_STATE.ATTACK:
                    spriteRenderer.sprite = spriteAttack;
                    break;
                case INDICATOR_STATE.PLACE_FROM_RESERVE:
                    spriteRenderer.sprite = spritePlaceFromReserve;
                    break;
            }
        }
        isSelected = isActive;
        spriteRendererActive.gameObject.SetActive(isSelected);


        if(state != INDICATOR_STATE.INACTIVE)
        {
            if (isEnemy)
            {
                spriteRendererActive.color = colorEnemy;
                spriteRenderer.color = colorEnemy;
            }
            else
            {
                spriteRendererActive.color = colorAlly;
                spriteRenderer.color = colorAlly;
            }
        }
        
    }

    public IEnumerator corDesactivate()
    {
        yield return null;
        yield return null;
        yield return new WaitForSeconds(0.05f);
        gameObject.SetActive(false);
        desactivateCoroutine = null;
    }
}
