using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CaptureText : MonoBehaviour
{
    public TextMeshProUGUI capturedText;

    public Sequence AnimateCaptureText(float direction = 1)
    {
        capturedText.alpha = 1;
        Sequence s = DOTween.Sequence();
        s.Append(transform.DOMoveY(transform.position.y + 0.05f * direction, 0.5f).SetEase(Ease.OutQuart));
        s.Append(DOTween.To(() => capturedText.alpha, x => capturedText.alpha = x, 0, 1));
        return s;
    }

    public void setText(string text)
    {
        capturedText.text = text;
    }
}
