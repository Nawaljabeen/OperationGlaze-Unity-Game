using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class carcontrol : MonoBehaviour
{
    [Header("References")]
    [SerializeField] public Rigidbody rb;
    [SerializeField] public Transform carBody;
    [SerializeField] public float rollAmount = 10f;
    [SerializeField] public float smooth = 1f;
    [SerializeField] public float returnSpeed = 3f;

    [SerializeField] private Transform[] raypoints;
    [SerializeField] private LayerMask driveable;
    [SerializeField] private Transform accelpoint;
    [SerializeField] private GameObject[] tires = new GameObject[4];
    [SerializeField] private GameObject[] frontyreparents = new GameObject[2];
    [SerializeField] private TrailRenderer[] skidmarks = new TrailRenderer[2];
    [SerializeField] private ParticleSystem[] skidsmoke = new ParticleSystem[2];
    [SerializeField] private AudioSource enginesound, skidsound;


    [Header("Suspension settings")]
    [SerializeField] private float springstiffness;
    [SerializeField] private float damperstiffness;
    [SerializeField] private float restlength;
    [SerializeField] private float springtravel;
    [SerializeField] private float wheelradius;
    [SerializeField] private AnimationCurve turningcurve;
    [SerializeField] private float dragcoeff = 1f;


    private int[] wheelsonground = new int[4];
    private bool isgrounded = false;

    [Header("Input")]
    private float moveinp = 0;  // to hold players inout
    private float steerinp = 0;

    [Header("Car Settings")]
    [SerializeField] public float acceleration = 25f;
    [SerializeField] public float deceleration = 10f;
    [SerializeField] public float maxspeed = 100f;
    [SerializeField] private float steerstrength = 15f;

    [Header("Visuals")]
    [SerializeField] private float tirerotspeed = 3000f;
    [SerializeField] private float maxsteerangle = 30f;
    [SerializeField] private float minskidveloc = 10f;

    [Header("Audio")]
    [SerializeField]
    [Range(0, 1)] private float minpitch = 1f;
    [SerializeField, Range(1, 5)] private float maxpitch = 5f;

    private Vector3 currentcarveloc = Vector3.zero;
    private float carvelocratio = 0;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();

    }

    private void FixedUpdate()
    {

        suspension();
        groundcheck();
        calculatecarveloc();
        movement();
        visuals();
        if (!isgrounded)
        {
            rb.AddForce(transform.forward * 30f, ForceMode.Force); // helps push forward
        }
        Enginesound();
    }
    private void Update()
    {
        carbodyvisuals(rb, carBody, rollAmount, smooth);
        getplayerinput();
    }

    private void movement()
    {
        if (isgrounded)
        {
            accel();
            decel();
            turn();
            sidedrag();

        }
    }

    private void accel()
    {
        rb.AddForceAtPosition(acceleration * moveinp * transform.forward, accelpoint.position, ForceMode.Acceleration);
    }
    private void decel()
    {
        rb.AddForceAtPosition(deceleration * moveinp * -transform.forward, accelpoint.position, ForceMode.Acceleration);
    }
    private void turn()
    {
        rb.AddRelativeTorque(steerstrength * steerinp * turningcurve.Evaluate(Mathf.Abs(carvelocratio))
            * Mathf.Sign(carvelocratio) * rb.transform.up, ForceMode.Acceleration);


    }

    private void sidedrag()
    {
        float currentsidewayspeed = currentcarveloc.x;
        float dragmag = -currentsidewayspeed * dragcoeff;
        Vector3 dragforce = transform.right * dragmag;
        rb.AddForceAtPosition(dragforce, rb.worldCenterOfMass, ForceMode.Acceleration);
        if (!isgrounded) return;

    }
    private void visuals()
    {
        tirevisuals();
        vfx();
    }

    private void tirevisuals()
    {
        float steerangle = maxsteerangle * steerinp;
        for (int i = 0; i < tires.Length; i++)
        {
            if (i < 2)
            {
                tires[i].transform.Rotate(Vector3.right, tirerotspeed * carvelocratio * Time.deltaTime, Space.Self);
                frontyreparents[i].transform.localEulerAngles = new Vector3(frontyreparents[i].transform.localEulerAngles.x,
                    steerangle, frontyreparents[i].transform.localEulerAngles.z);
            }
            else
            {
                tires[i].transform.Rotate(Vector3.right, tirerotspeed * moveinp * Time.deltaTime, Space.Self);

            }
        }
    }

 
        void carbodyvisuals(Rigidbody carRigidbody, Transform body, float maxRoll, float lerpSpeed)
    {
   
        Vector3 localVel = body.InverseTransformDirection(carRigidbody.velocity);

        float lateralAcceleration = localVel.x;
        float steer = Input.GetAxis("Horizontal");
        float targetZ = (-steer * rollAmount) + (-lateralAcceleration * 0.5f);

        // Clamp so it never over-tilts
        targetZ = Mathf.Clamp(targetZ, -rollAmount, rollAmount);


        Quaternion targetRot = Quaternion.Euler(0f, 0f, targetZ);
        float speed = Mathf.Abs(steer) > 0.01f ? smooth : returnSpeed;
        body.localRotation = Quaternion.Lerp(body.localRotation, targetRot, Time.deltaTime * speed);
    }

