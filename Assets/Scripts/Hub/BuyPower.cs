using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BuyPower : MonoBehaviour, IPointerDownHandler {

    public Text dataCostText;
    public Text transactionUnitsCostText;
    int dataCost;
    int transactionUnitsCost;
    StatsManager statsManager;
    MenuElementMover buttonMover;
    SoundFxManager soundFxManager;

    public void OnPointerDown(PointerEventData eventData) {
        if(Stats.Data >= dataCost && Stats.TransactionUnits >= transactionUnitsCost) {
            buttonMover.TransitionIn(transform.localPosition, transform.localPosition + Vector3.right * 50);
            Stats.Data -= dataCost;
            Stats.TransactionUnits -= transactionUnitsCost;
            Stats.MaxPower++;
            soundFxManager.PlayFx(SoundType.buy1);
            statsManager.UpdateUI();
            UpdateCosts();
        }
        else {
            buttonMover.TransitionOut(transform.localPosition, transform.localPosition);
            soundFxManager.PlayFx(SoundType.selectionFailed1);
        }
    }

    void Start() {
        soundFxManager = FindObjectOfType<SoundFxManager>();
        buttonMover = GetComponent<MenuElementMover>();
        statsManager = FindObjectOfType<StatsManager>();
        UpdateCosts();
    }

    void UpdateCosts() {
        if(Stats.MaxPower < 6) {
            dataCost = 0;
            transactionUnitsCost = 5;
        }
        else {
            dataCost = 0;
            transactionUnitsCost = 30;
        }
        if(dataCostText != null)
            dataCostText.text = dataCost.ToString();
        transactionUnitsCostText.text = transactionUnitsCost.ToString();
    }

}
