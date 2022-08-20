using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum MainRune {
    None, TimeBreaker, Opportunist
}

public enum SubRune {
    None, AScore, BScore, CScore, DScore, ATime, BTime, CTime, ACombo, BCombo, CCombo
}

public class RuneManager : MonoBehaviour
{
    static RuneManager instance = null;
	public static RuneManager Instance {
		get {
			if(instance == null){
				instance = (RuneManager)FindObjectOfType(typeof(RuneManager));
			}
			return instance;
		}
	}

    [Space]
    [SerializeField] Scrollbar mainRune_scrollbar = null; 
    [SerializeField] MagneticSelecter mainRuneSelecter = null;
    [SerializeField] GameObject[] mainRune_locker = null;
    
    [Space]
    [SerializeField] SubRuneIndicator[] subRuneIndicator = null;

    [Space]
    [SerializeField] GameObject mainRuneAvailable_object = null;

    [Header("CautionPanel")]
    [SerializeField] GameObject caution_object = null;
    [SerializeField] TextMeshProUGUI caution_txt = null;

    [Header("PurchasePanel")]
    [SerializeField] GameObject purchase_object = null;
    [SerializeField] TextMeshProUGUI purchase_hasCoin_txt = null;
    [SerializeField] Image purchase_img = null;
    [SerializeField] TextMeshProUGUI purchase_title_txt = null;
    [SerializeField] TextMeshProUGUI purchase_content_txt = null;
    [SerializeField] TextMeshProUGUI purchase_coin_txt = null;

    [Header("RequestionPanel")]
    [SerializeField] GameObject requestion_panel = null;
    [SerializeField] TextMeshProUGUI requestion_txt = null;

    Action<bool> callback;

    MainRune mainRune;
    List<SubRune> subRune_list = new List<SubRune>();

    //

    int purchase_index = 0;

    void Start() {
        LoadData();
    }

    void Update() {
        if(Input.GetKey(KeyCode.Escape)) {
			SceneLoader.Instance.Entry("MainMenu");
		}
    }

    void LoadData() {
        Data data = DataManager.Load();

        if(data.mainRuneAvailable.Value) {
            mainRuneAvailable_object.SetActive(true);
            //int mainRune_num = System.Enum.GetNames(typeof(SubRune)).Length;
            for(int i = 0; i < mainRune_locker.Length; i++) {
                mainRune_locker[i].SetActive(false);
            }
        } else {
            for(int i = 0; i < data.mainRune_list.Count; i++) {
                mainRune_locker[(int)data.mainRune_list[i]].SetActive(false);
            }
        }

        
        if(data.subRune_list.Count > 0) {
            for(int i = 0; i < data.subRune_list.Count; i++) {
                if(i < 3) {
                    subRuneIndicator[i].SetInfo(data.subRune_list[i]);
                } else {
                    break;
                }
            }

            subRune_list = data.subRune_list.ToList();
        }

        mainRune_scrollbar.value = 0;
    }

    public void SetPurchase(int index) { // index >= 0 : MainRune // index < 0 : RandomSubRune
        Data data = DataManager.Load();

        purchase_index = index;

        purchase_hasCoin_txt.text = "<sprite=0> " + data.Coin;

        if(index >= 0) {
            MainRune mainRune_display = (MainRune)index;

            purchase_img.sprite = ListManager.Instance.mainRune_sprite[index];
            purchase_title_txt.text = ListManager.GetLocalizedRuneName(index);
            purchase_coin_txt.text = "<sprite=0> ";
            switch(index) {
                case 0 :
                    purchase_coin_txt.text += "0";
                    break;
                case 1 :
                    purchase_coin_txt.text += "500";
                    break;
                case 2 :
                    purchase_coin_txt.text += "600";
                    break;
                default :
                    break;
            }
            purchase_content_txt.text = ListManager.GetLocalizedString("RuneSetting", "Requestion_PurchaseMainRune");
        } else {
            purchase_img.sprite = ListManager.Instance.subRune_sprite;
            
            purchase_title_txt.text = purchase_content_txt.text = ListManager.GetLocalizedString("RuneSetting", "SubRune_Random");
            purchase_coin_txt.text = "<sprite=0> " + (15 * (data.subRune_list.Count + 1));
            purchase_content_txt.text = ListManager.GetLocalizedString("RuneSetting", "Requestion_PurchaseSubRune");
        }
        
        purchase_object.SetActive(true);
    }

