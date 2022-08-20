using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class Piece : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	const float SENSITIVITY = 25f;

	RectTransform rectTransform;
    Image image;

	AnimationalComponent anim = AnimationalComponent.init;

	Axis axis;
	public Axis Axis { get { return axis; } }
	byte ID;
	public byte GetID() { return ID; }

	Vector3 position_last = Vector3.zero, position_target = Vector3.zero;
	bool isAnimating = false, isDraging = false;

	void Awake() {
		rectTransform = GetComponent<RectTransform>();
		image = GetComponent<Image>();
	}

	void Update() {
		rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, anim.position_target, Time.fixedDeltaTime * anim.position_speed);
		transform.localScale = Vector3.Lerp(transform.localScale, anim.scale_target, Time.fixedDeltaTime * anim.scale_speed);
	}

	public IEnumerator PlayAnimation(Sprite[] sprite_list, float space_time, bool back_to_origin_img = false) {
		var wfs = new WaitForSeconds(space_time);

		Sprite sprite_origin = image.sprite;

		int sprite_list_num =  sprite_list.Length;
		if(sprite_list_num > 0) {
			for(int i = 0; i < sprite_list_num; i++) {
				image.sprite = sprite_list[i];

				yield return wfs;
			}
		}

		if(back_to_origin_img) {
			image.sprite = sprite_origin;
		}
	}

	public void Init(Axis axis, byte ID) {
		this.axis = axis;
		this.ID = ID;
		this.isAnimating = true;
		this.isDraging = false;

		ListManager.Instance.SetPieceSprite(this.ID, this.image);

		Vector3 position_axis = GameManager.Instance.AxisToVector(axis);
		anim = new AnimationalComponent(
			new Vector3(position_axis.x, 1200, 0), position_axis, 8f
		);

		transform.SetParent(GameManager.Instance.AnimatingFieldObject.transform, false);

		rectTransform.anchoredPosition = anim.position_init;
		rectTransform.localScale = anim.scale_init;

		Invoke("SetParent", 1.3f);
	}

	void SetParent() {
		transform.SetParent(GameManager.Instance.AxisToTrasform(axis), false);
		
		rectTransform.anchoredPosition = Vector3.zero;
		anim.position_target = Vector3.zero;

		isAnimating = false;
		isDraging = false;
	}

	public void Move(Axis axis_target) {
		axis = axis_target;
		
		transform.SetParent(GameManager.Instance.AnimatingFieldObject.transform);

		anim.position_target = GameManager.Instance.AxisToVector(axis);

		isAnimating = true;
		Invoke("SetParent", 0.35f);
	}

	public void RemoveAnimation() {
		float space_time = 0.05f;
		StartCoroutine(PlayAnimation(ListManager.Instance.piece_destroy_effect, space_time));

		Invoke("ReturnObject", space_time * ListManager.Instance.piece_destroy_effect.Length);
	}

	void ReturnObject() {
		ObjectManager.Instance.ReturnObject(ObjectList.Piece, gameObject);
	}

	public void OnPointerEnter(PointerEventData EventData) {
		anim.scale_target = new Vector3(1.25f, 1.25f, 1f);
	}

	public void OnPointerExit(PointerEventData EventData) {
		anim.scale_target = new Vector3(1, 1, 1);
	}

	public void OnBeginDrag(PointerEventData eventData) {
		if(isAnimating) {
			return;
		}
		isDraging = true;

		position_target = Vector3.zero;
	}

	public void OnDrag(PointerEventData eventData) {
		if(isAnimating) {
			return;
		}
		RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.transform as RectTransform, eventData.position, Camera.main, out Vector2 position_target_adjusted);
		
		position_target = (Vector3)position_target_adjusted;
	}

	public void OnEndDrag(PointerEventData eventData) {
		if(isAnimating) {
			return;
		}

		if(-SENSITIVITY < position_target.x && position_target.x < SENSITIVITY && -SENSITIVITY < position_target.y && position_target.y < SENSITIVITY) {
			return;
		}

		if(0 < position_target.x) {
			if(position_target.x < position_target.y) {
				StartCoroutine(GameManager.Instance.MovePiece(this, new Axis(0, -1))); //Up
			} else if(position_target.y < -position_target.x) {
				StartCoroutine(GameManager.Instance.MovePiece(this, new Axis(0, 1))); //Down
			} else {
				StartCoroutine(GameManager.Instance.MovePiece(this, new Axis(1, 0))); // right
			}
		} else if(position_target.x < 0) {
			if(-position_target.x < position_target.y) {
				StartCoroutine(GameManager.Instance.MovePiece(this, new Axis(0, -1))); //Up
			} else if(position_target.y < position_target.x) {
				StartCoroutine(GameManager.Instance.MovePiece(this, new Axis(0, 1))); //Down
			} else {
				StartCoroutine(GameManager.Instance.MovePiece(this, new Axis(-1, 0))); // left
			}
		}
		
		isDraging = false;
	}
}
