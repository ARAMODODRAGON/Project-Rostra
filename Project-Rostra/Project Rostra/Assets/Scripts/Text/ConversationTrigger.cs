﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// written by: Sean Fowke
public class ConversationTrigger : MonoBehaviour
{
	//store a dialogue that we can change in the inspector
	public Dialogue dialogue;
	// is this text dependant on a choice made previously
	public float choiceCare;
	// these two dialogues will be spit out depending on the choice that was made. 
	// call this method in order to trigger the conversation
	public bool isChoiceDepend;
	// does the conversation only trigger once
	public Text pressZ;
	// does this directly trigger the text?
	public bool directTriggerText;
	protected bool played = false;
	public int addMilestone;
	public virtual void TriggerConvo()
	{
		DialogueManager.instance.StartConversation(dialogue);
	}
	// 1 = dwarf, 2 = guild, 3 = kill, 4 = spare, 5 = tell, 6 = lie
	public void TriggerChoiceDependantConvo()
	{
		DialogueManager.instance.ChoiceDependantConvo(choiceCare, dialogue);
	}

	public void TriggerNormalDialogue()
	{
		DialogueManager.instance.PlayNormalDialogue(dialogue);
	}

	public void TriggerCutsceneDialogue()
	{
		DialogueManager.instance.StartConversation(dialogue);
	}

	public void SetPressZ(bool b)
	{
		pressZ.enabled = b;
	}

	protected virtual void OnTriggerEnter2D(Collider2D col)
	{
		if (directTriggerText == true && col.gameObject.CompareTag("Player") && played == false)
		{
			TriggerConvo();
			played = true;
		}
	}

	public void AddMilestone(int i)
	{
		if (addMilestone > 0)
		{
			QuestManager.AddMilestone(i);
		}
	}
}
