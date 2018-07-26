using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class CloudManager{
    private string FILENAME;

    private string CLOUD_DATA;
    private string SAVE_DATA;

    public CloudManager(string filename){
        FILENAME = filename;
    }

    public void LoadData(){
        CLOUD_DATA = SAVE_DATA = "";
        PlayGamesPlatform.Instance.SavedGame.OpenWithAutomaticConflictResolution(FILENAME, DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseMostRecentlySaved, OnLoad);
    }
    public void SaveData(){
        PlayGamesPlatform.Instance.SavedGame.OpenWithAutomaticConflictResolution(FILENAME, DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseMostRecentlySaved, OnSave);
    }
    private void OnLoad(SavedGameRequestStatus status, ISavedGameMetadata game){
        if (status == SavedGameRequestStatus.Success){
            LoadGame(game);
        }
    }
    private void OnSave(SavedGameRequestStatus status, ISavedGameMetadata game){
        if (status == SavedGameRequestStatus.Success){
            SaveGame(game, SAVE_DATA);
        }
    }
    private void LoadGame(ISavedGameMetadata game){
        PlayGamesPlatform.Instance.SavedGame.ReadBinaryData(game, OnSavedGameDataRead);
    }

    private void SaveGame(ISavedGameMetadata game, string data){
        byte[] dataToSave = Encoding.ASCII.GetBytes(data);
        SavedGameMetadataUpdate update = new SavedGameMetadataUpdate.Builder().Build();
        PlayGamesPlatform.Instance.SavedGame.CommitUpdate(game, update, dataToSave, OnSavedGameDataWritten);
    }
    private void OnSavedGameDataRead(SavedGameRequestStatus status, byte[] savedData){
        if (status == SavedGameRequestStatus.Success){
            if (savedData.Length == 0)
                CLOUD_DATA = "Loaded Empty";
            else
                CLOUD_DATA = Encoding.ASCII.GetString(savedData);

            Debug.Log("Loaded Data:");
            Debug.Log(CLOUD_DATA);
        }
    }
    private void OnSavedGameDataWritten(SavedGameRequestStatus status, ISavedGameMetadata game){
        if (status == SavedGameRequestStatus.Success){
            Debug.Log("Saved Data:");
            Debug.Log(CLOUD_DATA);
        }
    }

    public string GetVariable(string KEY){
        string[] DATA = CLOUD_DATA.Split(new char[] { '*' }, StringSplitOptions.RemoveEmptyEntries);
        int id = 0;

        foreach (string s in DATA){
            if (s == KEY){
                return DATA[id + 1];
            }
            else
                id++;
        }
        return "0";
    }
    public void SaveVariable(string KEY, string VALUE){
        List<string> DATA = new List<string>();
        if (CLOUD_DATA == "Loaded Empty")
            CLOUD_DATA = "";

        DATA.AddRange(CLOUD_DATA.Split(new char[] { '*' }, StringSplitOptions.RemoveEmptyEntries));
        SAVE_DATA = "";//clean data

        bool keyfound = false;
        int id = 0;
        foreach (string s in DATA){
            if (s == KEY){
                DATA[id + 1] = VALUE;
                keyfound = true;
                break;
            }
            else
                id++;
        }
        if (!keyfound){
            DATA.Add(KEY);
            DATA.Add(VALUE);
        }
        //update old data+ new data
        foreach (string s in DATA){
            SAVE_DATA += s + "*";
        }
        CLOUD_DATA = SAVE_DATA;//when you post a new data update the cloud one
        SaveData();
    }
    public bool DataLoaded(){
        if (CLOUD_DATA != null)
            return CLOUD_DATA.Length > 1;
        else
            return false;
    }
}
