using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using Yarn.Unity.ActionAnalyser;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueRunner dialogueRunner;
    public bool hasTalkedBefore = false;
    public GameObject splitScreen;

    private void Start()
    {
        splitScreen.SetActive(false);
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && hasTalkedBefore == false)
        {
            dialogueRunner.StartDialogue("WhatPromotion");
            hasTalkedBefore = true;
            splitScreen.SetActive(true);
        }
        else
        {
            Repeat();
        }
    }

    public void Repeat()
    {
        if (Input.GetKeyDown(KeyCode.E) && hasTalkedBefore == true)
        {
            dialogueRunner.StartDialogue("Repeat");
        }
    }

    public void Skip()
    {
        if (Input.GetKeyDown(KeyCode.E) && dialogueRunner.IsDialogueRunning)
        {
            
        }
    }
}
