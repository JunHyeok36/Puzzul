using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectList
{
	Piece, Figure, Effect
}

public class ObjectManager : MonoBehaviour
{
    static ObjectManager instance = null;
	public static ObjectManager Instance {
		get {
			if(instance == null){
				instance = (ObjectManager)FindObjectOfType(typeof(ObjectManager));
			}
			return instance;
		}
	}

	[Header("Prefab")]
	[SerializeField] GameObject piece_prefab = null;
	Queue<GameObject> piece_queue = new Queue<GameObject>();
	[SerializeField] GameObject figure_prefab = null;
	Queue<GameObject> figure_queue = new Queue<GameObject>();
	[SerializeField] GameObject effect_prefab = null;
	Queue<GameObject> effect_queue = new Queue<GameObject>();

	void CreateObject(ObjectList target) {
		GameObject prefab_target = null;
		Queue<GameObject> queue_target = new Queue<GameObject>();

		switch(target) {
			case ObjectList.Piece :
				prefab_target = piece_prefab;
				queue_target = piece_queue;
				break;
			case ObjectList.Figure :
				prefab_target = figure_prefab;
				queue_target = figure_queue;
				break;
			case ObjectList.Effect :
				prefab_target = effect_prefab;
				queue_target = effect_queue;
				break;
			default :
				break;
		}

		GameObject object_new = Instantiate(prefab_target);

		object_new.transform.SetParent(transform);
		object_new.SetActive(false);

		queue_target.Enqueue(object_new);
	}

	public GameObject GetObject(ObjectList target) {
		GameObject object_target = null;
		
		switch(target) {
			case ObjectList.Piece :
				if(piece_queue.Count <= 0) {
					CreateObject(target);
				}
				object_target = piece_queue.Dequeue();
				break;
			case ObjectList.Figure :
				if(figure_queue.Count <= 0) {
					CreateObject(target);
				}
				object_target = figure_queue.Dequeue();
				break;
			case ObjectList.Effect :
				if(effect_queue.Count <= 0) {
					CreateObject(target);
				}
				object_target = effect_queue.Dequeue();
				break;
			default :
				break;
		}

		object_target.transform.SetParent(null);
		object_target.SetActive(true);

		return object_target;
	}

	public void ReturnObject(ObjectList target, GameObject object_return) {
		object_return.transform.SetParent(transform);
		object_return.SetActive(false);

		switch(target) {
			case ObjectList.Piece :
				piece_queue.Enqueue(object_return);
				break;
			case ObjectList.Figure :
				figure_queue.Enqueue(object_return);
				break;
			case ObjectList.Effect :
				effect_queue.Enqueue(object_return);
				break;
			default :
				break;
		}
	}
}
