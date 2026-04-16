using UnityEngine;
using Cinemachine;
using System.Collections;
using UnityEngine.UI;
using Unity.VisualScripting;

public class CarImpactCameraEffect : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCam;
    public float impactForwardOffset = -1f; // Negative Z moves camera closer to the car
    public float impactMoveSpeed = 10f;
    public float returnMoveSpeed = 5f;

    public float slowdownFactor = 0.6f;
    public float slowdownDuration = 0.4f;

    public GameObject impactsmoke;


    private CinemachineTransposer transposer;
    private Vector3 defaultOffset;
    private bool isImpacting = false;

    private int activeSlowdowns = 0;
    private float originalAcceleration;
    private float originalMaxSpeed;

    private carcontrol carControlScript;

    [Header("Impact UI")]
    [SerializeField] private Image impactimage;
    [SerializeField] private float fadeDuration = 1f;
    
    private Coroutine impactCoroutine;
    private Coroutine slowdownCoroutine;
    void Start()
    {
        if (virtualCam == null)
            virtualCam = FindObjectOfType<CinemachineVirtualCamera>();

        transposer = virtualCam.GetCinemachineComponent<CinemachineTransposer>();
        defaultOffset = transposer.m_FollowOffset;

        carControlScript = GetComponent<carcontrol>();
        if (impactimage != null)
        {
            impactimage.gameObject.SetActive(false);
        }

        originalAcceleration = carControlScript.acceleration;
        originalMaxSpeed = carControlScript.maxspeed;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("barrier1") || collision.gameObject.CompareTag("barrier2"))
        {


            handleimpact(collision);

        }
    }

    IEnumerator ImpactEffect()
    {
        isImpacting = true;
        Vector3 impactOffset = defaultOffset + new Vector3(0, 0, impactForwardOffset);

        // Move camera forward
        while (Vector3.Distance(transposer.m_FollowOffset, impactOffset) > 0.01f)
        {
            transposer.m_FollowOffset = Vector3.Lerp(transposer.m_FollowOffset, impactOffset, Time.deltaTime * impactMoveSpeed);
            yield return null;
        }

        // Move camera back
        while (Vector3.Distance(transposer.m_FollowOffset, defaultOffset) > 0.01f)
        {
            transposer.m_FollowOffset = Vector3.Lerp(transposer.m_FollowOffset, defaultOffset, Time.deltaTime * returnMoveSpeed);
            yield return null;
        }

        transposer.m_FollowOffset = defaultOffset;
        isImpacting = false;
    }

    private IEnumerator SlowdownEffect()
    {
        activeSlowdowns++; // increment count of active slowdowns

        // Apply slowdown
        carControlScript.acceleration = originalAcceleration * slowdownFactor;
        carControlScript.maxspeed = originalMaxSpeed * slowdownFactor;

        yield return new WaitForSeconds(slowdownDuration);

        activeSlowdowns--; // this slowdown ended

        // Only restore if no other slowdowns active
        if (activeSlowdowns <= 0)
        {
            carControlScript.acceleration = originalAcceleration;
            carControlScript.maxspeed = originalMaxSpeed;
            activeSlowdowns = 0; // safety
        }
    }

  
    private void ShowImpactUI()
    {
        if (impactCoroutine != null)
            StopCoroutine(impactCoroutine);

        impactimage.gameObject.SetActive(true);

        Color c = impactimage.color;
        c.a = 0.41f;
        impactimage.color = c;
        impactCoroutine = StartCoroutine(FadeImpact());
    }

    private IEnumerator FadeImpact()  
    {


        float elapsed = 0f;
        Color c = impactimage.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(0.41f, 0f, elapsed / fadeDuration);
            impactimage.color = c;
            yield return null;
        }

        c.a = 0f;
        impactimage.color = c;
        impactimage.gameObject.SetActive(false);

        impactCoroutine = null;

    }

    private void SpawnSmoke(Vector3 spawnpos, Quaternion spawnrot)
    {
        if (impactsmoke != null)
        {
            
            Vector3 finalPos = spawnpos + (Vector3.up * 0.5f);
            GameObject smokeInstance = Instantiate(impactsmoke, finalPos, spawnrot);

            
            Destroy(smokeInstance, 2f);
        }
        else
        {
            Debug.LogWarning("smoke prefab msising");
        }
    }
    private void handleimpact(Collision collision)
    {
        // Impact UI
        ShowImpactUI();

        // Camera effect
        if (!isImpacting)
            StartCoroutine(ImpactEffect());

        // Physics bounce-back
        Rigidbody rb = carControlScript.rb;
        Vector3 forward = transform.forward;

        float collisionForce = collision.relativeVelocity.magnitude;
        float bounceMagnitude = Mathf.Clamp(collisionForce * 0.3f, 0f, 5f);
        rb.AddForce(-forward * bounceMagnitude, ForceMode.VelocityChange);

        // Clamp backward velocity
        float maxBackward = -3f;
        float backwardSpeed = Vector3.Dot(rb.velocity, forward);
        if (backwardSpeed < maxBackward)
        {
            rb.velocity -= forward * (backwardSpeed - maxBackward);
        }

        // Reduce lateral slide slightly
        Vector3 localVel = transform.InverseTransformDirection(rb.velocity);
        localVel.x *= 0.2f;
        rb.velocity = transform.TransformDirection(localVel);

        // Slowdown (overlap-safe)
        StartCoroutine(SlowdownEffect());

        // Smoke effect
        ContactPoint contact = collision.contacts[0];

        Vector3 spawnpos = contact.point;

        Quaternion spawnrot = Quaternion.LookRotation(contact.normal);

        SpawnSmoke(spawnpos, spawnrot);



       
    }
}
