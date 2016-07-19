using UnityEngine;
using System.Collections;

public class SYUILanguage : MonoBehaviour {
	
	public bool isNeed = true;

	void Awake () {

		if (isNeed)
			OnLocalization();
	}


	void OnLocalization () {

		UnityEngine.UI.Text lbl = GetComponent<UnityEngine.UI.Text>();
		if (lbl != null) {

			string key = lbl.text;
			if (!string.IsNullOrEmpty(key)) {
				lbl.text = SYLanguage.Get(key);
			}
		}

	}




}
