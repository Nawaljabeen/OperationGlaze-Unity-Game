using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryUI : MonoBehaviour
{

    private TextMeshProUGUI NumDonutstext;
    void Start()
    {
        NumDonutstext = GetComponent<TextMeshProUGUI>();
    }
    

    public void updatedonuttext(playerinventory Playerinventory)
    {
        NumDonutstext.text = Playerinventory.numdonuts.ToString();
    }
   
}
