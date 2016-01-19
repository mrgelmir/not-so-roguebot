using UnityEngine;
using System.Collections;

[System.Serializable]
public class ActionData 
{
    [SerializeField] private string actionName;
	public string ActionName { get { return actionName; } }

    [SerializeField] private bool showAction;
    public bool ShowAction { get { return showAction; } }
	
    // add things like icons, tooltips, etc here
    
}
