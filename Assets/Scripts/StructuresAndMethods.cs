using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Symbol {
    a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p, q, r, s, t, u, v, w, x, y, z,
    zero, one, two, three, four, five, six, seven, eight, nine,
    add, sub, mul, div, eq, neq, exp, lthan, gthan,
    power, drain, data, power2, drain2, data2, transactionUnit, transactionUnit2
}

public enum BoardRules {
    letters, numbers, hacking
}

public enum MatchDirection {
    horizontal, vertical
}

public enum ScenarioStepType {
    dialogue,
    action,
    choice
}

public enum ScenarioGameMode {
    words,
    hacking,
    story,
    test
}

public enum GameStatus {
    playing,
    won,
    gameover
}

public enum SoundType {
    dataCollected,
    powerLost,
    death,
    selection1,
    selection2,
    selection3,
    selectionFailed1,
    buy1
}

public enum GamePhase {
    spawning,
    waitingInput,
    processing,
    restoring,
    destroying,
    refilling
}

[Serializable]
public class Scenario {
    public string name;
    public TextAsset story;
    public TextAsset puzzle;
}

[Serializable]
public class SoundClip {
    public string name;
    public AudioClip audioClip;
}

public class HackingBoardConfiguration {
    List<HackingNode> hackingNodes;
    public int TargetData { get; private set; }
    public int DataReward { get; private set; } = 1;

    public HackingBoardConfiguration() {
        hackingNodes = new List<HackingNode>();
    }

    public void SetTargetData(int targetData) {
        TargetData = targetData;
    }

    public void SetDataReward(int dataReward) {
        DataReward = dataReward;
    }

    public void AddHackingNode(HackingNode node) {
        hackingNodes.Add(node);
        if (hackingNodes.Sum(x => x.spawnOdds) > 1)
            throw new Exception("Hacking board odds over 1");
    }

    public Symbol GetRandomHackingNode() {
        int r = UnityEngine.Random.Range(1, 100);
        int total = 0;
        foreach(HackingNode n in hackingNodes) {
            total += (int)(n.spawnOdds * 100);
            if (r <= total)
                return n.symbol;
        }
        //it should never reach this section
        return hackingNodes[0].symbol;
    }
}

public class HackingNode {
    public Symbol symbol;
    public float spawnOdds;
}

public class WordMatch {
    public Vector2Int startPosition { get; private set; }
    public Vector2Int endPosition { get; private set; }
    public string word { get; private set; }

    public WordMatch(Vector2Int startPosition, Vector2Int endPosition, string word) {
        this.startPosition = startPosition;
        this.endPosition = endPosition;
        this.word = word;
    }
}

public class HackingResult {
    public int power = 0;
    public int data = 0;
    public int transactionUnits = 0;
    public int[] downShift;
}

public class WordMatchResult {
    public List<WordMatch> wordsMatched = new List<WordMatch>();
    public int[] downShift;
}

[Serializable]
public class MotionPreset {
    public bool rotationX;
    public bool rotationY;
    public bool rotationZ;
    public bool scaleX;
    public bool scaleY;
    public float duration;
    public int motionCurveIndex;
    public int rotationCurveIndex;
    public int scaleCurveIndex;
}

[Serializable]
public class CurvePreset {
    public string name;
    public AnimationCurve curve;
}

public static class Stats {
    public static int MaxPower { get; set; } = 3;
    public static int HackingPower { get; set; } = 0;
    public static int TransactionUnits { get; set; } = 0;
    public static int HackingTransactionUnits { get; set; } = 0;
    public static int HackingTargetData { get; set; } = 0;
    public static int HackingCurrentData { get; set; } = 0;
    public static int Data { get; set; } = 0;
    public static bool HackingMission2Unlocked { get; set; } = false;
    public static bool HackingMission3Unlocked { get; set; } = false;
    public static bool StoryMission1Unlocked { get; set; } = false;
    public static bool StoryMission1Completed { get; set; } = false;
    public static bool StoryMission2Unlocked { get; set; } = false;
    public static bool StoryMission2Completed { get; set; } = false;
    public static bool StoryMission3Unlocked { get; set; } = false;

    public static void ResetHackingStats() {
        HackingPower = MaxPower;
        HackingCurrentData = 0;
        HackingTransactionUnits = 0;
    }

    public static void InitHackingSession(int targetData) {
        HackingTargetData = targetData;
        ResetHackingStats();
    }
}

public static class LevelPreload {
    public static Scenario currentScenario;
}

public static class LogicHelper {

    public static HackingBoardConfiguration HackingBoardConfiguration { get; private set; }

