using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ActionPanel : MonoBehaviour
{
    [SerializeField] private ActionButton buttonPrefab;

    public void Reset()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void AddActionButton(BaseGameAction action, System.Action<BaseGameAction> onClick)
    {
        ActionButton newButton = Instantiate<ActionButton>(buttonPrefab);
        newButton.Setup(action, onClick);
        newButton.transform.SetParent(transform, false);
    }
}
