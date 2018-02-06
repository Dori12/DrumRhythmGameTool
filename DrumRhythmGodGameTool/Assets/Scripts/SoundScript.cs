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

    public float minTime;
    public GameObject ListNote;

    private AudioSource audioFile;
    private Text audioName;
    private Text currentTime;
    private Slider audioRemainTime;
    private InputField audioPitch;
    private InputField audioTimeInputField;
    private InputField ifFileName;

    private float pitch;
    private bool valueChange;
    private string fileName;

    //private PriorityQueue<NoteData> noteDatas;
    private List<NoteData> noteDatas;
    private List<GameObject> notesList;
    private Stack<NoteData> undoDatas;
    private Stack<GameObject> undoDataList;

    private ButtonChange[] bcSc = new ButtonChange[6];
    public bool isRemove;
    private int buttonIndex;

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
        ifFileName = GameObject.Find("FileNameInput").GetComponent<InputField>();

        bcSc[0] = GameObject.Find("HighHat").GetComponent<ButtonChange>();
        bcSc[1] = GameObject.Find("Snare").GetComponent<ButtonChange>();
        bcSc[2] = GameObject.Find("Base").GetComponent<ButtonChange>();
        bcSc[3] = GameObject.Find("HighTom").GetComponent<ButtonChange>();
        bcSc[4] = GameObject.Find("LowTom").GetComponent<ButtonChange>();
        bcSc[5] = GameObject.Find("Symbol").GetComponent<ButtonChange>();

        audioName.text = audioFile.clip.name;
        currentTime.text = audioFile.time.ToString();

        audioRemainTime.maxValue = audioFile.clip.length;

        audioFile.Play();
        audioFile.Stop();
    }
	
	// Update is called once per frame
	void Update () {
        CurrentTimePrint();
        PitchControl();
        SliderControl();
        TimeInputFieldUpdate();
        KeyUpdate();
        PlayNoteDataIndex();
    }

    void PlayNoteDataIndex()
    {
        if (!audioFile.isPlaying)
            return;
        if (noteDatas.Count < 1)
            return;

        if (noteDatas.Count > buttonIndex)
        {
            if (Math.Round(audioFile.time, 2) >= Math.Round(noteDatas[buttonIndex].time, 2))
            {
                for (int i = 0; i < 6; i++)
                {
                    if (bcSc[i].thisName == noteDatas[buttonIndex].name)
                    {
                        bcSc[i].ChangeColor();
                        break;
                    }
                }
                buttonIndex++;
            }
        }
    }

    void KeyUpdate()
    {
        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                ClickNote("HighHat");
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                ClickNote("Snare");
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                ClickNote("HighTom");
            }
            if (Input.GetKeyDown(KeyCode.F))
            {
                ClickNote("LowTom");
            }
            if (Input.GetKeyDown(KeyCode.G))
            {
                ClickNote("Symbol");
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ClickNote("Base");
            }
        }
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
            PlayIndexInit();
            float time = audioFile.time;
            for(int i=0; i<noteDatas.Count; i++)
            {
                if(Math.Round(time,2) >= Math.Round(noteDatas[i].time,2))
                {
                    buttonIndex = i;
                }
                else
                {
                    buttonIndex = i;
                    break;
                }
            }
            audioFile.Play();
            Debug.Log(buttonIndex);
        }
    }

    public void OnStopButton()
    {
        if (audioFile.isPlaying)
        {
            audioFile.Pause();
            PlayIndexInit();
        }
    }

    public void AudioTimeSet(float time)
    {
        audioFile.time = time;
    }

    public void ClickNote(string name)
    {
        AddNoteData(name, audioFile.time);
    }

    void AddNoteData(string _name, float _time)
    {
        NoteData notedata = new NoteData
        {
            name = _name,
            time = _time
        };

        double beatTime = ((notedata.time) / (minTime));
        beatTime = Math.Round(beatTime, 0);
        beatTime *= minTime;
        beatTime = Math.Round(beatTime, 1);

        notedata.time = (float)beatTime;

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
        File.WriteAllText(Application.dataPath + "/JsonFile/" + fileName + ".json", toJson);
        //File.WriteAllText(Application.persistentDataPath + "JsonFile/" + fileName" +".json", toJson);
    }

    public void Load()
    {
        PlayIndexInit();
        string jsonString = File.ReadAllText(Application.dataPath+ "/JsonFile/" + fileName + ".json");
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
        noteDatas.RemoveAt(index);
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

    public void ChangeFileName()
    {
        fileName = ifFileName.text;
    }

    public void PlayIndexInit()
    {
        buttonIndex = 0;
    }
}