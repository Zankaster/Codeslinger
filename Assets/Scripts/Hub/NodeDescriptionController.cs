using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NodeDescriptionController : MonoBehaviour, IPointerClickHandler {

    public Text descriptionText;
    public string nodeType;

    Image selectionImage;
    bool selected = false;
    float fillTime = 0.25f;
    private List<NodeDescriptionController> selectionControllers;
    MoverUI buttonMover;
    SoundFxManager soundFxManager;

    public void Awake() {
        buttonMover = GetComponent<MoverUI>();
        selectionImage = transform.GetChild(0).GetComponent<Image>();
        selectionControllers = FindObjectsOfType<NodeDescriptionController>().ToList();
        soundFxManager = FindObjectOfType<SoundFxManager>();
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (selected)
            return;

        foreach (NodeDescriptionController nc in selectionControllers)
            nc.AnotherSelected();
        SwitchSelected();
        soundFxManager.PlayFx(SoundType.selection1);
        buttonMover.MoveTo(transform.localPosition, transform.localPosition);
    }

    private void AnotherSelected() {
        if (selected)
            SwitchSelected();
    }

    private void SwitchSelected() {
        selected = !selected;
        StartCoroutine(FillSelectionImage());
        ShowNodeDescription();
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

    public void ShowNodeDescription() {
        if (selected)
            descriptionText.text = LogicHelper.nodesEffectsDescription[nodeType];
        else
            descriptionText.text = "";
    }
}