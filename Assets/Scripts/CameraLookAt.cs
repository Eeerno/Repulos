using UnityEngine;
using static UnityEditor.PlayerSettings;

class CameraLookAt : MonoBehaviour
{
    [SerializeField] float angularSpeedTowards;
    [SerializeField] float angularSpeedReset;
    [SerializeField] Transform cameraPivot;
    Quaternion defaultRotation;
    Vector3 targetPosition;
    public bool needToLook;

    void Start()
    {
        defaultRotation = cameraPivot.localRotation;
        needToLook = false;  
    }
        
    void Update()
    {
        if (needToLook)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetPosition - cameraPivot.position, Vector3.up);
            cameraPivot.rotation = Quaternion.RotateTowards(cameraPivot.rotation, targetRotation, angularSpeedTowards * Time.deltaTime);
        }
        else
        {
            cameraPivot.localRotation = Quaternion.RotateTowards(cameraPivot.localRotation, defaultRotation, angularSpeedReset * Time.deltaTime);
        }
    }

    public void addNewCamTarget(Vector3 tPos)
    {
        targetPosition = tPos;
        needToLook = true;
    }

    public void resetCamera()
    {
        needToLook = false;
    }
}
