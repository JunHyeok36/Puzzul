using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkinManager : MonoBehaviour
{
    static SkinManager instance = null;
	public static SkinManager Instance {
		get {
			if(instance == null){
				instance = (SkinManager)FindObjectOfType(typeof(SkinManager));
			}
			return instance;
		}
	}

    [Header("Indicator")]
    [SerializeField] MagneticSelecter skinSelecter = null;
    [SerializeField] GameObject[] skin_locker = null;

    [Header("CautionPanel")]
    [SerializeField] GameObject caution_panel = null;
    [SerializeField] TextMeshProUGUI caution_txt = null;

    [Header("PurchasePanel")]
    [SerializeField] GameObject purchase_panel = null;
    [SerializeField] TextMeshProUGUI purchase_hasCoin_txt = null;
    [SerializeField] TextMeshProUGUI purchase_content_txt = null;
    [SerializeField] TextMeshProUGUI purchase_coin_txt = null;

    byte purchase_index = 0;

    void Start() {
        Init();
    }

    void Update() {
        if(Input.GetKey(KeyCode.Escape)) {
			SceneLoader.Instance.Entry("MainMenu");
		}
    }

    void Init() {
        Data data = DataManager.Load();

        for(int i = 0; i < data.skin_list.Count; i++) {
            skin_locker[data.skin_list[i]].SetActive(false);
        }

        skinSelecter.SetDisplay((int)data.skin_index);
    }

    public void SelectSkin() {
        try {
            Data data = DataManager.Load();

            byte skin_index = (byte)skinSelecter.GetElementIndex();
            if(data.skin_list.Contains(skin_index)) {
 
                data.skin_index = skin_index;

                DataManager.Save(data);

                ListManager.Instance.ResetSkin();

                SetCaution(ListManager.GetLocalizedString("SkinSetting", "Caution_ChangeSkinSuccess"));
            } else {
                SetCaution(ListManager.GetLocalizedString("SkinSetting", "Caution_ChangeSkinFailed"));
            }
        } catch {
            SetCaution(ListManager.GetLocalizedString("UI", "Caution_Error"));
        }
    }

    #region Panel

    bool isEnableEscapeCautionPanel = false;

    void SetCaution(string caution_str, bool closable = true) {
        caution_txt.text = caution_str;

        isEnableEscapeCautionPanel = closable;
        caution_panel.SetActive(true);
    }

    public void UnsetCaution() {
        if(isEnableEscapeCautionPanel) {
            caution_panel.SetActive(false);
        }
    }

    public void SetPurchase(int purchase_index) {
        Data data = DataManager.Load();

        this.purchase_index = (byte)purchase_index;

        purchase_content_txt.text = ListManager.GetLocalizedString("SkinSetting", "Requestion_PurchaseSkin");

        purchase_hasCoin_txt.text = $"<sprite=0> {data.Coin}";

        if(this.purchase_index >= 0) {
            purchase_coin_txt.text = "<sprite=0> ";
            switch(this.purchase_index) {
                case 0 :
                    purchase_coin_txt.text += "0";
                    break;
                case 1 :
                    purchase_coin_txt.text += "800";
                    break;
                case 2 :
                    purchase_coin_txt.text += "800";
                    break;
                case 3 :
                    purchase_coin_txt.text += "1000";
                    break;
                case 4 :
                    purchase_coin_txt.text += "1000";
                    break;
                default :
                    purchase_coin_txt.text += "0";
                    break;
            }

            purchase_panel.SetActive(true);
        } else {
            SetCaution(ListManager.GetLocalizedString("UI", "Caution_Error"));
        }
    }

    public void UnsetPurchase() {
        purchase_index = 0;

        purchase_panel.SetActive(false);
    }

    public void Purchase() {
        Data data = DataManager.Load();

        int cost = 0;

        switch(purchase_index) {
            case 0 :
                cost = 0;
                break;
            case 1 :
                cost = 800;
                break;
            case 2 :
                cost = 800;
                break;
            case 3 :
                cost = 1000;
                break;
            case 4 :
                cost = 1000;
                break;
            default :
                break;
        }

        if(data.Coin >= cost) {
            data.Coin -= cost;

            if(!data.skin_list.Contains(purchase_index)) {
                data.skin_list.Add(purchase_index);
            }

            UnsetPurchase();
            DataManager.Save(data);
            Init();
            SetCaution(ListManager.GetLocalizedString("SkinSetting", "Caution_PurchaseSkinSuccess"));
        } else {
            SetCaution(ListManager.GetLocalizedString("SkinSetting", "Caution_PurchaseSkinFailed"));
        }
    }

    #endregion
}
