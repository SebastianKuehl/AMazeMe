using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timetracker : MonoBehaviour {

	private float currentTime;
	private int minute, seconds;

	void Update() {
		currentTime = Time.time;
		seconds = (int) currentTime;
		minute = (int)(seconds / 60);
		seconds %= 60;
		GetComponent<TextMeshProUGUI> ().text = "Time: " + minute + ":" + (seconds/10 < 1 ? "0" : "") + seconds;
	}
}
