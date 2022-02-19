using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NodeSpawner : MonoBehaviour, IPointerDownHandler {
    UITestSpawner manager;

    public void OnPointerDown(PointerEventData eventData) {
        manager.SpawnNodeTop();
    }

    // Start is called before the first frame update
    void Awake()
    {
        manager = FindObjectOfType<UITestSpawner>();
    }

    public void SpawnNodeTop() {
        manager.SpawnNodeTop();
    }
}
