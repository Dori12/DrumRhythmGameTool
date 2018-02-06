using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentPositionSetting : MonoBehaviour {

	// Use this for initialization
	void Start () {
        gameObject.GetComponent<RectTransform>().SetPositionAndRotation(new Vector3(0, 0), Quaternion.identity);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
