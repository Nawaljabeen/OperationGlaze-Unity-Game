using System.Collections;
using UnityEngine;

public class donut : MonoBehaviour
{
    public float spinspeed = 720f;
    public float launchForce = 50f;
    public float launchAngleDegrees = 30f;
    public float disappearDelay = 4f;
    public float angleVariation = 5f;
    public Rigidbody carRB;
    Vector3 startposition, launchvelocity;
    private float timer = 0f;
    private float gravity = 9.81f;


    private Rigidbody rb;

    private Collider col;
    private TrailRenderer trail;


    private bool hascollided = false;
    public bool hasbeencollected = false;
    private bool launched = false;
    private Quaternion startrotation;
    private bool isspinning = false;
     void Start()
    {
        
    }
    private void Update()
    {
        if (isspinning)
        {
            // Rotate around local X axis (like a wheel)
            transform.Rotate(Vector3.right * spinspeed * Time.deltaTime, Space.Self);
        }
        if (launched)
        {
            timer += Time.deltaTime;
            Vector3 currentOffset = (launchvelocity * timer) + (0.5f * Vector3.down * gravity * timer * timer);
            transform.position = startposition + currentOffset;
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

        
        if (!hascollided)
        {

            playerinventory Playerinventory = other.GetComponent<playerinventory>();
            if (Playerinventory != null)
            {
                Playerinventory.donutcollected();
                hascollided=true;
                isspinning = true;
            }


        }

        if (launched) return;
        if (!other.CompareTag("Player")) return;

        launched = true;
        collisionaudiocontroller.instance.comparetagsandplaysound(gameObject.tag);
        
       // Vector3 awayFromCar = (transform.position - other.transform.position).normalized;
        Vector3 cardirection = carRB.velocity.normalized;
        LaunchInArc(cardirection); // Use car’s facing direction
    }

    void LaunchInArc(Vector3 forwardDirection)
    {
        if (trail != null)
            trail.enabled = true;
        rb.isKinematic = true;
        startposition = transform.position;
        timer = 0f;

        Transform childmodel = transform.GetChild(0); // 0 means first child
        Animator childanim = GetComponentInChildren<Animator>();
        if (childanim != null)
        {
            childanim.enabled = false;
        }
        childmodel.localRotation = Quaternion.identity;
        float angleRad = launchAngleDegrees * Mathf.Deg2Rad;
        Vector3 arcDirection = forwardDirection.normalized * Mathf.Cos(angleRad) + Vector3.up * Mathf.Sin(angleRad);
     
        // Add a little yaw variation for realism
        float randomYaw = Random.Range(-angleVariation, angleVariation);
        arcDirection = Quaternion.Euler(0, randomYaw, 0) * arcDirection;

        launchvelocity = arcDirection * launchForce;

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
        

        if (col != null) col.isTrigger = true;
        if (trail != null) trail.enabled = false;
    }
}
