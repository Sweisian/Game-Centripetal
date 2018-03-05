
using System;
using UnityEngine;
using UnityEngine.Audio;


public class AudioManager : MonoBehaviour {

    public Sound[] sounds;

   // public static AudioManager instance;

    void Awake()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;

            s.source.playOnAwake = true;
            s.source.rolloffMode = AudioRolloffMode.Linear;

            Debug.Log(s.source);
        }
    }

    void Start()
    {
        Play("coinCollect");
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("Sound: " + name + " not found!");
            return;
        }
        s.source.Play();
    }

}
