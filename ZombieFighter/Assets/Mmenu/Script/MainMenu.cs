using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
	public GameObject Buttons;
	public GameObject CreditsPanel;
	public GameObject QuitPanel;
	public GameObject Options;
	public GameObject PlayMenu;
	public Slider audiosl;
	public Slider graphicsl;
	public Toggle fullscreen;
	public string SceneName;

	void Start (){
		QualitySettings.SetQualityLevel ((int)PlayerPrefs.GetFloat("Quality"));
		AudioListener.volume = PlayerPrefs.GetFloat("Volume");
		int qualityLevel = QualitySettings.GetQualityLevel();

	}
	void Update (){

	}
	public void InGame(bool a){
		if (a == true) {
			SceneManager.LoadScene("Scene_1");
		} else {
			//continue
		}
	}
public void Play(bool a){
		if (a == true) {
			PlayMenu.SetActive(a);
			Buttons.SetActive(!a);
			//Animation pl = PlayMenu.GetComponent<Animation>();
			//pl.Play("EnterPlayMenu");
		}else {
			PlayMenu.SetActive(a);
		}

	}
	public void ShowMenu(bool a){

	}
	public void Option(bool a){
		if (a == true) {
			Options.SetActive(a);
			Buttons.SetActive(!a);
			//Animation Opt = Options.GetComponent<Animation>();
			//Opt.Play("OptionEnter");



		}if (a == false) {
			//Animation d = Buttons.GetComponent<Animation> ();

			//d.Play ("mainbuttonenter");
			Options.SetActive (false);
		}
		}
		

	public void Credits(bool a){
		if (a == true) {
			CreditsPanel.SetActive(a);
			Buttons.SetActive(!a);
			//Animation cr = CreditsPanel.GetComponent<Animation>();
			//cr.Play("EnterCredits");

		}else {
			CreditsPanel.SetActive(a);
		}
		
	}
	public void Quit(bool a){
		if (a == true) {
			QuitPanel.SetActive(a);
			Buttons.SetActive(!a);
			//Animation q = QuitPanel.GetComponent<Animation>();
			//q.Play("EnterQuit");
		}else {
			QuitPanel.SetActive(a);
		}
		
	}
	public void Exit(bool a){
		if (a == false) {
			Option(false);
			Buttons.SetActive(true);
			CreditsPanel.SetActive(false);
			QuitPanel.SetActive(false);
			Options.SetActive(false);

			PlayMenu.SetActive(false);
			saveSettings();

		}
		if (a == true) {
			Application.Quit();
		}
	}
	public void saveSettings(){
		PlayerPrefs.SetFloat ("Quality", QualitySettings.GetQualityLevel ());
		PlayerPrefs.SetFloat ("Volume", AudioListener.volume);
	}
	public void FullScreen(bool a){
		if (Screen.fullScreen == a) {
			Screen.fullScreen = !a;
		} else {
			Screen.fullScreen = a;
		}
	}

}

