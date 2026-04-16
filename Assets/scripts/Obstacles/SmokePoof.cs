using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokePoof : MonoBehaviour
{
    private SpriteRenderer sr;
    public float duration = 0.5f;
    private float timer = 0f;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        // Scale up
        float scale = Mathf.Lerp(1f, 1.5f, timer / duration);
        transform.localScale = Vector3.one * scale;

        // Fade out
        float alpha = Mathf.Lerp(1f, 0f, timer / duration);
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);

        // Destroy after done
        if (timer >= duration)
        {
            Destroy(gameObject);
        }
    }
}
