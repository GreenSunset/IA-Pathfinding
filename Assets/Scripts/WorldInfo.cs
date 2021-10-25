using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldInfo : MonoBehaviour
{
    public static Vector2Int Size = new Vector2Int(1,1);
    public static Vector2Int Beginning = new Vector2Int(-1, -1);
    public static Vector2Int End = new Vector2Int(-1, -1);
    public static HeuristicFunc Heuristic = new ManhatanHeuristic();
    public static bool DoDiagonals = false;
    public static List<Vector2Int> Obstacles = new List<Vector2Int>();
    public static bool ManualObstacles = true;
    public static bool ManualObjectives = true;
    public static float CostWeight = 1; //Not implemented
}
