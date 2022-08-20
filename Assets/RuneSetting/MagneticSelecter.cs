using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MagneticSelecter : MonoBehaviour
{
    RectTransform rectTransform;

	[SerializeField] int element_size = 1200;
    [SerializeField] int element_space = 300;
    [SerializeField] int selectedElement_index = 0, element_num = 0, MIN = 0, MAX = 0;
    float[] element_xPos;

    Vector2 velocity = Vector2.zero;
    Vector2 position_target = Vector2.zero;
    Vector2 position_beginDrag = Vector2.zero;

    void Awake() {
        rectTransform = GetComponent<RectTransform>();

        Init();
    }

    void Update() {
        if(Input.GetMouseButtonUp(0)) {
            position_target.x = SetXPos(rectTransform.anchoredPosition.x);
        }
        if(!Input.GetMouseButton(0)) {
            rectTransform.anchoredPosition = Vector2.SmoothDamp(rectTransform.anchoredPosition, position_target, ref velocity, 0.1f);
        }
    }

    void Init() {
        selectedElement_index = 0;
        element_num = transform.childCount;
        element_xPos = new float[element_num];

        int width = element_num * element_size + (element_num - 1) * element_space;
        MAX = width / 2; MIN = -MAX;
        float xPos = (float)MIN;
        for(int i = 0; i < element_num; i++) {
            xPos += element_size / 2;
            element_xPos[i] = xPos;
            xPos += element_space + element_size / 2;
        }

        position_target = new Vector2(element_xPos[element_xPos.Length - 1], rectTransform.anchoredPosition.y);
        //rectTransform.anchoredPosition = position_target;//
    }

    float SetXPos(float xPos_target) {
        float xPos_l = MIN, xPos_r = xPos_l + element_size + element_space / 2;
        for(int i = 0; i < element_num; i++) {
            if(xPos_l < xPos_target && xPos_target < xPos_r) {
                selectedElement_index = element_xPos.Length - 1 - i;
                return element_xPos[i];
            }
            xPos_l += element_size + element_space / 2;
            xPos_r += element_size + element_space / 2;
        }

        return element_xPos[0];
    }

    public int GetElementIndex() {
        return selectedElement_index;
    }

    public void SetDisplay(int index) {
        selectedElement_index = index;
        position_target.x = element_xPos[element_xPos.Length - 1 - index];
    }


}
