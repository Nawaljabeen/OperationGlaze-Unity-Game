using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collisionaudiocontroller : MonoBehaviour
{
    public static collisionaudiocontroller instance;


    public AudioSource audioSource;
    public AudioClip[] audioclips;

    public string[] tags;

    [Header("Donut pitch streak")]
    public float basepitch = 1f;
    public float pitchinc = 0.2f;   // increase per donut in streak
    public float maxpitch = 2f;
    public float resetdist = 15f;

    
    private Transform player;
    private int streak = 0;
    private Vector3 lastpickuppos;
    private bool lastpicked = false;
    [System.Serializable]
    public class soundeffect {
        public string tag;
        public AudioClip[] clips;
    }


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        } //refers to object jispe ye script hai
        else
        {
            Destroy (gameObject);
        }

        GameObject gobj = GameObject.FindGameObjectWithTag("Player");
        if(gobj != null) player = gobj.transform; 
    }

    public void comparetagsandplaysound(string tag)
    {
       
        for (int i = 0; i < tags.Length; i++)
        {
            

            if (tags[i] == tag)
            {
                

         
                if (tag == "donut")
                {
                    if (player == null)
                    {
                        GameObject go = GameObject.FindGameObjectWithTag("Player");
                        if (go != null) player = go.transform;
                    }

                    if (player != null && lastpicked)
                    {
                        float dist = Vector3.Distance(player.position, lastpickuppos);
                        if (dist > resetdist)
                        {
                            streak = 0; 
                        }
                    }

                    streak++;
                    float pitch = basepitch + (streak - 1) * pitchinc;
                    pitch = Mathf.Min(pitch, maxpitch);

                    audioSource.pitch = pitch;
                    audioSource.PlayOneShot(audioclips[i]);

                    if (player != null)
                    {
                        lastpickuppos = player.position;
                        lastpicked = true;
                    }
                }
                else
                {
                    audioSource.pitch = basepitch;
                    if (i == 3)
                    {
                        int randindex = UnityEngine.Random.Range(3, 5);
                        if (audioclips.Length > randindex)
                        {
                            audioSource.PlayOneShot(audioclips[randindex]);
                        }
                    }
                    else
                    {

                        audioSource.PlayOneShot(audioclips[i]);
                    }
                }

                return; 
            }
        }
    }
}
