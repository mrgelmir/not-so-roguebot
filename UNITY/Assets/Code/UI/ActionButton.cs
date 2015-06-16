using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour
{
    [SerializeField]
    private Text label;
    [SerializeField]
    private Button button;

    private BaseGameAction actionReference;
    private System.Action<BaseGameAction> onClick;

    public void Setup(BaseGameAction action, System.Action<BaseGameAction> onClick)
    {
        this.onClick = onClick;
        actionReference = action;

        name = "Action - " + action.Data.ActionName;
        label.text = action.Data.ActionName;

        button.onClick.AddListener(Clicked);
    }

    private void Clicked()
    {
        if(onClick != null)
            onClick(actionReference);
    }

}
