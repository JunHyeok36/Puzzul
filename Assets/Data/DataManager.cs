using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class DataManager
{
    static string path = Path.Combine(Application.persistentDataPath, "data.bin");

	public static void CheckingData() {
		if(File.Exists(path)) {
			Data data = Load();
			InitData(data);
			Save(data);
		} else {
			//Data is not exist
			BinaryFormatter formatter = new BinaryFormatter();
			FileStream file = new FileStream(path, FileMode.Create);
			file.Position = 0;
			Data data = new Data();
			InitData(data);
			formatter.Serialize(file, data);
			file.Close();
		}
	}

	public static void Save(Data data) {
		BinaryFormatter formatter = new BinaryFormatter();
		FileStream file = new FileStream(path, FileMode.Create);
		file.Position = 0;
		formatter.Serialize(file, data);
		file.Close();
	}

	public static Data Load() {
		try {
			BinaryFormatter formatter = new BinaryFormatter();
			FileStream file = new FileStream(path, FileMode.Open);
			file.Position = 0;
			Data data = (Data)formatter.Deserialize(file);
			file.Close();
			return data;
		} catch {
			return null;
		}
	}

	public static void InitData(Data data) {
		if(!data.audio_BGM.HasValue) {data.audio_BGM = -20f;}
		if(!data.audio_SFX.HasValue) {data.audio_SFX = -20f;}
		if(!data.mainRuneAvailable.HasValue) {data.mainRuneAvailable = false;}
		if(!data.highscore.HasValue) {data.highscore = 0;}
		if(!data.longHighscore.HasValue) {data.longHighscore = 0;}
		if(!data.lastscore.HasValue) {data.lastscore = 0;}
		if(!data.Coin.HasValue) {data.Coin = 0;}
		if(data.mainRune_list.Count <= 0) {data.mainRune_list.Add(MainRune.None);}
		if(!data.skin_index.HasValue) {data.skin_index = 0;}
		if(data.skin_list.Count <= 0) {data.skin_list.Add(0);}
		if(!data.autoLoginGooglePlayServices.HasValue) {data.autoLoginGooglePlayServices = false;}
		if(!data.lang.HasValue) {data.lang = Application.systemLanguage; }
	}

	public static void InitData() {
		Data data = new Data();

		InitData(data);

		Save(data);
	}

	public static bool ExistData() {
		return File.Exists(path);
	}
}
