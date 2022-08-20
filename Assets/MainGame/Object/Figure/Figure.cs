using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum Style {
	Normal, Bold
}

public class Figure: MonoBehaviour
{
	RectTransform rectTransform;

	AnimationalComponent anim = AnimationalComponent.init;

	[SerializeField] TextMeshProUGUI figure_txt = null;

    int intger_value = 0;

	const float DURATION = 2.5f;

	void Awake() {
		rectTransform = GetComponent<RectTransform>();
	}

	void Update() {
		//rectTransform.anchoredPosition = Vector3.Lerp(rectTransform.anchoredPosition, anim.position_target, Time.deltaTime * anim.position_speed);
		anim.color_currnet.a = Mathf.Lerp(anim.color_currnet.a, anim.color_target.a, Time.deltaTime * anim.color_speed);
		figure_txt.color = anim.color_currnet;
	}

	public void SetInfo(Axis axis_target, int intger_value, Style style = Style.Normal) {
		rectTransform = GetComponent<RectTransform>();

		this.intger_value = intger_value;

		Vector3 position_target = GameManager.Instance.AxisToVector(axis_target);
		rectTransform.anchoredPosition = position_target;
		rectTransform.localScale = anim.scale_init;

		anim.position_target = position_target;//new Vector3(position_target.x, position_target.y + 10, 0);
		figure_txt.color = anim.color_init;
		anim.color_currnet = anim.color_init;
		anim.color_target = new Color(1, 1, 1, 0);
		anim.color_speed = 2f;

		if(intger_value > 0) {
			figure_txt.text = $"+{intger_value}";
		} else if(intger_value < 0) {
			figure_txt.text = intger_value.ToString();
		} else {
			figure_txt.text = "+0";
		}

		Invoke("RemoveFigure", DURATION);
	}

	public void SetInfo(Axis axis_target, string str, Style style = Style.Normal) {
		figure_txt.text = str;

		Invoke("RemoveFigure", DURATION);
	}

	void RemoveFigure() {
		ObjectManager.Instance.ReturnObject(ObjectList.Figure, gameObject);
	}
}
