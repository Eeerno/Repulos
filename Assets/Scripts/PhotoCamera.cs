using TMPro;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

class PhotoCamera : MonoBehaviour
{
    [SerializeField] GameObject PhotoCounterUI;
    [SerializeField] Locations locations;
    [SerializeField] CameraLookAt cameraLookAt;
    [SerializeField] TMP_Text completedJobsText;
    [SerializeField] TMP_Text photoCounterText;
    [SerializeField] public float photoCooldown = 2;
    private int photosNeeded = 0;
    private int photosTaken = 0;
    private float photoElapsed = 0;
    private int completedJobs = 0;

    private void Start()
    {
        completedJobsText.text = completedJobs.ToString();
        hidePhotoCounter();
    }

    void Update()
    {
        photoElapsed += Time.deltaTime;
        completedJobsText.text = completedJobs.ToString();
        // Handle Photo Camera
        bool pictureTaken = Input.GetKeyDown(KeyCode.Return);
        if (cameraLookAt.needToLook)
        {
            photoCounterText.text = photosTaken.ToString() + "/" + photosNeeded.ToString();
            if (pictureTaken && photoElapsed > photoCooldown)
            {
                TakePhoto();
                if (photosTaken >= photosNeeded)
                {
                    markJobAsCompleted();
                    cameraLookAt.resetCamera();
                    Debug.Log($"Oké, kész vagyunk {completedJobs} fotósorozattal.");
                    locations.assignNewTarget();
                    Invoke(nameof(hidePhotoCounter), 1);
                }
                else
                {
                    Debug.Log($"Szuper, csináljunk még {photosNeeded - photosTaken} ilyen jó képet.");
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        cameraLookAt.addNewCamTarget(other.transform.position);
        showPhotoCounter();
    }

    void OnTriggerExit(Collider other)
    {
        cameraLookAt.resetCamera();
    }

    public void TakePhoto()
    {
        photosTaken++;
        photoElapsed = 0;
        photoCounterText.text = photosTaken.ToString() + "/" + photosNeeded.ToString();
    }

    public void addNewPhotoJob(int required)
    {
        photosTaken = 0;
        photosNeeded = required;
    }

    public void markJobAsCompleted()
    {
        completedJobs++;
        completedJobsText.text = completedJobs.ToString();
    }

    public void showPhotoCounter()
    {
        PhotoCounterUI.SetActive(true);
    }

    public void hidePhotoCounter()
    {
        PhotoCounterUI.SetActive(false);
    }

    public int PhotosNeeded => photosNeeded;

}
