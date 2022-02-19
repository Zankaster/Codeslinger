using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameInstructionsTooltip : MonoBehaviour, IPointerClickHandler {
    bool toolTipVisible = false;
    GameObject toolTipDescription;
    MoverUI toolTipMover;
    MoverUI buttonMover;

    public void Awake() {
        buttonMover = GetComponent<MoverUI>();
        toolTipDescription = transform.GetChild(0).gameObject;
        toolTipMover = toolTipDescription.GetComponent<MoverUI>();
    }

    public void OnPointerClick(PointerEventData eventData) {
        toolTipVisible = !toolTipVisible;
        bool transitionStarted;
        buttonMover.MoveTo(transform.localPosition, transform.localPosition);
        if (toolTipVisible) {
            transitionStarted = toolTipMover.TransitionIn(toolTipDescription.transform.localPosition, toolTipDescription.transform.localPosition);
        }
        else {
            transitionStarted = toolTipMover.TransitionOut(toolTipDescription.transform.localPosition, toolTipDescription.transform.localPosition);
        }
        if (transitionStarted == false)
            toolTipVisible = !toolTipVisible;
    }

}
