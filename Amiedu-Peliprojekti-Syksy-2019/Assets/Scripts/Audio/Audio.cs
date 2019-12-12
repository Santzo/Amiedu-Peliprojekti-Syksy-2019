using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Unity.Collections;
public class Audio : MonoBehaviour
{
    [System.Serializable]
    public class Clip
    {
        public string tag;
        public AudioClip clip;
        [HideInInspector] public AudioSource source;
        public bool music;

        public Clip()
        {
            tag = "New Audio Clip";
        }
    }

    public AudioMixer master;
    public static Audio ad;
    public List<Clip> clip;

    public Dictionary<string, Clip> audioLib = new Dictionary<string, Clip>();

    [HideInInspector]
    public float MasterVolume
    {
        get
        {
            master.GetFloat("masterVol", out float _volume);
            return _volume;
        }
        set
        {
            master.GetFloat("masterVol", out float _volume);
            if (value == -30) value = -80;
            if (value == _volume) return;
            master.SetFloat("masterVol", value);
        }
    }
    [HideInInspector]
    public float MusicVolume
    {
        get
        {
            master.GetFloat("musicVol", out float _volume);

            return _volume;
        }
        set
        {
            master.GetFloat("musicVol", out float _volume);
            if (value == -30) value = -80;
            if (value == _volume) return;

            master.SetFloat("musicVol", value);
        }
    }
    [HideInInspector]
    public float SoundVolume
    {
        get
        {
            master.GetFloat("soundVol", out float _volume);
            return _volume;
        }
        set
        {
            master.GetFloat("soundVol", out float _volume);
            if (value == -30) value = -80;
            if (value == _volume) return;

            master.SetFloat("soundVol", value);
        }
    }


    private void Awake()
    {
        if (ad == null)
        {
            ad = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        master = Resources.Load<AudioMixer>("Audio/Master");

        foreach (Clip clp in clip)
        {
            AudioSource ads = gameObject.AddComponent<AudioSource>();
            ads.outputAudioMixerGroup = clp.music ? master.FindMatchingGroups("Music")[0] : master.FindMatchingGroups("Sound")[0];
            clp.source = ads;
            audioLib.Add(clp.tag, clp);
        }


    }
    private void Start()
    {
        MasterVolume = PlayerPrefs.GetFloat("MasterVolume", 0f);
        MusicVolume = PlayerPrefs.GetFloat("MusicVolume", 0f);
        SoundVolume = PlayerPrefs.GetFloat("SoundVolume", 0f);
    }
    public static AudioSource AddAudioSource()
    {
        var source = ad.gameObject.AddComponent<AudioSource>();
        return source;
    }

    public void RemoveAudioSource(AudioSource source)
    {
        Destroy(source);
    }

    public static void PlaySound(string clip, float volume = 1f, float pitch = 1f)
    {
        ad.audioLib[clip].source.pitch = pitch;
        ad.audioLib[clip].source.PlayOneShot(ad.audioLib[clip].clip, volume);
    }

    public static void PlayOnLoop(string clip, float volume = 1f, float pitch = 1f)
    {
        ad.audioLib[clip].source.clip = ad.audioLib[clip].clip;
        ad.audioLib[clip].source.pitch = pitch;
        ad.audioLib[clip].source.volume = volume;
        ad.audioLib[clip].source.loop = true;
        ad.audioLib[clip].source.Play();
    }

    public static void StopMusic(string clip)
    {
        if (ad.audioLib[clip].source.isPlaying)
            ad.audioLib[clip].source.Stop();
    }

    public static bool IsPlaying(string clip)
    {
        return ad.audioLib[clip].source.isPlaying;
    }

    public static void PauseMusic(string clip)
    {
        if (ad.audioLib[clip].source.isPlaying) ad.audioLib[clip].source.Pause();
        else ad.audioLib[clip].source.UnPause();
    }

    public static void SetVolume(string clip, float volume)
    {
        volume = Mathf.Clamp(volume, 0f, 1f);
        ad.audioLib[clip].source.volume = volume;
    }

    public static float CurrentVolume(string clip)
    {
        return ad.audioLib[clip].source.volume;
    }
    public static void VolumeFade(string clip, float from, float to, float time, bool pause = false, bool stop = false)
    {
        ad.StartCoroutine(_VolumeFade(clip, from, to, time, pause, stop));
    }

    private static IEnumerator _VolumeFade(string clip, float from, float to, float time, bool pause = false, bool stop = false)
    {
        ad.audioLib[clip].source.volume = from;
        if (!ad.audioLib[clip].source.isPlaying) ad.audioLib[clip].source.Play();
        from = Mathf.Clamp(from, 0f, 1f);
        to = Mathf.Clamp(to, 0f, 1f);

        float start = Time.realtimeSinceStartup;
        float end = Time.realtimeSinceStartup + time;
        float add = (to - from) / time;
        yield return null;
        while (end > start)
        {
            float _add = Time.realtimeSinceStartup - start;
            start = Time.realtimeSinceStartup;
            ad.audioLib[clip].source.volume += _add * add;
            yield return null;
        }
        if (pause) PauseMusic(clip);
        if (!pause && stop) StopMusic(clip);
        yield return null;
    }

    public static void ResetAllMusicAndSound()
    {
        foreach (KeyValuePair<string, Audio.Clip> _clp in ad.audioLib)
        {
            StopMusic(_clp.Key);
        }
    }


    public void VolumeReset(string _name, float volume)
    {

        volume = Mathf.Clamp(volume, 0f, 100f);
        float _volume = -20f + (volume * 0.2f);
        if (volume < 50f) _volume += -49f + (volume);
        if (volume == 0f) _volume = -80f;
        if (_name == "Master")
            master.SetFloat("masterVol", _volume);
        if (_name == "Sound")
            master.SetFloat("soundVol", _volume);
        if (_name == "Music")
            master.SetFloat("musicVol", _volume);
    }
    public float ReturnVol(string volume)
    {
        float _volume;
        master.GetFloat(volume, out _volume);
        Debug.Log(_volume);
        return _volume;
    }
    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", MasterVolume);
        PlayerPrefs.SetFloat("MusicVolume", MusicVolume);
        PlayerPrefs.SetFloat("SoundVolume", SoundVolume);
    }
}
public static class AudioExtensions
{
    public static void PlaySound(this AudioSource original, AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        if (clip == null) return;
        original.pitch = pitch;
        original.PlayOneShot(clip, volume);
    }

}



