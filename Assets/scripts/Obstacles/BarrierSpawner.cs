using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Burst.CompilerServices;
//using Unity.Mathematics;
using UnityEngine;

[System.Serializable]
public class Obstaclesettings
{
    public GameObject prefab;
    public float yoffset;
}

public class BarrierSpawner : MonoBehaviour
{
    public Obstaclesettings[] barrier; //refer to barrier obj
    public GameObject spawnsmoke;
    
    public Transform car;  // to track car position
    public float spawnradius = 30f;    // radius where in barrier shall spawn
    public LayerMask roadlayer;   //layer to detect road

    private Transform[] spawnpoints;  //list of spawnpoints
    private HashSet<Transform> usepoints = new HashSet<Transform>();  // to check same spawn point dnst keep spawning barriers again

    private void Start()
    {
        spawnpoints = GetComponentsInChildren<Transform>();
    }

    private void Update()

    {
        /*point is a variable of type transform(tracks position of an object)
         initially its pointing to the parent gameobject ka Transform so then it checks if point == transform and since this 
        is true, the parent gameobejct gets skipped
    

        */
        foreach (Transform point in spawnpoints)   //check each spawpoint
        {
            if (point == transform) continue; // skip parent, to skip parent gameobject cuz it isnt a spawnpoint
                                              // because when u get all the transforms inside the spawnpoints game object through:spawnPoints = GetComponentsInChildren<Transform>();
                                              //it makes a list like [0] BarrierSpawnPoints.transform  ← this is the parent itself
                                              //[1] SpawnPoint1.transform
                                              //  [2] SpawnPoint2.transform

            float distance = Vector3.Distance(car.position, point.position); //distance between the car and spawnpoint

            if (distance < spawnradius && !usepoints.Contains(point) && IsOnRoad(point.position))
            {



                // Pass spawn position and spawn point rotation
                StartCoroutine(spawnthesmoke(point.position, point.rotation));
                usepoints.Add(point);

                // prevent spawning twice at same point
            }
        }
    }

    private IEnumerator spawnthesmoke(Vector3 spawnpos, Quaternion spawnrot)
    {
        // Spawn smoke slightly above the ground
        if (spawnsmoke != null)
        {
            Instantiate(spawnsmoke, spawnpos + Vector3.up * 0.5f, spawnrot);
        }

        yield return new WaitForSeconds(0.3f);

        Obstaclesettings obstacle = barrier[UnityEngine.Random.Range(0, barrier.Length)];
        Vector3 adjustedpos = spawnpos + Vector3.up * obstacle.yoffset;
        Instantiate(obstacle.prefab, adjustedpos, spawnrot);
    }
    

    private bool IsOnRoad(Vector3 position)
    {
        // Optional: Raycast down to check if spawn point is above road
        if (Physics.Raycast(position + Vector3.up * 2f, Vector3.down, 10f, roadlayer))
        {
            return true;
        }
        return false;

    }

}

