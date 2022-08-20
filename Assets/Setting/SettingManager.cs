using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using Newtonsoft.Json;
using TMPro;

public class SettingManager : MonoBehaviour
{
    [Header("CautionPanel")]
    [SerializeField] GameObject caution_panel = null;
    [SerializeField] TextMeshProUGUI caution_txt = null;

    [Header("RequestionPanel")]
    [SerializeField] GameObject requestion_panel = null;
    [SerializeField] TextMeshProUGUI requestion_txt = null;

    [Header("GooglePlayServices")]
    [SerializeField] GameObject googlePlayService_beforeLogin_panel = null;
    [SerializeField] GameObject googlePlayService_afterLogin_panel = null;

    [Header("AudioMixer")]
    [SerializeField] AudioMixer masterMixer = null;
    [SerializeField] Slider BGM_slider = null;
    [SerializeField] Slider SFX_slider = null;

    [Header("LanguageSetting")]
    [SerializeField] TMP_Dropdown language_dropdown = null;
    public void OnLanguageDropdownValueChanged() {
        Data data = DataManager.Load();

        switch (language_dropdown.value) {
            default : data.lang = SystemLanguage.English; break;
            case 1 : data.lang = SystemLanguage.Korean; break;
        }

        DataManager.Save(data);

        ListManager.ResetLanguage();
    }

    public const float AUDIO_MIN = -40f, AUDIO_MAX = 0f;
    public const string AUDIO_BGM = "BGM", AUDIO_SFX = "SFX";

    bool caution_closable = true;
    Action<bool> callback;

    void Start() {
        Data data = DataManager.Load();

        BGM_slider.value = data.audio_BGM.Value;
        SFX_slider.value = data.audio_SFX.Value;

        if(Social.localUser.authenticated) {
            SetAfterLogin();
		} else {
            SetBeforeLogin();
        }

        switch (data.lang.Value) {
            default : language_dropdown.value = 0; break;
            case SystemLanguage.Korean : language_dropdown.value = 1; break;
        }
    }

    void Update() {
        if(BGM_slider.value <= AUDIO_MIN) {
            masterMixer.SetFloat(AUDIO_BGM, -80f);
        } else {
            masterMixer.SetFloat(AUDIO_BGM, BGM_slider.value);
        }

        if(SFX_slider.value <= AUDIO_MIN) {
            masterMixer.SetFloat(AUDIO_SFX, -80f);
        } else {
            masterMixer.SetFloat(AUDIO_SFX, SFX_slider.value);
        }

        if(caution_closable && Input.GetKey(KeyCode.Escape)) {
			SceneLoader.Instance.Entry("MainMenu");
		}
    }

    void SetCaution(string caution_str, bool closable = true) {
        caution_txt.text = caution_str;

        caution_closable = closable;

        caution_panel.SetActive(true);
    }

    public void UnsetCaution() {
        if(caution_closable) {
            caution_panel.SetActive(false);
        }
    }

    void SetRequestion(string requestion_str, Action<bool> callback = null) {
        requestion_txt.text = requestion_str;

        this.callback = callback;

        requestion_panel.SetActive(true);
    }

    public void UnsetRequestion() {
        callback?.Invoke(false);

        requestion_panel.SetActive(false);
    }

    public void OkayRequestion() {
        callback?.Invoke(true);
    }

    public void LoginGooglePlayServices() {
        SetCaution(ListManager.GetLocalizedString("GPGS", "Caution_Login"), false);
        GPGSManager.Instance.Login(false, (success, localUser) => {
            if(success) {
                Data data = DataManager.Load();
                data.autoLoginGooglePlayServices = true;
                DataManager.Save(data);
                SetAfterLogin();
                SetCaution(ListManager.GetLocalizedString("GPGS", "Caution_LoginSuccess"));
            } else {
                SetBeforeLogin();
                SetCaution(ListManager.GetLocalizedString("GPGS", "Caution_LoginFailed"));
            }
        });
    }

    public void LogoutGooglePlayServices() {
        GPGSManager.Instance.Logout();

        Data data = DataManager.Load();
        data.autoLoginGooglePlayServices = false;
        DataManager.Save(data);

        SetBeforeLogin();
    }

    public void RequestSaveWithGooglePlayServices() {
        SetRequestion(ListManager.GetLocalizedString("GPGS", "Requestion_CloudSave"), SaveWithGooglePlayServices);
    }

    public void RequestLoadWithGooglePlayServices() {
        SetRequestion(ListManager.GetLocalizedString("GPGS", "Requestion_CloudLoad"), LoadWithGooglePlayServices);
    }

    void SaveWithGooglePlayServices(bool success) {
        SetCaution(ListManager.GetLocalizedString("GPGS", "Caution_CloudSaving"), false);
        Data data = DataManager.Load();

        string data_str = JsonConvert.SerializeObject(data);

        GPGSManager.Instance.SaveWithCloud("data", data_str, (success) => {
            if(success) {
                SetCaution(ListManager.GetLocalizedString("GPGS", "Caution_CloudSaveSuccess"));
            } else {
                SetCaution(ListManager.GetLocalizedString("GPGS", "Caution_CloudSaveFailed"));
            }
        });

        UnsetRequestion();
    }

    void LoadWithGooglePlayServices(bool success) {
        SetCaution(ListManager.GetLocalizedString("GPGS", "Caution_CloudLoading"), false);

        GPGSManager.Instance.LoadWithCloud("data", (success, data_str) => {
            if(success) {
                Data data_origin = DataManager.Load();
                Data data = JsonConvert.DeserializeObject<Data>(data_str);

                data.audio_BGM = data_origin.audio_BGM;
                data.audio_SFX = data_origin.audio_SFX;
                data.mainRuneAvailable = false;
                data.subRune_list.Clear();

                DataManager.Save(data);

                SetCaution(ListManager.GetLocalizedString("GPGS", "Caution_CloudLoadSuccess"));
            } else {
                SetCaution(ListManager.GetLocalizedString("GPGS", "Caution_CloudLoadFailed"));
            }
        });

        UnsetRequestion();
    }

    void SetBeforeLogin() {
        googlePlayService_beforeLogin_panel.SetActive(true);
        googlePlayService_afterLogin_panel.SetActive(false);
    }

    void SetAfterLogin() {
        googlePlayService_afterLogin_panel.SetActive(true);
        googlePlayService_beforeLogin_panel.SetActive(false);
    }

    public void BackToMainMenu() {
        Data data = DataManager.Load();

        data.audio_BGM = BGM_slider.value;
        data.audio_SFX = SFX_slider.value;

        DataManager.Save(data);

        SceneLoader.Instance.Entry("MainMenu");
    }
}
