using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class ConfirmationPopupMenu : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    public void ActivateMenu(UnityAction confirmAction, UnityAction cancelAction)
    {
        this.gameObject.SetActive(true);

        // remove any existing listeners just to make sure there aren't any previous ones hanging around
        // note - this only removes listeners added through code
        confirmButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();

        // assign the onClick listeners
        confirmButton.onClick.AddListener(() => {
            DeactivateMenu();
            confirmAction();
        });
        cancelButton.onClick.AddListener(() => {
            DeactivateMenu();
            cancelAction();
        });
    }

    private void DeactivateMenu() 
    {
        this.gameObject.SetActive(false);
    }
}