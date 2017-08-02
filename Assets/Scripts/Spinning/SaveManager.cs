using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class SaveData
{
    public List<PatternSaveData> ps;
}

[Serializable]
public class PatternSaveData
{
    public List<PatternData> patterns;
}

public class SaveManager : MonoBehaviour
{

    public SpinMaster[] spinMasters;
	public InputField fileNameInput;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void save()
    {
        SaveData data = new SaveData();
        data.ps = new List<PatternSaveData>();
        for (int i = 0; i < spinMasters.Length; i++)
        {
            PatternSaveData pp = new PatternSaveData();
            pp.patterns = spinMasters[i].patterns;
            data.ps.Add(pp);
        }
        string s = JsonUtility.ToJson(data);
        Debug.Log(s);

#if UNITY_EDITOR
        string path = EditorUtility.SaveFilePanel(
                "Save sequence",
                "",
                "sequence.spin",
                "spin");
#else
		string dir =  Directory.GetCurrentDirectory()+"/sequences"; 
		if(!Directory.Exists(dir)) Directory.CreateDirectory(dir);
		string path = dir+"/"+fileNameInput.text+".spin";
#endif

        if (path.Length != 0)
        {
            File.WriteAllText(path, s);
        }


    }

    public void load()
    {
#if UNITY_EDITOR
        string path = EditorUtility.OpenFilePanel(
                "Open sequence",
                "",
                "spin");
#else
		string dir =  Directory.GetCurrentDirectory()+"/sequences"; 
		if(!Directory.Exists(dir)) Directory.CreateDirectory(dir);
		string path = dir+"/"+fileNameInput.text+".spin";
#endif
        if (path.Length != 0)
        {
            string data = File.ReadAllText(path);

            SaveData sd = JsonUtility.FromJson<SaveData>(data);
            for(int i=0;i<spinMasters.Length;i++)
            {
                spinMasters[i].clearPatterns();
                spinMasters[i].patterns = sd.ps[i].patterns;
                spinMasters[i].updateUI();
            }
        }
    }
}
