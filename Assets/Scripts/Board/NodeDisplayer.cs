using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeDisplayer : MonoBehaviour {

    Node node;
    public NodeMover nodeMover;
    public Image borderImage;
    public Image symbolImage;
    Animator animator;
    

    public void SetNode(Node node) {
        this.node = node;
        node.AddEventToPositionChange(ChangePosition);
        node.AddEventToNodeRestore(RestorePosition);
        node.AddEventToNodeErase(EraseNode);
        if (node.postionY >= 0)
            nodeMover.SpawnNode(new Vector2(node.postionX, node.postionX % 2 == 0 ? node.postionY - 7 : node.postionY + 13), new Vector2(node.postionX, node.postionY));
        else
            nodeMover.SpawnNode(new Vector2(-4, -4), new Vector2(-4, -4));
        
        symbolImage.sprite =
            GameObject.FindGameObjectWithTag("SpriteCollector").
            GetComponent<SpriteCollector>().GetSprite("s" + node.GetSymbolName());
    }

    public Node GetNode() {
        return node;
    }

    private void ChangePosition(int x, int y) {
        //Debug.Log("Changing position of node " + node.GetSymbolName() + " at position " + node.postionX + "-" + node.postionY + " to position " + x + "-" + y);
        RestoreUI(y);
        nodeMover.MoveNode(new Vector2Int(node.postionX, node.postionY), new Vector2Int(x, y));
    }

    private void RestorePosition(int x, int y) {
        //Debug.Log("Restoring position of node " + node.GetSymbolName() + " at position " + node.postionX + "-" + node.postionY + " to position " + x + "-" + y);
        RestoreUI(y);
        nodeMover.RestoreNode(new Vector2Int(node.postionX, node.postionY), new Vector2Int(x, y));
    }

    void RestoreUI(int y) {
        if (y < 0) {
            borderImage.enabled = false;
            symbolImage.enabled = false;
        }
        else {
            borderImage.enabled = true;
            symbolImage.enabled = true;
        }
    }

    private void EraseNode() {
        StartCoroutine(EraseRoutine());
    }

    private IEnumerator EraseRoutine() {
        yield return StartCoroutine(nodeMover.AnimateDestruction(new Vector2(node.postionX, node.postionY), new Vector2(node.postionX, node.postionY)));
        Destroy(transform.parent.gameObject);
    }
}
