using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TeseScript : MonoBehaviour {

	public Text label;

	// Use this for initialization
	void Start () {
	
		label.text = SYLanguage.Get ("超牛逼多语言自动提取 {0}", "哈哈");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
