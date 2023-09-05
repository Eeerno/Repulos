using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class PlaneControl : MonoBehaviour
{
    [SerializeField] GameObject plane;
    [SerializeField] float speed = 2;
    [SerializeField] float angularSpeed = 80;
    [SerializeField, Range(0, 90)] float planeMaxTiltAngle = 65;
    [Space]
    [SerializeField] float turnCoefficient = 2;
    [SerializeField] float turboDuration = 2;
    [SerializeField] float turboMultiplier = 3;
    [SerializeField] float turboCooldown = 4;
    [Space]
    [SerializeField] LayerMask heightRaycastMask;
    [SerializeField] float rayAngle = 1;
    [SerializeField] float rayLimit = 15;
    [SerializeField] float planeHeight = 8;
    [SerializeField] float heightAdjustFactor = 8;
    [Space]
    [SerializeField] Locations locations;
    [SerializeField] CameraLookAt cameraLookAt;

    ParticleSystem smokeParticleSystem;

    float speedMultiplier = 1;
    float turboLeft = 0;
    float travelled = 0;
    float bounceHeight = 0;
    Vector3 lastPos;

    Vector3 currentEulerAngles;
    Quaternion currentRotation;
    Vector3 rayDirection;
    Vector3 point;

    int points = 0;

    private void Start()
    {
        turboLeft = 0 - turboCooldown;
        smokeParticleSystem = plane.GetComponent<ParticleSystem>();
        //smokeParticleSystem.duration = turboDuration;
    }

    void Update()
    {

        float horizontal = Input.GetAxis("Horizontal");     //-1 v 1 v 0 attól függõen h jobbra balra gombokat nyomom-e, itt most negálni is kell h jó legyen
        //float turn = horizontal * angularSpeed * Time.deltaTime;

        //modifying the Vector3, based on input multiplied by speed and time
        currentEulerAngles = plane.transform.rotation.eulerAngles;
        currentEulerAngles += new Vector3(0, 0, -horizontal) * Time.deltaTime * angularSpeed;

        float currentZ = currentEulerAngles.z;
        if (currentZ < 180 && currentZ > planeMaxTiltAngle) currentEulerAngles.z = planeMaxTiltAngle;
        if (currentZ > 180 && currentZ < 360 - planeMaxTiltAngle) currentEulerAngles.z = 360 - planeMaxTiltAngle;

        //moving the value of the Vector3 into Quanternion.eulerAngle format
        currentRotation.eulerAngles = currentEulerAngles;

        //apply the Quaternion.eulerAngles change to the gameObject
        plane.transform.rotation = currentRotation;



        // Plane turn
        //currentZ * Time.deltaTime
        if (currentZ < 180) currentZ *= -1;
        if (currentZ > 180) currentZ = Mathf.Abs(360 - currentZ);
        transform.Rotate(0, currentZ / turnCoefficient * Time.deltaTime, 0, Space.World);

        // Plane movement
        bool boostPressed = Input.GetKeyDown(KeyCode.Space);
        if (boostPressed && turboLeft <= 0 - turboCooldown)
        {
            turboLeft = turboDuration;
            speedMultiplier = turboMultiplier;
        }

        lastPos = transform.position;
        Vector3 newPos = lastPos;
        Vector3 bounce = lastPos;
        
        //// Calculate position 
        Vector3 direction = transform.forward;
        newPos += direction * speed * speedMultiplier * Time.deltaTime;     // Need to know new position to calculate bounce
       
        //// Calculate bounce
        travelled += Vector3.Distance(lastPos, newPos) * 0.3f;
        bounceHeight = Mathf.Sin(travelled) * 0.015f;
        bounce.y += bounceHeight;
        plane.transform.position = bounce;

        //// Raycast to find optimal height
        //TODO figure this out
        //Vector3 rayDirection = transform.rotation.eulerAngles;
        rayDirection = transform.forward;
        rayDirection = Vector3.RotateTowards(rayDirection, -Vector3.up, rayAngle, 0);
        //Debug.Log(rayDirection);

        RaycastHit hitInfo;
        
        bool hit = Physics.Raycast(transform.position, rayDirection, out hitInfo, rayLimit, heightRaycastMask);
        //Debug.Log(hit);

        if (hit)
        {
            //Vector3 point = hitInfo.point;
            point = hitInfo.point;
            //Debug.Log(point.y);
            newPos.y = Mathf.Lerp(newPos.y, point.y + planeHeight, heightAdjustFactor * Time.deltaTime);
        }

        //// Set new position
        transform.position = newPos;
        travelled %= 2*Mathf.PI;

        // Handle turbo boost
        if (turboLeft > 0 - turboCooldown)
        {
            turboLeft -= Time.deltaTime;
            smokeParticleSystem.Play();
        }
        if (turboLeft < 0 && speedMultiplier > 1)
        {
            speedMultiplier = Mathf.Lerp(speedMultiplier, 1, Time.deltaTime);
            smokeParticleSystem.Stop();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        cameraLookAt.addNewTarget(other.transform.position);
        Debug.Log("Entered collider");
    }

    void OnTriggerExit(Collider other)
    {
        cameraLookAt.resetCamera();
        Debug.Log("Exited collider");
    }

    //void OnTriggerExit(Collider other)
    //{
    //    points++;
    //    string s = points > 1 ? "!" : "s!";
    //    Debug.Log($"Gained a point! You have {points} point{s}");
    //    locations.assignNewTarget();
    //}

    private void OnDrawGizmos()
    {
        // Draw height check raycast point
        /*
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(point, 1);
        */
    }
}

