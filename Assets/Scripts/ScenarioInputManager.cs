using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScenarioInputManager : MonoBehaviour, IPointerDownHandler {

    GameManager boardManager;

    private void Awake() {
        boardManager = FindObjectOfType<GameManager>();
    }

    public void OnPointerDown(PointerEventData eventData) {
        boardManager.OnScreenPressed();
    }

}
