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
    private InputField audioTimeInputField;

    private float pitch;
    private bool valueChange;

    //private PriorityQueue<NoteData> noteDatas;
    private List<NoteData> noteDatas;
    private List<GameObject> notesList;
    private Stack<NoteData> undoDatas;
    private Stack<GameObject> undoDataList;
    public bool isRemove;

    // Use this for initialization
    void Start () {
        audioFile = GetComponentInChildren<AudioSource>();
        audioName = GameObject.Find("AudioFileNameText").GetComponent<Text>();
        audioRemainTime = GetComponentInChildren<Slider>();
        currentTime = GameObject.Find("CurrentTimeText").GetComponent<Text>();
        audioPitch = GameObject.Find("PitchInputField").GetComponent<InputField>();
        audioTimeInputField = GameObject.Find("TimeInputField").GetComponent<InputField>();
        pitch = 0.0f;
        noteDatas = new List<NoteData>();
        notesList = new List<GameObject>();
        isRemove = false;
        undoDatas = new Stack<NoteData>();
        undoDataList = new Stack<GameObject>();

        audioName.text = audioFile.clip.name;
        currentTime.text = audioFile.time.ToString();

        audioRemainTime.maxValue = audioFile.clip.length;

    }
	
	// Update is called once per frame
	void Update () {
        CurrentTimePrint();
        PitchControl();
        SliderControl();
        TimeInputFieldUpdate();
    }

    void CurrentTimePrint()
    {
        double time = Math.Round(Convert.ToDouble(audioFile.time), 1);
        double length = Math.Round(Convert.ToDouble(audioFile.clip.length), 1);
        currentTime.text = time + " / " + length;
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

    void TimeInputFieldUpdate()
    {
        if (audioFile.isPlaying)
        {
            audioTimeInputField.text = audioFile.time.ToString();
        }
        else
        {

        }
    }

    public void ChangeTimeInputField()
    {
        float time;
        bool parsed = float.TryParse(audioTimeInputField.text, out time);

        if(!parsed)
        {
            time = 0.0f;
        }

        audioFile.time = time;
        audioRemainTime.value = time;
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
        if (!audioFile.isPlaying)
        {
            audioFile.Play();
        }
    }

    public void OnStopButton()
    {
        if (audioFile.isPlaying)
        {
            audioFile.Pause();
        }
    }

    public void AudioTimeSet(float time)
    {
        audioFile.time = time;
    }

    public void ClickNote(string name)
    {
        AddNoteData(name, audioFile.time);
        //NoteData notedata = new NoteData
        //{
        //    name = noteName,
        //    time = audioFile.time
        //};

        //if (noteDatas.Count > 0)
        //{
        //    bool largest = true;
        //    for (int i = 0; i < noteDatas.Count; i++)
        //    {
        //        if (notedata.time <= noteDatas[i].time)
        //        {
        //            noteDatas.Insert(i, notedata);
        //            largest = false;
        //            AddNoteList(i);
        //            break;
        //        }
        //    }

        //    if(largest)
        //    {
        //        noteDatas.Add(notedata);
        //        AddNoteList(noteDatas.Count-1);
        //    }
        //}
        //else
        //{
        //    noteDatas.Add(notedata);
        //    AddNoteList(0);
        //}
    }

    void AddNoteData(string name, float time)
    {
        NoteData notedata = new NoteData
        {
            name = name,
            time = time
        };

        if (noteDatas.Count > 0)
        {
            bool largest = true;
            for (int i = 0; i < noteDatas.Count; i++)
            {
                if (notedata.time <= noteDatas[i].time)
                {
                    noteDatas.Insert(i, notedata);
                    largest = false;
                    AddNoteList(i);
                    break;
                }
            }

            if (largest)
            {
                noteDatas.Add(notedata);
                AddNoteList(noteDatas.Count - 1);
            }
        }
        else
        {
            noteDatas.Add(notedata);
            AddNoteList(0);
        }
    }

    public void Save()
    {
        string toJson = JsonHelper.ToJson<NoteData>(noteDatas.ToArray(), prettyPrint: true);
        DirectoryInfo dir = Directory.CreateDirectory(Application.dataPath + "/JsonFile");
        //Directory.CreateDirectory(Application.persistentDataPath + "/JsonFile");
        File.WriteAllText(Application.dataPath + "/JsonFile/data.json", toJson);
        //File.WriteAllText(Application.persistentDataPath + "JsonFile/data.json", toJson);
    }

    public void Load()
    {
        string jsonString = File.ReadAllText(Application.dataPath+ "/JsonFile/data.json");
        NoteData[] data = JsonHelper.FromJson<NoteData>(jsonString);
        noteDatas.Clear();
        notesList.Clear();
        Button[] childs = GameObject.Find("Content").transform.GetComponentsInChildren<Button>();

        foreach(Button child in childs)
        {
            Destroy(child.gameObject);
        }

        for (int i=0; i<data.Length; i++)
        {
            noteDatas.Add(data[i]);
            AddNoteList(i);
        }
    }

    void AddNoteList(int index)
    {
        GameObject listNote = Instantiate(ListNote, GameObject.Find("Content").GetComponent<Transform>(), false);
        for (int i=index; i< notesList.Count; i++)
        {
            notesList[i].transform.Translate(new Vector3(0, -32));
            notesList[i].GetComponent<NoteList>().index = i+1;
        }
        notesList.Insert(index, listNote);
        notesList[index].transform.Translate(new Vector3(0, -32 * index));
        notesList[index].GetComponentInChildren<Text>().text = Math.Round(noteDatas[index].time, 2) + " / " + noteDatas[index].name;
        notesList[index].GetComponent<NoteList>().index = index;
        notesList[index].GetComponent<NoteList>().isCreate = true;
        undoDatas.Push(noteDatas[index]);//
        undoDataList.Push(notesList[index]);//
    }

    public void RemoveNoteList(int index)
    {
        notesList[index].GetComponent<NoteList>().isCreate = false;
        undoDatas.Push(noteDatas[index]);//
        undoDataList.Push(notesList[index]);//
        notesList.RemoveAt(index);
        for(int i=index; i < notesList.Count; i++)
        {
            notesList[i].transform.Translate(new Vector3(0, +32));
            notesList[i].GetComponent<NoteList>().index = i;
        }
    }

    public void ToggleRemoveMode()
    {
        if (isRemove)
        {
            GameObject.Find("Remove").GetComponentInChildren<Text>().text = "Remove OFF";
        }
        else
        {
            GameObject.Find("Remove").GetComponentInChildren<Text>().text = "Remove ON";
        }

        isRemove = !isRemove;
    }

    public float GetAudioTime()
    {
        return audioFile.time;
    }

    public List<NoteData> GetNoteData()
    {
        return noteDatas;
    }
}