    public static void SetHackingBoardConfiguration(HackingBoardConfiguration hbc) {
        HackingBoardConfiguration = hbc;
        Stats.HackingTargetData = HackingBoardConfiguration.TargetData;
    } 

    public static Symbol ConvertSymbolCharToEnum(string name) {
        try {
            return symbolCharToEnum[name];
        }
        catch {
            throw new System.Exception("Symbol not found in the translation dictionary");
        }
    }

    public static string ConvertSymbolEnumToBoardString(this Symbol symbol) {
        try {
            return symbolEnumToBoardString[symbol];
        }
        catch {
            throw new System.Exception("Symbol not found in the translation dictionary");
        }
    }

    public static void Swap<T>(IList<T> listA, IList<T> listB, int indexA, int indexB) {
        T tmp = listA[indexB];
        listA[indexB] = listB[indexA];
        listB[indexA] = tmp;
    }

    public static readonly Dictionary<Symbol, string> symbolEnumToBoardString = new Dictionary<Symbol, string>() {
        {Symbol.a,"a"},
        {Symbol.b,"b"},
        {Symbol.c,"c"},
        {Symbol.d,"d"},
        {Symbol.e,"e"},
        {Symbol.f,"f"},
        {Symbol.g,"g"},
        {Symbol.h,"h"},
        {Symbol.i,"i"},
        {Symbol.j,"j"},
        {Symbol.k,"k"},
        {Symbol.l,"l"},
        {Symbol.m,"m"},
        {Symbol.n,"n"},
        {Symbol.o,"o"},
        {Symbol.p,"p"},
        {Symbol.q,"q"},
        {Symbol.r,"r"},
        {Symbol.s,"s"},
        {Symbol.t,"t"},
        {Symbol.u,"u"},
        {Symbol.v,"v"},
        {Symbol.w,"w"},
        {Symbol.x,"x"},
        {Symbol.y,"y"},
        {Symbol.z,"z"},
        {Symbol.zero,"0"},
        {Symbol.one,"1"},
        {Symbol.two,"2"},
        {Symbol.three,"3"},
        {Symbol.four,"4"},
        {Symbol.five,"5"},
        {Symbol.six,"6"},
        {Symbol.seven,"7"},
        {Symbol.eight,"8"},
        {Symbol.nine,"9"},
        {Symbol.add,"+"},
        {Symbol.sub,"-"},
        {Symbol.mul,"*"},
        {Symbol.div,"/"},
        {Symbol.gthan,">"},
        {Symbol.lthan,"<"},
        {Symbol.eq,"="},
        {Symbol.neq,"!"},
        {Symbol.power,"ù"},
        {Symbol.drain,"è"},
        {Symbol.data,"à"}
    };

    public static readonly Dictionary<string, Symbol> symbolCharToEnum = new Dictionary<string, Symbol>() {
        {"a",Symbol.a},
        {"b",Symbol.b},
        {"c",Symbol.c},
        {"d",Symbol.d},
        {"e",Symbol.e},
        {"f",Symbol.f},
        {"g",Symbol.g},
        {"h",Symbol.h},
        {"i",Symbol.i},
        {"j",Symbol.j},
        {"k",Symbol.k},
        {"l",Symbol.l},
        {"m",Symbol.m},
        {"n",Symbol.n},
        {"o",Symbol.o},
        {"p",Symbol.p},
        {"q",Symbol.q},
        {"r",Symbol.r},
        {"s",Symbol.s},
        {"t",Symbol.t},
        {"u",Symbol.u},
        {"v",Symbol.v},
        {"w",Symbol.w},
        {"x",Symbol.x},
        {"y",Symbol.y},
        {"z",Symbol.z},
        {"0",Symbol.zero},
        {"1",Symbol.one},
        {"2",Symbol.two},
        {"3",Symbol.three},
        {"4",Symbol.four},
        {"5",Symbol.five},
        {"6",Symbol.six},
        {"7",Symbol.seven},
        {"8",Symbol.eight},
        {"9",Symbol.nine},
        {"+",Symbol.add},
        {"-",Symbol.sub},
        {"*",Symbol.mul},
        {"/",Symbol.div},
        {">",Symbol.gthan},
        {"<",Symbol.lthan},
        {"=",Symbol.eq},
        {"!",Symbol.neq}
    };

    public static readonly Dictionary<string, string> nodesEffectsDescription = new Dictionary<string, string>() {
        { "Data", "+1 Data" },
        { "Power", "+1 Power" },
        { "Drain", "-1 Power" },
        { "TransactionUnit", "+1 Transaction Units" },
        { "Data2", "+2 Data" },
        { "Power2", "+2 Power" },
        { "Drain2", "-2 Power" },
        { "TransactionUnit2", "+2 Transaction Units" }
    };
}
