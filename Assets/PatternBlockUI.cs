using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PatternBlockUI : MonoBehaviour {


    public int blockIndex;

    Text titleText;
    Text infoText;

    public PatternData pattern;
    public BlockEditorUI editor;
    public SpinMaster spinMaster;
    public RectTransform progressionBG;

    private void Awake()
    {
        titleText = transform.Find("Title").GetComponent<Text>();
        infoText = transform.Find("Infos").GetComponent<Text>();
        progressionBG = transform.Find("ProgressionBG").GetComponent<RectTransform>();
    }

    public void setPattern(PatternData p)
    {
        pattern = p;
        updateData();
    }

    public void remove()
    {
        if (editor.currentBlock == this) editor.editBlock(null);
        spinMaster.removePattern(pattern);
    }

    public void edit()
    {
        editor.editBlock(this);
    }

    public void updateData()
    {
        blockIndex = spinMaster.patterns.IndexOf(pattern);
        titleText.text = "Block " + blockIndex;

        string info = "Time : " + pattern.time + "\nCycles : " + pattern.cycles + "\nFrequencies :";
        foreach(Vector3 f in pattern.frequencies)
        {
            info += "\n" + f.x + " / " + f.y + " / " + f.z;
        }

        infoText.text = info;
    }

    public void setEditing(bool value)
    {
        GetComponent<Image>().color = value ? Color.yellow : Color.white;
    }

    public void setRelativeProgression(float value)
    {
        progressionBG.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value * 100);
    }

    public void setUIColor(Color c)
    {
        progressionBG.GetComponent<Image>().color = c;
    }
}
