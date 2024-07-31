using UnityEngine;
using Cinemachine;

public class MainGameSceneController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera mainSceneVirtualCamera;
    private void Start()
    {
        DataPersistenceManager.instance.LoadGame();

        CameraManager.instance.SwitchToVirtualCamera(mainSceneVirtualCamera);
        PlayerManager.instance.SetVirtualCameraFollow(mainSceneVirtualCamera);
    }
}
