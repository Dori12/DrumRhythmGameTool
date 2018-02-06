using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}