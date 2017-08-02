using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Config : MonoBehaviour {

    public Canvas canvas;

	// Use this for initialization
	void Start () {
#if !UNITY_EDITOR && UNITY_WEBGL
         WebGLInput.captureAllKeyboardInput = false;
#endif
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.G)) canvas.enabled = !canvas.enabled;
	}
}
