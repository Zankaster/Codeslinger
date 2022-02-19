using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeMover : MoverUI {
    
    public override void Awake() {
        base.Awake();
    }

    public void SpawnNode(Vector2 origin, Vector2 destination) {
        StartCoroutine(SpawnNodeRoutine(origin, destination));
    }

    public void MoveNode(Vector2 origin, Vector2 destination) {
        StartCoroutine(MoveNodeRoutine(origin, destination));
    }

    public void RestoreNode(Vector2 origin, Vector2 destination) {
        StartCoroutine(RestoreNodeRoutine(origin, destination));
    }

    private IEnumerator SpawnNodeRoutine(Vector2 origin, Vector2 destination) {
        yield return StartCoroutine(AnimateMotion(origin, destination, transitionInPreset[0]));
        boardManager.SpawnFinished();
    }

    private IEnumerator MoveNodeRoutine(Vector2 origin, Vector2 destination) {
        yield return StartCoroutine(AnimateMotion(origin, destination, motionPreset[0]));
        boardManager.MotionFinished();
    }

    private IEnumerator RestoreNodeRoutine(Vector2 origin, Vector2 destination) {
        yield return StartCoroutine(AnimateMotion(origin, destination, motionPreset[0]));
        boardManager.RestoreFinished();
    }

    public IEnumerator AnimateDestruction(Vector2 origin, Vector2 destination) {
        yield return StartCoroutine(AnimateMotion(origin, destination, transitionOutPreset[0]));
        OnDestroyFinished();
    }

    protected override IEnumerator AnimateMotion(Vector2 origin, Vector2 destination, MotionPreset motionPreset) {
        yield return StartCoroutine(AnimationUI(origin, destination, motionPreset));
    }

    protected override void OnAnimationFinished() {
        boardManager.BoardAnimationFinished();
    }

    protected override void OnDestroyFinished() {
        boardManager.BoardDestroyFinished();
    }
}