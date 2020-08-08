using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class OverlayButtonEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	public GameObject overlayObject;
	
	public void OnPointerDown(PointerEventData eventData)
	{
		if (GetComponent<Button>().IsInteractable())
			overlayObject.SetActive(true);
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		overlayObject.SetActive(false);
	}
	
    void Awake()
    {
	    overlayObject.SetActive(false);
    }
}
