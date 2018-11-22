using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour {

    public AudioClip bgm; //regular bgm
    public AudioClip bgmWumpus; //battle music during wumpus fight
    public AudioClip helloWorld; //hello world
    public AudioClip goodbyeWorld; //goodbye world
    public AudioClip intro; //intro song
	public AudioClip winMusic; //plays during credits

	//Reference to audio clips for sfx
	public AudioClip hurt;
	public AudioClip pickup;
	public AudioClip ozzyRun;
	public AudioClip shoot;
    public AudioClip portal;

    private AudioSource talkingAudio; //audio for "hello world" and "goodbye world"
    private AudioSource bgmAudio; //bgm audio
    private AudioSource monologueAudio; //audio for ozzy's monologue
	private AudioSource sfxAudio; //sfx audio
    private AudioSource clapAudio; //audio for clapping sfx only

    public void Init() {
		//find all audiosources
		talkingAudio = GetComponent<AudioSource>();
        bgmAudio = GameObject.Find("BGM").GetComponent<AudioSource>();
        monologueAudio = GameObject.Find("Monologue").GetComponent<AudioSource>();
		sfxAudio = GameObject.Find("SFX").GetComponent<AudioSource>();

        clapAudio = GameObject.Find("ClapAudio").GetComponent<AudioSource>();
    }

	//plays sfx depending on sound name
	public void playSound(string sound) {
		if (sound.Equals ("hurt")) {
			sfxAudio.clip = hurt;
		}
		if (sound.Equals ("ozzyRun")) {
			sfxAudio.clip = ozzyRun;
		}
		if (sound.Equals ("pickup")) {
			sfxAudio.clip = pickup;
		}
		if (sound.Equals ("shoot")) {
			sfxAudio.clip = shoot;
		}
        if (sound.Equals("portal")) {
            sfxAudio.clip = portal;
        }
		sfxAudio.Play ();
	}

    public void playClap() {
        clapAudio.Play();
    }

	//plays BGM
    public void playBGM() {
        bgmAudio.loop = true;
        bgmAudio.clip = bgm;
        bgmAudio.Play();
    }

    //resets sfxaudio to original
    private void resetSettings() {
		talkingAudio.loop = false;
		talkingAudio.bypassEffects = true;
		talkingAudio.bypassListenerEffects = true;
		talkingAudio.bypassReverbZones = true;
		talkingAudio.volume = 1f;
    }

    //play hello world sound
    public void playHello() {
        resetSettings();
		talkingAudio.clip = helloWorld;
		talkingAudio.Play();
    }

    //play goodbye world sound
    public void playGoodbye() {
        resetSettings();
		talkingAudio.loop = false;
		talkingAudio.clip = goodbyeWorld;
		talkingAudio.Play();
    }

    //change osborne's monologue volume
    public void changeMonologueVolume(float volume) {
        monologueAudio.volume = volume;
    }

    //play the bgm wumpus during the final battle
    public void playWumpus() {
        bgmAudio.clip = bgmWumpus;
        bgmAudio.Play();
    }

	//play winmusic, and when that's done, return to menu screen
	public void playWinMusic() {
		bgmAudio.loop = true;
		sfxAudio.enabled = false;
		StartCoroutine(playWinMusicEnum());
	}

	//play winning music
	//Waits for song to end before returning to main menu
	IEnumerator playWinMusicEnum() {
		bgmAudio.clip = winMusic;
		bgmAudio.Play();
		yield return new WaitForSeconds(bgmAudio.clip.length);
		SceneManager.LoadScene (0);
	}
}
