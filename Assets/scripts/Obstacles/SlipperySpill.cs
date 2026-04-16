using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;


public class SlipperySpill : MonoBehaviour
{
    public float spinforce = 2000f;
    [SerializeField] private TrailRenderer[] skidmarks = new TrailRenderer[2];
    public Gradient newgradient;

    [Header("Impact UI")]
    [SerializeField] private Image impactimage;
    [SerializeField] private Sprite[] images;
    [SerializeField] private float fadeDuration = 1f;

    private Gradient orig_gradient;
    Vector3 rotation = Vector3.zero;
    private void Start()
    {

        impactimage.gameObject.SetActive(false);
        foreach (var skidmark in skidmarks)
        {
            orig_gradient = skidmark.colorGradient;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            Rigidbody rb = other.GetComponent<Rigidbody>();
           // StartCoroutine(slippyspill(rb));
            StartCoroutine(Smooth360(rb));
            
        }
    }



    

    /* IEnumerator slippery(Rigidbody rb)
      {
          float original_angular_drag = rb.angularDrag;

          if (rb != null)
          {
              rb.angularDrag = 2f;
              rb.AddTorque(Vector3.up * spinforce, ForceMode.Impulse);
              yield return new WaitForSeconds(0.3f);
              rb.AddTorque(Vector3.up * (-spinforce * 2), ForceMode.Impulse);




              Debug.Log("crossed the slippy");
          }

          yield return new WaitForSeconds(0.5f);
          rb.angularDrag = original_angular_drag;

      }
 */
    IEnumerator slippyspill(Rigidbody rb)
    {

        float angdrag = rb.angularDrag;
        if (rb != null)
        {

            foreach (var skidmark in skidmarks)
            {

                skidmark.Clear();
                skidmark.colorGradient = newgradient;
                skidmark.emitting = true;
            }
           
            showsplat();
            rb.angularDrag = 6f;
            


            float elapsed = 0f;
            Color c = impactimage.color;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                c.a = Mathf.Lerp(0.61f, 0f, elapsed / fadeDuration);
                impactimage.color = c;
                yield return null;
            }

            c.a = 0f;
            impactimage.color = c;
            impactimage.gameObject.SetActive(false);

            yield return new WaitForSeconds(2f);
            rb.angularDrag = angdrag;
            setrealgradient();
         
        }


    }
    IEnumerator Smooth360(Rigidbody rb)
    {
      
        Quaternion startRotation = rb.transform.rotation;
        

        float elapsed = 0f;
        float duration = 1f; 

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float percent = elapsed / duration;

           
            float currentAngle = percent * 360f;
            rb.transform.rotation = startRotation * Quaternion.Euler(0, currentAngle, 0);

            yield return null;
        }

        
        rb.transform.rotation = startRotation;

        rb.isKinematic = false;
    }

    private void setrealgradient()
    {
        foreach (var skidmark in skidmarks)
        {
            skidmark.colorGradient = orig_gradient;
        }
    }


    private void showsplat()
    {
        if (images.Length > 0)
        {
            int randomindex = UnityEngine.Random.Range(0, images.Length);
            impactimage.sprite = images[randomindex];
            impactimage.gameObject.SetActive(true);
            Color c = impactimage.color;
            c.a = 0.41f;
            impactimage.color = c;
        }
        
    }

    
}

    