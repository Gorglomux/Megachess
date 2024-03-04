using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum INDICATOR_STATE { ENEMY_ACTIVE,ALLY_INACTIVE, ALLY_ACTIVE, ALLY_TARGETED, INACTIVE,PLACE_FROM_RESERVE}
public class Indicator : MonoBehaviour
{
    public Sprite spriteEnemy;
    public Sprite spriteAllyInactive;
    public Sprite spriteAllyActive;
    public Sprite spriteAllyTargeted;
    public Sprite spritePlaceFromReserve;
    public INDICATOR_STATE currentState;

    public SpriteRenderer spriteRenderer;

    private Coroutine desactivateCoroutine;
    // Start is called before the first frame update
    void Start()
    {
    }


    public void SetState(INDICATOR_STATE state )
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
                case INDICATOR_STATE.ENEMY_ACTIVE:
                    spriteRenderer.sprite = spriteEnemy;
                    break;
                case INDICATOR_STATE.ALLY_ACTIVE:
                    spriteRenderer.sprite = spriteAllyActive;
                    break;
                case INDICATOR_STATE.ALLY_INACTIVE:
                    spriteRenderer.sprite = spriteAllyInactive;
                    break;
                case INDICATOR_STATE.ALLY_TARGETED:
                    spriteRenderer.sprite = spriteAllyTargeted;
                    break;
                case INDICATOR_STATE.INACTIVE:
                    desactivateCoroutine = StartCoroutine(corDesactivate());
                    break;
                case INDICATOR_STATE.PLACE_FROM_RESERVE:
                    spriteRenderer.sprite = spritePlaceFromReserve;
                    break;
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
