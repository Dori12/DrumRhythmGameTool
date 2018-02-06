using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ButtonChange : MonoBehaviour {

    public string thisName;
	// Use this for initialization
	void Start () {

        thisName = gameObject.GetComponentInChildren<Text>().text;
	}
	
	// Update is called once per frame
	void Update () {
	}

    public void ChangeColor()
    {
        if (thisName == gameObject.GetComponentInChildren<Text>().text)
        {
            gameObject.GetComponent<Image>().color = new Color(255, 0, 0, 255);
            Invoke("ResetColor", 0.1f);
        }
    }

    void ResetColor()
    {
        gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 255);
    }
}
