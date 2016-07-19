using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System;

public class SYLanguage {

	static Dictionary<string, string> mDictionary = new Dictionary<string, string>();

	public static string Get (string key, params object[] args) {

		return GetValue(mDictionary, key, args);
	}

	public static bool IsContainsKey (string key) {
	
		return mDictionary.ContainsKey (key);
	}

	static string GetValue (Dictionary<string, string> dic, string key, params object[] args) {

		if (string.IsNullOrEmpty(key)) {
			//Debug.LogError("Key is Null");
			return "";
		}

		key = key.Trim ();
		string val = "";

		if ( dic.TryGetValue(key, out val) ) {
			if ( string.IsNullOrEmpty(val) )
				val = key;
		}
		else {
			val = key;
		}

		int m = GetCountOfChar(val, '{');
		if (m != args.Length) {
			Debug.LogError(string.Format("多语言参数不匹配 key={0}", key));
			return val;
		}

		return string.Format(val, args);

	}

	public static bool LoadData(TextAsset textAsset) {

		return LoadTextAsset(textAsset, mDictionary);
	}

	static bool LoadTextAsset (TextAsset textAsset, Dictionary<string, string> dic) {

		if (textAsset == null || dic == null)
			return false;

		char[] separator1 = new char[] { '\n' };
		char[] separator2 = new char[] { '=' };
		string[] sArr1 = textAsset.text.Split(separator1);

		dic.Clear();

		for (int i=0; i<sArr1.Length; i++) {

			string line = sArr1[i];

			string[] sArr2 = line.Split(separator2);
			if (sArr2.Length < 2)
				continue;

			string akey = sArr2[0].Trim();
			string avalue = sArr2[1].Trim();

			if (!dic.ContainsKey(akey))
				dic.Add(akey, avalue);
			else 
				Debug.LogError(string.Format("{0}存在相同的key={1}", textAsset.name, akey) );

		}

		return true;
	}

	static int GetCountOfChar (string str, char ch) {

		int m = 0;
		for (int i=0; i<str.Length; i++)
			if (ch == str[i])
				m ++;
		return m;

	}


}
