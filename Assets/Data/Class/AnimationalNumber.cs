[System.Serializable]
public class AnimationalNumber<T> where T : struct
{
	public T init, currnet, target, weight;

	public AnimationalNumber(T init, T weight, T currnet, T target) {
		this.init = init;
		this.weight = weight;
		this.currnet = currnet;
		this.target = target;
	}

	public AnimationalNumber<T> Clone() {
		return new AnimationalNumber<T>(init, weight, currnet, target);
	}
}
