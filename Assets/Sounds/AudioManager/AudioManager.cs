using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public  List<AudioClip> sfxList;
    public  List<AudioClip> musicList;

    public AudioSource mainMusicAudioSource;
    public AudioSource fightMusicAudioSource;

    public AudioMixerSnapshot inFight;
    public AudioMixerSnapshot inElevatorOrGameOver;
    
    public static AudioManager instance;

    private float masterVolume = 1;
    private float soundEffectsVolume = 1;
    private float musicVolume = 1;

    public AudioSource soundEffectsAudioSource;
    public AudioSource ambianceAudioSource;

    public int pooledSourcesCount = 10;
    int currentSourceIndex = 0;
    public List<AudioSource> pooledSoundEffectAudioSources;
    public float savedMainMusicTime = 0f;


    public List<AudioClip> fightMusics;
    // Start is called before the first frame update
    void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this);


        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 0.6f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
        soundEffectsVolume= PlayerPrefs.GetFloat("SoundEffectVolume", 0.5f);
        UpdateSoundValues();
        //PlayMainMusic("music_long_dark");
        PlayAmbiance("ambiance_winds");
        for (int i = 0; i < pooledSourcesCount; i++)
        {
            AudioSource audioSource = soundEffectsAudioSource.gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
            pooledSoundEffectAudioSources.Add(audioSource);
        }

    }

    public void SetInFight()
    {
        inFight.TransitionTo(1f);
    }

    public void SetInElevator()
    {
        StopAmbiance();
        inElevatorOrGameOver.TransitionTo(1f);
    }
    public void SetBeforeFight()
    {
        PlayAmbiance("ambiance_winds");
    }

    public void changeVolume(string affected,  float audioValue)
    {
        switch (affected)
        {
            case "Master":
                masterVolume = audioValue;
                PlayerPrefs.SetFloat("MasterVolume", audioValue);

                break;
            case "Sound Effects":
                soundEffectsVolume  = audioValue;
                PlayerPrefs.SetFloat("SoundVolume", audioValue);
                break;
            case "Music":
                musicVolume  = audioValue;
                PlayerPrefs.SetFloat("MusicVolume", audioValue);
                break;
        }
        UpdateSoundValues();
    }
    public void UpdateSoundValues()
    {
        //PASSER PAR DES AUDIO MIXERS A LA PLACE
        //musicAudioSource.volume = masterVolume * musicVolume;
        //soundEffectsAudioSource.volume = masterVolume * soundEffectsVolume;
        //foreach (AudioSource audioSource in pooledSoundEffectAudioSources)
        //{
        //    audioSource.volume = masterVolume * soundEffectsVolume;
        //}
    }
    public AudioSource PlaySound(string soundName, float volumeScale = 1, float pitch = 1)
    {
        AudioSource asource = null;
        //if (GameManager.instance != null && GameManager.instance.muted)
        //{
        //    return;
        //}
        AudioClip sfx = sfxList.Where((AudioClip x) => x.name.ToUpper() == soundName.ToUpper()).FirstOrDefault();
        if (sfx != null)
        {
            pooledSoundEffectAudioSources[currentSourceIndex].pitch = pitch;
            pooledSoundEffectAudioSources[currentSourceIndex].PlayOneShot(sfx, volumeScale);
            asource = pooledSoundEffectAudioSources[currentSourceIndex];
        }
        else
        {
            Debug.LogError("Invalid audio clip name for " + soundName);
        }
        currentSourceIndex = (currentSourceIndex + 1)%pooledSourcesCount;
        return asource;
    }
    public void PlayFightMusic(string musicName = "")
    {
        AudioClip music = null;
        if(musicName != "")
        {
            music = musicList.Where((AudioClip x) => x.name.ToUpper() == musicName.ToUpper()).First();
        }
        else
        {
            music = fightMusics[GlobalHelper.rand.Next(fightMusics.Count)];
        }
        
        //if (GameManager.instance != null && GameManager.instance.muted)
        //{
        //    return;
        //}
        if (music != null)
        {
            savedMainMusicTime = mainMusicAudioSource.time;
            StartCoroutine(crossFadeMusic(mainMusicAudioSource,fightMusicAudioSource, music));
        }
        else
        {
            Debug.LogError("Invalid audio clip name for " + musicName);
        }
    }
    public void PlayMainMusic()
    {
        if (mainMusicAudioSource.isPlaying)
        {
            return;
        }
        string musicName = "music_long_dark";
        //if (GameManager.instance != null && GameManager.instance.muted)
        //{
        //    return;
        //}
        AudioClip music = musicList.Where((AudioClip x) => x.name.ToUpper() == musicName.ToUpper()).First();
        if (music != null)
        {
            StartCoroutine(crossFadeMusic(fightMusicAudioSource,mainMusicAudioSource, music));
            mainMusicAudioSource.time = savedMainMusicTime;
        }
        else
        {
            Debug.LogError("Invalid audio clip name for " + musicName);
        }
    }
    public IEnumerator crossFadeMusic(AudioSource source1, AudioSource source2, AudioClip clip)
    {
        source2.clip = clip;
        StartCoroutine(FadeOutMusic(source1));
        yield return StartCoroutine(FadeInMusic(source2));

    }
    public IEnumerator FadeOutMusic(AudioSource asource)
    {
        float t = 0;
        while(asource.volume > 0.05f)
        {
            t += Time.fixedUnscaledDeltaTime;
            yield return new WaitForFixedUpdate();
            asource.volume = Mathf.Lerp(1, 0, t);
        }

        asource.volume = 0f;
        asource.Stop();
    }
    public IEnumerator FadeInMusic(AudioSource asource)
    {
        asource.Play();

        float t = 0;
        while (asource.volume < 0.95f)
        {
            t += Time.fixedUnscaledDeltaTime;
            yield return new WaitForFixedUpdate();
            asource.volume = Mathf.Lerp(0, 1, t);
        }
        asource.volume = 1f;
        
    }

    public void StopAmbiance()
    {
        StartCoroutine(FadeOutMusic(ambianceAudioSource));
    }
    public void PlayAmbiance(string ambianceName)
    {
        AudioClip ambiance = sfxList.Where((AudioClip x) => x.name.ToUpper() == ambianceName.ToUpper()).First();
        if (ambiance != null)
        {
            StartCoroutine(FadeInMusic(ambianceAudioSource));
            ambianceAudioSource.clip = ambiance;
            ambianceAudioSource.Play();
        }
        else
        {
            Debug.LogError("Invalid audio clip name for " + ambianceName);
        }
    }

    public void StopMusic()
    {
        mainMusicAudioSource.Stop();
        fightMusicAudioSource.Stop();
    }
}
