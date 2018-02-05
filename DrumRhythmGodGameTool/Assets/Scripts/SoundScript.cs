using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using UnityEngine.EventSystems;

[Serializable]
public class NoteData : IComparable
{
    public string name;
    public float time;

    public int CompareTo(object other)
    {
        if ((other is NoteData) == false) {
            return 0;
        }

        return time.CompareTo((other as NoteData).time);
    }
}

public class SoundScript : MonoBehaviour {

    public GameObject ListNote;

    private AudioSource audioFile;
    private Text audioName;
    private Text currentTime;
    private Slider audioRemainTime;
    private InputField audioPitch;

    private float pitch;

    private PriorityQueue<NoteData> noteDatas;
    

    // Use this for initialization
    void Start () {
        audioFile = GetComponentInChildren<AudioSource>();
        audioName = GameObject.Find("AudioFileNameText").GetComponent<Text>();
        audioRemainTime = GetComponentInChildren<Slider>();
        currentTime = GameObject.Find("CurrentTimeText").GetComponent<Text>();
        audioPitch = GetComponentInChildren<InputField>();
        pitch = 0.0f;
        noteDatas = new PriorityQueue<NoteData>();

        audioName.text = audioFile.clip.name;
        currentTime.text = audioFile.time.ToString();

        audioRemainTime.maxValue = audioFile.clip.length;

    }
	
	// Update is called once per frame
	void Update () {
        CurrentTimePrint();
        PitchControl();
        SliderControl();
    }

    void CurrentTimePrint()
    {
        currentTime.text = audioFile.time + " / " + audioFile.clip.length;
    }

    void PitchControl()
    {
        bool parsed = float.TryParse(audioPitch.text, out pitch);
        if (!parsed)
        {
            pitch = 0.0f;
        }
        audioFile.pitch = pitch;
    }

    void SliderControl()
    {
        if(!audioFile.isPlaying)
        {
            audioFile.time = audioRemainTime.value;
        }
        else
        {
            audioRemainTime.value = audioFile.time;
        }
    }

    public void OnPlayButton()
    {
        audioFile.Play();
    }

    public void OnStopButton()
    {
        audioFile.Pause();
    }

    public void AudioTimeSet(float time)
    {
        audioFile.time = time;
    }

    public void ClickNote(string noteName)
    {
        NoteData notedata = new NoteData
        {
            name = noteName,
            time = audioFile.time
        };

        noteDatas.Add(notedata);
    }

    public void Save()
    {
        string toJson = JsonHelper.ToJson<NoteData>(noteDatas.GetItemList().ToArray(), prettyPrint: true);
        File.WriteAllText(Application.dataPath + "/Saves/data.json", toJson);
    }

    public void Load()
    {
        string jsonString = File.ReadAllText(Application.dataPath + "/Saves/data.json");
        NoteData[] data = JsonHelper.FromJson<NoteData>(jsonString);
        noteDatas.Clear();
        for(int i=0; i<data.Length; i++)
        {
            noteDatas.Add(data[i]);
            GameObject listNote = Instantiate(ListNote, GameObject.Find("Content").GetComponent<Transform>(), false);
            listNote.GetComponent<Transform>().Translate(new Vector3(0, -30 * i));
            listNote.GetComponentInChildren<Text>().text = noteDatas.GetItemList()[i].time + " / " + noteDatas.GetItemList()[i].name;
        }
    }
}