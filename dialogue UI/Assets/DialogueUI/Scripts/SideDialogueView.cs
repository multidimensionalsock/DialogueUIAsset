using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;



public class SideDialogueView : DialogueViewBase
{
	//UI elements
	[SerializeField] TextMeshProUGUI dialogue;
	[SerializeField] Image characterImage;

	//run time elements
	List<RectTransform> UIelements;
	[SerializeField] CharacterList characterList;
	GameObject linePrefab;

	public void Start()
	{
		
		linePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/DialogueUI/Prefabs/DialogueLinePrefab.prefab");
		
	}

	public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
	{
		if (gameObject.activeInHierarchy == false)
		{
			onDialogueLineFinished();
			return;
		}
		
		CharacterInformation character = CheckIfCharacterInDatabase(dialogueLine.CharacterName);
		if (character != null )
		{
			
			GameObject currentLine = Instantiate(linePrefab);
			characterImage.sprite = character.m_characterImage; //set sprite for current character
			currentLine.GetComponent<TextMeshProUGUI>().text = "<b><color=#" + character.m_characterColor + "> " + dialogueLine.CharacterName
				+ ": </color></b>" + dialogueLine.Text.Text;

			//get rect transform to use later 
			RectTransform currentLineRect = currentLine.GetComponent<RectTransform>();
			currentLineRect.position = new Vector3(12f, 20f, 0f);
			
			if (UIelements == null || UIelements.Count == 0)
			{
				UIelements = new List<RectTransform>();
			}
			else
			{
				//move elemts of list up by the height of the new line
				for (int i = 0; i < UIelements.Count; i++)
				{
					UIelements[i].position = new Vector3(0f, UIelements[i].position.y + currentLineRect.rect.height, 0f);
				}
			}
			UIelements.Add(currentLineRect);
			
		}
		else
		{
			Debug.Log("there is no character information found");
		}

	}

	public override void InterruptLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
	{
		if (gameObject.activeInHierarchy == false)
		{
			// This line view isn't active; it should immediately report that
			// it's finished presenting.
			onDialogueLineFinished();
			return;
		}
	}

	public override void DismissLine(Action onDismissalComplete)
	{
		if (gameObject.activeInHierarchy == false)
		{
			// This line view isn't active; it should immediately report that
			// it's finished dismissing.
			onDismissalComplete();
			return;
		}
	}

	public override void RunOptions(DialogueOption[] dialogueOptions, Action<int> onOptionSelected)
	{
		base.RunOptions(dialogueOptions, onOptionSelected);
	}

	CharacterInformation CheckIfCharacterInDatabase(string name)
	{
		for (int i = 0; i < characterList.characters.Length; i++)
		{
			if (name == characterList.characters[i].m_characterName)
			{
				return characterList.characters[i];
			}
		}
		return null;
	}
}
