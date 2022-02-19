using System.Collections.Generic;

public class ScenarioStep
{
    public ScenarioStepType scenarioStepType { get; private set; }
    public string dialogue;
    public List<string> wordsToMatch;

    public ScenarioStep(ScenarioStepType scenarioStepType, string dialogue, List<string> wordsToMatch) {
        this.scenarioStepType = scenarioStepType;
        this.dialogue = dialogue;
        this.wordsToMatch = wordsToMatch;
    }
}