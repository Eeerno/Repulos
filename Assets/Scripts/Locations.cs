using UnityEngine;

public class Locations : MonoBehaviour
{
    [SerializeField] GameObject[] locations;
    public GameObject currentTarget = null;
    int currentIndex = 0;

    private void Start()
    {
        assignNewTarget();
    }

    void Update()
    {
        
    }

    public void assignNewTarget()
    {
        if (currentTarget != null)
        {
            currentTarget.SetActive(false);
        }

        int randomIndex = currentIndex;
        while ( currentIndex == randomIndex ) {                     // Try not to get the same as last time 
            randomIndex = Random.Range(0, locations.Length);
        }
        currentIndex = randomIndex;
        GameObject newTarget = locations[currentIndex];
        newTarget.SetActive(true);
        currentTarget = newTarget;
        Debug.Log("Your next target is: " + locations[currentIndex].name);
    }

    private void OnDrawGizmos()
    {
        if (currentTarget != null)
        {
            Gizmos.color = Color.red;
            Vector3 gizmoPos = new( currentTarget.transform.position.x,
                                    currentTarget.transform.position.y + 10,
                                    currentTarget.transform.position.z );
            Gizmos.DrawSphere(gizmoPos, 5);
        }
    }
}