using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gamaudi : MonoBehaviour
{	
	public static gamaudi Instance { get; private set; }
    [SerializeField] private AudioSource SFXSource;

    
	[Header("--------- Audio Clip -------")]
	public AudioClip correct;
	public AudioClip incorrect;
	public AudioClip cutString;
	public AudioClip death;
	

	public void PLaySFX(AudioClip clip)
	{
		SFXSource.PlayOneShot(clip);
	}
}
