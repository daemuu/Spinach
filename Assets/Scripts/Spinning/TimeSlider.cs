using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeSlider : MonoBehaviour {

    public SpinMaster[] spinMasters;
    public Text timeLabel;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame 
	void Update () {
		
	}

    public void progressionUpdate(float value)
    {
        float maxTime = 0;
        foreach(SpinMaster s in spinMasters)
        {
            maxTime = Mathf.Max(maxTime, s.totalTime);
        }

        float targetTime = value * maxTime;
        foreach (SpinMaster s in spinMasters)
        {
            s.setTime(targetTime);
        }

        timeLabel.text = targetTime.ToString();
    }
}
