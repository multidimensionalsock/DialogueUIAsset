using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ScrollDialogue : MonoBehaviour
{
	[SerializeField] PlayerInput m_input;
	ScrollRect m_rect;
    void Start()
    {
        //m_input= GetComponent<PlayerInput>();
		m_input.currentActionMap.FindAction("ScrollDialogueBar").performed += Scroll;
		m_rect= GetComponent<ScrollRect>();
	}

    public void Scroll(InputAction.CallbackContext context)
	{
		
		float scrollDirection = context.ReadValue<float>();
		Debug.Log(scrollDirection);
		//m_rect.onValueChanged(ScrollDirection);

		m_rect.verticalNormalizedPosition = scrollDirection * 5f;

	}

}
