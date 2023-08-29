using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Yarn.Unity;

public class DialogueView : DialogueViewBase
{
	List<RectTransform> UIelements;
	[SerializeField] CharacterList characterList;
	[SerializeField] GameObject linePrefab;
	[SerializeField] Image characterImage;
		Action advanceHandler = null;

	PlayerInput m_input;

		public void Start()
		{
		UIelements = new List<RectTransform>();
		m_input = GetComponent<PlayerInput>();
		m_input.currentActionMap.FindAction("NextLine").performed += CallNextLine;
		}

	void CallNextLine(InputAction.CallbackContext context)
	{
		UserRequestedViewAdvancement();
	}

		public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
		{
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
				+ ": </color></b>" + dialogueLine.TextWithoutCharacterName.Text;

			//get rect transform to use later 
			RectTransform currentLineRect = currentLine.GetComponent<RectTransform>();
			Canvas.ForceUpdateCanvases();
			currentLineRect.localPosition = new Vector3(100f, -90f, 0f);

			if (UIelements.Count == 0)
			{
				Debug.Log("list was zero");
			}
			else
			{
				float yincrease;
				switch (currentLine.GetComponent<TextMeshProUGUI>().textInfo.lineCount)
				{
					case 1:
						yincrease = currentLineRect.rect.height * 2.5f;
						break;
					case 2:
						yincrease = currentLineRect.rect.height * 1.5f;
						break;
					default:
						yincrease= currentLineRect.rect.height;
						break;
				}
				//move elemts of list up by the height of the new line
				for (int i = 0; i < UIelements.Count; i++)
				{
					
					UIelements[i].position = new Vector3(UIelements[i].position.x, UIelements[i].position.y + yincrease, UIelements[i].position.z);
				}
			}
			UIelements.Add(currentLineRect);

		}
		else
		{
			Debug.Log("there is no character information found");
		}
			advanceHandler = requestInterrupt;
			//advanceHandler = null;
			onDialogueLineFinished();
		}

	public override void RunOptions(DialogueOption[] dialogueOptions, Action<int> onOptionSelected)
	{

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

	public override void InterruptLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
		{
			if (gameObject.activeInHierarchy == false)
			{
				onDialogueLineFinished();
				return;
			}

			advanceHandler = null;

			onDialogueLineFinished();
		}
		public override void DismissLine(Action onDismissalComplete)
		{
			if (gameObject.activeInHierarchy == false)
			{
				onDismissalComplete();
				return;
			}

			advanceHandler = () =>
			{
				advanceHandler = null;
				onDismissalComplete();
			};
		}
		public override void UserRequestedViewAdvancement()
		{
			advanceHandler?.Invoke();
		}
	}
