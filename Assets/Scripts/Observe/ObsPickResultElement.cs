using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Observe
{
	public class ObsPickResultElement : MonoBehaviour
	{
		public Text constelName, charName;
		public Image charThumb;
		public Slider favBar;
		public Text favText, favLevel;
		public GameObject favIncreased;
		public Text favIncrement;

		public void SetAndApplyData(int charKey, int deltaFav)
		{
			var charData = Variables.Characters[charKey];

			charData.Favority += deltaFav;
			if (!charData.Observed)
			{
				charData.Observed = true;
				if (Variables.LobbyCharList.Count < Variables.GetStoreValue(2))
					Variables.LobbyCharList.Add(charKey);
			}

			charName.text = charData.Name;
			constelName.text = Variables.Constels[charData.MainConstel].Name;
			charThumb.sprite = Resources.Load<Sprite>("Characters/" + charData.InternalName + "/image_obspopup");
			
			int progress, required;
			int level = GameManager.Instance.CheckFavority(charKey, out progress, out required);
			favLevel.text = level.ToString();
			favText.text = required < 0 ? "FULL" : progress.ToString() + "/" + required.ToString();
			favBar.maxValue = required < 0 ? 1 : required;
			favBar.value = required < 0 ? 1 : progress;
			favIncrement.text = "(+" +  deltaFav.ToString() + ")";
			if (progress - deltaFav < 0)
				favIncreased.SetActive(true);

			if (level > charData.StoryUnlocked)
				charData.StoryUnlocked = level;
		}
	}
}