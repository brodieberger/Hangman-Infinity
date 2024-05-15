using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    public AudioClip mainMenuSound;
    public AudioClip ScoreAttackSound;
    public AudioClip EndlessSound;

    //Audio for sfxSource
    public AudioClip correct;
	public AudioClip incorrect;
	public AudioClip cutString;
	public AudioClip death;
     private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  //Make this object persistance across scenes
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    public void PlayMainMenuMusic()
    {
        musicSource.clip = mainMenuSound;
        musicSource.Play();
    }


    public void PlayScoreAttackMusic()
    {
        musicSource.clip = ScoreAttackSound;
        musicSource.Play();
    }

    public void PlayEndlessMusic()
    {
        musicSource.clip = EndlessSound;
        musicSource.Play();
    }

    public void StopMusic(){
        musicSource.Stop();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
}
