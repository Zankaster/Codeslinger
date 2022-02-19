using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoverUI : MonoBehaviour
{
    public bool motionInWidthUnits = true;
    public List<AnimationCurve> lerpCurves;
    Vector2Int position;
    int objectWidth;
    GameObject UIgo;
    public List<MotionPreset> motionPreset;
    public List<MotionPreset> transitionInPreset;
    public List<MotionPreset> transitionOutPreset;
    int yPositionSign = -1;
    Coroutine motionCoroutine;
    Coroutine transitionInCoroutine;
    Coroutine transitionOutCoroutine;
    bool transitioning = false;
    protected GameManager boardManager;

    public virtual void Awake() {
        boardManager = FindObjectOfType<GameManager>();
        objectWidth = motionInWidthUnits? (int)GetComponent<RectTransform>().rect.width : 1;
        yPositionSign = motionInWidthUnits ? -1 : 1;
        UIgo = transform.gameObject;
    }

    public virtual void MoveTo(Vector2 origin, Vector2 destination) {
        StartCoroutine(AnimateMotion(origin, destination, motionPreset[0]));
    }

    public IEnumerator TransitionInAndOut(Vector2 origin, Vector2 destination) {
        yield return StartCoroutine(AnimateMotion(origin, destination, transitionInPreset[0]));
        yield return StartCoroutine(AnimateMotion(origin, destination, transitionOutPreset[0]));
    }

    public void SubNodeAnimation(Vector2 origin, Vector2 destination) {
        StartCoroutine(AnimateMotion(origin, destination, transitionInPreset[1]));
    }

    public virtual bool TransitionIn(Vector2 origin, Vector2 destination) {
        if (transitioning)
            return false;
        if(transitionInCoroutine != null)
            StopCoroutine(transitionInCoroutine);
        transitionInCoroutine = StartCoroutine(AnimateMotion(origin, destination, transitionInPreset[0]));
        return true;
    }

    public virtual bool TransitionOut(Vector2 origin, Vector2 destination) {
        if (transitioning)
            return false;
        if (transitionOutCoroutine != null)
            StopCoroutine(transitionOutCoroutine);
        transitionOutCoroutine = StartCoroutine(AnimateMotion(origin, destination, transitionOutPreset[0]));
        return true;
    }

    protected virtual IEnumerator AnimateMotion(Vector2 origin, Vector2 destination, MotionPreset motionPreset) {
        yield return StartCoroutine(AnimationUI(origin, destination, motionPreset));
    }

    protected IEnumerator AnimationUI(Vector2 origin, Vector2 destination, MotionPreset motionPreset) {
        float journey = 0f;
        float percent = 0;
        float motionPercent = 0;
        float rotationPercent = 0;
        float scalePercent = 0;
        transitioning = true;
        while (journey <= motionPreset.duration) {
            journey = journey + Time.deltaTime;
            percent = Mathf.Clamp01(journey / motionPreset.duration);
            motionPercent = lerpCurves[(int)motionPreset.motionCurveIndex].Evaluate(percent);
            rotationPercent = lerpCurves[(int)motionPreset.rotationCurveIndex].Evaluate(percent);
            scalePercent = lerpCurves[(int)motionPreset.scaleCurveIndex].Evaluate(percent);
            UIgo.transform.localPosition = Vector2.LerpUnclamped(new Vector2(origin.x, yPositionSign * origin.y) * objectWidth, new Vector2(destination.x, yPositionSign * destination.y) * objectWidth, motionPercent);
            UIgo.transform.localEulerAngles = new Vector3(rotationPercent * 360 * (motionPreset.rotationX ? 1 : 0), rotationPercent * 360 * (motionPreset.rotationY ? 1 : 0), rotationPercent * 360 * (motionPreset.rotationZ ? 1 : 0));
            UIgo.transform.localScale = new Vector3(motionPreset.scaleX ? scalePercent : 1, motionPreset.scaleY ? scalePercent : 1, 1);
            yield return null;
        }
        transitioning = false;
    }

    protected virtual void OnAnimationFinished() {

    }

    protected virtual void OnDestroyFinished() {

    }
}
