using System;
using System.Collections;
using System.Collections.Generic;
using TheGamerUrso.Core;
using UnityEngine;
using UnityEngine.Audio;


[Serializable]
public class AudioTrack
{
    public string Name;
    public AudioClip audioClip;
}

public class AudioManager : ServiceComponent<IAudioService>,IAudioService
{
    // public static AudioManager instance;
    [HideInInspector] public List<AudioTrack> SoundClips = new List<AudioTrack>();
    public List<AudioTrack> MusicClips = new List<AudioTrack>();

    public AudioSource source;
    public AudioSource BackgroundMusic;

    private static float musicVolume = 0.75f;
    private static float soundVolume = 0.75f;

    public AudioMixerGroup MusicMixerGroup;
    public AudioMixerGroup SFXMixerGroup;


    public Dictionary<string, AudioClip> ListOfSoundClips = new Dictionary<string, AudioClip>();

    public Dictionary<string, AudioClip> ListOfAudioClips = new Dictionary<string, AudioClip>();

    [HideInInspector] public bool fadeIn = false;
    [HideInInspector] public bool fadeOut = false;

    [HideInInspector] public bool crossfade = false;
    [HideInInspector] public float targetVolume;

    private IDataService dataService;

    private List<GameObject> SoundSFX;

    public bool MusicIsDone()
    {
        return !BackgroundMusic.isPlaying;
    }

    public float GetSoundVolume()
    {
        return soundVolume;
    }
    public float GetMusicVolume()
    {
        return musicVolume;
    }

