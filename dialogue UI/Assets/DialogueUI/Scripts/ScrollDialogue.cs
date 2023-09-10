using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Yarn.Analysis;

public class ScrollDialogue : MonoBehaviour
{
	[SerializeField] PlayerInput m_input;
	Coroutine m_scroll;
	RectTransform m_contentRectTransform;

    void Start()
    {
        //m_input= GetComponent<PlayerInput>();
		m_input.currentActionMap.FindAction("ScrollDialogueBar").performed += Scroll;
		m_input.currentActionMap.FindAction("ScrollDialogueBar").canceled += StopScroll;
		m_contentRectTransform = transform.GetChild(0).GetComponent<RectTransform>();
	}

    public void Scroll(InputAction.CallbackContext context)
	{
		
		float scrollDirection = context.ReadValue<float>();
		m_scroll = StartCoroutine(ScrollCoroutine(scrollDirection));

		m_contentRectTransform.offsetMax = new Vector2(0, m_contentRectTransform.offsetMax.y - scrollDirection); //top
		m_contentRectTransform.offsetMin = new Vector2(0, m_contentRectTransform.offsetMin.y + scrollDirection); // bottom
	}

	public void StopScroll(InputAction.CallbackContext context)
	{
		StopCoroutine(m_scroll);
		m_scroll= null;
	}

	IEnumerator ScrollCoroutine(float scrollDirection)
	{
		while (1 == 1)
		{
			m_contentRectTransform.offsetMax = new Vector2(0, m_contentRectTransform.offsetMax.y + scrollDirection); //top
			m_contentRectTransform.offsetMin = new Vector2(0, m_contentRectTransform.offsetMin.y - scrollDirection); // bottom

			yield return new WaitForFixedUpdate();
		}

		

	}


}


