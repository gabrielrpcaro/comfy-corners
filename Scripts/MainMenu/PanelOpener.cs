using UnityEngine;
using UnityEngine.UI;

public class PanelOpener : MonoBehaviour
{
    public GameObject optionsPanel;

    public void ToggleOptionsMenu()
    {
        optionsPanel.SetActive(!optionsPanel.activeSelf);
    }

}
