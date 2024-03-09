using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraMovementManager : MonoBehaviour
{
    public GameObject cameraMovementTransform;
    public Camera cam;
    public Volume v;


    public Vector3 initialCameraPosition;
    public float initialOrthoSize;

    public void Start()
    {

        initialCameraPosition = cam.transform.position;
        initialOrthoSize = cam.orthographicSize;
    }
    private void Update()
    {
        if(tweenActive.Count == 0 && cam.transform.position != initialCameraPosition)
        {
            camToZero();
        }
    }
    public Tween camToZero()
    {
        Tween t = cameraMovementTransform.transform.DOMove(initialCameraPosition, 0.2f).SetEase(Ease.OutQuint);

        tweenActive.Add(t);
        t.onComplete += () =>
        {
            cam.transform.position = initialCameraPosition;
            tweenActive.Remove(t);
        };
        return t;
    }

    public Tween ResetCameraPosition(bool instant = false)
    {
        Tween t = cameraMovementTransform.transform.DOMove(initialCameraPosition, 0.2f).SetEase(Ease.OutQuint);

        tweenActive.Add(t);
        t.onComplete += () =>
        {
            tweenActive.Remove(t);
        };
        return t;


    }
    Tween zoomTween = null;
    public Tween ZoomToPosition(Vector3 position, float strength = -1f, float duration = -2f )
    {
        if(zoomTween != null)
        {
            return transform.DOMove(transform.position, 0);
        }
        if (duration <= 0)
        {
            duration = GlobalHelper.DEFAULT_CAMERA_ZOOM_DURATION;
        }
        if (strength <= 0)
        {
            strength = GlobalHelper.DEFAULT_CAMERA_ZOOM_STRENGTH;
        }

        cameraMovementTransform.transform.DOScale(new Vector3(strength, strength,1), duration *.8f).SetEase(Ease.OutQuint);
        zoomTween = cameraMovementTransform.transform.DOMove(position + new Vector3(0, 0, -10), duration).SetEase(Ease.OutQuart);
        zoomTween.onComplete += () =>
        {
            zoomTween = null;
        };
        tweenActive.Add(zoomTween);
        zoomTween.onComplete += () =>
        {
            tweenActive.Remove(zoomTween);
        };

        return zoomTween;

    }
    public void ResetZoomPosition()
    {
        cameraMovementTransform.transform.DOScale(new Vector3(1, 1, 1), 0.5f).SetEase(Ease.OutQuint);
    }
    public void FlashCamera(float strength = -1f, float duration = -2f)
    {
        if (duration <= 0)
        {
            duration = GlobalHelper.DEFAULT_CAMERA_FLASH_DURATION;
        }
        if (strength <= 0)
        {
            strength = GlobalHelper.DEFAULT_CAMERA_FLASH_BRIGHTNESS;
        }

        ColorAdjustments colorAdjustment; 
        v.profile.TryGet<ColorAdjustments>(out colorAdjustment);
        DOTween.To(() => colorAdjustment.colorFilter.value, x => colorAdjustment.colorFilter.value = x, Color.white * strength, duration).onComplete += () =>
        {
            DOTween.To(() => colorAdjustment.colorFilter.value, x => colorAdjustment.colorFilter.value = x, Color.white, duration);
        };
        DOTween.To(() => colorAdjustment.saturation.value, x => colorAdjustment.saturation.value = x, -100, duration).onComplete += () =>
        {
            DOTween.To(() => colorAdjustment.saturation.value, x => colorAdjustment.saturation.value = x, 0, duration);
        };

    }
    List<Tween> tweenActive = new List<Tween>();
    public Tween ShakeCamera(float speedMultiplier, float delay =-1f)
    {
        if (delay <= 0)
        {
            delay = GlobalHelper.DEFAULT_CAMERA_SHAKE_DURATION;
        }

        float value = speedMultiplier / 300f;
        Tween t = cam.DOShakePosition(delay*0.8f, Vector3.one * value).SetEase(Ease.OutBounce);
        tweenActive.Add(t);
        t.onComplete += () =>
        {
            tweenActive.Remove(t);
        };
        return t;
    }

}
