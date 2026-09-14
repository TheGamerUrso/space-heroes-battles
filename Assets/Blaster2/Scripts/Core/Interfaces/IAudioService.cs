using System.Security.Cryptography;
using UnityEngine;

public interface IAudioService
{
    void PlayRandomMusic(bool force = false);
    void PlayMusic(string IdTrack, bool loop = true);
    void PlayMusicById(string IdTrack, bool loop = true);

    void SetPitch(float pitch);
    void SetMusicVolume(float volume);
    void SetSoundVolume(float volume);

    void PlaySound(AudioClip clip, bool usePitch = false, float minRange = .8f, float maxRange = 1.2f);
    void PlaySoundByClip(AudioClip clip, bool usePitch = false, float minRange = .8f, float maxRange = 1.2f);
    void PlaySoundByClip(AudioSource source, string IdTrack, int mixGroupIndex = 0, bool usePitch = false, float minRange = .8f, float maxRange = 1.2f);
}
