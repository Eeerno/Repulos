using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.FilePathAttribute;

public class PlaneControl : MonoBehaviour
{
    [SerializeField] GameObject plane;
    [SerializeField] float speed = 15;
    [SerializeField] float angularSpeed = 80;
    [SerializeField, Range(0, 90)] float planeMaxTiltAngle = 65;
    [Space]
    [SerializeField] float turnCoefficient = 2;
    [SerializeField] float turboDuration = 2;
    [SerializeField] float turboMultiplier = 3;
    [SerializeField] float turboCooldown = 0;
    [Space]
    [SerializeField] LayerMask altitudeRaycastMask;
    [SerializeField] float rayAngle = 1;
    [SerializeField] float rayLimit = 15;
    [SerializeField] float planeAltitude = 8;
    [SerializeField] float altitudeAdjustFactor = 8;

    ParticleSystem smokeParticleSystem;

    float speedMultiplier = 1;
    float turboLeft = 0;
    float travelled = 0;
    float bounceHeight = 0;
    Vector3 lastPos;

    Vector3 currentEulerAngles;
    Quaternion currentRotation;
    Vector3 point;

    private void Start()
    {
        turboLeft = 0 - turboCooldown;
        smokeParticleSystem = plane.GetComponent<ParticleSystem>();
    }

    void Update()
    {
        // Plane tilt
        float horizontal = Input.GetAxis("Horizontal");

            //modifying the Vector3, based on input multiplied by speed and time
        currentEulerAngles = plane.transform.rotation.eulerAngles;
        currentEulerAngles += new Vector3(0, 0, -horizontal) * Time.deltaTime * angularSpeed;

            //limiting the tilt to the max angle
        float currentZ = currentEulerAngles.z;
        if (currentZ < 180 && currentZ > planeMaxTiltAngle) currentEulerAngles.z = planeMaxTiltAngle;
        if (currentZ > 180 && currentZ < 360 - planeMaxTiltAngle) currentEulerAngles.z = 360 - planeMaxTiltAngle;

            //moving the value of the Vector3 into Quanternion.eulerAngle format
        currentRotation.eulerAngles = currentEulerAngles;

            //apply the Quaternion.eulerAngles change to the gameObject
        plane.transform.rotation = currentRotation;



        // Plane turn
        if (currentZ < 180) currentZ *= -1;
        if (currentZ > 180) currentZ = Mathf.Abs(360 - currentZ);
        transform.Rotate(0, currentZ / turnCoefficient * Time.deltaTime, 0, Space.World);

        // Plane movement
            //Pre-movement stuff
        bool boostPressed = Input.GetKeyDown(KeyCode.Space);
        if (boostPressed && turboLeft <= 0 - turboCooldown)
        {
            turboLeft = turboDuration;
            speedMultiplier = turboMultiplier;
        }

        lastPos = transform.position;
        Vector3 newPos = lastPos;
        Vector3 bounce = lastPos;

            // 1 Calculate position ( need to know new position to calculate bounce )
        Vector3 direction = transform.forward;
        newPos += direction * speed * speedMultiplier * Time.deltaTime;
       
            // 2 Calculate bounce
        travelled += Vector3.Distance(lastPos, newPos) * 0.3f;
        bounceHeight = Mathf.Sin(travelled) * 0.015f;
        bounce.y += bounceHeight;
        plane.transform.position = bounce;

            // 3 Raycast to find optimal height
        Vector3 rayDirection = transform.forward;
        rayDirection = Vector3.RotateTowards(rayDirection, -Vector3.up, rayAngle, 0);

        RaycastHit hitInfo;
        bool hit = Physics.Raycast(transform.position, rayDirection, out hitInfo, rayLimit, altitudeRaycastMask);

        if (hit)
        {
            point = hitInfo.point;
            newPos.y = Mathf.Lerp(newPos.y, point.y + planeAltitude, altitudeAdjustFactor * Time.deltaTime);
        }

            // 4 Set new position
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

    private void OnDrawGizmos()
    {
        // Draw altitude checking raycast point
        //Gizmos.color = Color.red;
        //Gizmos.DrawSphere(point, 1);
    }
}

