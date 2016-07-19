using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

public class SYLanguageTool {


	static List<string>Localization = new List<string>();
	static string staticWriteText = "";

	//UIPrefab文件夹目录
	static string UIPrefabPath = "";
	//脚本的文件夹目录
	static string[] ScriptPath;
	//多语言文件
	static string LocalizationPath = "";
	//导出的中文KEY
	static string OutPath = "";
	//
	static Regex regex;


	static string childPath = "";
	static bool isPrefabChanged = false;


	static void Set () {
	
		UIPrefabPath = Application.dataPath + "/SYLanguage/Examples/Prefabs";
		ScriptPath = new string[]{ string.Format("{0}/SYLanguage/Examples/Scripts", Application.dataPath) };

		LocalizationPath = Application.dataPath +"/Resources/LanguageFiles/localization.txt";
		OutPath = Application.dataPath +"/Resources/LanguageFiles/out.txt";

		regex = new Regex("SYLanguage.Get*.\\(\".*?\"");

		Localization.Clear();
		staticWriteText ="";
	}


	[MenuItem("Tools/提取中文key &w")]
	static void PickUpChinese() {

		Set ();

		if (File.Exists (LocalizationPath)) {

			string assetPath = LocalizationPath.Substring (LocalizationPath.IndexOf ("Assets/"));
			TextAsset textAsset = AssetDatabase.LoadAssetAtPath (assetPath, typeof(TextAsset)) as TextAsset;

			if (!SYLanguage.LoadData (textAsset))
				Debug.LogError ("多语言文件读取失败");
		}
		else {
		
			using(StreamWriter writer = new StreamWriter(LocalizationPath, false, Encoding.UTF8))
			{
				writer.Write("");
			}
		}

		Export();
	}

	[MenuItem("Tools/检查中文key &c")]
	static void CheckChinese() {

		Set ();

		if (File.Exists(LocalizationPath)) {

			string assetPath =  LocalizationPath.Substring(LocalizationPath.IndexOf("Assets/"));
			TextAsset textAsset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(TextAsset)) as TextAsset;

			if (!SYLanguage.LoadData(textAsset) )
				Debug.LogError("多语言文件读取失败");
		}
		else {

			using(StreamWriter writer = new StreamWriter(LocalizationPath, false, Encoding.UTF8))
			{
				writer.Write("");
			}
		}

		Export(true);
	}


	static void Export(bool isCheck = false)
	{
		//提取Prefab上的中文
		staticWriteText +="----------------Prefab----------------------\n";
		LoadDiectoryPrefab(new DirectoryInfo(UIPrefabPath), isCheck);
		
		//提取CS中的中文
		staticWriteText +="----------------Script----------------------\n";
		foreach (string path in ScriptPath)
			LoadDiectoryCS(new DirectoryInfo(path), isCheck);

		//最终把提取的中文生成出来
		WriteToFile(OutPath, staticWriteText);
	}


	//递归所有UI Prefab
	static  void  LoadDiectoryPrefab(DirectoryInfo dictoryInfo, bool isCheck)
	{
		if(!dictoryInfo.Exists)   return;
		FileInfo[] fileInfos = dictoryInfo.GetFiles("*.prefab", SearchOption.AllDirectories);
		foreach (FileInfo files in fileInfos)
		{
			string path = files.FullName;
			//staticWriteText += string.Format("----------------{0}----------------------\n", path);
			string assetPath =  path.Substring(path.IndexOf("Assets/"));
			GameObject prefab = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
			GameObject instance = GameObject.Instantiate(prefab) as GameObject;

			isPrefabChanged = false;
			childPath = instance.transform.name;

			SearchPrefabString(instance.transform, isCheck);

			if (isPrefabChanged) {
				PrefabUtility.ReplacePrefab(instance, prefab);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}

			GameObject.DestroyImmediate(instance);
		}
	}

	//提取Prefab上的中文
	static void SearchPrefabString(Transform root, bool isCheck = false)
	{

		foreach(Transform chind in root)
		{
			SYUILanguage l = chind.GetComponent<SYUILanguage>();
			UnityEngine.UI.Text label = chind.GetComponent<UnityEngine.UI.Text>();

			if( label != null )
			{
				string text = label.text.Trim();

				if (l != null) {

					if (l.isNeed) {
						if (isCheck)
							Add (text, childPath);
						else
							Add (text);
					}
				}
				else {

					Regex reg = new Regex(".*[\u4E00-\u9FA5]+.*");

					if(reg.IsMatch(text))
					{
						isPrefabChanged = true;
						chind.gameObject.AddComponent<SYUILanguage>();
						if (isCheck)
							Add (text, childPath);
						else
							Add (text);
					}
				}

			}

			if(chind.childCount >0)
				SearchPrefabString(chind, isCheck);
		}
	}



	//递归所有C#代码
	static  void  LoadDiectoryCS(DirectoryInfo dictoryInfo, bool isCheck = false)
	{

		if(!dictoryInfo.Exists)   
			return;

		FileInfo[] fileInfos = dictoryInfo.GetFiles("*.cs", SearchOption.AllDirectories);
		foreach (FileInfo files in fileInfos)
		{
			string path = files.FullName;
			childPath = files.Name;
			//staticWriteText += string.Format("----------------{0}----------------------\n", path);
			string assetPath =  path.Substring(path.IndexOf("Assets/"));
			TextAsset textAsset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(TextAsset)) as TextAsset;
			string text = textAsset.text;

			MatchCollection mc = regex.Matches(text);

			foreach(Match m in mc)
			{
				string format = m.Value;

				Regex reg = new Regex("\".*?\"");
				string key = reg.Match(format).Value.Replace("\"", "");

				if (isCheck)
					Add (key, childPath);
				else
					Add (key);

			}


		}
	}

	static void Add (string text, string pathName = "") {

		if ( string.IsNullOrEmpty(text) )
			return;

		text = text.Trim();

		if ( !SYLanguage.IsContainsKey(text) && !Localization.Contains(text) ) {

			Localization.Add(text);		
			staticWriteText += string.Format("{0} = {1}\n",  text, pathName);
		}

	}

	static void WriteToFile (string path, string text) {

		if (System.IO.File.Exists (path)) 
		{
			File.Delete (path);
		}
		using(StreamWriter writer = new StreamWriter(path, false, Encoding.UTF8))
		{
			writer.Write(text);
		}
		AssetDatabase.Refresh();
	}



}
