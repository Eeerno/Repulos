using UnityEngine;

public class Locations : MonoBehaviour
{
    [SerializeField] GameObject[] locations;
    [SerializeField] MapScript map;
    [SerializeField] PhotoCamera photoCamera;
    [SerializeField] int minPhotosNeeded = 2;
    [SerializeField] int maxPhotosNeeded = 5;
    GameObject currentTarget = null;
    int currentIndex = 0;

    private void Start()
    {
        assignNewTarget();
    }

    public void assignNewTarget()
    {
        if (currentTarget != null)
        {
            currentTarget.SetActive(false);
        }

        int randomIndex = currentIndex;
        do {                                                   
            randomIndex = Random.Range(0, locations.Length);
        } while (currentIndex == randomIndex);                   // Try not to get the same as last time 
        currentIndex = randomIndex;
        
        GameObject newTarget = locations[currentIndex];
        newTarget.SetActive(true);
        currentTarget = newTarget;
        map.assignNewTarget(newTarget);
        Debug.Log("A következõ célpont: " + locations[currentIndex].name);
        int random = Random.Range(minPhotosNeeded, maxPhotosNeeded + 1);
        photoCamera.addNewPhotoJob(random);
        Debug.Log($"{photoCamera.PhotosNeeded} képet szeretnék róla!" );
    }
}