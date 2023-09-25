using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class DialogueTrigger : MonoBehaviour
{
    RaycastHit HitInfo;
    public DialogueRunner dialogueRunner;
    public bool hasTalkedBefore = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && hasTalkedBefore == false)
        {
            dialogueRunner.StartDialogue("WhatPromotion");
            hasTalkedBefore = true;
        }
        else
        {
            Repeat();
        }
    }

    public void StartDialogue()
    {
        if (Input.GetKeyDown(KeyCode.E) && hasTalkedBefore == false)
        {
            dialogueRunner.StartDialogue("WhatPromotion");
            hasTalkedBefore = true;
        }
    }

    public void Repeat()
    {
        if (Input.GetKeyDown(KeyCode.E) && hasTalkedBefore == true)
        {
            dialogueRunner.StartDialogue("Repeat");
        }
    }
}
