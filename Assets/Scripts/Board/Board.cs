using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using UnityEngine;

public class Board {

    private List<List<Node>> board;

    public ReadOnlyCollection<Node> BoardNodes {
        get {
            return board.SelectMany(x => x).ToList().AsReadOnly();
        }
    }

    public int height { get; private set; }
    public int width { get; private set; }
    public BoardRules boardRules { get; private set; }
    private int hackingMinReserveNodes = 12;
    private int hackingReserveAddQty = 20;

    public Board(int height, int width, BoardRules boardRules) {
        this.height = height;
        this.width = width;
        this.boardRules = BoardRules.letters;
    }

    public void GenerateBoardFromFile(TextAsset boardFile) {
        List<List<Node>> nboard = new List<List<Node>>();
        var rows = boardFile.text.Split(new string[] { "\r\n" }, StringSplitOptions.None);
        for (int i = 0; i < width; i++)
            nboard.Add(new List<Node>());
        for (int i = 0; i < rows.Length; i++) {
            for (int j = 0; j < rows[i].Length; j++) {
                nboard[j].Add(new Node(LogicHelper.ConvertSymbolCharToEnum(rows[i][j].ToString()), j, i + (height - rows.Length)));
            }
        }
        board = nboard;
    }

    public void GenerateRandomHackingBoard(HackingBoardConfiguration config) {
        List<List<Node>> nboard = new List<List<Node>>();
        for (int i = 0; i < width; i++)
            nboard.Add(new List<Node>());
        for (int i = 0; i < nboard.Count; i++) {
            for (int j = 0; j < 20; j++) {
                nboard[i].Add(new Node(config.GetRandomHackingNode(), i, j + height - 20));
            }
        }
        board = nboard;
    }

    public Node GetNodeAtPosition(int x, int y) {
        try {
            return board[x][board[x].Count - height + y];
        }
        catch (Exception e) {
            Debug.Log("Error for x = " + x + ", y = " + y + " -> index = " + (board[x].Count - height + y) + "\n" + e.Message);
        }
        return null;
    }

    public bool SwapNodes(Node node1, Node node2) {
        if (node2 == null) {
            Debug.Log("Interaction canceled, 2nd node not selected");
            return false;
        }
        Debug.Log("Interaction between " + node1.GetSymbolName() + " - " + node2.GetSymbolName());
        if (!node1.IsAdjacentToNode(node2)) {
            Debug.Log("Nodes are not adjacent: " + node1.GetSymbolName() + " " + node1.postionX + " " + node1.postionY + "  - " + node2.GetSymbolName() + " " + node2.postionX + " " + node2.postionY);
            return false;
        }
        int x = node1.postionX, y = node1.postionY;
        node1.ChangePositionTo(node2.postionX, node2.postionY);
        node2.ChangePositionTo(x, y);
        LogicHelper.Swap(board[node1.postionX], board[node2.postionX], board[node2.postionX].IndexOf(node1), board[node1.postionX].IndexOf(node2));
        return true;
    }

    public bool RestoreNodes(Node node1, Node node2) {
        int x = node1.postionX, y = node1.postionY;
        node1.RestorePositionTo(node2.postionX, node2.postionY);
        node2.RestorePositionTo(x, y);
        LogicHelper.Swap(board[node1.postionX], board[node2.postionX], board[node2.postionX].IndexOf(node1), board[node1.postionX].IndexOf(node2));
        return true;
    }

    public HackingResult HackNode(Node node) {
        HackingResult result = new HackingResult();
        Node currentNode;
        for (int i = node.postionX - 1; i <= node.postionX + 1; i++) {
            for (int j = node.postionY - 1; j <= node.postionY + 1; j++) {
                if (i < 0 || i >= width || j < 0 || j >= height)
                    continue;
                currentNode = GetNodeAtPosition(i, j);
                currentNode.markedForDeletion = true;

                switch (currentNode.symbol) {
                    case Symbol.power:
                        result.power++;
                        break;
                    case Symbol.drain:
                        result.power--;
                        break;
                    case Symbol.data:
                        result.data++;
                        break;
                    case Symbol.transactionUnit:
                        result.transactionUnits++;
                        break;
                    case Symbol.power2:
                        result.power+=2;
                        break;
                    case Symbol.drain2:
                        result.power-=2;
                        break;
                    case Symbol.data2:
                        result.data+=2;
                        break;
                    case Symbol.transactionUnit2:
                        result.transactionUnits+=2;
                        break;
                }
            }
        }
        result.downShift = CalculateNodeRepositioning();
        return result;
    }

    public List<Node> RefillHackingNodesReserve(HackingBoardConfiguration config) {
        List<Node> addedNodes = new List<Node>();
        for(int i = 0; i < width; i++) {
            if(board[i].Count() < hackingMinReserveNodes) {
                for(int j = 0; j < hackingReserveAddQty; j++) {
                    Node n = new Node(config.GetRandomHackingNode(), i, height - board[i].Count - 1);
                    addedNodes.Add(n);
                    board[i].Insert(0, n);
                }
            }
        }
        return addedNodes;
    }

