using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ButtonChange : MonoBehaviour {

    private SoundScript soundSc;
	// Use this for initialization
	void Start () {
        soundSc = GameObject.Find("Canvas").GetComponent<SoundScript>();
	}
	
	// Update is called once per frame
	void Update () {
        for(int i=0; i<soundSc.GetNoteData().Count; i++)
        {
            if(Math.Round(soundSc.GetAudioTime(), 2) == Math.Round(soundSc.GetNoteData()[i].time,2))
            {
                if (soundSc.GetNoteData()[i].name == gameObject.GetComponentInChildren<Text>().text)
                {
                    ChangeColor();
                }
            }
        }
	}

    void ChangeColor()
    {
        gameObject.GetComponent<Image>().color = new Color(255, 0, 0, 255);
        Invoke("ResetColor", 0.1f);
    }

    void ResetColor()
    {
        gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 255);
    }
}
