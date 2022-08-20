[System.Serializable]
public struct Axis
{
	public static Axis ZERO = new Axis(0, 0);
	public static Axis[] DIRECTION = new Axis[] {new Axis(-1, -1), new Axis(-1, 0), new Axis(-1, 1), 
													new Axis(0, -1), 				 new Axis(0, 1), 
													new Axis(1, -1), new Axis(1, 0), new Axis(1, 1)};
	public int x, y;

	public Axis(int x, int y) {
		this.x = x; this.y = y;
	}

	public static bool Available(Axis axis, int condition_x, int condition_y) {
		if(0 <= axis.x && axis.x < condition_x && 0 <= axis.y && axis.y < condition_y) {
			return true;
		} else {
			return false;
		}
	}

	public static bool Compare(Axis axis_r, Axis axis_l) {
		if(axis_r.x == axis_l.x && axis_r.y == axis_l.y) {
			return true;
		} else {
			return false;
		}
	}

	public static Axis Add(Axis axis_r, Axis axis_l) {
		return new Axis(axis_r.x + axis_l.x, axis_r.y + axis_l.y);
	}

	public void Add(Axis axis) {
		this.x += axis.x; this.y += axis.y;
	}

	public static Axis Random(int condition_x, int condition_y) {
		return new Axis(UnityEngine.Random.Range(0, condition_x), UnityEngine.Random.Range(0, condition_y));
	}

	public Axis Clone() {
		return new Axis(x, y);
	}
}