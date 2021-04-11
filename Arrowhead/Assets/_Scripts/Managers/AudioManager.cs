using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;

    public static AudioManager Instance()
    {
        return instance;
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public AudioSource audioSource;
    public AudioClip[] audioClips;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        Play();
    }

    void Play()
    {
        if (!audioSource.isPlaying)
        {
            ChangeSong();
            audioSource.Play();
        }
    }

    void ChangeSong()
    {
        audioSource.clip = audioClips[RandomInt()];
    }

    int RandomInt()
    {
        return Random.Range(0, 3);
    }
}
