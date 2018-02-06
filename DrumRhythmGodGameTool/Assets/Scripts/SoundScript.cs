using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using UnityEngine.EventSystems;

[Serializable]
public class NoteData
{
    public string name;
    public float time;
}

public class SoundScript : MonoBehaviour {

    public float minTime;
    public int demicalPoint;
    public GameObject ListNote;

    private AudioSource audioFile;
    private Text audioName;
    private Text currentTime;
    private Slider audioRemainTime;
    private InputField audioPitch;
    private InputField audioTimeInputField;
    private InputField ifFileName;
    private InputField ifJumpInterval;

    private float pitch;
    private bool valueChange;
    private string fileName;

    //private PriorityQueue<NoteData> noteDatas;
    private List<NoteData> noteDatas;
    private List<GameObject> notesList;
    private Stack<NoteData> undoDatas;
    private Stack<NoteDataList> undoDataList;
    private Stack<NoteData> redoDatas;
    private Stack<NoteDataList> redoDataList;

    private ButtonChange[] bcSc = new ButtonChange[6];
    public bool isRemove;
    private int buttonIndex;
    public float ButtonListInterval;


    // Use this for initialization
    void Start () {
        audioFile = GetComponentInChildren<AudioSource>();
        audioName = GameObject.Find("AudioFileNameText").GetComponent<Text>();
        audioRemainTime = GetComponentInChildren<Slider>();
        currentTime = GameObject.Find("CurrentTimeText").GetComponent<Text>();
        audioPitch = GameObject.Find("PitchInputField").GetComponent<InputField>();
        audioTimeInputField = GameObject.Find("TimeInputField").GetComponent<InputField>();
        noteDatas = new List<NoteData>();
        notesList = new List<GameObject>();
        isRemove = false;
        undoDatas = new Stack<NoteData>();
        undoDataList = new Stack<NoteDataList>();
        redoDatas = new Stack<NoteData>();
        redoDataList = new Stack<NoteDataList>();
        ifFileName = GameObject.Find("FileNameInput").GetComponent<InputField>();
        ifJumpInterval = GameObject.Find("JumpIntervalInputField").GetComponent<InputField>();

        bcSc[0] = GameObject.Find("HighHat").GetComponent<ButtonChange>();
        bcSc[1] = GameObject.Find("Snare").GetComponent<ButtonChange>();
        bcSc[2] = GameObject.Find("Cymbal").GetComponent<ButtonChange>();
        bcSc[3] = GameObject.Find("HighTom").GetComponent<ButtonChange>();
        bcSc[4] = GameObject.Find("LowTom").GetComponent<ButtonChange>();
        bcSc[5] = GameObject.Find("Base").GetComponent<ButtonChange>();

        audioName.text = audioFile.clip.name;
        currentTime.text = audioFile.time.ToString();
        audioRemainTime.maxValue = audioFile.clip.length;
        pitch = 0.0f;

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
                if (notesList.Count > 0)
                {
                    notesList[buttonIndex - 1].GetComponent<Image>().color = new Color(255, 0, 0, 255);
                }
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
                ClickNote("Cymbal");
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ClickNote("Base");
            }
            if(Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (audioFile.time - (float)(Convert.ToDecimal(ifJumpInterval.text)) <= 0.0f)
                {
                    audioFile.time = 0.0f;
                }
                else
                {
                    audioFile.time -= (float)(Convert.ToDecimal(ifJumpInterval.text));
                }
                audioRemainTime.value = audioFile.time;
                audioTimeInputField.text = audioFile.time.ToString();
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (audioFile.time + (float)(Convert.ToDecimal(ifJumpInterval.text)) >= audioFile.clip.length)
                {
                    audioFile.time = audioFile.clip.length-0.01f;
                }
                else
                {
                    audioFile.time += (float)(Convert.ToDecimal(ifJumpInterval.text));
                }
                audioRemainTime.value = audioFile.time;
                audioTimeInputField.text = audioFile.time.ToString();
            }
        }

        if(Input.GetKey(KeyCode.LeftControl))
        {
            if(Input.GetKeyDown(KeyCode.Z))
            {
                Undo();
            }
            if(Input.GetKeyDown(KeyCode.X))
            {
                //Redo();
            }
        }
    }

    void CurrentTimePrint()
    {
        double time = Math.Round(Convert.ToDouble(audioFile.time), demicalPoint);
        double length = Math.Round(Convert.ToDouble(audioFile.clip.length), demicalPoint);
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

            for (int i = 0; i < notesList.Count; i++)
            {
                if (buttonIndex > i)
                {
                    notesList[i].GetComponent<Image>().color = new Color(255, 0, 0, 255);
                }
                else
                {
                    notesList[i].GetComponent<Image>().color = new Color(255, 255, 255, 255);
                }
            }

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
        beatTime = Math.Round(beatTime, demicalPoint);
        notedata.time = (float)beatTime;

        int num = 0;
        for(int i =0; i<noteDatas.Count; i++)
        {
            if(noteDatas[i].time == notedata.time)
            {
                if (noteDatas[i].name != "Base" && notedata.name != "Base")
                {
                    num++;
                }
                if (noteDatas[i].name == notedata.name)
                {
                    return;
                }
            }

            if(num>1 && notedata.name != "Base")
            {
                return;
            }
        }

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
                    RedoStackInit();
                    break;
                }
            }

            if (largest)
            {
                noteDatas.Add(notedata);
                AddNoteList(noteDatas.Count - 1);
                RedoStackInit();
            }
        }
        else
        {
            noteDatas.Add(notedata);
            AddNoteList(0);
            RedoStackInit();
        }
    }

    public void Save()
    {
        string toJson = JsonHelper.ToJson<NoteData>(noteDatas.ToArray(), prettyPrint: true);
        Directory.CreateDirectory(Application.dataPath + "/JsonFile");
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

        UndoStackInit();
    }

    void AddNoteList(int index)
    {
        GameObject listNote = Instantiate(ListNote, GameObject.Find("Content").GetComponent<Transform>(), false);
        for (int i = index; i < notesList.Count; i++)
        {
            notesList[i].GetComponent<RectTransform>().position += new Vector3(0, -ButtonListInterval);
            notesList[i].GetComponent<NoteList>().index = i + 1;
        }
        notesList.Insert(index, listNote);

        notesList[index].GetComponent<RectTransform>().position += (new Vector3(0, -ButtonListInterval * index));
        notesList[index].GetComponentInChildren<Text>().text = Math.Round(noteDatas[index].time, demicalPoint) + " / " + noteDatas[index].name;
        notesList[index].GetComponent<NoteList>().index = index;
        notesList[index].GetComponent<NoteList>().isCreate = true;

        NoteData notedata = new NoteData();
        notedata.name = noteDatas[index].name;
        notedata.time = noteDatas[index].time;
        

        NoteDataList notelist = new NoteDataList();
        notelist.index = index;
        notelist.isCreate = true;

        undoDatas.Push(notedata);
        undoDataList.Push(notelist);
    }

    public void RemoveNoteList(int index)
    {
        notesList[index].GetComponent<NoteList>().isCreate = false;

        NoteData notedata = new NoteData();
        notedata.name = noteDatas[index].name;
        notedata.time = noteDatas[index].time;

        NoteDataList notelist = new NoteDataList();
        notelist.index = index;
        notelist.isCreate = false;

        undoDatas.Push(notedata);//
        undoDataList.Push(notelist);//

        notesList.RemoveAt(index);
        noteDatas.RemoveAt(index);
        for(int i=index; i < notesList.Count; i++)
        {
            notesList[i].GetComponent<RectTransform>().position += new Vector3(0, +ButtonListInterval);
            notesList[i].GetComponent<NoteList>().index = i;
        }

        RedoStackInit();
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

    void UndoStackInit()
    {
        undoDataList.Clear();
        undoDatas.Clear();
    }

    void UndoAddNoteList(int index)
    {
        GameObject listNote = Instantiate(ListNote, GameObject.Find("Content").GetComponent<Transform>(), false);
        for (int i = index; i < notesList.Count; i++)
        {
            notesList[i].GetComponent<RectTransform>().position += new Vector3(0, -ButtonListInterval);
            notesList[i].GetComponent<NoteList>().index = i + 1;
        }
        notesList.Insert(index, listNote);

        notesList[index].GetComponent<RectTransform>().position += (new Vector3(0, -ButtonListInterval * index));
        notesList[index].GetComponentInChildren<Text>().text = Math.Round(noteDatas[index].time, demicalPoint) + " / " + noteDatas[index].name;
        notesList[index].GetComponent<NoteList>().index = index;
        notesList[index].GetComponent<NoteList>().isCreate = true;
        
    }

    public void UndoRemoveNoteList(int index)
    {
        notesList[index].GetComponent<NoteList>().isCreate = false;

        notesList.RemoveAt(index);
        noteDatas.RemoveAt(index);
        for (int i = index; i < notesList.Count; i++)
        {
            notesList[i].GetComponent<RectTransform>().position += new Vector3(0, +ButtonListInterval);
            notesList[i].GetComponent<NoteList>().index = i;
        }
    }

    void UndoAddNoteData(string _name, float _time)
    {
        NoteData notedata = new NoteData
        {
            name = _name,
            time = _time
        };

        double beatTime = ((notedata.time) / (minTime));
        beatTime = Math.Round(beatTime, 0);
        beatTime *= minTime;
        beatTime = Math.Round(beatTime, demicalPoint);
        notedata.time = (float)beatTime;

        int num = 0;
        for (int i = 0; i < noteDatas.Count; i++)
        {
            if (noteDatas[i].time == notedata.time)
            {
                if (noteDatas[i].name != "Base" && notedata.name != "Base")
                {
                    num++;
                }
                if (noteDatas[i].name == notedata.name)
                {
                    return;
                }
            }

            if (num > 1 && notedata.name != "Base")
            {
                return;
            }
        }

        if (noteDatas.Count > 0)
        {
            bool largest = true;
            for (int i = 0; i < noteDatas.Count; i++)
            {
                if (notedata.time <= noteDatas[i].time)
                {
                    noteDatas.Insert(i, notedata);
                    largest = false;
                    UndoAddNoteList(i);
                    break;
                }
            }

            if (largest)
            {
                noteDatas.Add(notedata);
                UndoAddNoteList(noteDatas.Count - 1);
            }
        }
        else
        {
            noteDatas.Add(notedata);
            UndoAddNoteList(0);
        }
    }

    void Undo()
    {
        NoteData notedata = new NoteData();
        NoteDataList notelist = new NoteDataList();

        if (undoDataList.Count > 0)
        {
            notelist = undoDataList.Pop();
            redoDataList.Push(notelist);
        }
        else
        {
            return;
        }

        if (undoDatas.Count > 0)
        {
            notedata = undoDatas.Pop();
            redoDatas.Push(notedata);
        }
        else
        {
            return;
        }

        if(notelist.isCreate)
        {
            GameObject.Find("Content").transform.GetChild(notelist.index).GetComponent<NoteList>().UndoRemoveNote();
        }
        else
        {
            UndoAddNoteData(notedata.name, notedata.time);
        }
    }

    void Redo()
    {
        NoteData notedata = new NoteData();
        NoteDataList notelist = new NoteDataList();

        if (redoDataList.Count > 0)
        {
            notelist = redoDataList.Pop();
            undoDataList.Push(notelist);
        }
        else
        {
            return;
        }

        if (redoDatas.Count > 0)
        {
            notedata = redoDatas.Pop();
            undoDatas.Push(notedata);
        }
        else
        {
            return;
        }

        if (!notelist.isCreate)
        {
            GameObject.Find("Content").transform.GetChild(notelist.index).GetComponent<NoteList>().UndoRemoveNote();
        }
        else
        {
            UndoAddNoteData(notedata.name, notedata.time);
        }
    }

    void RedoStackInit()
    {
        //redoDataList.Clear();
        //redoDatas.Clear();
    }
}