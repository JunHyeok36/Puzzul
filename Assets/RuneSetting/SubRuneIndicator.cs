using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SubRuneIndicator : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI title = null;
    [SerializeField] TextMeshProUGUI content = null;

    SubRune subRune;

    public void SetInfo(SubRune subRune) {
        switch(subRune) {
            case SubRune.AScore :
                title.text = ListManager.GetLocalizedString("RuneSetting", "SubRune_AdditionalScore") + "A";
                content.text = ListManager.GetLocalizedString("RuneSetting", "SubRune_AdditionalScore_Detail") + " +200";
                break;
            case SubRune.BScore :
                title.text = ListManager.GetLocalizedString("RuneSetting", "SubRune_AdditionalScore") + "B";
                content.text = ListManager.GetLocalizedString("RuneSetting", "SubRune_AdditionalScore_Detail") + " +100";
                break;
            case SubRune.CScore :
                title.text = ListManager.GetLocalizedString("RuneSetting", "SubRune_AdditionalScore") + "C";
                content.text = ListManager.GetLocalizedString("RuneSetting", "SubRune_AdditionalScore_Detail") + " +50";
                break;
            case SubRune.DScore :
                title.text = ListManager.GetLocalizedString("RuneSetting", "SubRune_AdditionalScore") + "D";
                content.text = ListManager.GetLocalizedString("RuneSetting", "SubRune_AdditionalScore_Detail") + " +10";
                break;
            case SubRune.ATime :
                title.text = ListManager.GetLocalizedString("RuneSetting", "SubRune_AdditionalTime") + "A";
                content.text = ListManager.GetLocalizedString("RuneSetting", "SubRune_AdditionalTime_Detail") + " +3초";
                break;
            case SubRune.BTime :
                title.text = ListManager.GetLocalizedString("RuneSetting", "SubRune_AdditionalTime") + "B";
                content.text = ListManager.GetLocalizedString("RuneSetting", "SubRune_AdditionalTime_Detail") + " +2초";
                break;
            case SubRune.CTime :
                title.text = ListManager.GetLocalizedString("RuneSetting", "SubRune_AdditionalTime") + "C";
                content.text = ListManager.GetLocalizedString("RuneSetting", "SubRune_AdditionalTime_Detail") + " +1초";
                break;
            case SubRune.ACombo :
                title.text = ListManager.GetLocalizedString("RuneSetting", "SubRune_AdditionalCombo") + "A";
                content.text = ListManager.GetLocalizedString("RuneSetting", "SubRune_AdditionalCombo_Detail") + " +×0.5";
                break;
            case SubRune.BCombo :
                title.text = ListManager.GetLocalizedString("RuneSetting", "SubRune_AdditionalCombo") + "B";
                content.text = ListManager.GetLocalizedString("RuneSetting", "SubRune_AdditionalCombo_Detail") + " +×0.3";
                break;
            case SubRune.CCombo :
                title.text = ListManager.GetLocalizedString("RuneSetting", "SubRune_AdditionalCombo") + "C";
                content.text = ListManager.GetLocalizedString("RuneSetting", "SubRune_AdditionalCombo_Detail") + " +×0.1";
                break;
            default :
                break;
        }
    }

    public SubRune GetSubRune() {
        return subRune;
    }
}
