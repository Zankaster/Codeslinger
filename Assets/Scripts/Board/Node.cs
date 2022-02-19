using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node {

    public Symbol symbol { get; private set; }
    public int postionX { get; private set; }
    public int postionY { get; private set; }
    public int downShift { get; private set; }
    public bool markedForDeletion { get; set; }
    public bool markedForHacking { get; set; }

    public delegate void ChangePosition(int x, int y);
    public event ChangePosition OnPositionChanged;
    public delegate void RestorePosition(int x, int y);
    public event RestorePosition OnPositionRestored;
    public delegate void EraseNode();
    public event EraseNode OnNodeErase;
    

    public Node(Symbol symbol, int x, int y) {
        this.symbol = symbol;
        this.postionX = x;
        this.postionY = y;
        markedForDeletion = false;
        markedForHacking = false;
    }

    public string GetSymbolName() {
        return symbol.ToString().ToLower();
    }

    public void SetDownShift(int y) {
        downShift = y;
    }

    public void ChangePositionTo(int x, int y) {
        OnPositionChanged(x, y);
        this.postionX = x;
        this.postionY = y;
    }

    public void RestorePositionTo(int x, int y) {
        OnPositionRestored(x, y);
        this.postionX = x;
        this.postionY = y;
    }

    public void CheckIfMarkedForDeletion() {
        if (markedForDeletion) {
            OnNodeErase();
        }
    }

    public void DestroyNode() {
        OnNodeErase();
    }

    public bool IsAdjacentToNode(Node n2) {
        return (postionX == n2.postionX && Mathf.Abs(postionY - n2.postionY) == 1) ||
            (postionY == n2.postionY && Mathf.Abs(postionX - n2.postionX) == 1);
    }

    public void AddEventToPositionChange(ChangePosition func) {
        OnPositionChanged += func;
    }

    public void AddEventToNodeErase(EraseNode func) {
        OnNodeErase += func;
    }

    public void AddEventToNodeRestore(RestorePosition func) {
        OnPositionRestored += func;
    }
}