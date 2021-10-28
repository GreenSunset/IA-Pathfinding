using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldInfo : MonoBehaviour
{
    // Tamaño del mapa
    public static Vector2Int Size = new Vector2Int(10,10);
    // Posicion del Comienzo y Final
    public static Vector2Int Beginning = new Vector2Int(-1, -1);
    public static Vector2Int End = new Vector2Int(-1, -1);
    public static float Distance = 0f;
    // Posicion de los Obstaculos
    public static List<Vector2Int> Obstacles = new List<Vector2Int>();
    public static float numberObstacles = 0f;
    // Heuristica
    public static HeuristicFunc Heuristic = new EuclideanHeuristic();
    // Tipo de movimiento
    public static bool DoDiagonals = true;
    // Ajustes Manuales
    public static bool ManualObstacles = true;
    public static bool ManualObjectives = true;
    public static float CostWeight = 1; //Not implemented
}
