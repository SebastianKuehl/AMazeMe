using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TreasureChanceScript : MonoBehaviour {
    
    
	// Update is called once per frame
	void Update () {
        float value = GameObject.Find("TreasureSlider").GetComponent<Slider>().value;
        GetComponent<TextMeshProUGUI>().text = (int) (value * 100) + "% for each treasure\n   to show up on map:";
	}
}
