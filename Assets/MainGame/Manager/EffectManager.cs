using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EffectList {
    None, Explode, TimeExplode
}

public class EffectManager : MonoBehaviour
{
    static EffectManager instance = null;
	public static EffectManager Instance {
		get {
			if(instance == null){
				instance = (EffectManager)FindObjectOfType(typeof(EffectManager));
			}
			return instance;
		}
	}

    [SerializeField] GameObject opportunist_Effect = null;

    public void SetEffect(Axis axis_target, EffectList effect_list, AudioClip audioClip, float height_multiple = 1f, float width_multiple = 1f, float space_time = 0.05f , int repeat = 1) {
        if(Axis.Available(axis_target, GameManager.X_MAX, GameManager.Y_MAX) && effect_list != EffectList.None) {
            Vector3 vector_target = GameManager.Instance.AxisToVector(axis_target);

            GameObject effect_object = ObjectManager.Instance.GetObject(ObjectList.Effect);
            effect_object.transform.SetParent(GameManager.Instance.AnimatingFieldObject.transform, false);
            effect_object.transform.localScale = new Vector3(1, 1, 1);
            
            RectTransform effect_rectTransform = effect_object.GetComponent<RectTransform>();
            effect_rectTransform.anchoredPosition = vector_target;
            effect_rectTransform.sizeDelta = new Vector2(170f * height_multiple, 170f * width_multiple);

            Effect effect = effect_object.GetComponent<Effect>();

            effect.SetInfo(ListManager.Instance.GetEffectSprites(effect_list), audioClip, space_time, repeat);
        }
    }

    public IEnumerator SetMainRuneEffect(MainRune mainRune) {
        switch(mainRune) {
            case MainRune.None :
                break;
            case MainRune.TimeBreaker :
                break;
            case MainRune.Opportunist :
                opportunist_Effect.SetActive(true);
                yield return new WaitForSeconds(0.5f);
                opportunist_Effect.SetActive(false);
                break;
            default :
                break;
        }
    }

    
}