    public void UnsetPurchase() {
        purchase_object.SetActive(false);
    }

    public void Purchase(bool free = false) {
        Data data = DataManager.Load();

        int cost = 0;

        if(purchase_index >= 0) {
            switch(purchase_index) {
                case 1 :
                    cost = 500;
                    break;
                case 2 :
                    cost = 600;
                    break;
                default :
                    break;
            }
        } else {
            cost = (15 * (data.subRune_list.Count + 1));
        }

        if(data.Coin >= cost || free) {
            if(!free) {
                data.Coin -= cost;
            }

            if(purchase_index >= 0) {
                if(!data.mainRune_list.Contains((MainRune)purchase_index)) {
                    data.mainRune_list.Add((MainRune)purchase_index);
                }
            } else {
                int index_random = UnityEngine.Random.Range(1, System.Enum.GetNames(typeof(SubRune)).Length);
                if(data.subRune_list.Count < 3) {
                    data.subRune_list.Add((SubRune)index_random);
                } else {
                    data.subRune_list[-purchase_index - 1] = (SubRune)index_random;
                }
            }

            UnsetPurchase();
            DataManager.Save(data);
            LoadData();
            SetCaution(ListManager.GetLocalizedString("RuneSetting", "Caution_PurchaseRuneSuccess"));
        } else {
            SetCaution(ListManager.GetLocalizedString("RuneSetting", "Caution_PurchaseRuneFailed"));
        }
    }

    void SetCaution(string caution_str) {
        caution_object.SetActive(true);
        caution_txt.text = caution_str;
    }

    public void UnsetCaution() {
        caution_object.SetActive(false);
    }

    void SetRequestion(string requestion_str, Action<bool> callback = null) {
        requestion_txt.text = requestion_str;

        this.callback = callback;

        requestion_panel.SetActive(true);
    }

    public void UnsetRequestion() {
        requestion_panel.SetActive(false);

        callback?.Invoke(false);
    }

    public void OkayRequestion() {
        callback?.Invoke(true);
    }

    public void SetRewardAds() {
        if(purchase_index >= 0) {
            SetMainRuneRewardAds();
        } else {
            SetSubRuneRewardAds();
        }
    }

    void SetMainRuneRewardAds() {
        AdmobManager.Instance.ShowRewardAd(new AdmobManager.RewardMethod(GetMainRuneReward));
    }

    void SetSubRuneRewardAds() {
        AdmobManager.Instance.ShowRewardAd(new AdmobManager.RewardMethod(GetSubRuneReward));
    }

    void GetMainRuneReward() {
        Data data = DataManager.Load();

        data.mainRuneAvailable = true;

        DataManager.Save(data);

        LoadData();

        UnsetPurchase();

        SetCaution(ListManager.GetLocalizedString("RuneSetting", "Caution_GetRuneTemporary"));
    }

    void GetSubRuneReward() {
        purchase_index = -1;

        Purchase(true);
    }

    public void EntryGame() {
        Data data = DataManager.Load();
        if(data.mainRuneAvailable.Value || data.mainRune_list.Contains((MainRune)mainRuneSelecter.GetElementIndex())) {
            ListManager.Instance.SetRuneData((MainRune)mainRuneSelecter.GetElementIndex(), subRune_list);
            ResetData();
            int ranNum = UnityEngine.Random.Range(0, 100);
            if(ranNum < 40) {
                AdmobManager.Instance.ShowFrontAd(new AdmobManager.RewardMethod(StartGame));
            } else {
                StartGame();
            }
        } else {
            SetCaution(ListManager.GetLocalizedString("RuneSetting", "Caution_UseRuneError"));
        }
    }

    public void StartGame() {
        ListManager.Instance.SetInGameBGM();
        SceneLoader.Instance.Entry("MainGame");
    }

    void ResetData() {
        Data data = DataManager.Load();
        data.subRune_list.Clear();

        if(data.mainRuneAvailable.Value) {
			data.mainRuneAvailable = false;
		}

        DataManager.Save(data);
    }
}
