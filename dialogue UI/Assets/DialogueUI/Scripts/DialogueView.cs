using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Yarn.Unity;

public class DialogueView : DialogueViewBase
{
	List<RectTransform> UIelements;
	List<RectTransform> currentOptionLines;
	[SerializeField] CharacterList characterList;
	[SerializeField] GameObject linePrefab;
	[SerializeField] Image characterImage;
	int currentOptionPosition = -1;
	Action advanceHandler = null;
	Action<int> SelectOption = null;
	[SerializeField] Color32 OptionHighlightColour;
	[SerializeField] Color32 StandardTextColour;

	PlayerInput m_input;

	private void OnEnable()
	{
		UIelements = new List<RectTransform>();
		m_input = GetComponent<PlayerInput>();
		m_input.currentActionMap.FindAction("NextLine").performed += CallNextLine;
		m_input.currentActionMap.FindAction("PickOptions").performed += OptionPicker;
	}

	void CallNextLine(InputAction.CallbackContext context)
	{
		if (currentOptionLines != null)
		{
			RemoveLines();
			SelectOption(currentOptionPosition);
		}
		else
		{
			UserRequestedViewAdvancement();
		}
		
	}

	void OptionPicker(InputAction.CallbackContext context)
	{
		if (currentOptionLines == null)
			return;

		float direction = context.ReadValue<float>();

		switch (direction)
		{
			case 1:
				if (currentOptionPosition == 0)
					break;

				currentOptionPosition -= 1;
				//currentOptionLines[currentOptionPosition].gameObject.GetComponent<TextMeshProUGUI>().color = OptionHighlightColour;

				break;

			case -1:
				if (currentOptionPosition == currentOptionLines.Count)
					break;
				currentOptionPosition += 1;
				//currentOptionLines[currentOptionPosition].gameObject.GetComponent<TextMeshProUGUI>().color = OptionHighlightColour;
				break;
		}
		
		//currentOptionLines 
		//currentOptionPosition	
	}

	public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
		{
			if (gameObject.activeInHierarchy == false)
			{
				onDialogueLineFinished();
				return;
			}

			RectTransform currentLineRect = OutputLine(dialogueLine);
			UIelements.Add(currentLineRect);

			advanceHandler = requestInterrupt;
			onDialogueLineFinished();
		}

	public override void RunOptions(DialogueOption[] dialogueOptions, Action<int> onOptionSelected)
	{
		if (SelectOption != null)
		{
			SelectOption = null;
		}
		SelectOption = onOptionSelected;

		 if (currentOptionLines == null && dialogueOptions.Length > 0)
		{
			currentOptionLines = new List<RectTransform>();
		}

		for (int i = 0; i < dialogueOptions.Length; i++)
		{
			RectTransform currentLineRect = OutputLine(dialogueOptions[i].Line);
			Debug.Log(currentLineRect.gameObject.GetComponent<TextMeshProUGUI>().color);
			currentOptionLines.Add(currentLineRect);
		}

		currentOptionPosition = 0;

	}

	private RectTransform OutputLine(LocalizedLine dialogueLine)
	{
		CharacterInformation character = CheckIfCharacterInDatabase(dialogueLine.CharacterName);
		if (character != null)
		{

			GameObject currentLine = Instantiate(linePrefab);
			currentLine.transform.SetParent(this.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0));
			characterImage.sprite = character.m_characterImage; //set sprite for current character
			//currentLine.GetComponent<TextMeshProUGUI>().color = StandardTextColour;
			currentLine.GetComponent<TextMeshProUGUI>().text = "<b><color=#" + character.m_characterColor + "> " + dialogueLine.CharacterName
				+ ": </color></b>" + dialogueLine.TextWithoutCharacterName.Text;
			


			//get rect transform to use later 
			RectTransform currentLineRect = currentLine.GetComponent<RectTransform>();
			Canvas.ForceUpdateCanvases();
			currentLineRect.localPosition = new Vector3(100f, -90f, 0f);

			Debug.Log(UIelements);
			if (UIelements.Count < 0)
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
						yincrease = currentLineRect.rect.height;
						break;
				}
				//move elemts of list up by the height of the new line
				//RectTransform parent = currentLine.transform.parent.GetComponent<RectTransform>();
				//	parent.sizeDelta = new Vector2(parent.sizeDelta.x, parent.sizeDelta.y + yincrease); 
				for (int i = 0; i < UIelements.Count; i++)
				{
					UIelements[i].position = new Vector3(UIelements[i].position.x, UIelements[i].position.y + yincrease, UIelements[i].position.z);
				}
				if (currentOptionLines != null)
				{
					if (currentOptionLines.Count > 0)
					{
						for (int i = 0; i < currentOptionLines.Count; i++)
						{
							currentOptionLines[i].position = new Vector3(currentOptionLines[i].position.x, currentOptionLines[i].position.y + yincrease, currentOptionLines[i].position.z);
						}
					}
				}
				return currentLineRect;
			}
			

		}
		else
		{
			Debug.Log("there is no character information found");
		}
		return null;
	}

	private void RemoveLines()
	{
		if (currentOptionLines == null)
			return;

		RectTransform currentLine = currentOptionLines[currentOptionPosition];
		currentLine.localPosition = new Vector3(100f, -90f, 0f);
		currentOptionLines[currentOptionPosition].gameObject.GetComponent<TextMeshProUGUI>().color = StandardTextColour;

		UIelements.Add(currentLine);
		currentOptionLines.RemoveAt(currentOptionPosition);

		for (int i = 0; i < currentOptionLines.Count; i++)
		{
			float yincrease;
			switch (currentOptionLines[i].GetComponent<TextMeshProUGUI>().textInfo.lineCount)
			{
				case 1:
					yincrease = currentOptionLines[i].rect.height * 2.5f;
					break;
				case 2:
					yincrease = currentOptionLines[i].rect.height * 1.5f;
					break;
				default:
					yincrease = currentOptionLines[i].rect.height;
					break;
			}
			//move elemts of list up by the height of the new line
			//RectTransform parent = currentLine.transform.parent.GetComponent<RectTransform>();
			//	parent.sizeDelta = new Vector2(parent.sizeDelta.x, parent.sizeDelta.y + yincrease); 
			for (int j = 0; j < UIelements.Count; j++)
			{
				UIelements[j].position = new Vector3(UIelements[j].position.x, UIelements[j].position.y - yincrease, UIelements[j].position.z);
			}
		}
		currentOptionLines = null;

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
