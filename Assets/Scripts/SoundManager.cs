using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


//Unity Inspector를 통해서 받아오는 SoundType 변수가 다른 코드에 존재하므로,
//새로운 enum 값을 추가할 땐 종류가 아닌 추가 순서에 따라 값을 적어둬야 함. - 19.03.21 SMZ
public enum SoundType{
	Default,
	BgmTitle, BgmMain,
	ClickImportant, ClickNormal, ClickDialogue,
	BgmDark, AlbumPage, GachaResult, Stardust
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
    public static void Play(SoundType type)
    {
        Debug.Log("Play : " + type.ToString());
        Instance.PlaySound(type);
    }
    public static void Play(string BgmKey)
    {
        Debug.Log("Play : " + BgmKey);
        Instance.PlaySound(BgmKey);
    }
    public static void FadeMusicVolume(float targetVolume, float duration)
	{
		Instance.musicPlayer.DOFade(targetVolume, duration);
	}
	public void PlaySound(SoundType type){
		AudioClip clip;
		if (soundDictionary.TryGetValue(type.ToString(), out clip)){
			if (clip == null){
				Debug.LogWarning("NullSoundTypeException : There is no given type of sound file! (" + type.ToString() + ")");
				return;
			}
			if (type.ToString().Contains("Bgm")){
				if (musicPlayer.clip != clip){
					musicPlayer.clip = clip;
					musicPlayer.volume = 1;
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
    public void PlaySound(string BgmKey)
    {
        AudioClip clip;
        if (soundDictionary.TryGetValue(BgmKey, out clip))
        {
            if (clip == null)
            {
                Debug.LogWarning("NullSoundTypeException : There is no given type of sound file! (" + BgmKey+ ")");
                return;
            }
            if (BgmKey.Contains("Bgm"))
            {
                if (musicPlayer.clip != clip)
                {
                    musicPlayer.clip = clip;
                    musicPlayer.volume = 1;
                    musicPlayer.Play();
                }
            }
            else
            {
                soundPlayer.clip = clip;
                soundPlayer.Play();
            }
        }
        else
        {
            Debug.LogWarning("NullSoundTypeException : There is no given type on SoundType enum! (" + BgmKey + ")");
        }
    }
}
