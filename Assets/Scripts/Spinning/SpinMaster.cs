using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

[Serializable]
public struct JointData
{
    public float jointLength;
    public float jointWidth;
    public bool useTrail; 
}

[Serializable]
public class PatternData
{
    public float cycles = 1;
    public float time = 0;
    public Vector3[] frequencies;

    public float getTime()
    {
        return time > 0 ? time : cycles;
    }
}

public class SpinMaster : MonoBehaviour {

    public static SpinMaster instance;
    
    [Header("Joints")]
    public JointData[] jointData;
    JointControl[] joints;
    public GameObject jointPrefab;

    [Header("Pattern")]
    public Vector3[] startOffset;
    public List<PatternData> patterns;

    [Header("Settings")]
    public float speed;


    [Header("Time")]
    public bool isPlaying;
    [Range(0,1)]
    public float progression;
    public float totalTime;
    public float curTime;
    public float keepBeforeLooping;

    
    //debug
    public int curPattern;
    public float relPatternTime;


    [Header("UI")]
    public Color uiColor;
    public RectTransform blockContainer;
    public GameObject blockUIPrefab;
    public BlockEditorUI editor;

	public RectTransform timelineEditor;

    //public int patternIndex;


    private void Awake()
    {
        instance = this;

        joints = new JointControl[jointData.Length];
        
        Transform curParent = transform;
        for(int i=0;i<joints.Length;i++)
        {
            JointControl j = Instantiate(jointPrefab).GetComponent<JointControl>();
            j.transform.SetParent(curParent, false);
            j.order = i + 1;

            curParent = j.transform;
            joints[i] = j;
            
        }

        addPattern();
        
    }

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update() {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isPlaying) curTime = 0;
            isPlaying = !isPlaying;
        }

        if (isPlaying)
        {
            curTime += Time.deltaTime;
            if (curTime > totalTime + keepBeforeLooping) curTime = 0;
        } else
        {
            curTime = progression * totalTime;
        }

        totalTime = 0;
        foreach (PatternData p in patterns)
        {
            totalTime += p.getTime();
        }

        float curPatternStartTime = 0;
        for (int i = 0; i < patterns.Count; i++)
        {
            float relP01 = 0;
            
            if (curTime <= curPatternStartTime + patterns[i].getTime() && curTime >= curPatternStartTime)
            {
                curPattern = i;
                relPatternTime = curTime - curPatternStartTime;
                relP01 = relPatternTime / patterns[i].getTime();
            } else if (curTime > curPatternStartTime)
            { 
                 relP01 = 1;
            }

            curPatternStartTime += patterns[i].getTime();

            blockContainer.GetChild(i).GetComponent<PatternBlockUI>().setRelativeProgression(relP01);

        }


        for (int i=0;i<joints.Length;i++)
        {
            joints[i].setFromData(jointData[i]);
            if (i > 0) joints[i].transform.localPosition = Vector3.down * joints[i-1].jointLength;

            joints[i].setRotation(getJointRotationForTime(i,curTime));
        }
    }

    /*
    private void OnGUI()
    {
        GUI.color = Color.black;
        GUI.Label(new Rect(10, 10, 100, 20), "Num Patterns : " + patterns.Count);
        GUI.Label(new Rect(10, 30, 100, 20), "Total time : " + totalTime);
        GUI.Label(new Rect(10, 50, 200, 20), "Cur time : " + curTime);
    }
    */

    public Vector3 getJointRotationForTime(int jointIndex, float time)
    {
        if (jointIndex >= joints.Length) return Vector3.zero;
           
        Vector3 result = startOffset[jointIndex];
        if (patterns.Count == 0) return result;

        float curPatternStartTime = 0;
        foreach (PatternData p in patterns)
        {
            if (time <= curPatternStartTime + p.getTime())
            {
                float relTime = time - curPatternStartTime;
                result += getJointPositionForPatternAndRelativeTime(jointIndex, p, relTime);
                break;
            }else
            {
                result += getJointPositionForPatternAndRelativeTime(jointIndex, p, p.getTime());
                curPatternStartTime += p.getTime();
            }
        }

        return result;

    }

    public Vector3 getJointPositionForPatternAndRelativeTime(int jointIndex, PatternData pattern, float time)
    {
        if (jointIndex >= joints.Length) return Vector3.zero;
        time = Mathf.Clamp(time,0, pattern.getTime());

        Vector3 endRotation = pattern.frequencies[jointIndex] * pattern.cycles;
        float relTime = time / pattern.getTime();

        return Vector3.Lerp(Vector3.zero, endRotation, relTime);
    }

    //Pattern management
    public void clearPatterns()
    {
        patterns.Clear();
        updateUI();
    }


    public void addPattern(string data = "")
    {
        string[] dSplit = data.Split(new char[] { ',' });
        bool emptyData = data.Length == 0;

        PatternData p = new PatternData();

        if(!emptyData)
        {
            p.time = float.Parse(dSplit[0]);
            p.cycles = float.Parse(dSplit[1]);
        }

        p.frequencies = new Vector3[joints.Length];

        for(int i=0;i<3;i++)
        {
            p.frequencies[i] = emptyData?((i == 2)?Vector3.forward:Vector3.zero):new Vector3(float.Parse(dSplit[2+i*3]), float.Parse(dSplit[2+i*3+1]), float.Parse(dSplit[2+i*3+2]));
        }

        patterns.Add(p);
        updateUI();
    }

    public void removePattern(PatternData p)
    {
        patterns.Remove(p);
        updateUI();
    }

    public void updateUI()
    {
        for(int i=0;i<blockContainer.childCount;i++) Destroy(blockContainer.GetChild(i).gameObject);
        for(int i=0;i<patterns.Count;i++)
        {
            PatternBlockUI p = Instantiate(blockUIPrefab).GetComponent<PatternBlockUI>();
            p.GetComponent<RectTransform>().SetParent(blockContainer);
            p.spinMaster = this;
            p.editor = editor;
            p.setUIColor(uiColor);
            p.setPattern(patterns[i]);
        }
    }

    public void setProgression(float p)
    {
        progression = p;
    }

    public void setTime(float t)
    {
        progression = t / totalTime;
    }

    public void togglePlay()
    {
        isPlaying = !isPlaying;
    }

	public void updateStartOffsetFromUI()
	{
		for (int i = 0; i < 3; i++) 
		{
			float tx = 0, ty = 0, tz = 0;
			float.TryParse (timelineEditor.Find ("F" + i + "X").GetComponent<InputField> ().text, out tx);
			float.TryParse (timelineEditor.Find ("F" + i + "Y").GetComponent<InputField> ().text, out ty);
			float.TryParse (timelineEditor.Find ("F" + i + "Z").GetComponent<InputField> ().text, out tz);

			startOffset [i] = new Vector3 (tx, ty, tz);
		}
	}
}

