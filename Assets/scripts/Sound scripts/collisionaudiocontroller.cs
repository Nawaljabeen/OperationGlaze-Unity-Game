using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collisionaudiocontroller : MonoBehaviour
{
    public static collisionaudiocontroller instance;


    public AudioSource audioSource;
    public soundeffect[] soundEffects;


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
    public class soundeffect
    {
        public string tag;
        public AudioClip[] clips;
    }


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } //refers to object jispe ye script hai
        else
        {
            Destroy(gameObject);
        }

        GameObject gobj = GameObject.FindGameObjectWithTag("Player");
        if (gobj != null) player = gobj.transform;
    }

    public void comparetagsandplaysound(string tag)
    {

        foreach (var effect in soundEffects)
        {
            if (effect.tag == tag)
            {
                handlepitchandplay(effect, tag);
                return;
            }
        }

    }
    private void handlepitchandplay(soundeffect effect, string tag)
    {
        if (tag == "donut")
        {
            updatedonutstreak();
            float pitch = basepitch + (streak - 1) * pitchinc;
            audioSource.pitch = Mathf.Min(pitch, maxpitch);
        }
        else
        {
            audioSource.pitch = basepitch;
        }
        if (effect.clips.Length > 0)// randomziation logic for picking out of many audioclips
        {
            int randindx = UnityEngine.Random.Range(0, effect.clips.Length);
            audioSource.PlayOneShot(effect.clips[randindx]);
        }
    }

    private void updatedonutstreak()
    {
        if (player != null && lastpicked)
        {
            float dist = Vector3.Distance(player.position, lastpickuppos);
            if (dist > resetdist)
            {
                streak = 0;
            }
        }
        streak++;

        if(player != null)
        {
            lastpickuppos = player.position;
            lastpicked = true;  
        }
    }
    
}
