using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Album
{
    public class AlbumPage : MonoBehaviour
    {
        public RectTransform ElementParent;
        public GameObject ElementTemplate;

        public void CreateElement(int charIndex)
        {
            var newObj = Instantiate(ElementTemplate);
            var newElement = newObj.GetComponent<AlbumPageElement>();
            newElement.CharIndex = charIndex;

            var data = Variables.Characters[charIndex];
            if (data.Observed)
                newElement.Set(data);
            else
            {
                newObj.GetComponent<UnityEngine.UI.Button>().interactable = false;
                newElement.MaskObject.SetActive(true);
            }
            newObj.transform.SetParent(ElementParent);
            newObj.transform.localScale = Vector3.one;
            newObj.transform.localPosition = Vector3.zero;
            newObj.SetActive(true);
        }
    }
}