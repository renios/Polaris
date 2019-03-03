using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType{
	Default,
	BgmTitle, BgmMain,
	ClickImportant, ClickNormal, ClickDialogue
}
public class SoundManager : MonoBehaviour {

	static SoundManager _instance;
	static SoundManager Instance{
		get{
			if (_instance == null){
				_instance = GameObject.FindObjectOfType<SoundManager>();
				if (_instance == null){
					GameObject newInstance = new GameObject();
					newInstance.AddComponent<SoundManager>();
					newInstance.name = "SoundManager";
					_instance = newInstance.GetComponent<SoundManager>();
					DontDestroyOnLoad(newInstance);
				}
			}
			return _instance;
		}
	}

	Dictionary<string, AudioClip> soundDictionary = new Dictionary<string, AudioClip>();
	AudioSource musicPlayer;
	AudioSource soundPlayer;
	public void Awake(){
		musicPlayer = gameObject.AddComponent<AudioSource>();
		musicPlayer.loop = true;

		soundPlayer = gameObject.AddComponent<AudioSource>();
		soundPlayer.loop = false;
		
		var sounds = Resources.LoadAll("Sounds", typeof(AudioClip));
		foreach (var s in sounds){
			AudioClip clip = (AudioClip)s;
			string name = clip.name;
			soundDictionary.Add(name, clip);
		}
	}
	public static void Play(SoundType type){
		Debug.Log("Play : " + type.ToString());
		Instance.PlayMusic(type);
	}
	public void PlayMusic(SoundType type){
		AudioClip clip;
		if (soundDictionary.TryGetValue(type.ToString(), out clip)){
			if (clip == null){
				Debug.LogWarning("NullSoundTypeException : There is no given type of sound file! (" + type.ToString() + ")");
				return;
			}
			if (type.ToString().Contains("Bgm")){
				if (musicPlayer.clip != clip){
					musicPlayer.clip = clip;
					musicPlayer.Play();
				}
			}
			else {
				soundPlayer.clip = clip;
				soundPlayer.Play();
			}
		}
		else {
			Debug.LogWarning("NullSoundTypeException : There is no given type on SoundType enum! (" + type.ToString() + ")");
		}
	}
}
