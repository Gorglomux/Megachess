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
    public Tween ResetCameraPosition(bool instant = false)
    {
        return cameraMovementTransform.transform.DOMove(initialCameraPosition, 0.2f).SetEase(Ease.OutQuint);


    }

    public Tween ZoomToPosition(Vector3 position, float strength = -1f, float duration = -2f )
    {
        if (duration <= 0)
        {
            duration = GlobalHelper.DEFAULT_CAMERA_ZOOM_DURATION;
        }
        if (strength <= 0)
        {
            strength = GlobalHelper.DEFAULT_CAMERA_ZOOM_STRENGTH;
        }
        print("Move to " + position);
        cam.DOOrthoSize(strength, duration *.8f).SetEase(Ease.OutQuint);
        return cameraMovementTransform.transform.DOMove(position + new Vector3(0, 0, -10), duration).SetEase(Ease.OutQuart);

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

    public Tween ShakeCamera(float speedMultiplier, float delay =-1f)
    {
        if (delay <= 0)
        {
            delay = GlobalHelper.DEFAULT_CAMERA_SHAKE_DURATION;
        }

        float value = speedMultiplier / 300f;
        return cam.DOShakePosition(delay, Vector3.one * value).SetEase(Ease.OutBounce);
    }

}
