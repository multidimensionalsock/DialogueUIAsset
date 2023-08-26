using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;
using UnityEngine.InputSystem;


public class SideDialogueView : DialogueViewBase
{
	//UI elements
	[SerializeField] Image characterImage;

	//run time elements
	List<RectTransform> UIelements;
	[SerializeField] CharacterList characterList;
	[SerializeField] GameObject linePrefab;
	PlayerInput m_input;
	Action advanceHandler = null;
	Effects.CoroutineInterruptToken currentStopToken = new Effects.CoroutineInterruptToken();
	LocalizedLine currentLine = null;

	public void Start()
	{
		m_input= GetComponent<PlayerInput>();
		//linePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/DialogueUI/Prefabs/DialogueLinePrefab.prefab");
		m_input.currentActionMap.FindAction("NextLine").performed += NextLine;
	}

	void NextLine(InputAction.CallbackContext context)
	{
		gameObject.GetComponent<TextLineProvider>();
		UserRequestedViewAdvancement();
	}

	public override void UserRequestedViewAdvancement()
	{
		// We have received a signal that the user wants to proceed to the next
		// line.

		// Invoke our 'advance line' handler, which (depending on what we're
		// currently doing) will be a signal to interrupt the line, stop the
		// current animation, or do nothing.
		advanceHandler?.Invoke();
	}

	public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
	{
		advanceHandler = onDialogueLineFinished;
		currentLine = dialogueLine;
		if (gameObject.activeInHierarchy ==false)
		{
			onDialogueLineFinished();
			return;
		}

		CharacterInformation character = CheckIfCharacterInDatabase(dialogueLine.CharacterName);
		if (character != null)
		{

			GameObject currentLine = Instantiate(linePrefab);
			currentLine.transform.SetParent(this.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0));
			characterImage.sprite = character.m_characterImage; //set sprite for current character
			currentLine.GetComponent<TextMeshProUGUI>().text = "<b><color=#" + character.m_characterColor + "> " + dialogueLine.CharacterName
				+ ": </color></b>" + dialogueLine.Text.Text;

			//get rect transform to use later 
			RectTransform currentLineRect = currentLine.GetComponent<RectTransform>();
			currentLineRect.localPosition = new Vector3(12f, -130f, 0f);

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
		currentStopToken.Complete();
		onDialogueLineFinished?.Invoke();

	}

	public override void InterruptLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
	{
		advanceHandler = onDialogueLineFinished;
		currentLine= dialogueLine;
		if (gameObject.activeInHierarchy == false)
		{
			onDialogueLineFinished();
			return;
		}

		CharacterInformation character = CheckIfCharacterInDatabase(dialogueLine.CharacterName);
		if (character != null)
		{

			GameObject currentLine = Instantiate(linePrefab);
			currentLine.transform.SetParent(this.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0));
			characterImage.sprite = character.m_characterImage; //set sprite for current character
			currentLine.GetComponent<TextMeshProUGUI>().text = "<b><color=#" + character.m_characterColor + "> " + dialogueLine.CharacterName
				+ ": </color></b>" + dialogueLine.Text.Text;

			//get rect transform to use later 
			RectTransform currentLineRect = currentLine.GetComponent<RectTransform>();
			currentLineRect.localPosition = new Vector3(12f, -130f, 0f);

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
		currentStopToken.Complete();
	}

	public override void DismissLine(Action onDismissalComplete)
	{
		//called once at the end of the dialogue
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
