using System.Collections;
using UnityEngine;

public class barrierflyeffect : MonoBehaviour
{
    public float spinspeed = 720f;
    public float launchForce = 50f;
    public float launchAngleDegrees = 30f;
    public float disappearDelay = 4f;
    public float angleVariation = 5f;

    private Rigidbody rb;
    private Collider col;
    private TrailRenderer trail;
    private bool launched = false;
    private Quaternion startrotation;
    private bool isspinning = false;
    void Start()
    {
        startrotation = transform.rotation;
    }
    private void Update()
    {
        if (isspinning)
        {
            // Rotate around local X axis (like a wheel)
            transform.Rotate(Vector3.forward * spinspeed * Time.deltaTime, Space.Self);
        }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        trail = GetComponent<TrailRenderer>();
        ResetDonut();
    }

    void OnTriggerEnter(Collider other)
    {
        if (launched) return;
        if (!other.CompareTag("Player")) return;

        launched = true;
        Vector3 awayFromCar = (transform.position - other.transform.position).normalized;
        LaunchInArc(awayFromCar); // Use car’s facing direction
    }

    void LaunchInArc(Vector3 forwardDirection)
    {
        if (trail != null)
            trail.enabled = true;

        col.isTrigger = false;

        rb.isKinematic = false;
        rb.useGravity = true;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.position += Vector3.up * 0.2f;

        float angleRad = launchAngleDegrees * Mathf.Deg2Rad;

        // Combine forward and upward to get arc
        Vector3 arcDirection = forwardDirection.normalized * Mathf.Cos(angleRad) + Vector3.up * Mathf.Sin(angleRad);

        // Add a little yaw variation for realism
        float randomYaw = Random.Range(-angleVariation, angleVariation);
        arcDirection = Quaternion.Euler(0, randomYaw, 0) * arcDirection;

        // DO NOT normalize here — we want full strength
        rb.AddForce(arcDirection * launchForce, ForceMode.Impulse);
        isspinning = true;

        //rotation
        // Vector3 torque = transform.* spinspeed * Time.deltaTime; //angular velocity and torque use radians so we r using degtorad
        // rb.AddTorque(torque, ForceMode.Impulse);


        StartCoroutine(FadeAndDisable());
    }

    IEnumerator FadeAndDisable()
    {
        yield return new WaitForSeconds(disappearDelay);
        ResetDonut();
        gameObject.SetActive(false);
    }

    void ResetDonut()
    {
        launched = false;
        isspinning = false;

        rb.useGravity = false;
        rb.isKinematic = true;

      //  if (col != null) col.isTrigger = true;
        if (trail != null) trail.enabled = false;
    }
}
