using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointControl : MonoBehaviour {

    public int order;

    [Header("Spin")]
    public Vector3 frequency;

    [Header("Trail")]
    public bool useTrail;
    public TrailRenderer trail;

    public float jointLength;
    public float jointWidth;

    Transform cube;


    public void setFromData(JointData data)
    {
        jointLength = data.jointLength;
        jointWidth = data.jointWidth;
        useTrail = data.useTrail;
    }


    // Use this for initialization
    void Start () {
        cube = transform.Find("Cube");
	}

    void Update()
    {
        trail.enabled = useTrail;

        cube.transform.localPosition = new Vector3(0, -jointLength/2,0);
        cube.transform.localScale = new Vector3(jointWidth,jointWidth, jointLength);
        trail.transform.localPosition = new Vector3(0, -jointLength, 0);
    }

    // Update is called once per frame
    void FixedUpdate () {
        //Debug.Log("Speed : " + SpinMaster.instance.speed);
        transform.Rotate(frequency * 360 * Time.fixedDeltaTime * SpinMaster.instance.speed);
	}

    public void setRotation(Vector3 newRotation)
    {
        transform.localRotation = Quaternion.Euler(newRotation*360);
    }
    
}
