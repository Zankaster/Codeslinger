using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class MissionSelectionController : MonoBehaviour, IPointerClickHandler {

    Image selectionImage;
    bool selected = false;
    float fillTime = 1f;
    private List<MissionSelectionController> selectionControllers;
    private MenuElementMover missionDetailFrame;
    HubController hubController;
    public bool selectOnStart;
    public string levelName;
    public ScenarioGameMode gameMode;
    SoundFxManager soundFxManager;
    bool selectionSuccess = false;

    public void Awake() {
        soundFxManager = FindObjectOfType<SoundFxManager>();
        hubController = FindObjectOfType<HubController>();
        selectionImage = transform.GetChild(0).GetComponent<Image>();
        selectionControllers = FindObjectsOfType<MissionSelectionController>().ToList();
        missionDetailFrame = transform.GetComponentInChildren<MenuElementMover>();
    }

    public void Start() {
        if (selectOnStart) {
            SwitchSelected();
            hubController.SelectLevel(levelName);
            hubController.SelectGameMode(gameMode);
        }
        else
            HideFrame();
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (selected)
            return;

        foreach (MissionSelectionController mc in selectionControllers) {
            selectionSuccess = mc.AnotherSelected();
            if (!selectionSuccess)
                return;
        }
        SwitchSelected();
        soundFxManager.PlayFx(SoundType.selection2);
        hubController.SelectLevel(levelName);
        hubController.SelectGameMode(gameMode);
    }

    private bool AnotherSelected() {
        if (selected)
            return SwitchSelected();
        return true;
    }

    private void HideFrame() {
        missionDetailFrame.MoveTo(missionDetailFrame.gameObject.transform.localPosition, missionDetailFrame.gameObject.transform.localPosition);
    }

    private bool SwitchSelected() {
        bool transitionStarted;
        selected = !selected;

        if (selected) 
            transitionStarted = missionDetailFrame.TransitionIn(missionDetailFrame.gameObject.transform.localPosition, missionDetailFrame.gameObject.transform.localPosition);
        else 
            transitionStarted = missionDetailFrame.TransitionOut(missionDetailFrame.gameObject.transform.localPosition, missionDetailFrame.gameObject.transform.localPosition);

        if (!transitionStarted) {
            selected = !selected;
            return false;
        }
        StartCoroutine(FillSelectionImage());
        return true;
    }

    IEnumerator FillSelectionImage() {
        float total = 0f;
        float percent = 0;
        while (total <= fillTime) {
            total = total + Time.deltaTime;
            percent = Mathf.Clamp01(total / fillTime);
            if (!selected)
                percent = 1 - percent;
            selectionImage.fillAmount = percent;
            yield return null;
        }
    }
}
