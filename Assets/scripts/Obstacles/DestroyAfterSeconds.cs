using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour
{
   
        public float destroyTime = 0.6f;

        void Start()
        {
            Destroy(gameObject, destroyTime);
        }
    
}
