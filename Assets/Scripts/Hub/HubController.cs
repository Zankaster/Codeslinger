using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubController : MonoBehaviour {

    string levelName;
    ScenarioGameMode gameMode;

    void Awake() {
        ProgressSaveAndLoad.LoadGame();
    }

    public void SelectLevel(string levelName) {
        this.levelName = levelName;
    }

    public string GetSelectedLevel() {
        return levelName;
    }

    public void SelectGameMode(ScenarioGameMode gameMode) {
        this.gameMode = gameMode;
    }

    public ScenarioGameMode GetSelectedGameMode() {
        return gameMode;
    }
}
