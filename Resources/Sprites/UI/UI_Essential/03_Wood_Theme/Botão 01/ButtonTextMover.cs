using UnityEngine;
using TMPro; // Import the TextMeshPro namespace
using UnityEngine.UI;
using UnityEngine.EventSystems;

[ExecuteAlways]
public class ButtonTextMover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private TextMeshProUGUI buttonText; // Reference to the TextMeshProUGUI component inside the button
    private Button button; // Reference to the Button component
    private Vector4 originalMargins; // Store the original margins
    private bool wasInteractable; // Track the previous interactable state

    void Start()
    {
        Initialize();
        wasInteractable = button.interactable;
        UpdateMargin();
    }

    void Initialize()
    {
        // Find the Button and TextMeshProUGUI component within the button's children
        button = GetComponent<Button>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>();

        if (button == null)
        {
            Debug.LogError("Button component not found on this GameObject.");
            return;
        }

        if (buttonText == null)
        {
            Debug.LogError("TextMeshProUGUI component not found in children of the button.");
            return;
        }

        originalMargins = buttonText.margin;
        Debug.Log("Original margins: " + originalMargins);
    }

    void Update()
    {
        // Check if the interactable state has changed
        if (button.interactable != wasInteractable)
        {
            UpdateMargin();
            wasInteractable = button.interactable;
        }
    }

    private void UpdateMargin()
    {
        if (button != null && buttonText != null)
        {
            if (!button.interactable)
            {
                SetTextMarginBottom(-25f); // Set margin to -25 when button is not interactable
            }
            else
            {
                ResetTextMargin();
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button.interactable)
        {
            SetTextMarginBottom(-14f); // Adjust margin for Highlighted state
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (button.interactable)
        {
            ResetTextMargin(); // Reset margin when the pointer exits
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (button.interactable)
        {
            SetTextMarginBottom(-35f); // Adjust margin for Pressed state
        }
    }

    private void SetTextMarginBottom(float marginBottom)
    {
        Vector4 newMargins = buttonText.margin;
        newMargins.w = marginBottom; // Apply the negative value to the bottom margin (w component)
        buttonText.margin = newMargins;
    }

    private void ResetTextMargin()
    {
        buttonText.margin = originalMargins; // Reset to the original margins
    }
}
