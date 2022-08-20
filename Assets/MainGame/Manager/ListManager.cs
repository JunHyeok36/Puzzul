using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ListManager : MonoBehaviour
{
	public static ListManager Instance;

	[Header("Piece")]
	public const int PIECE_ID_MAX = 10;
    public Sprite[] piece_sprite = new Sprite[PIECE_ID_MAX];
	public Color[] piece_color = new Color[PIECE_ID_MAX];
	public Sprite[] piece_destroy_effect = new Sprite[7];
	public byte GetRandomPiece() {
		return (byte)Random.Range(0, PIECE_ID_MAX);
	}

	[Header("Effect")]
	[SerializeField] Sprite[] effect_explode;
	[SerializeField] Sprite[] effect_timeExplode;

	[Header("SkinSprites")]
    public Skin[] skins;

	[Header("RuneSprite")]
	public Sprite[] mainRune_sprite;
	public Sprite subRune_sprite;

	[Header("Audio")]
	[SerializeField] AudioMixer masterMixer = null;
	[SerializeField] AudioSource audioSource = null;
	[SerializeField] AudioClip[] m_audios_BGM_OutGame; public AudioClip[] AudioClipsOutGameBGM { get { return m_audios_BGM_OutGame; } }
	[SerializeField] AudioClip[] m_audios_BGM_InGame; public AudioClip[] AudioClipsInGameBGM { get { return m_audios_BGM_InGame; } }
	[SerializeField] AudioClip m_audio_gameover = null; public AudioClip GameOverClip { get { return m_audio_gameover; } }
	[SerializeField] AudioClip m_audio_comboed = null; public AudioClip ComboAudioClip { get { return m_audio_comboed; } }
	[SerializeField] AudioClip m_audio_denied = null; public AudioClip DeniedAudioClip { get { return m_audio_denied; } }
	[SerializeField] AudioClip m_audio_explode = null; public AudioClip ExplodeAudioClip { get { return m_audio_explode; } }
	public void SetInGameBGM() {
		int bgmIndex_random = Random.Range(0, m_audios_BGM_InGame.Length);
		audioSource.loop = true;
		audioSource.clip = m_audios_BGM_InGame[bgmIndex_random];
		audioSource.Play();
	}
	public void SetOutGameBGM() {
		int bgmIndex_random = Random.Range(0, m_audios_BGM_OutGame.Length);
		audioSource.loop = true;
		audioSource.clip = m_audios_BGM_OutGame[bgmIndex_random];
		audioSource.Play();
	}
	public void SetGameOverBGM() {
		audioSource.loop = false;
		audioSource.clip = m_audio_gameover;
		audioSource.Play();
	}
	public void StopBGM() {
		audioSource.Pause();
	}
	public void ContinueBGM() {
		audioSource.UnPause();
	}

	[Header("RuneInfo")]
	public MainRune mainRune;
	public List<SubRune> subRune_list = new List<SubRune>();

	void Awake() {
		if (Instance != null) {
            Destroy(gameObject);
        } else {
			Instance = this;
        	DontDestroyOnLoad(gameObject);
		}

		Application.targetFrameRate = 120;

		DataManager.CheckingData();

		Data data = DataManager.Load();

		if(Social.localUser.authenticated) {
			if(data.autoLoginGooglePlayServices.Value) {
				GPGSManager.Instance.Login(true, (success, localUser) => {
					if(success) {
						MainMenuManager.Instance.GooglePlayServicesTools.SetActive(true);
					} else {
						MainMenuManager.Instance.GooglePlayServicesTools.SetActive(false);
					}
				});
			}
		}
	}

	void Start() {
		StartCoroutine(InitSetting());

		IEnumerator InitSetting() {
			yield return null;

			ResetLanguage();
			ResetAudioMixer();
			ResetSkin();

			SetOutGameBGM();
		}
	}

	public void ResetAudioMixer() {
		Data data = DataManager.Load();

		if(data.audio_BGM.Value <= SettingManager.AUDIO_MIN) {
			masterMixer.SetFloat(SettingManager.AUDIO_BGM, -80f);
		} else {
			masterMixer.SetFloat(SettingManager.AUDIO_BGM, data.audio_BGM.Value);
		}

		if(data.audio_SFX.Value <= SettingManager.AUDIO_MIN) {
			masterMixer.SetFloat(SettingManager.AUDIO_SFX, -80f);
		} else {
			masterMixer.SetFloat(SettingManager.AUDIO_SFX, data.audio_SFX.Value);
		}
	}

	public void ResetSkin() {
		Data data = DataManager.Load();

		Skin skin_target = skins[data.skin_index.Value];

		piece_sprite = skin_target.sprites;
		piece_color = skin_target.colors;
	}

	public void SetPieceSprite(byte ID, Image image) {
		image.sprite = piece_sprite[ID];
		image.color = piece_color[ID];
	}

	public Sprite[] GetEffectSprites(EffectList effectList) {
		switch(effectList) {
			case EffectList.Explode :
				return effect_explode;
			case EffectList.TimeExplode :
				return effect_timeExplode;
			default :
				return null;
		}
	}

	public void SetRuneData(MainRune mainRune, List<SubRune> subRune_list) {
		this.mainRune = mainRune;
		this.subRune_list = subRune_list.ToList();
	}

	    public static void ResetLanguage() {
        Data data = DataManager.Load();

        switch(data.lang.Value) {
            default : LoadLocale("en"); break;
            case SystemLanguage.Korean : LoadLocale("ko"); break;
        }

        void LoadLocale(string languageIdentifier) {
            LocaleIdentifier localeCode = new LocaleIdentifier(languageIdentifier);
            for(int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; i++) {
                Locale aLocale = LocalizationSettings.AvailableLocales.Locales[i];
                LocaleIdentifier anIdentifier = aLocale.Identifier;
                if(anIdentifier == localeCode) {
                    LocalizationSettings.SelectedLocale = aLocale;
                }
            }
        }
    }

    public static string GetLocalizedString(string tableName, string keyName) {
        LocalizedString localizeString = new LocalizedString() { TableReference = tableName, TableEntryReference = keyName};
        var stringOperation = localizeString.GetLocalizedStringAsync();
        if (stringOperation.IsDone && stringOperation.Status == AsyncOperationStatus.Succeeded) {
            return stringOperation.Result;
        } else {
            return null;
        }
    }

    public static string GetLocalizedString(string keyName) {
        return GetLocalizedString("UI", keyName);
    }

    public static string GetLocalizedRuneName(int runeIndex) {
        switch(runeIndex) {
            default : return GetLocalizedString("RuneSetting", "Rune_None");
			case 1 : return GetLocalizedString("RuneSetting", "Rune_TimeBreaker");
            case 2 : return GetLocalizedString("RuneSetting", "Rune_Opportunist");
        }
    }
}
