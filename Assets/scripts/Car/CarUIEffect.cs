using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UIElements;

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
    [SerializeField] private GameObject playbutton;
    [SerializeField] private GameObject replaybutton;
    [SerializeField] private GameObject bubble_text;
    private carcontrol carcontrolscript;

    
    private float popupduration = 1.5f;
    private float origfontsize;
    private float leastfontsize = 0f;

    
    private Coroutine impactCoroutine;

    void Start()
    {
        carcontrolscript = GetComponent<carcontrol>();
        if (impactimage != null) impactimage.gameObject.SetActive(false);
        carcontrolscript.gameObject.SetActive(false);
    }
    private void startgame()
    {
        if (playbutton != null)
        {
            playbutton.SetActive(true);
            carcontrolscript.gameObject.SetActive(true);
        }
        if (replaybutton != null)
        {
            replaybutton.SetActive(false);
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