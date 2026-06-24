using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class cutscenecontroller : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera Vcam_main;
    [SerializeField] CinemachineVirtualCamera Vcam_cutscene;
    [SerializeField] string targettag = "initiator";
    [SerializeField] PostProcessVolume blurvolume;


    private bool istriggered = false;
    
    private float targetvolume = 1f;
    private float percentage = 5f;

 
    private void OnTriggerEnter(Collider other) {

        if (other.gameObject.CompareTag(targettag) && !istriggered)
        {
            
            Triggercamchange();
            TriggerBlur();
        }
    }

    public void Triggercamchange()
    {
        istriggered = true;
        Vcam_cutscene.Priority = 15;
    }

    public void TriggerBlur()
    {
        targetvolume = 1f;
       
    }

    private void Update()

    {
        if (istriggered)
        {
            blurvolume.weight = Mathf.Lerp(blurvolume.weight, targetvolume, percentage * Time.deltaTime);
        }
    }
}
