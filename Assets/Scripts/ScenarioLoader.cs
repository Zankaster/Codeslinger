using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioLoader : MonoBehaviour {

    [SerializeField]
    List<Scenario> scenarios;

    public Scenario LoadScenario(string levelName) {
        return scenarios.Where(x => x.name == levelName).First();
    }

}