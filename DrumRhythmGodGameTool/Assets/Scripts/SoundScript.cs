using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundScript : MonoBehaviour {

    [SerializeField]
    private AudioSource audioFile;
    [SerializeField]
    private Text audioName;
    [SerializeField]
    private Text currentTime;
    [SerializeField]
    private Slider audioRemainTime;

    // Use this for initialization
    void Start () {
        audioFile = GetComponentInChildren<AudioSource>();
        audioName = GameObject.Find("AudioFileNameText").GetComponent<Text>();
        audioRemainTime = GetComponentInChildren<Slider>();
        currentTime = GameObject.Find("CurrentTimeText").GetComponent<Text>();

        audioName.text = audioFile.clip.name;
        currentTime.text = "" + audioFile.time;
	}
	
	// Update is called once per frame
	void Update () {
        currentTime.text = "" + audioFile.time;
    }
}