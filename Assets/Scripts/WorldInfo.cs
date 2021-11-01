using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldInfo : MonoBehaviour
{
    public static Vector2Int Size = new Vector2Int(10,10);
    // Posicion del Comienzo y Final
    public static Vector2Int Beginning = ConstCoordinates.Invalid;
    public static Vector2Int End = ConstCoordinates.Invalid;
    // Posicion de los Obstaculos
    public static List<Vector2Int> Obstacles = new List<Vector2Int>();
    // Heuristica
    public static HeuristicFunc Heuristic = new EuclideanHeuristic();
    // Tipo de movimiento
    public static bool DoDiagonals = true;
    // Ajustes Manuales
    public static bool ManualObstacles = true;
    public static bool ManualObjectives = true;
    public static float CostWeight = 1; //Not implemented
    public static bool RealtimeSolution = true; //Not implemented
}
