using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureTextManager : MonoBehaviour
{
    public GameObject captureTextPrefab;

    public CaptureText[]  CaptureTexts;

    public int pooledCaptureTextCount = 10;
    public int currentCaptureTextCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        print("Start pooling");
        CaptureTexts = new CaptureText[pooledCaptureTextCount];
        for (int i = 0; i < pooledCaptureTextCount; i++)
        {
            CaptureTexts[i] = GameObject.Instantiate(captureTextPrefab, this.transform).GetComponent<CaptureText>();
            CaptureTexts[i].gameObject.SetActive(false);
        }
        print("Pooling done !");
    }

    public CaptureText getNext()
    {
        currentCaptureTextCount = (currentCaptureTextCount + 1) % pooledCaptureTextCount;
        CaptureText captureText = CaptureTexts[currentCaptureTextCount];

        return CaptureTexts[currentCaptureTextCount];
    }

    public void CaptureAtPosition(Vector3 position)
    {
        CaptureText text = getNext();
        text.gameObject.SetActive(true);
        text.transform.position = position;
        text.capturedText.text = "Captured";
        text.AnimateCaptureText(-1).onComplete += () =>
        {
            DisableText(text);
        };
    }

    public void DisplayAtPosition(Vector3 position, string t)
    {
        CaptureText text = getNext();
        text.gameObject.SetActive(true);
        text.transform.position = position;
        text.setText(t);
        text.AnimateCaptureText().onComplete += () =>
        {
            DisableText(text);
        };
    }

    public void DisableText(CaptureText text)
    {
        text.gameObject.SetActive(false);
    }
}
