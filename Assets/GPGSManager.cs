using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using GooglePlayGames.BasicApi.Events;

public class GPGSManager
{
    static GPGSManager instance = new GPGSManager();
    public static GPGSManager Instance { get { return instance; } }

    public bool IsAuthenticated { get { return Social.localUser.authenticated; } }

    void Init() {
        var config = new PlayGamesClientConfiguration.Builder()
            .EnableSavedGames()
            .Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
    }

    public void Login(bool autoLogin = false, Action<bool, ILocalUser> callback = null) { // (autoLogin, loginSuccess, loginUserInfo)
        Init();
        PlayGamesPlatform.Instance.Authenticate(autoLogin ? SignInInteractivity.CanPromptOnce : SignInInteractivity.CanPromptAlways, (result) => {
            callback?.Invoke(result == SignInStatus.Success, Social.localUser);
        });
    }
    public void Logout() => PlayGamesPlatform.Instance.SignOut();

    public void ShowAchievementsUI() => Social.ShowAchievementsUI();
    public void RevealAchievement(string gpgsID, float progress, Action<bool> callback = null) => Social.ReportProgress(gpgsID, progress, (bool success) => { callback?.Invoke(success); });
    public void UnlockAchievement(string gpgsID, Action<bool> callback = null) => RevealAchievement(gpgsID, 100f, callback);

    public void ShowLeaderboardUI() => Social.ShowLeaderboardUI();
    public void ShowLeaderboardUI(string gpgsID) => PlayGamesPlatform.Instance.ShowLeaderboardUI(gpgsID);
    public void ReportLeaderboard(string gpgsID, long score, Action<bool> callback = null) => Social.ReportScore(score, gpgsID, (bool success) => { callback?.Invoke(success); });

    public void FetchEvent(string gpgsID, Action<bool, IEvent> callback = null) {
        PlayGamesPlatform.Instance.Events.FetchEvent(
            DataSource.ReadCacheOrNetwork, 
            gpgsID, 
            (status, ievent) => {
                if(status == ResponseStatus.Success) {
                    callback?.Invoke(true, ievent);
                } else {
                    callback?.Invoke(false, null);
                }
            }
        );
    } 
    public void FetchAllEvents(Action<bool, List<IEvent>> callback = null) {
        PlayGamesPlatform.Instance.Events.FetchAllEvents(
            DataSource.ReadCacheOrNetwork,
            (status, ievents) => {
                if(status == ResponseStatus.Success) {
                    callback?.Invoke(true, ievents);
                } else {
                    callback?.Invoke(false, null);
                }
            }
        );
    }
    public void IncrementEvent(string gpgsID, uint amount) => PlayGamesPlatform.Instance.Events.IncrementEvent(gpgsID, amount);

    public void SaveWithCloud(string filename, string serializedFile, Action<bool> callback = null) {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        savedGameClient.OpenWithAutomaticConflictResolution(
            filename,
            DataSource.ReadCacheOrNetwork,
            ConflictResolutionStrategy.UseLastKnownGood,
            (status, game) => {
                if(status == SavedGameRequestStatus.Success) {
                    var updatedMetadata = new SavedGameMetadataUpdate.Builder().Build();
                    savedGameClient.CommitUpdate(
                        game, 
                        updatedMetadata, 
                        System.Text.Encoding.UTF8.GetBytes(serializedFile), 
                        (ustatus, ugame) => {
                            callback?.Invoke(ustatus == SavedGameRequestStatus.Success);
                        }
                    );
                } else {
                    callback?.Invoke(false);
                }
            }
        );
    }
    public void LoadWithCloud(string filename, Action<bool, string> callback = null) {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        savedGameClient.OpenWithAutomaticConflictResolution(
            filename, 
            DataSource.ReadCacheOrNetwork, 
            ConflictResolutionStrategy.UseLastKnownGood, 
            (status, game) => {
                if(status == SavedGameRequestStatus.Success) {
                    savedGameClient.ReadBinaryData(game, (lstatus, lserializedFile) => {
                        if(lstatus == SavedGameRequestStatus.Success) {
                            string data_loaded = System.Text.Encoding.UTF8.GetString(lserializedFile);
                            callback?.Invoke(true, data_loaded);
                        } else {
                            callback?.Invoke(false, null);
                        }
                    }
                );
                } else {
                    callback?.Invoke(false, null);
                }
            }
        );
    }
    public void DelectWithCloud(string filename, Action<bool> callback = null) {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        savedGameClient.OpenWithAutomaticConflictResolution(
            filename,
            DataSource.ReadCacheOrNetwork,
            ConflictResolutionStrategy.UseLongestPlaytime,
            (status, game) => {
                if(status == SavedGameRequestStatus.Success) {
                    savedGameClient.Delete(game);
                    callback?.Invoke(true);
                } else {
                    callback?.Invoke(false);
                }
            }
        );
    }
}