    public WordMatchResult CalculateInteraction(List<string> wordsToMatch) {
        WordMatchResult wordMatchResult = new WordMatchResult();
        int[] downShift = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        wordMatchResult.downShift = downShift;
        bool changes;
        do {
            var wordMatches = CheckWordMatches(wordsToMatch);
            wordMatchResult.wordsMatched.AddRange(wordMatches);
            changes = MarkNodesToDelete(wordMatches);
            downShift = CalculateNodeRepositioning();
            for (int i = 0; i < downShift.Length; i++)
                wordMatchResult.downShift[i] += downShift[i];
            DestroyMarkedNodes();
        } while (changes);
        return wordMatchResult;
    }

    public int[] CalculateNodeRepositioning() {
        int[] downShift = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < board[x].Count; y++) {
                int nDeletions = 0;
                for (int i = y; i < board[x].Count; i++) {
                    if (board[x][i].markedForDeletion)
                        nDeletions++;
                }
                if (nDeletions > 0 && !board[x][y].markedForDeletion) {
                    board[x][y].SetDownShift(nDeletions);
                    downShift[x] = nDeletions;
                }
            }
        }
        return downShift;
    }

    public void DestroyMarkedNodes() {
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < board[i].Count; j++) {
                try {
                    if (board[i][j].markedForDeletion) {
                        var n = board[i][j];
                        board[i][j].DestroyNode();
                        board[i].Remove(n);
                        j--;
                    }
                }
                catch (Exception ex) {
                    Debug.Log("Errore per i=" + i + ", j=" + j + ". " + ex.Message);
                }
            }
        }
    }

    public void DestroyAllBoard() {
        for(int i = 0; i < width; i++) {
            for(int j = 0; j < height; j++) {
                GetNodeAtPosition(i, j).DestroyNode();
            }
        }
    }

    public void RearrangeNodes() {
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < board[i].Count; j++) {
                try {
                    if (board[i][j].downShift != 0) {
                        board[i][j].ChangePositionTo(board[i][j].postionX, board[i][j].postionY + board[i][j].downShift);
                        board[i][j].SetDownShift(0);
                    }
                }
                catch (Exception ex) {
                    Debug.Log("Errore per i=" + i + ", j=" + j + ". " + ex.Message);
                }
            }
        }
    }

    public bool MarkNodesToDelete(List<WordMatch> wordMatches) {
        for (int k = 0; k < wordMatches.Count; k++) {
            //finds board elements with x and y between the ones of the current matched word
            board.SelectMany(n => n)
                  .Where(t =>
                  t.postionY <= wordMatches[k].endPosition.y &&
                  t.postionY >= wordMatches[k].startPosition.y &&
                  t.postionX <= wordMatches[k].endPosition.x &&
                  t.postionX >= wordMatches[k].startPosition.x).
                  Select(v => v).ToList().ForEach(x => x.markedForDeletion = true);
        }
        var r = board.SelectMany(n => n.Where(x => x.markedForDeletion)).ToList();
        return r.Count > 0;
    }

    public List<WordMatch> CheckWordMatches(List<string> words) {
        List<string> horizontal = new List<string>();
        List<string> vertical = new List<string>();
        List<WordMatch> wordMatches = new List<WordMatch>();
        int matchIndex = -1;

        for (int i = 0; i < height; i++)
            horizontal.Add(string.Concat(board.SelectMany(v => v.Where(x => x.postionY == i).Select(y => y.symbol.ConvertSymbolEnumToBoardString()))));

        for (int i = 0; i < width; i++)
            vertical.Add(string.Concat(board.SelectMany(v => v.Where(x => x.postionX == i && x.postionY >= 0).Select(y => (y.symbol.ConvertSymbolEnumToBoardString())))));

        foreach (string w in words) {
            for (int i = 0; i < horizontal.Count; i++) {
                matchIndex = horizontal[i].IndexOf(w);
                if (matchIndex != -1) {
                    wordMatches.Add(new WordMatch(new Vector2Int(matchIndex, i), new Vector2Int(matchIndex + w.Length - 1, i), w));
                    Debug.Log("Found word " + w + " at " + matchIndex + "-" + i);
                }
            }

            for (int i = 0; i < vertical.Count; i++) {
                matchIndex = vertical[i].IndexOf(w);
                if (matchIndex != -1) {
                    wordMatches.Add(new WordMatch(new Vector2Int(i, matchIndex), new Vector2Int(i, matchIndex + w.Length - 1), w));
                    Debug.Log("Found word " + w + " at " + i + "-" + matchIndex);
                }
            }
        }
        return wordMatches;
    }
}