using System.Collections.Generic;
using UnityEngine;

public class SpriteCollector : MonoBehaviour {

    public List<Sprite> symbolsList;
    Dictionary<string, Sprite> symbolsDictionary;

    void Start() {
        symbolsDictionary = new Dictionary<string, Sprite>();
        foreach (Sprite s in symbolsList)
            symbolsDictionary.Add(s.name.ToLower(), s);
    }

    public Sprite GetSprite(string name) {
        try {
            return symbolsDictionary[name];
        }
        catch {
            throw new System.Exception("Sprite " + name + " not found in the SpriteCollector");
        }
    }
}
