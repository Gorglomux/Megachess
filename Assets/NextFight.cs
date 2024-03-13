using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NextFight : MonoBehaviour
{
    public Transform star;
    public Transform Target;
    public TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = star.position;
    }

    public Tween StartAnimate(string prefix)
    {
        text.text = prefix;
        return transform.DOMove(Target.position, 0.5f).SetEase(Ease.OutBack,2);
        
    }

    public Tween StopAnimate()
    {

        return transform.DOMove(star.position, 0.5f).SetEase(Ease.OutBack,2);
    }
}
