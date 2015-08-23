using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Button))]
public class LoadLevelButton : MonoBehaviour 
{
	[SerializeField]
	private Button button;

	[SerializeField]
	private int LevelToLoad = 0;

	protected void OnEnable()
	{

		if (button == null)
			button = GetComponent<Button>();

		if(button != null)
			button.onClick.AddListener(OnClick);
	}

	protected void OnDisable()
	{
		if (button == null)
			button = GetComponent<Button>();

		if (button != null)
			button.onClick.RemoveListener(OnClick);
	}

	private void OnClick()
	{
		Application.LoadLevel(LevelToLoad);
	}
}
