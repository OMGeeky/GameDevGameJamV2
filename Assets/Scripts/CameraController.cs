using Cinemachine;

using UnityEngine;


public class CameraController : MonoBehaviour
{
    [SerializeField] private Character root;
    [SerializeField] private CinemachineVirtualCamera vCam;

    private void OnEnable()
    {
        if ( vCam == null )
            vCam = GetComponent<CinemachineVirtualCamera>();
            
        root.Spawned += RootOnSpawned;
    }

    private void RootOnSpawned( Character obj )
    {
        var objTransform = obj.transform;
        vCam.Follow = objTransform;
        vCam.LookAt = objTransform;
    }
}