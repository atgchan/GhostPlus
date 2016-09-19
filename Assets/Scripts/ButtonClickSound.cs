using UnityEngine;
using System.Collections;

public class ButtonClickSound : MonoBehaviour {

    public AudioClip sound;

    AudioSource source { get { return GetComponent<AudioSource>(); } }

    void Start()
    {
        gameObject.AddComponent<AudioSource>();
        source.clip = sound;
        source.playOnAwake = false;
    }

    public void PlaySound()
    {
        source.PlayOneShot(sound);
    }
	
}
