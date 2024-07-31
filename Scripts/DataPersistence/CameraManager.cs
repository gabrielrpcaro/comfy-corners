using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;
    private CinemachineVirtualCamera currentVirtualCamera;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SwitchToVirtualCamera(CinemachineVirtualCamera newVirtualCamera)
    {
        if (currentVirtualCamera != null)
        {
            currentVirtualCamera.Priority = 0;
        }

        currentVirtualCamera = newVirtualCamera;
        currentVirtualCamera.Priority = 10;
    }
}
