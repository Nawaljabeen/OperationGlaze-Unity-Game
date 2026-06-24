using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerCountdown : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timertext;

    private float currenttime = 80f;
    private bool active = true;

    public cutscenecontroller cutscenescript;
    private void Update()
    {
        if (!active) return;
        currenttime = currenttime - Time.deltaTime;

        updatetimerui();
        if (currenttime <= 0)
        {
            stoptimer();
            cutscenescript.Triggercamchange();
        }




    }
    public void stoptimer()
    {
        active = false;
        currenttime = 0f;
        updatetimerui();
    }

    public void updatetimerui()
    {
        TimeSpan t = TimeSpan.FromSeconds(currenttime);
        timertext.text = t.ToString(@"mm\:ss");
    }
}



