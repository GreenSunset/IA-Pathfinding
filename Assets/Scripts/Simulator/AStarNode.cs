using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExploredNode
{
    public Vector2Int Position;
    public float Cost;
    public ExploredNode Parent;
    public GameObject Instance;

    public ExploredNode(AStarNode node)
    {
        Position = node.Position;
        Cost = node.Cost;
        Parent = node.Parent;
    }

    public void ToggleHide()
    {
        Instance.SetActive(!Instance.activeSelf);
    }
}

public class AStarNode
{
    public Vector2Int Position;
    public ExploredNode Parent;
    public float HeuristicDistance;
    public float Cost;
    public float Value => Cost * WorldInfo.CostWeight + HeuristicDistance;

    public AStarNode(Vector2Int position, float cost = 0, float heuristic = 0, ExploredNode parent = null)
    {
        Position = position;
        Cost = cost;
        HeuristicDistance = heuristic;
        Parent = parent;
    }
}
