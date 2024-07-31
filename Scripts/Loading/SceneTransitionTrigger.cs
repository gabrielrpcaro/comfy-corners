using UnityEngine;

public class SceneTransitionTrigger : MonoBehaviour
{
    [Header("Scene Transition Settings")]
    public string targetScene;
    public Vector3 targetSpawnPosition;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("ops, bati");
            // Update the target spawn position in the SceneTransitionManager
            SceneTransitionManager.instance.SetSpawnPosition(targetSpawnPosition);

            // Transition to the target scene
            SceneTransitionManager.instance.TransitionToLoadingScene(targetScene);
        }
    }
}
