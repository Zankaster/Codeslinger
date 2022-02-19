using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NodeInputManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler  {
    GameManager boardManager;
    public NodeDisplayer nodeDisplayer;
    public NodeMover symbolMover;
    private void Awake() {
        boardManager = FindObjectOfType<GameManager>();
        symbolMover = transform.GetChild(0).GetComponent<NodeMover>();
    }

    public void OnPointerDown(PointerEventData eventData) {
        if (boardManager.gamePhase == GamePhase.waitingInput && boardManager.gameMode == ScenarioGameMode.words)
            symbolMover.TransitionIn(symbolMover.transform.localPosition, symbolMover.transform.localPosition);
        boardManager.NodeClick(nodeDisplayer.GetNode());
    }

    public void OnPointerUp(PointerEventData eventData) {
        boardManager.NodeRelease();
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if(boardManager.gamePhase == GamePhase.waitingInput && boardManager.gameMode == ScenarioGameMode.words)
            symbolMover.SubNodeAnimation(symbolMover.transform.localPosition, symbolMover.transform.localPosition);
        boardManager.NodeHover(nodeDisplayer.GetNode());
    }
}
