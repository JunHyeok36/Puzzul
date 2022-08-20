using UnityEngine;

[System.Serializable]
public class AnimationalComponent
{
	public static AnimationalComponent init = new AnimationalComponent(new Vector3(0, 0, 0), new Vector3(0, 0, 0), 1f);

	public Vector3 position_init, position_target;
	public float position_speed;
	public Vector3 scale_init, scale_target;
	public float scale_speed;
	public Color color_init, color_currnet, color_target;
	public float color_speed;

	public AnimationalComponent(Vector3 position_init, Vector3 position_target, float position_speed) {
		this.position_init = position_init;
		this.position_target = position_target;
		this.position_speed = position_speed;

		this.scale_init = new Vector3(1, 1, 1);
		this.scale_target = this.scale_init;
		this.scale_speed = 10f;

		this.color_init = new Color(1, 1, 1, 1);
		this.color_currnet = this.color_init;
		this.color_target = this.color_init;
		this.color_speed = 10f;
	}

	public void SetPosition(Vector3 position_init, Vector3 position_target, float position_speed) {
		this.position_init = position_init;
		this.position_target = position_target;
		this.position_speed = position_speed;
	}

	public void SetScale(Vector3 scale_init, Vector3 scale_target, float scale_speed) {
		this.scale_init = scale_init;
		this.scale_target = scale_target;
		this.scale_speed = scale_speed;
	}

	public void SetColor(Color color_init, Color color_currnet, Color color_target, float scale_speed) {
		this.color_init = color_init;
		this.color_currnet = color_currnet;
		this.color_target = color_target;
		this.color_speed = scale_speed;
	}
}
