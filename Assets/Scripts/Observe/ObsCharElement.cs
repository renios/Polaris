using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Observe
{
    public class ObsCharElement : MonoBehaviour
    {
        public GameObject known, unknown;
        public Image image;
        public Slider progressBar;
        public Text favLevel, favProgress;
        public Text charName;

        public void Set(int charKey)
        {
            var chara = Variables.Characters[charKey];
            if(!chara.Observed)
            {
                known.SetActive(false);
                unknown.SetActive(true);
            }
            else
            {
                known.SetActive(true);
                unknown.SetActive(false);

                int fav_lev, fav_cur, fav_req;
                image.sprite = Resources.Load<Sprite>("Characters/" + chara.InternalName + "/image_obs");
                fav_lev = GameManager.Instance.CheckFavority(charKey, out fav_cur, out fav_req);
                progressBar.maxValue = fav_req;
                progressBar.value = fav_cur;
                favLevel.text = fav_lev.ToString();
                favProgress.text = fav_cur.ToString() + "/" + fav_req.ToString();
                if (fav_req < 0)
                    favProgress.text = "FULL";
                charName.text = chara.Name;
            }
        }
    }
}