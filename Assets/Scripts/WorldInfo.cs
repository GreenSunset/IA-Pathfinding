using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldInfo : MonoBehaviour
{
    // Tamaño del mapa
    public static Vector2Int Size = new Vector2Int(1,1);
    // Posicion del Comienzo y Final
    public static Vector2Int Beginning = new Vector2Int(-1, -1);
    public static Vector2Int End = new Vector2Int(-1, -1);
    // Posicion de los Obstaculos
    public static List<Vector2Int> Obstacles = new List<Vector2Int>();
    // Heuristica
    public static HeuristicFunc Heuristic = new ManhatanHeuristic();
    // Tipo de movimiento
    public static bool DoDiagonals = false;
    // Ajustes Manuales
    public static bool ManualObstacles = true;
    public static bool ManualObjectives = true;
    public static float CostWeight = 1; //Not implemented
}
