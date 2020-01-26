using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]
public class AudioTrack
{
    public string Name;
    public AudioClip audioClip;
}

public class AudioManager : Singleton<AudioManager>
{
    // public static AudioManager instance;
    public AudioTrack[] SoundCLips;

    public AudioTrack[] MusicClips;
    public AudioSource MusicSource;
    public AudioSource SFXSource;
    public AudioSource[] SfxSources;

    public AudioMixerGroup MusicMixerGroup;
    public AudioMixerGroup SFXMixerGroup;

    public AudioMixerGroup[] audioMixerGroups;

    public Dictionary<string, AudioClip> ListOfSoundClips = new Dictionary<string, AudioClip>();

    public Dictionary<string, AudioClip> ListOfAudioClips = new Dictionary<string, AudioClip>();


    public void SetSoundVolume(float value)
    {
        SFXMixerGroup.audioMixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20);
    }
    public void SetMusicVolume(float value)
    {
        audioMixerGroups[0].audioMixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);
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
            foreach (AudioTrack item in SoundCLips)
            {
                ListOfSoundClips.Add(item.Name, item.audioClip);
            }
        }
        catch (NullReferenceException e)
        {
            Debug.LogWarning(e.Message, gameObject);
        }
    }

    public bool DonePlaying()
    {
        return !MusicSource.isPlaying;
    }

    public bool PlayingMusic()
    {
        if (MusicSource.isPlaying)
        {
            return true;
        }
        return false;
    }

    public static void PlaySound(string IdTrack, int mixGroupIndex, bool repeat = false)
    {
        AudioManager.Instance.PlaySoundById(IdTrack, mixGroupIndex);
    }

    public static void PlayRandomMusic(bool force = false)
    {
        AudioManager.Instance.PlayRandomSong(force);
    }

    public static void SetMusic(string IdTrack, bool loop = true)
    {

        AudioManager.Instance.SetMusicByName(IdTrack, loop);
    }

    public void SetMusicByName(string IdTrack, bool loop = true)
    {
        AudioClip randomClip;
        if (ListOfAudioClips.TryGetValue(IdTrack, out randomClip))
        {
            if (MusicSource.clip == null || (MusicSource.clip != randomClip || !MusicSource.isPlaying))
            {
                MusicSource.loop = loop;
                MusicSource.clip = randomClip;
                MusicSource.Play();
            }
        }
    }

    public void PlayRandomSong(bool force = false)
    {
        if (!PlayingMusic() || force)
        {
            string idTrack = "Music";
            var number = UnityEngine.Random.Range(1, 8);

            AudioClip randomClip;
            if (ListOfAudioClips.TryGetValue(idTrack + number, out randomClip))
            {
                if (MusicSource.clip != randomClip || !MusicSource.isPlaying)
                {
                    MusicSource.loop = false;
                    MusicSource.clip = randomClip;
                    MusicSource.Play();
                }
            }
        }
    }

    public bool MusicIsDone()
    {
        return !MusicSource.isPlaying;
    }
    public void StopSoundEffect()
    {
        SFXSource.Stop();
    }
    public void PlaySoundById(string IdTrack, int mixGroupIndex, bool repeat = false)
    {
        AudioMixerGroup previous = SFXSource.outputAudioMixerGroup;
        AudioClip audioClip;
        if (ListOfSoundClips.TryGetValue(IdTrack, out audioClip))
        {
            SFXSource.outputAudioMixerGroup = audioMixerGroups[mixGroupIndex];
            if (repeat)
            {
                SFXSource.clip = audioClip;
                SFXSource.loop = true;
                SFXSource.Play();
            }
            else
            {
                SFXSource.PlayOneShot(audioClip);
            }

        }
        SFXSource.outputAudioMixerGroup = previous;
    }

    public void PlaySound(string IdTrack)
    {
        AudioClip audioClip;
        if (ListOfSoundClips.TryGetValue(IdTrack, out audioClip))
        {
            SFXSource.PlayOneShot(audioClip);
        }
    }


    public static void PlaySound(AudioSource source, AudioClip audioClip, int mixGroupIndex = 0, bool usePitch = false)
    {
        AudioManager.Instance.PlaySoundByClip(source, audioClip, mixGroupIndex, usePitch);
    }

    public void PlaySoundByClip(AudioSource source, AudioClip audioClip, int mixGroupIndex = 0, bool usePitch = false)
    {
        if (source != null)
        {
            if (usePitch)
            {
                PlaySoundWithRandomPitch(source, audioClip, 2, 3);
            }
            else
            {
                source.PlayOneShot(audioClip);
            }
        }
        else {
            if (SFXSource != null)
            {
                if (mixGroupIndex == 0)
                {
                    if (usePitch)
                    {
                        PlaySoundWithRandomPitch(SfxSources[mixGroupIndex],audioClip, 2, 3);
                    }
                    else
                    {
                        SfxSources[mixGroupIndex].pitch = 1;
                        SfxSources[mixGroupIndex].PlayOneShot(audioClip);
                    }

                }
                else if (mixGroupIndex == 1)
                {
                    SfxSources[mixGroupIndex].PlayOneShot(audioClip);
                }
                else if (mixGroupIndex == 2)
                {
                    SfxSources[mixGroupIndex].PlayOneShot(audioClip);
                }
                else if (mixGroupIndex == 3)
                {
                    PlaySoundWithRandomPitch(SfxSources[mixGroupIndex],audioClip,.8f, 1);
                }
                else if (mixGroupIndex == 4)
                {
                    PlaySoundWithRandomPitch(SfxSources[mixGroupIndex],audioClip, .2f, 0.9f);
                }
                else
                {
                    SfxSources[mixGroupIndex].PlayOneShot(audioClip);
                }
            }
        }
    }

    public void PlaySoundWithRandomPitch(AudioSource source,AudioClip audioClip, float minRange, float maxRange)
    {
        float prevPitch = source.pitch;

        source.pitch = UnityEngine.Random.Range(minRange, maxRange);

        source.PlayOneShot(audioClip);

        //SfxSources[mixGroupIndex].pitch = prevPitch;
    }
}