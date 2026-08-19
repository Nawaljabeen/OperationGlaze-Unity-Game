using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System.Collections.Generic;

using Unity.VisualScripting;

public class CarUIEffect : MonoBehaviour
{
    [Header("Score UI")] 
    [SerializeField] private UnityEngine.UI.Image impactimage;
    [SerializeField] private GameObject scoretext;

    [SerializeField] private float rangeX = 100f;
    [SerializeField] private float rangeY = 200f;
    [SerializeField] private float lifetime = 2.0f;
    [SerializeField] private float fadeDuration = 1f;

    [Header("Play UI")]
    [SerializeField] public GameObject playbutton;
    [SerializeField] public GameObject replaybutton;
    [SerializeField] private GameObject bubble_text;
    public carcontrol carcontrolscript; // here i learnt that public and serialize field is same thing - public already serializes so if im
    //using a priv var then ill hv to srialize it but thats not tbr case for public

    
    private float popupduration = 1.5f;
    private float origfontsize;
    private float leastfontsize = 0f;

    
    private Coroutine impactCoroutine;

    void Start()
    {
      
        if (impactimage != null) impactimage.gameObject.SetActive(false);
        if (carcontrolscript != null)
        {
            carcontrolscript.enabled = false;

            // Explicitly shut off all car audio & effects
            carcontrolscript.ToggleEngineSound(false);
            carcontrolscript.togglesmoke(false);
            carcontrolscript.toggleskidsound(false);
        }

        if (playbutton != null)
        {
            playbutton.SetActive(true);
            if (replaybutton != null) replaybutton.SetActive(false);

            Button playbtn = playbutton.GetComponent<Button>();
            if (playbtn != null)
            {
                playbtn.onClick.AddListener(startgame);
            }
        }
    }




    void startgame()
    {
        if (playbutton != null) playbutton.SetActive(false);

        if (carcontrolscript != null)
        {
            carcontrolscript.enabled = true;
            carcontrolscript.ToggleEngineSound(true);
        }
    }
    public void ShowBarrierImpactUI()
    {
        if (impactCoroutine != null) StopCoroutine(impactCoroutine);
        impactimage.gameObject.SetActive(true);
        Color c = impactimage.color;
        c.a = 0.41f;
        impactimage.color = c;
        impactCoroutine = StartCoroutine(FadeImpact());

        
    }

    private IEnumerator FadeImpact()
    {
        float elapsed = 0f;
        Color c = impactimage.color;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(0.41f, 0f, elapsed / fadeDuration);
            impactimage.color = c;
            yield return null;
        }
        c.a = 0f;
        impactimage.color = c;
        impactimage.gameObject.SetActive(false);
        impactCoroutine = null;
    }
    public void spawnrandomtext(string scoretodisplay, Color color)
    {
        if (scoretext == null) return;
        
        GameObject newtext = Instantiate(scoretext, transform);
        float randomx = Random.Range(-rangeX, rangeX);
        float randomy = Random.Range(-rangeY, rangeY);

        newtext.GetComponent<RectTransform>().anchoredPosition = new Vector2(randomx, randomy);
        TextMeshProUGUI tmpro = newtext.GetComponent<TextMeshProUGUI>();
        origfontsize = tmpro.fontSize;
        if (tmpro != null)
        {
            tmpro.text = scoretodisplay;
            tmpro.color = color;
            StartCoroutine(textpopupnaway(tmpro));
        }
        Destroy(newtext.gameObject, lifetime);

                    
    }

    private IEnumerator textpopupnaway(TextMeshProUGUI tmpro)
    {
        origfontsize = tmpro.fontSize;
        tmpro.fontSize = origfontsize + 10f;

        yield return new WaitForSeconds(0.2f);
        float elapsed = 0.5f; //should be local so every popup has its own elapsedtime, if u make it global all popups will share it

        while (elapsed < popupduration)
        {
            float t = elapsed/popupduration;
            elapsed = elapsed + Time.deltaTime;

            tmpro.fontSize = Mathf.Lerp(origfontsize + 10, leastfontsize, t);
            yield return null;
        }
        
    }
}