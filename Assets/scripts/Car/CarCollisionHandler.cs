using UnityEngine;
using System.Collections;

public class CarCollisionHandler : MonoBehaviour
{
    private carcontrol carControlScript;
    public float slowdownFactor = 0.6f;
    public float slowdownDuration = 0.4f;
    private int activeSlowdowns = 0;
    private float originalAcceleration;
    private float originalMaxSpeed;

    // References to the other scripts
    public CarCameraEffect cameraScript;
    public CarUIEffect uiScript;
    public CarVFXEffect vfxScript;


    Color redcol = new Color(0.8867924f, 0.3865076f, 0.3896675f, 1f);
    Color lightbluecol = new Color(0.514151f, 0.858662f, 1f, 1f);
    void Start()
    {

      
        carControlScript = GetComponent<carcontrol>();
        originalAcceleration = carControlScript.acceleration;
        originalMaxSpeed = carControlScript.maxspeed;

       
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("barrier1") || collision.gameObject.CompareTag("barrier2"))
        {
            handleimpactwithbarrier(collision);
        }
        

    }

    private void OnTriggerEnter(Collider other)
    {

        donut donutscript = other.transform.root.GetComponent<donut>();
        if (uiScript != null && donutscript !=null && !donutscript.hasbeencollected)
        { 

           
            if (other.transform.root.gameObject.CompareTag("donut"))
            {

                donutscript.hasbeencollected = true;
                    uiScript.spawnrandomtext("+1", lightbluecol);

                
            }
            else if (other.transform.root.gameObject.CompareTag("bigdonut"))
            {
                donutscript.hasbeencollected = true;
                uiScript.spawnrandomtext("+5",lightbluecol);
                
            }
        }
    }


    private void handleimpactwithbarrier(Collision collision)
    {

        if (uiScript != null) uiScript.ShowBarrierImpactUI();
        uiScript.spawnrandomtext("-5",redcol);
        if (cameraScript != null) cameraScript.TriggerCameraEffect();
        if (vfxScript != null) vfxScript.HandleSmoke(collision);


        Rigidbody rb = carControlScript.rb;
        Vector3 forward = transform.forward;


        float collisionForce = collision.relativeVelocity.magnitude;
        float bounceMagnitude = Mathf.Clamp(collisionForce * 0.3f, 0f, 5f);
        rb.AddForce(-forward * bounceMagnitude, ForceMode.VelocityChange);

        float maxBackward = -3f;
        float backwardSpeed = Vector3.Dot(rb.velocity, forward);
        if (backwardSpeed < maxBackward)
        {
            rb.velocity -= forward * (backwardSpeed - maxBackward);
        }

        Vector3 localVel = transform.InverseTransformDirection(rb.velocity);
        localVel.x *= 0.2f;
        rb.velocity = transform.TransformDirection(localVel);

        StartCoroutine(SlowdownEffect());
    }

    private IEnumerator SlowdownEffect()
    {
        activeSlowdowns++;
        carControlScript.acceleration = originalAcceleration * slowdownFactor;
        carControlScript.maxspeed = originalMaxSpeed * slowdownFactor;
        yield return new WaitForSeconds(slowdownDuration);
        activeSlowdowns--;
        if (activeSlowdowns <= 0)
        {
            carControlScript.acceleration = originalAcceleration;
            carControlScript.maxspeed = originalMaxSpeed;
            activeSlowdowns = 0;
        }
    }

}