    public void SetSoundVolume(float value)
    {
        soundVolume = value;
        SFXMixerGroup.audioMixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20);
    }
    public void SetMusicVolume(float value)
    {
        musicVolume = value;
        MusicMixerGroup.audioMixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);
    }

    protected override void Awake()
    {
        base.Awake();
        dataService = GameContext.Get<IDataService>();
    }

    private void Start()
    {
        try
        {
            foreach (AudioTrack item in MusicClips)
            {
                ListOfAudioClips.Add(item.Name, item.audioClip);
            }
        }
        catch (NullReferenceException e)
        {
            Debug.LogWarning(e.Message, gameObject);
        }

        try
        {
            foreach (AudioTrack item in SoundClips)
            {
                ListOfSoundClips.Add(item.Name, item.audioClip);
            }
        }
        catch (NullReferenceException e)
        {
            Debug.LogWarning(e.Message, gameObject);
        }


        PlayerData playerData = dataService.GetPlayerData();

        if (playerData == null)
        {
            SetMusicVolume(musicVolume);
            SetSoundVolume(soundVolume);
        }
        else
        {
            SetMusicVolume(playerData.MusicVolume);

            SetSoundVolume(playerData.SFXVolume);
        }

    }

    public bool PlayingMusic()
    {
        if (BackgroundMusic.isPlaying)
        {
            return true;
        }
        return false;
    }

    public void PlayRandomMusic(bool force = false)
    {
            PlayRandomSong(force);
    }

    public void PlayMusic(string IdTrack, bool loop = true)
    {
        PlayMusicById(IdTrack, loop);
    }

    public void PlaySound(AudioClip clip, bool usePitch = false, float minRange = .8f, float maxRange = 1.2f)
    {
        PlaySoundByClip(clip, usePitch, minRange, maxRange);
    }

    public void PlayMusicById(string IdTrack, bool loop = true)
    {
        AudioClip randomClip;
        if (ListOfAudioClips.TryGetValue(IdTrack, out randomClip))
        {
            if (BackgroundMusic.clip == null || (BackgroundMusic.clip != randomClip || !BackgroundMusic.isPlaying))
            {
                BackgroundMusic.loop = loop;
                BackgroundMusic.clip = randomClip;
                BackgroundMusic.Play();
            }
        }
    }

    public void PlayRandomSong(bool force = false)
    {
        if (!PlayingMusic() || force)
        {
            string idTrack = "Track";
            var number = UnityEngine.Random.Range(1, 11);

            AudioClip randomClip;
            if (ListOfAudioClips.TryGetValue(idTrack + number, out randomClip))
            {
                if (BackgroundMusic.clip != randomClip || !BackgroundMusic.isPlaying)
                {
                    BackgroundMusic.loop = true;
                    BackgroundMusic.clip = randomClip;
                    PlayMusic(randomClip);
                }
            }
        }
    }

    public void SetPitch(float pitch)
    {
        BackgroundMusic.pitch = pitch;
    }

    public void PlaySoundByClip(AudioClip clip, bool usePitch = false, float minRange = .8f, float maxRange = 1.2f)
    {
        if (usePitch)
        {
            float prevPitch = source.pitch;
            source.pitch = UnityEngine.Random.Range(minRange, maxRange);
        }
        source.PlayOneShot(clip);

    }

    public void PlaySoundByClip(AudioSource source, string IdTrack, int mixGroupIndex = 0, bool usePitch = false, float minRange = .8f, float maxRange = 1.2f)
    {
        AudioClip audioClip;
        if (ListOfSoundClips.TryGetValue(IdTrack, out audioClip))
        {
            if (source == null)
            {
                return;
            }

            if (usePitch)
            {
                float prevPitch = source.pitch;
                source.pitch = UnityEngine.Random.Range(minRange, maxRange);
            }
            else
            {
                source.pitch = 1;
            }

            source.PlayOneShot(audioClip);
        }
    }

    public void PlayPrevious()
    {
        int currentTrack = 0;
        for (int i = 0; i < MusicClips.Count; i++)
        {
            if (MusicClips[i].audioClip == BackgroundMusic.clip)
            {
                currentTrack = i;
                break;
            }
        }

        currentTrack--;

        if (currentTrack < 0)
        {
            currentTrack = MusicClips.Count - 1;
        }

        PlayMusic(MusicClips[currentTrack].audioClip);
    }

    public void PlayNext()
    {
        int currentTrack = 0;
        for (int i = 0; i < MusicClips.Count; i++)
        {
            if (MusicClips[i].audioClip == BackgroundMusic.clip)
            {
                currentTrack = i;
                break;
            }
        }

        currentTrack++;

        if (currentTrack >= MusicClips.Count)
        {
            currentTrack = 0;
        }

        PlayMusic(MusicClips[currentTrack].audioClip);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (!crossfade)
        {
            crossfade = true;
            StartCoroutine(Crossfade(clip));
        }
    }

    public void StopMusic()
    {
        if (!crossfade)
        {
            StartCoroutine(FadeOut());
        }
    }

    void Update()
    {
        if (crossfade)
        {
            if (fadeIn)
            {
                targetVolume += Time.deltaTime;
            }

            if (fadeOut)
            {
                targetVolume -= Time.deltaTime;
            }
            BackgroundMusic.volume = targetVolume;
        }
    }

    IEnumerator FadeOut()
    {
        crossfade = true;
        if (BackgroundMusic.isPlaying)
        {
            while (targetVolume > 0)
            {
                fadeOut = true;
                yield return null;
            }
            BackgroundMusic.Stop();
        }

        crossfade = false;
    }

    IEnumerator Crossfade(AudioClip clip)
    {
        targetVolume = 1;

        if (BackgroundMusic.isPlaying)
        {
            while (targetVolume > 0)
            {
                fadeOut = true;
                yield return null;
            }
            BackgroundMusic.Stop();
        }

        fadeOut = false;

        yield return new WaitForSeconds(.1f);

        BackgroundMusic.clip = clip;
        BackgroundMusic.Play();



        targetVolume = 0;
        while (targetVolume <= 1)
        {
            fadeIn = true;
            yield return null;
        }


        fadeIn = false;
        crossfade = false;
    }
}