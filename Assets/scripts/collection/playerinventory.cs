using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class playerinventory : MonoBehaviour
{
    public int numdonuts { get; private set; }
    public UnityEvent<playerinventory> OnDonutCollected;

    public void donutcollected()
    {
        numdonuts++;
        OnDonutCollected.Invoke(this);
    }

    public void barriercollided()
    {
        numdonuts = numdonuts - 3; 
        OnDonutCollected.Invoke(this);
    }
}
