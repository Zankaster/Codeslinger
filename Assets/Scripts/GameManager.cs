using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public GamePhase gamePhase;
    private Board board;
    private ScenarioManager scenarioManager;
    private StatsManager statsManager;
    public GameObject nodePrefab;
    public RectTransform nodesConsole;
    int nodeLength = 28;
    Node node1, node2;
    public TextAsset puzzleFile;
    public TextAsset scenarioStory;
    public ScenarioGameMode gameMode;
    public MenuElementMover gameOverText;
    public MenuElementMover gameWonText;
    SoundFxManager soundFxManager;
    public string nextScene;
    public string nextLevelName;
    ScenarioLoader scenarioLoader;
    public bool hackingInProgress { get; set; }
    public bool waitingForAnimationsFinish { get; set; }
    public bool boardEvaluationNeeded { get; private set; }
    DigitalGlitch digitalGlitch;

    void Awake() {
        digitalGlitch = FindObjectOfType<DigitalGlitch>();
        scenarioLoader = FindObjectOfType<ScenarioLoader>();
        if (scenarioLoader != null)
            LevelPreload.currentScenario = scenarioLoader.LoadScenario(nextLevelName);
        ProgressSaveAndLoad.LoadGame();
        if (gameMode == ScenarioGameMode.words && puzzleFile == null) {
            puzzleFile = LevelPreload.currentScenario.puzzle;
            scenarioStory = LevelPreload.currentScenario.story;
        }
    }

    void Start() {
        InitHackingBoardConfigurationIfNull();
        boardEvaluationNeeded = false;
        waitingForAnimationsFinish = false;
        soundFxManager = FindObjectOfType<SoundFxManager>();
        scenarioManager = FindObjectOfType<ScenarioManager>();
        statsManager = FindObjectOfType<StatsManager>();
        scenarioManager.SetGameMode(gameMode);
        if (gameMode == ScenarioGameMode.words) {
            waitingForAnimationsFinish = true;
            scenarioManager.ReadScenario(scenarioStory);
            board = new Board(8, 11, BoardRules.letters);
            board.GenerateBoardFromFile(puzzleFile);
            for (int i = 0; i < board.BoardNodes.Count; i++) {
                InstantiateNode(board.BoardNodes[i]);
            }
            scenarioManager.AdvanceScenario();
        }
        else if (gameMode == ScenarioGameMode.hacking) {
            gamePhase = GamePhase.spawning;
            waitingForAnimationsFinish = true;
            board = new Board(8, 11, BoardRules.hacking);
            board.GenerateRandomHackingBoard(LogicHelper.HackingBoardConfiguration);
            for (int i = 0; i < board.BoardNodes.Count; i++) {
                InstantiateNode(board.BoardNodes[i]);
            }
        }
        else if (gameMode == ScenarioGameMode.story) {
            scenarioManager.ReadScenario(scenarioStory);
            scenarioManager.AdvanceScenario();
        }
    }

    public void InstantiateNode(Node node) {
        var nodeGameObject = Instantiate(nodePrefab, nodesConsole, false);
        nodeGameObject.transform.localPosition = new Vector3(node.postionX * nodeLength, -(node.postionY) * nodeLength);
        nodeGameObject.transform.GetChild(0).GetComponent<NodeDisplayer>().SetNode(node);
    }



    public void NodeClick(Node node) {
        switch (gameMode) {
            case ScenarioGameMode.words:
                if (gamePhase != GamePhase.waitingInput)
                    return;
                node1 = node;
                break;
            case ScenarioGameMode.hacking:
                if (gamePhase != GamePhase.waitingInput)
                    return;
                gamePhase = GamePhase.processing;
                waitingForAnimationsFinish = true;
                hackingInProgress = true;

                var hackingResult = board.HackNode(node);

                if (hackingResult.data > 0)
                    soundFxManager.PlayFx(SoundType.dataCollected);
                if (hackingResult.power < 0) {
                    StartCoroutine(GlitchScreen(1.5f));
                }

                var gameStatus = statsManager.UpdateStats(hackingResult);
                if (gameStatus == GameStatus.gameover) {
                    board.DestroyAllBoard();
                    StartCoroutine(ShowHackingFailed());
                }
                else if (gameStatus == GameStatus.won) {
                    Stats.TransactionUnits += Stats.HackingTransactionUnits;
                    Stats.Data += LogicHelper.HackingBoardConfiguration.DataReward;
                    ProgressSaveAndLoad.SaveGame();
                    Debug.Log(Stats.TransactionUnits);
                    board.DestroyAllBoard();
                    StartCoroutine(ShowHackingSuccess());
                }
                else if (gameStatus == GameStatus.playing) {
                    gamePhase = GamePhase.destroying;
                    board.DestroyMarkedNodes();
                }
                break;
        }
    }

    private IEnumerator ShowHackingFailed() {
        yield return new WaitForSeconds(.5f);
        yield return StartCoroutine(gameOverText.TransitionInAndOut(gameOverText.transform.localPosition, gameOverText.transform.localPosition));
        GoToNextScene();
    }

    private IEnumerator ShowHackingSuccess() {
        yield return new WaitForSeconds(.5f);
        yield return StartCoroutine(gameWonText.TransitionInAndOut(gameOverText.transform.localPosition, gameOverText.transform.localPosition));
        GoToNextScene();
    }

    public void NodeHover(Node node) {
        if (gamePhase != GamePhase.waitingInput)
            return;
        switch (gameMode) {
            case ScenarioGameMode.words:
                if (node1 == null)
                    return;
                node2 = node;
                break;
        }
    }

    public void NodeRelease() {
        if (gameMode != ScenarioGameMode.words)
            return;
        if (scenarioManager.GetScenarioStepType() != ScenarioStepType.action && scenarioManager.GetScenarioStepType() != ScenarioStepType.choice)
            return;
        if (gamePhase != GamePhase.waitingInput)
            return;
        gamePhase = GamePhase.processing;
        if (board.SwapNodes(node1, node2)) {
            soundFxManager.PlayFx(SoundType.selection2);
            boardEvaluationNeeded = true;
        }
        else {
            soundFxManager.PlayFx(SoundType.powerLost);
            gamePhase = GamePhase.waitingInput;
        }
    }

    public void BoardAnimationFinished() {
        if (gameMode == ScenarioGameMode.words && gamePhase == GamePhase.processing && boardEvaluationNeeded) {
            if (boardEvaluationNeeded) {
                boardEvaluationNeeded = false;
                EvaluateBoard();
            }
        }
    }

    internal void BoardDestroyFinished() {
        if (gameMode == ScenarioGameMode.words || (statsManager.GetGameStatus() != GameStatus.gameover && gamePhase == GamePhase.destroying)) {
            gamePhase = GamePhase.refilling;
            waitingForAnimationsFinish = true;
            board.RearrangeNodes();
            if (gameMode == ScenarioGameMode.hacking) {
                List<Node> addedNodes = board.RefillHackingNodesReserve(LogicHelper.HackingBoardConfiguration);
                for (int i = 0; i < addedNodes.Count; i++) {
                    InstantiateNode(addedNodes[i]);
                }
            }
        }
    }

    private void EvaluateBoard() {
        switch (gameMode) {
            case ScenarioGameMode.words:
                if (scenarioManager.GetScenarioStepType() == ScenarioStepType.dialogue)
                    return;
                var result = board.CalculateInteraction(scenarioManager.GetWordsToMatch());

                if (result.wordsMatched.Count > 0) {
                    soundFxManager.PlayFx(SoundType.dataCollected);
                    gamePhase = GamePhase.destroying;
                    scenarioManager.RemoveMatchedWords((result.wordsMatched));
                    if (scenarioManager.GetWordsToMatch().Count == 0)
                        scenarioManager.AdvanceScenario(true);
                    else if (scenarioManager.GetScenarioStepType() == ScenarioStepType.choice) {
                        string choice = result.wordsMatched[0].word;
                        if (choice == "freedom") {
                            nextScene = "EpilogueGood";
                        }
                        else {
                            nextScene = "EpilogueBad";
                        }
                        scenarioManager.AdvanceScenario(true);
                    }

                }
                else {
                    soundFxManager.PlayFx(SoundType.powerLost);
                    gamePhase = GamePhase.restoring;
                    board.RestoreNodes(node2, node1);
                }

                node1 = null;
                node2 = null;
                break;
        }
    }

    public void OnScreenPressed() {
        if (!scenarioManager.EvaluateAdvanceScenario())
            GoToNextScene();
    }

    public void GoToNextScene() {
        if (LevelPreload.currentScenario != null) {
            if (LevelPreload.currentScenario.name == "Story1") {
                Stats.StoryMission1Completed = true;
                ProgressSaveAndLoad.SaveGame();
            }
            else if (LevelPreload.currentScenario.name == "Story2") {
                Stats.StoryMission2Completed = true;
                ProgressSaveAndLoad.SaveGame();
            }
        }
        SceneManager.LoadScene(nextScene);
    }

    public void InitHackingBoardConfigurationIfNull() {
        if (LogicHelper.HackingBoardConfiguration != null)
            return;
        HackingBoardConfiguration hackingBoardConfiguration = new HackingBoardConfiguration();
        HackingNode n1 = new HackingNode() { symbol = Symbol.power, spawnOdds = .25f };
        HackingNode n2 = new HackingNode() { symbol = Symbol.drain, spawnOdds = .35f };
        HackingNode n3 = new HackingNode() { symbol = Symbol.data, spawnOdds = .35f };
        HackingNode n4 = new HackingNode() { symbol = Symbol.transactionUnit, spawnOdds = .05f };
        hackingBoardConfiguration.AddHackingNode(n1);
        hackingBoardConfiguration.AddHackingNode(n2);
        hackingBoardConfiguration.AddHackingNode(n3);
        hackingBoardConfiguration.AddHackingNode(n4);
        hackingBoardConfiguration.SetTargetData(5);
        Stats.HackingPower = 6;
        Stats.MaxPower = 6;
        LogicHelper.SetHackingBoardConfiguration(hackingBoardConfiguration);
    }

    public void SpawnFinished() {
        if (gamePhase == GamePhase.spawning) {
            gamePhase = GamePhase.waitingInput;
        }
    }

    public void MotionFinished() {
        Debug.Log("motionfinished");
        if (gamePhase == GamePhase.refilling) {
            gamePhase = GamePhase.waitingInput;
            Debug.Log("Waiting Input from refilling");
        }
        else if (gamePhase == GamePhase.processing) {
            if (boardEvaluationNeeded) {
                boardEvaluationNeeded = false;
                EvaluateBoard();
            }
        }
    }

    public void RestoreFinished() {
        Debug.Log("restorefinished");
        if (gamePhase == GamePhase.restoring) {
            gamePhase = GamePhase.waitingInput;
            Debug.Log("Waiting Input from restoring");
        }
    }

    IEnumerator GlitchScreen(float time) {
        digitalGlitch.intensity = 0.2f;
        yield return new WaitForSeconds(time);
        digitalGlitch.intensity = 0.0f;
    }

}