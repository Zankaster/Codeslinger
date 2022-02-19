using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuElementMover : MoverUI
{
    public override void Awake() {
        base.Awake();
    }

    public IEnumerator AnimateElement(Vector2 origin, Vector2 destination) {
        yield return StartCoroutine(AnimateMotion(origin, destination, transitionInPreset[0]));
    }
}
