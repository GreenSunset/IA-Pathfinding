using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstCoordinates
{
    static public Vector2Int Invalid = new Vector2Int(-1, -1);

    static public Vector2Int[] BasicDirections = new Vector2Int[4]
        {new Vector2Int(0, -1), new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(-1, 0)};
    static public Vector2Int[] Diagonals = new Vector2Int[4]
        {new Vector2Int(-1, -1), new Vector2Int(1, -1), new Vector2Int(-1, 1), new Vector2Int(1, 1)};
}
public class ExploredNode
{
    public Vector2Int Position;
    public float Cost;
    public ExploredNode Parent;

    public ExploredNode(AStarNode node)
    {
        Position = node.Position;
        Cost = node.Cost;
        Parent = node.Parent;
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
