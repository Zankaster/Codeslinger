using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Text;

public class ScenarioManager : MonoBehaviour {
    Text scenarioText;
    int index = -1;
    List<ScenarioStep> scenarioSteps;
    ScenarioGameMode gameMode;
    public float readingSpeed = 0.01f;
    bool rollingText = false;
    Coroutine fillTextBox = null;

    void Awake() {
        scenarioText = GameObject.Find("ScenarioText").GetComponent<Text>();
    }

    public bool EvaluateAdvanceScenario() {
        if (index == -1 || scenarioSteps[index].scenarioStepType == ScenarioStepType.dialogue) {
            return AdvanceScenario();
        }
        return true;
    }

    public void SetGameMode(ScenarioGameMode gameMode) {
        this.gameMode = gameMode;
    }

    public bool AdvanceScenario(bool forceNextStep = false) {
        switch (gameMode) {
            case ScenarioGameMode.words:
                if (rollingText) {
                    rollingText = false;
                    if (fillTextBox != null)
                        StopCoroutine(fillTextBox);
                    if (forceNextStep) {
                        if (index + 1 >= scenarioSteps.Count) {
                            return false; //on a false, the gamemanager loads the next level
                        }
                        index++;
                        fillTextBox = StartCoroutine(FillTextBox(scenarioSteps[index].dialogue));
                    }
                    else
                        scenarioText.text = scenarioSteps[index].dialogue;
                }
                else {
                    if (index + 1 >= scenarioSteps.Count) {
                        return false; //on a false, the gamemanager loads the next level
                    }
                    index++;
                    fillTextBox = StartCoroutine(FillTextBox(scenarioSteps[index].dialogue));
                }
                break;
            case ScenarioGameMode.story:
                if (rollingText) {
                    rollingText = false;
                    if(fillTextBox != null)
                        StopCoroutine(fillTextBox);
                    scenarioText.text = scenarioSteps[index].dialogue;
                }
                else {
                    if (index + 1 >= scenarioSteps.Count) {
                        return false; //on a false, the gamemanager loads the next level
                    }
                    index++;
                    fillTextBox = StartCoroutine(FillTextBox(scenarioSteps[index].dialogue));
                }
                break;
        }
        return true;
    }

    IEnumerator FillTextBox(string text) {
        rollingText = true;
        StringBuilder filledText = new StringBuilder();
        var splitText = text.Split(' ');

        foreach (string s in splitText) {
            for (int j = 0; j < s.Length; j++)
                filledText.Append("_");

            for (int i = 0; i < s.Length; i++) {
                filledText[filledText.Length - s.Length + i] = s[i];
                scenarioText.text = filledText.ToString();
                yield return new WaitForSeconds(readingSpeed);
            }
            filledText.Append(" ");
        }
        rollingText = false;
    }

    public void RemoveMatchedWords(List<WordMatch> wordsMatched) {
        var words = wordsMatched.Select(x => x.word).ToList();
        for (int i = 0; i < words.Count; i++) {
            for (int j = 0; j < scenarioSteps[index].wordsToMatch.Count; j++) {
                if (scenarioSteps[index].wordsToMatch[j] == words[i])
                    scenarioSteps[index].wordsToMatch.Remove(scenarioSteps[index].wordsToMatch[j]);
            }
        }
    }

    public List<string> GetWordsToMatch() {
        if (scenarioSteps[index].scenarioStepType != ScenarioStepType.action && scenarioSteps[index].scenarioStepType != ScenarioStepType.choice)
            return null;
        else
            return scenarioSteps[index].wordsToMatch;
    }

    public ScenarioStepType GetScenarioStepType() {
        return scenarioSteps[index].scenarioStepType;
    }

    public void ReadScenario(TextAsset scenario) {
        scenarioSteps = new List<ScenarioStep>();
        var stepsRaw = scenario.text.Split(new string[] { "/*" }, StringSplitOptions.None);
        var steps = stepsRaw.Skip(1).Take(stepsRaw.Length - 2).ToList();
        for (int i = 0; i < steps.Count; i++) {
            var sequence = steps[i].Split(new string[] { "\r\n" }, StringSplitOptions.None).Skip(1).Take(steps[i].Length - 2).ToList();
            if (sequence[0] == "dialogue") {
                ScenarioStep s = new ScenarioStep(
                    ScenarioStepType.dialogue,
                    string.Concat(sequence.Skip(1).Select(x => x + (sequence.Count > 3 ? "\r\n" : ""))), //check to avoid putting carriage return on single line dialogue steps
                    null);
                scenarioSteps.Add(s);
            }
            else if (sequence[0] == "action") {
                ScenarioStep s = new ScenarioStep(
                    ScenarioStepType.action,
                    sequence[1],
                    sequence.Skip(3).Take(sequence.Count - 4).ToList());
                scenarioSteps.Add(s);
            }
            else if (sequence[0] == "choice") {
                ScenarioStep s = new ScenarioStep(
                    ScenarioStepType.choice,
                    sequence[1],
                    sequence.Skip(3).Take(sequence.Count - 4).ToList());
                scenarioSteps.Add(s);
            }
        }
    }
}