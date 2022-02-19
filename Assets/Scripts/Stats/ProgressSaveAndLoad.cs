using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class ProgressSaveAndLoad {

    public static void SaveGame() {
        PlayerPrefs.SetInt("MaxPower", Stats.MaxPower);
        PlayerPrefs.SetInt("Data", Stats.Data);
        PlayerPrefs.SetInt("TransactionUnits", Stats.TransactionUnits);
        PlayerPrefs.SetInt("StoryMission1Unlocked", Stats.StoryMission1Unlocked ? 1 : 0);
        PlayerPrefs.SetInt("StoryMission1Completed", Stats.StoryMission1Completed ? 1 : 0);
        PlayerPrefs.SetInt("StoryMission2Unlocked", Stats.StoryMission2Unlocked ? 1 : 0);
        PlayerPrefs.SetInt("StoryMission2Completed", Stats.StoryMission2Unlocked ? 1 : 0);
        PlayerPrefs.SetInt("StoryMission3Unlocked", Stats.StoryMission3Unlocked ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static void LoadGame() {
        Stats.MaxPower = PlayerPrefs.GetInt("MaxPower", Stats.MaxPower);
        Stats.Data = PlayerPrefs.GetInt("Data", Stats.Data);
        Stats.TransactionUnits = PlayerPrefs.GetInt("TransactionUnits", Stats.TransactionUnits);
        Stats.StoryMission1Unlocked = (PlayerPrefs.GetInt("StoryMission1Unlocked", Stats.StoryMission1Unlocked ? 1 : 0) == 1 ? true : false);
        Stats.StoryMission2Unlocked = (PlayerPrefs.GetInt("StoryMission2Unlocked", Stats.StoryMission2Unlocked ? 1 : 0) == 1 ? true : false);
        Stats.StoryMission3Unlocked = (PlayerPrefs.GetInt("StoryMission3Unlocked", Stats.StoryMission3Unlocked ? 1 : 0) == 1 ? true : false);
        Stats.StoryMission1Completed = (PlayerPrefs.GetInt("StoryMission1Completed", Stats.StoryMission1Completed ? 1 : 0) == 1 ? true : false);
        Stats.StoryMission2Completed = (PlayerPrefs.GetInt("StoryMission2Completed", Stats.StoryMission2Completed ? 1 : 0) == 1 ? true : false);
    }
}
