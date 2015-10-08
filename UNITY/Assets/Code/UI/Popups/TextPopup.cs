using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextPopup : MonoBehaviour
{
	[SerializeField]
	private GameObject Visual;
	[SerializeField]
	private Text title;
	[SerializeField]
	private Text body;

	private bool visible = false;
	private System.Action onDismiss;

	public void Show(string titleText, string bodyText, System.Action onDismiss)
	{
		this.onDismiss = onDismiss;

		title.text = titleText;
		body.text = bodyText;

		visible = true;
		SetVisibility();
	}

	public void Dismiss()
	{
		visible = false;
		SetVisibility();

		if(onDismiss != null)
		{
			// call and unsubscribe
			onDismiss();
			onDismiss = null;
		}
	}

	protected void Start()
	{
		if(!visible)
		{
			// if this is spawned, make sure it is hidden, unless Show is called
			Dismiss();
		}
	}

	private void SetVisibility()
	{
		Visual.SetActive(visible);
	}
}
