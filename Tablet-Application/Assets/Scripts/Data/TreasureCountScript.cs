using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreasureCountScript : MonoBehaviour {

    public int treasureCount = 0;

    public Text myText;

	// Update is called once per frame
	void FixedUpdate () {
        myText.text = treasureCount.ToString();
    }
}
