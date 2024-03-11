using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{

    bool initialized = false;

    public Slider MasterSlider;
    public Slider SoundEffectsSlider;
    public Slider MusicSlider;
    public Slider AnimationSpeedSlider;
    public Slider ScreenShakeSlider;

    // Start is called before the first frame update
    void Start()
    {
        MasterSlider.value = PlayerPrefs.GetFloat("MasterVolume", 0.6f) * MasterSlider.maxValue;
        MusicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.7f) * MasterSlider.maxValue;
        SoundEffectsSlider.value = PlayerPrefs.GetFloat("SoundVolume", 0.7f) * MasterSlider.maxValue;
        AnimationSpeedSlider.value = map(PlayerPrefs.GetFloat("AnimationSpeed", 1), GlobalHelper.GlobalVariables.gameInfos.minScreenShake, GlobalHelper.GlobalVariables.gameInfos.maxAnimationSpeed, AnimationSpeedSlider.minValue, AnimationSpeedSlider.maxValue);
        ScreenShakeSlider.value = map(PlayerPrefs.GetFloat("ScreenShake", 1), GlobalHelper.GlobalVariables.gameInfos.minScreenShake, GlobalHelper.GlobalVariables.gameInfos.maxScreenShake, AnimationSpeedSlider.minValue, AnimationSpeedSlider.maxValue);

        initialized = true;
    }




    public void OnSliderValueChanged(Slider slider)
    {
        if (!initialized)
        {
            return;
        }
        if (AudioManager.instance == null)
        {
            return;
        }
        //AudioManager.instance.PlaySound("SFX_MainButtonClick01", 1, 0.5f + slider.value / slider.maxValue);
        AudioManager.instance.changeVolume(slider.gameObject.name, slider.value / slider.maxValue);
    }


    public void OnAnimationSpeedChanged()
    {
        PlayerPrefs.SetFloat("AnimationSpeed", map(AnimationSpeedSlider.value, AnimationSpeedSlider.minValue, AnimationSpeedSlider.maxValue, GlobalHelper.GlobalVariables.gameInfos.minAnimationSpeed, GlobalHelper.GlobalVariables.gameInfos.maxAnimationSpeed));
        DOTween.timeScale = PlayerPrefs.GetFloat("AnimationSpeed");
    }


    public void OnScreenShakeAmountChanged()
    {
        PlayerPrefs.SetFloat("ScreenShake", map(ScreenShakeSlider.value, ScreenShakeSlider.minValue, ScreenShakeSlider.maxValue, GlobalHelper.GlobalVariables.gameInfos.minScreenShake, GlobalHelper.GlobalVariables.gameInfos.maxScreenShake));
        GlobalHelper.ScreenShakeMultiplier = PlayerPrefs.GetFloat("ScreenShake");
    }


    public void OnScreenShakeSliderRelease()
    {
        GlobalHelper.getCamMovement().ShakeCamera(2).SetUpdate(true);
    }

    public void OnBackToTitle()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void OnContinue()
    {
        GlobalHelper.UI().HidePauseMenu();
    }

    float map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }
}
