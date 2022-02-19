using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsManager : MonoBehaviour {
    public Image hackingPowerBar;
    public Text hackingPowerCounter;
    public Image hackingDataBar;
    public Text hackingTransactionUnitsCounter;
    public Text maxPowerCounter;
    public Text transactionUnitsCounter;
    public Text dataCounter;
    private GameManager gameManager;

    public void Awake() {
        gameManager = FindObjectOfType<GameManager>();
    }

    public void Start() {
        UpdateUI();
    }

    public GameStatus UpdateStats(HackingResult hackingResult) {
        Stats.HackingPower = Mathf.Clamp(Stats.HackingPower + hackingResult.power, 0, Stats.MaxPower);
        Stats.HackingCurrentData = Mathf.Clamp(Stats.HackingCurrentData + hackingResult.data, 0, Stats.HackingTargetData);
        Stats.HackingTransactionUnits += hackingResult.transactionUnits;
        UpdateUI();
        return GetGameStatus();
    }

    public void UpdateUI() {
        if(hackingPowerCounter != null)
            hackingPowerCounter.text = Stats.HackingPower + "/" + Stats.MaxPower;
        if(hackingPowerBar != null)
            hackingPowerBar.fillAmount =(float)Stats.HackingPower / Stats.MaxPower;
        if(hackingDataBar != null)
            hackingDataBar.fillAmount = (float) Stats.HackingCurrentData / Stats.HackingTargetData;
        if(hackingTransactionUnitsCounter != null)
            hackingTransactionUnitsCounter.text = Stats.HackingTransactionUnits.ToString();
        if (maxPowerCounter != null)
            maxPowerCounter.text = Stats.MaxPower.ToString();
        if (transactionUnitsCounter != null)
            transactionUnitsCounter.text = Stats.TransactionUnits.ToString();
        if (dataCounter != null)
            dataCounter.text = Stats.Data.ToString();
    }

    public int GetGainedTransactionUnits() {
        return Stats.HackingTransactionUnits;
    }

    public GameStatus GetGameStatus() {
        if (Stats.HackingPower <= 0)
            return GameStatus.gameover;
        else if (Stats.HackingCurrentData >= Stats.HackingTargetData)
            return GameStatus.won;
        else
            return GameStatus.playing;
    }
}