private void vfx()
    {
        if (isgrounded && Mathf.Abs(currentcarveloc.x) > minskidveloc)
        {
            toggleskidmarks(true);
            togglesmoke(true);
            toggleskidsound(true);
        }
        else
        {
            toggleskidmarks(false);
            togglesmoke(false);
            toggleskidsound(false);
        }
    }
    private void toggleskidmarks(bool toggle)
    {
        foreach (var skidmark in skidmarks)
        {

            skidmark.emitting = true;
        }
    }

    private void togglesmoke(bool toggle)
    {
        foreach (var smoke in skidsmoke)
        {


            if (toggle)
            {
                smoke.Play();
            }
            else
            {
                smoke.Stop();
            }
        }
    }



    //audio 

    private void Enginesound()
    {
        enginesound.pitch = Mathf.Lerp(minpitch, maxpitch, Mathf.Abs(carvelocratio));

    }
    private void toggleskidsound(bool toggle)
    {
        skidsound.mute = !toggle;
    }
    private void setyrepos(GameObject tire, Vector3 targetposition)
    {
        tire.transform.position = targetposition;
    }

    private void getplayerinput()
    {
        moveinp = 1f;
        steerinp = Input.GetAxis("Horizontal");
    }

    private void suspension()
    {
        for (int i = 0; i < raypoints.Length; i++)
        {

            RaycastHit hit;
            float maxlen = restlength + springtravel;


            if (Physics.Raycast(raypoints[i].position, -raypoints[i].up, out hit, maxlen + wheelradius, driveable))
            {
                wheelsonground[i] = 1;
                float currentspringlen = hit.distance - wheelradius;
                float springcompression = (restlength - currentspringlen) / springtravel;



                float springvelocity = Vector3.Dot(rb.GetPointVelocity(hit.point), raypoints[i].up);
                float dampforce = damperstiffness * springvelocity;

                float springforce = springstiffness * springcompression;
                float netforce = springforce - dampforce;

                rb.AddForceAtPosition(netforce * raypoints[i].up, raypoints[i].position);

                if (Vector3.Angle(hit.normal, Vector3.up) > 15f || Vector3.Angle(hit.normal, Vector3.up) < -15f) // 30 degrees or steeper
                {
                    Vector3 jumpDir = (transform.forward + Vector3.up).normalized;
                    rb.AddForce(jumpDir * 500f, ForceMode.Impulse);
                }

                //visuals 
                setyrepos(tires[i], hit.point + raypoints[i].up * wheelradius);

                Debug.DrawLine(raypoints[i].position, hit.point, Color.red);


            }
            else

            {
                // Push the tire to its "unsprung" position visually
                setyrepos(tires[i], raypoints[i].position - raypoints[i].up * (restlength + springtravel - wheelradius));

                wheelsonground[i] = 0;
                Debug.DrawLine(raypoints[i].position, raypoints[i].position + (wheelradius + maxlen) * -raypoints[i].up, Color.green);
            }
        }



    }


    private void groundcheck()
    {
        int tempgroundedwheels = 0;
        for (int i = 0; i < wheelsonground.Length; i++)
        {
            tempgroundedwheels += wheelsonground[i];
        }

        if (tempgroundedwheels > 1)
        {
            isgrounded = true;
        }
        else
        {
            isgrounded = false;
        }

    }

    private void calculatecarveloc()
    {
        currentcarveloc = transform.InverseTransformDirection(rb.velocity);
        carvelocratio = currentcarveloc.z / maxspeed;

    }
}


  /*  private void OnTriggerEnter(Collider other)
     {
         if (other.CompareTag("barrier") || other.transform.parent != null && other.transform.parent.CompareTag("barrier"))
         {
             StartCoroutine(SoftBarrierHit());

             // Optional: Let barrier destroy itself
             Destroy(other.transform.root.gameObject);
         }
     }
    /*
     private IEnumerator SlowDownBriefly()
     {
         //float originalAccel = acceleration;
         float originalMaxSpeed = maxspeed;

      //   acceleration *= 0.4f;
         maxspeed *= 0.8f;

         yield return new WaitForSeconds(0.5f);  // adjust as needed

        // acceleration = originalAccel;
         maxspeed = originalMaxSpeed;
     }  */

   /* private IEnumerator SoftBarrierHit()
    {
        // --- Slight slowdown ---
        float originalMaxSpeed = maxspeed;
        maxspeed *= 0.85f; // reduce to 85% for a moment

        // --- Camera Z Damping tweak ---
        var transposer = vCam.GetCinemachineComponent<CinemachineTransposer>();
        if (transposer != null)
        {
            transposer.m_ZDamping = 2.5f; // increase lag effect
            yield return new WaitForSeconds(0.15f); // hold lag
            transposer.m_ZDamping = defaultZDamping; // restore
        }

        // --- Speed restore ---
        yield return new WaitForSeconds(0.2f);
        maxspeed = originalMaxSpeed;
    }
    */





