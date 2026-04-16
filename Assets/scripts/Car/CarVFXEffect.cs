using UnityEngine;

public class CarVFXEffect : MonoBehaviour
{
    public GameObject impactsmoke;

    public void HandleSmoke(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];
        Vector3 spawnpos = contact.point;
        Quaternion spawnrot = Quaternion.LookRotation(contact.normal);
        SpawnSmoke(spawnpos, spawnrot);
    }

    private void SpawnSmoke(Vector3 spawnpos, Quaternion spawnrot)
    {
        if (impactsmoke != null)
        {
            Vector3 finalPos = spawnpos + (Vector3.up * 0.5f);
            GameObject smokeInstance = Instantiate(impactsmoke, finalPos, spawnrot);
            Destroy(smokeInstance, 2f);
        }
    }
}