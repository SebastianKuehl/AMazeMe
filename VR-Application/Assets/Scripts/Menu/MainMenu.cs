using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

	public void PlayGame() {
		GameObject menuObj = GameObject.Find ("Settings");
		DataScript script = menuObj.GetComponent<DataScript> ();

		Dropdown dropdownMaze = GameObject.Find ("LabyrinthDropdown").GetComponent<Dropdown>();
		script.mazeSize = int.Parse(dropdownMaze.options [dropdownMaze.value].text.Substring(0,2));

		Dropdown dropdownTreasure = GameObject.Find ("TreasureDropdown").GetComponent<Dropdown>();
		string text = dropdownTreasure.options [dropdownTreasure.value].text;
		script.treasures = int.Parse(text.Substring(0,text.Length));

        Slider treasureChance = GameObject.Find("TreasureSlider").GetComponent<Slider>();
        script.treasureChance = Mathf.Round(treasureChance.value * 100f) / 100f;

		Toggle controller = GameObject.Find ("Toggle").GetComponent<Toggle> ();
		script.useController = controller.isOn;

		SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex + 1);
	}

	public void QuitGame() {
		Debug.Log ("Quit!");
		Application.Quit ();
	}
}
