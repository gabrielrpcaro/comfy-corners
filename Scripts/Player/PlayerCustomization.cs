using System.Collections.Generic;
using UnityEngine;

public class PlayerCustomization : MonoBehaviour
{
    public enum BodyPartType
    {
        Head,
        Acessory,
        Hair,
        Torso,
        Legs,
        Feet,
        Body
    }

    [System.Serializable]
    public class BodyPart
    {
        public BodyPartType partType;
        public GameObject partObject;
        public string style;
        public string color;
        public Animator animator;
    }

    public BodyPart[] bodyParts = new BodyPart[7];
    public bool isMainPlayer;
    public bool isSitting { get; set; }
    public BodyPartCustomization bodyPartCustomization;

    void Start()
    {
        if (bodyPartCustomization != null)
        {
            ApplyCustomization(bodyPartCustomization);
        }
        InitializeBodyParts();
    }

    void InitializeBodyParts()
    {
        foreach (BodyPart part in bodyParts)
        {
            InitializeBodyPart(part);
        }
        ReinitializeAnimations();
    }

    public void InitializeBodyPart(BodyPart part)
    {
        if (part.partObject == null)
        {
            Debug.LogError($"Part object for {part.partType} is not assigned.");
            return;
        }
        if (!string.IsNullOrEmpty(part.style) && !string.IsNullOrEmpty(part.color))
        {
            part.animator = part.partObject.GetComponent<Animator>() ?? part.partObject.AddComponent<Animator>();
            LoadAnimations(part);
            LoadStaticSprite(part);
        }
        else
        {
            part.partObject.SetActive(false);
        }
    }

    void LoadAnimations(BodyPart part)
    {
        if (part.animator == null)
        {
            Debug.LogError($"Animator component missing for {part.partType}. Cannot load animations.");
            return;
        }
        string resourcePath = $"Sprites/Player/{part.partType}/{part.style}/{part.color}";
        AnimationClip[] clips = Resources.LoadAll<AnimationClip>(resourcePath);

        if (clips.Length > 0)
        {
            AnimatorOverrideController animatorOverride = new AnimatorOverrideController(part.animator.runtimeAnimatorController);

            foreach (var clip in clips)
            {
                animatorOverride[clip.name] = clip;
            }

            part.animator.runtimeAnimatorController = animatorOverride;
        }
        else
        {
            Debug.LogWarning($"No animations found for {part.partType} with style {part.style} and color {part.color}");
        }
    }

    void LoadStaticSprite(BodyPart part)
    {
        string resourcePath = $"Sprites/Player/{part.partType}/{part.style}/{part.color}/static";
        Sprite staticSprite = Resources.Load<Sprite>(resourcePath);

        if (staticSprite != null)
        {
            SpriteRenderer spriteRenderer = part.partObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                // Deactivate and reactivate the SpriteRenderer to ensure it updates immediately
                spriteRenderer.enabled = false;
                spriteRenderer.sprite = staticSprite;
                spriteRenderer.enabled = true;
            }
            else
            {
                Debug.LogError($"SpriteRenderer is null for {part.partType}. Cannot assign static sprite.");
            }
        }
        else
        {
            Debug.LogWarning($"No static sprite found for {part.partType} with style {part.style} and color {part.color}");
        }
    }

    public void ApplyCustomization(BodyPartCustomization customization)
    {
        foreach (var customizationData in customization.bodyParts)
        {
            var bodyPart = System.Array.Find(bodyParts, part => part.partType == customizationData.partType);
            if (bodyPart != null)
            {
                bodyPart.style = customizationData.style;
                bodyPart.color = customizationData.color;
                InitializeBodyPart(bodyPart);
            }
        }
        ReinitializeAnimations();
    }

    public void SetMovementState(bool isRunning, bool isMoving)
    {
        foreach (BodyPart part in bodyParts)
        {
            if (part.animator != null)
            {
                part.animator.SetBool("isMoving", isMoving);
                part.animator.SetBool("isRunning", isRunning);
            }
        }
    }

    public void SetSittingState(bool isSitting)
    {
        foreach (BodyPart part in bodyParts)
        {
            if (part.animator != null)
            {
                part.animator.SetBool("isSitting", isSitting);
            }
        }
        this.isSitting = isSitting;
    }

    public void SetDirection(float moveX, float moveY)
    {
        foreach (BodyPart part in bodyParts)
        {
            if (part.animator != null)
            {
                part.animator.SetFloat("MoveX", moveX);
                part.animator.SetFloat("MoveY", moveY);
            }
        }
    }

    public void ReinitializeAnimations()
    {
        foreach (BodyPart part in bodyParts)
        {
            if (part.animator != null)
            {
                // Disable and enable the animator to reset its state
                part.animator.enabled = false;
                part.animator.enabled = true;

                // Reload animations
                LoadAnimations(part);
                LoadStaticSprite(part);

                // Reset all animator parameters to their default values
                part.animator.Rebind();
                part.animator.Update(0f);
            }
        }
    }
}