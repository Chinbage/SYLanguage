using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;

public class TestScene : MonoBehaviour {

	public GameObject hang;
	public GameObject prefab;

	void Start () {

		string assetPath = "LanguageFiles/localization";
		TextAsset textAsset = Resources.Load (assetPath) as TextAsset;

		if (!SYLanguage.LoadData (textAsset))
			Debug.LogError ("多语言文件读取失败");
		

		GameObject go = GameObject.Instantiate(prefab) as GameObject;
		Transform t = go.transform;
		t.parent = hang.transform;
		t.localPosition = Vector3.zero;
		t.localRotation = Quaternion.identity;
		t.localScale = Vector3.one;
		go.layer = hang.layer;

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
