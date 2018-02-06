using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class NoteDataList
{
    public int index;
    public bool isCreate;
}

public class NoteList : MonoBehaviour {
    public int index;
    public bool isCreate;

    private SoundScript soundSc;

    void Start()
    {
        soundSc = GameObject.Find("Canvas").GetComponent<SoundScript>();
    }

    public void RemoveNote()
    {
        if (soundSc.isRemove)
        {
            soundSc.RemoveNoteList(index);
            Destroy(gameObject);
        }
    }

    public void UndoRemoveNote()
    {
        soundSc.UndoRemoveNoteList(index);
        Destroy(gameObject);
    }
}