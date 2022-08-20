using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
	protected static SceneLoader instance = null;

	public static SceneLoader Instance {
		get {
			if(instance == null){
				instance = (SceneLoader) FindObjectOfType(typeof(SceneLoader));
			}
			return instance;
		}
	}

	[SerializeField] Animator load_image_animator = null; 

	public static void Load(string scene_name) {
		SceneManager.LoadScene(scene_name);
	}

	public void Entry(string scene_name) {
		StartCoroutine(PlayLoadAnimation(scene_name));
	}

	IEnumerator PlayLoadAnimation(string scene_name) {
		var wfs = new WaitForSeconds(0.25f);

		load_image_animator.SetTrigger("Appear");

		yield return wfs;

		SceneManager.LoadScene(scene_name);
	}
}
