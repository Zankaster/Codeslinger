using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITestSpawner : MonoBehaviour {

    
    public GameObject nodePrefab;
    public Transform nodesConsole;
    public int nodeLength = 28;
    public List<AnimationCurve> lerpCurves;
    List<GameObject> gameObjects;
    public Toggle rotationX;
    public Toggle rotationY;
    public Toggle rotationZ;
    public Toggle scaleX;
    public Toggle scaleY;
    public Slider duration;
    public Slider motionCurveIndex;
    public Slider rotationCurveIndex;
    public Slider scaleCurveIndex;

    public void Start() {
        gameObjects = new List<GameObject>();
    }

    public void SpawnNodeTop() {
        DestroyAll();
        Node node = new Node(Symbol.a, 0, 7);
        var nodeGameObject = Instantiate(nodePrefab, nodesConsole, false);
        gameObjects.Add(nodeGameObject);
        nodeGameObject.transform.localPosition = new Vector3(node.postionX * nodeLength, -(node.postionY) * nodeLength);
        MoveToWithRotation(nodeGameObject.transform, new Vector2(0, 0), new Vector2(node.postionX, node.postionY));
    }

    public void SpawnGrid() {
        DestroyAll();
        for (int i = 0; i < 11; i++) {
            for (int j = 0; j < 8; j++) {
                SpawnNode(new Vector2(0, 0), new Vector2(i, j));
            }
        }
    }
    
    public void DestroyAll() {
        StopAllCoroutines();
        for (int i = 0; i < gameObjects.Count; i++)
            Destroy(gameObjects[i]);
    }

    public void SpawnGridVerse() {
        DestroyAll();
        for (int i = 0; i < 11; i++) {
            for (int j = 0; j < 8; j++) {
                SpawnNode(new Vector2(i, i%2 == 0 ? j - 7 : j+13), new Vector2(i, j));
            }
        }
    }

    public void SpawnGridTop() {
        DestroyAll();
        for (int i = 0; i < 11; i++) {
            for (int j = 0; j < 8; j++) {
                SpawnNode(new Vector2(i,j-20), new Vector2(i, j));
            }
        }
    }

    public void SpawnGridInPosition() {
        DestroyAll();
        for (int i = 0; i < 11; i++) {
            for (int j = 0; j < 8; j++) {
                SpawnNode(new Vector2(i, j), new Vector2(i, j));
            }
        }
    }

    public void SpawnNode(Vector2 start, Vector2 end) {
        Node node = new Node(Symbol.f, (int)end.x, (int)end.y);
        var nodeGameObject = Instantiate(nodePrefab, nodesConsole, false);
        gameObjects.Add(nodeGameObject);
        nodeGameObject.transform.localPosition = new Vector3(node.postionX * nodeLength, -(node.postionY) * nodeLength);
        MoveToWithRotation(nodeGameObject.transform, new Vector2(start.x, start.y), new Vector2(node.postionX, node.postionY));
    }

    public void MoveToWithRotation(Transform gameObject, Vector2 origin, Vector2 destination) {
        StartCoroutine(AnimateMotion2(gameObject, origin, destination));
    }

    IEnumerator AnimateMotion2(Transform gameObject, Vector2 origin, Vector2 destination) {
        float journey = 0f;
        float percent = 0;
        float motionPercent = 0;
        float rotationPercent = 0;
        float scalePercent = 0;
        while (journey <= duration.value) {
            journey = journey + Time.deltaTime;
            percent = Mathf.Clamp01(journey / duration.value);
            motionPercent = lerpCurves[(int)motionCurveIndex.value].Evaluate(percent);
            rotationPercent = lerpCurves[(int)rotationCurveIndex.value].Evaluate(percent);
            scalePercent = lerpCurves[(int)scaleCurveIndex.value].Evaluate(percent);
            gameObject.transform.localPosition = Vector2.LerpUnclamped(new Vector2(origin.x, -origin.y) * nodeLength, new Vector2(destination.x, -destination.y) * nodeLength, motionPercent);
            gameObject.transform.eulerAngles = new Vector3(rotationPercent * 360 * (rotationX.isOn ? 1: 0), rotationPercent * 360 * (rotationY.isOn ? 1 : 0), rotationPercent * 360 * (rotationZ.isOn ? 1 : 0));
            gameObject.transform.localScale = new Vector3(scaleX.isOn ? scalePercent : 1, scaleY.isOn ? scalePercent : 1, 1);
            yield return null;
        }
    }
}
