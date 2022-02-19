using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChangePageController : MonoBehaviour, IPointerClickHandler {

    GameObject missionPanel;
    GameObject shopPanel;
    MoverUI missionPanelMover;
    MoverUI shopPanelMover;
    MoverUI buttonMover;
    Text buttonDescription;
    SoundFxManager soundFxManager;

    bool showingMissions = true;

    public void OnPointerClick(PointerEventData eventData) {
        showingMissions = !showingMissions;
        bool transitionStarted;
        if (showingMissions) {
            transitionStarted  = shopPanelMover.TransitionOut(shopPanel.transform.localPosition, new Vector2(480, 0));
            if (!transitionStarted) {
                showingMissions = !showingMissions;
                return;
            }
            soundFxManager.PlayFx(SoundType.selection3);
            buttonMover.MoveTo(transform.localPosition, transform.localPosition);
            buttonDescription.text = "Go to shop";
            missionPanelMover.TransitionIn(missionPanel.transform.localPosition, new Vector2(0, 0));
        }
        else {
            transitionStarted = missionPanelMover.TransitionOut(missionPanel.transform.localPosition, new Vector2(-480, 0));
            if (!transitionStarted) {
                showingMissions = !showingMissions;
                return;
            }
            soundFxManager.PlayFx(SoundType.selection3);
            buttonMover.MoveTo(transform.localPosition, transform.localPosition);
            buttonDescription.text = "Go to missions";
            shopPanelMover.TransitionIn(shopPanel.transform.localPosition, new Vector2(0, 0));
        }
    }

    void Awake() {
        soundFxManager = FindObjectOfType<SoundFxManager>();
        buttonMover = GetComponent<MoverUI>();
        missionPanel = transform.parent.Find("Missions").gameObject;
        shopPanel = transform.parent.Find("Shop").gameObject;
        missionPanelMover = missionPanel.GetComponent<MoverUI>();
        shopPanelMover = shopPanel.GetComponent<MoverUI>();
        buttonDescription = transform.GetChild(0).GetComponent<Text>();
    }
}
