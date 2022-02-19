using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour, IPointerClickHandler {

    string levelName;
    ScenarioGameMode gameMode;
    private MenuElementMover buttonMover;
    HubController hubController;
    ScenarioLoader scenarioLoader;
    SoundFxManager soundFxManager;
    bool startingLevel = false;

    void Awake() {
        soundFxManager = FindObjectOfType<SoundFxManager>();
        buttonMover = GetComponent<MenuElementMover>();
        hubController = FindObjectOfType<HubController>();
        scenarioLoader = FindObjectOfType<ScenarioLoader>();
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (startingLevel)
            return;
        StartCoroutine(StartLevel());
    }

    private IEnumerator StartLevel() {
        levelName = hubController.GetSelectedLevel();
        gameMode = hubController.GetSelectedGameMode();
        if (!CheckRequirements()) {
            //error message
            soundFxManager.PlayFx(SoundType.selectionFailed1);
            yield break;
        }
        soundFxManager.PlayFx(SoundType.selection2);
        startingLevel = true;
        yield return StartCoroutine(ButtonSuccess());
        if (gameMode == ScenarioGameMode.hacking) {
            InitializeHackingBoardConfig();
            Stats.InitHackingSession(LogicHelper.HackingBoardConfiguration.TargetData);
            ProgressSaveAndLoad.SaveGame();
            SceneManager.LoadScene("Hacking");
        }
        else if (gameMode == ScenarioGameMode.words) {
            InitializeWordBoardConfig();
            ProgressSaveAndLoad.SaveGame();
            SceneManager.LoadScene("Words");
        }
    }

    private IEnumerator ButtonSuccess() {
        yield return StartCoroutine(buttonMover.AnimateElement(transform.localPosition, transform.localPosition));
    }

    protected void InitializeWordBoardConfig() {
        LevelPreload.currentScenario = scenarioLoader.LoadScenario(levelName);
    }

    public bool CheckRequirements() {
        if (levelName == "Hacking001") {
            return true;
        }
        else if (levelName == "Hacking010") {
            if (Stats.MaxPower >= 4)
                return true;
        }
        else if (levelName == "Hacking011") {
            if (Stats.MaxPower >= 6)
                return true;
        }
        else if (levelName == "Story1") {
            if (Stats.StoryMission1Unlocked)
                return true;
            else if(Stats.Data >= 2) {
                Stats.Data -= 2;
                Stats.StoryMission1Unlocked = true;
                return true;
            }
        }
        else if (levelName == "Story2") {
            if (Stats.StoryMission2Unlocked )
                return true;
            else if (Stats.Data >= 4 && Stats.StoryMission1Completed) {
                Stats.Data -= 4;
                Stats.StoryMission2Unlocked = true;
                return true;
            }
        }
        else if (levelName == "Story3") {
            if (Stats.StoryMission3Unlocked )
                return true;
            else if (Stats.Data >= 6 && Stats.StoryMission2Completed) {
                Stats.Data -= 6;
                Stats.StoryMission3Unlocked = true;
                return true;
            }
        }
        return false;
    }

    protected void InitializeHackingBoardConfig() {
        if (levelName == "Hacking001") {
            HackingBoardConfiguration hackingBoardConfiguration = new HackingBoardConfiguration();
            HackingNode n1 = new HackingNode() { symbol = Symbol.power, spawnOdds = .25f };
            HackingNode n2 = new HackingNode() { symbol = Symbol.drain, spawnOdds = .35f };
            HackingNode n3 = new HackingNode() { symbol = Symbol.data, spawnOdds = .35f };
            HackingNode n4 = new HackingNode() { symbol = Symbol.transactionUnit, spawnOdds = .05f };
            hackingBoardConfiguration.AddHackingNode(n1);
            hackingBoardConfiguration.AddHackingNode(n2);
            hackingBoardConfiguration.AddHackingNode(n3);
            hackingBoardConfiguration.AddHackingNode(n4);
            hackingBoardConfiguration.SetTargetData(30);
            hackingBoardConfiguration.SetDataReward(1);
            LogicHelper.SetHackingBoardConfiguration(hackingBoardConfiguration);
        }
        else if (levelName == "Hacking010") {
            HackingBoardConfiguration hackingBoardConfiguration = new HackingBoardConfiguration();
            HackingNode n1 = new HackingNode() { symbol = Symbol.power, spawnOdds = .20f };
            HackingNode n2 = new HackingNode() { symbol = Symbol.drain, spawnOdds = .30f };
            HackingNode n3 = new HackingNode() { symbol = Symbol.data, spawnOdds = .27f };
            HackingNode n4 = new HackingNode() { symbol = Symbol.transactionUnit, spawnOdds = .05f };
            HackingNode n5 = new HackingNode() { symbol = Symbol.power2, spawnOdds = .05f };
            HackingNode n6 = new HackingNode() { symbol = Symbol.drain2, spawnOdds = .05f };
            HackingNode n7 = new HackingNode() { symbol = Symbol.data2, spawnOdds = .05f };
            HackingNode n8 = new HackingNode() { symbol = Symbol.transactionUnit2, spawnOdds = .03f };
            hackingBoardConfiguration.AddHackingNode(n1);
            hackingBoardConfiguration.AddHackingNode(n2);
            hackingBoardConfiguration.AddHackingNode(n3);
            hackingBoardConfiguration.AddHackingNode(n4);
            hackingBoardConfiguration.AddHackingNode(n5);
            hackingBoardConfiguration.AddHackingNode(n6);
            hackingBoardConfiguration.AddHackingNode(n7);
            hackingBoardConfiguration.AddHackingNode(n8);
            hackingBoardConfiguration.SetTargetData(45);
            hackingBoardConfiguration.SetDataReward(2);
            LogicHelper.SetHackingBoardConfiguration(hackingBoardConfiguration);
        }
        else if (levelName == "Hacking011") {
            HackingBoardConfiguration hackingBoardConfiguration = new HackingBoardConfiguration();
            HackingNode n5 = new HackingNode() { symbol = Symbol.power2, spawnOdds = .2f };
            HackingNode n6 = new HackingNode() { symbol = Symbol.drain2, spawnOdds = .35f };
            HackingNode n7 = new HackingNode() { symbol = Symbol.data2, spawnOdds = .35f };
            HackingNode n8 = new HackingNode() { symbol = Symbol.transactionUnit2, spawnOdds = .1f };
            hackingBoardConfiguration.AddHackingNode(n5);
            hackingBoardConfiguration.AddHackingNode(n6);
            hackingBoardConfiguration.AddHackingNode(n7);
            hackingBoardConfiguration.AddHackingNode(n8);
            hackingBoardConfiguration.SetTargetData(60);
            hackingBoardConfiguration.SetDataReward(3);
            LogicHelper.SetHackingBoardConfiguration(hackingBoardConfiguration);
        }
    }
}