using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPickerElement : MonoBehaviour
{
	public GameObject truePanel, falsePanel;
	public Image image;
	public Text text;
	public Toggle toggle;
	public CharacterPicker picker;
	
	[HideInInspector] public bool picked;

	int index;

	public void Set(KeyValuePair<int, CharacterData> dataSet)
	{
		index = dataSet.Key;
		if(dataSet.Value.Observed)
	    {
		    truePanel.SetActive(true);
		    falsePanel.SetActive(false);
		    image.sprite = Resources.Load<Sprite>("Characters/" + dataSet.Value.InternalName + "/image_albumbutton");
		    text.text = dataSet.Value.Name;
	    }
	    else
	    {
		    truePanel.SetActive(false);
		    falsePanel.SetActive(true);
		    text.text = "???";
	    }
    }

    public void Pick()
    {
	    if (picker.waiting)
	    {
		    if (!picked)
		    {
			    var succeed = picker.TryAddCharacter(index);
			    if (succeed)
				    picked = true;
			    else
				    toggle.isOn = false;
		    }
		    else
		    {
			    picker.RemoveCharacter(index);
			    picked = false;
		    }
	    }
    }
}