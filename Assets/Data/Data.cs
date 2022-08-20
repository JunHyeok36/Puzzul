using System.Collections.Generic;

[System.Serializable]
public class Data
{
    public float? audio_BGM, audio_SFX;
    public int? highscore, lastscore;
    int? coin;
    public ulong? longHighscore, longLastscore;
    public bool? mainRuneAvailable;
    public List<MainRune> mainRune_list = new List<MainRune>();
    public List<SubRune> subRune_list = new List<SubRune>();
    public byte? skin_index;
    public List<byte> skin_list = new List<byte>();

    public int? Coin { 
        get { return coin; }
        set { 
            if(value > 1_000_000_000) {
                coin = 1_000_000_000;
            } else {
                coin = value;
            }
        } 
    }

    public bool? autoLoginGooglePlayServices;

    public UnityEngine.SystemLanguage? lang;

    public ulong GetHighScore() {
        if((ulong)highscore.Value > longHighscore.Value) {
            return (ulong)highscore.Value;
        } else {
            return longHighscore.Value;
        }
    }

    public ulong GEtLastScore() {
        if((ulong)lastscore.Value > longLastscore.Value) {
            return (ulong)lastscore.Value;
        } else {
            return longLastscore.Value;
        }
    }
}
