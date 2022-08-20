using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class Effect : MonoBehaviour
{
    [SerializeField] AudioSource audioSource = null;
    [SerializeField] Image effect_img = null;

    Sprite[] sprites_effect;
    float space_time = 0.1f;
    int repeat = 1;

    public void SetInfo(Sprite[] sprites_effect, AudioClip audioClip, float space_time, int repeat) {
        this.sprites_effect = sprites_effect;
        this.audioSource.clip = audioClip;
        this.space_time = space_time;
        this.repeat = repeat;
        StartCoroutine(PlayAnimation());
    }

    IEnumerator PlayAnimation() {
        var wfs = new WaitForSeconds(space_time);

        audioSource.Play();
        for(int r = 0; r < repeat; r++) {
            for(int i = 0; i < sprites_effect.Length; i++) {
                effect_img.sprite = sprites_effect[i];

                yield return wfs;
            }
        }

        Remove();
    }

    public void Remove() {
        ObjectManager.Instance.ReturnObject(ObjectList.Effect, gameObject);
    }
}
