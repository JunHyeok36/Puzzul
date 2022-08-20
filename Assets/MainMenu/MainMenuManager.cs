using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    static MainMenuManager instance = null;
	public static MainMenuManager Instance {
		get {
			if(instance == null){
				instance = (MainMenuManager)FindObjectOfType(typeof(MainMenuManager));
			}
			return instance;
		}
	}
	[SerializeField] TextMeshProUGUI highscore_txt = null;

	[Header("GooglePlayServices")]
	[SerializeField] GameObject googlePlayServices_tools = null;
	public GameObject GooglePlayServicesTools { get { return googlePlayServices_tools; } }
	void Start() {
		Init();

		if(Social.localUser.authenticated) {
			GooglePlayServicesTools.SetActive(true);
		}
	}

	void Update() {
		if(Input.GetKey(KeyCode.Escape)) {
			QuitApplication();
		}
	}

	void Init() {
		Data data = DataManager.Load();
		
		highscore_txt.text = System.String.Format("{0:#,0}", data.GetHighScore());
	}

	public void SetAchievementUI() {
		if(Social.localUser.authenticated) {
			GPGSManager.Instance.ShowAchievementsUI();
		}
	}

	public void SetLeaderboardUI() {
		if(Social.localUser.authenticated) {
			GPGSManager.Instance.ShowLeaderboardUI(GPGSIds.leaderboard_ranking);
		}
	}

	public void QuitApplication() {
		Application.Quit();
	}
}
