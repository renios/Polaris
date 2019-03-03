using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonClickSound : MonoBehaviour
{
    public SoundType type;
    void Start(){
        var button = GetComponent<Button>();
        if (button != null){
            button.onClick.AddListener(PlaySound);
        }
        else {
            Debug.LogWarning("NullButtonComponentException : No button component is on this object!");
        }
    }
    void PlaySound()
    {  
        Debug.Log("Button Pressed!");
        SoundManager.Play(type);
    }
}