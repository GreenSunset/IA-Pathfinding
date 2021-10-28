using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AStarSolver2 : MonoBehaviour
{
    // Soluciona el problema y controla las instancias de nodos explorados y la solución

    //Prefabs
    public GameObject ExploredPrefab;
    public GameObject SolutionPrefab;

    //Objetos
    public Transform ExploredContainer;
    public Transform SolutionContainer;
    private List<GameObject> Solution;
    private List<GameObject> ExploredWaypoints;

    //Variables de algoritmo
    private List<AStarNode> Candidates;
    private List<ExploredNode> Explored;
    private int ExpandIndex;
    private bool IsPossible;

    //Variables de opciones
    public float Speed = 4;
    public int Step;
    public float Threshold = 0.2f;

    //Datos
    public int NodesExplored = 0;
    public int NodesGenerated = 0;
    public float SimulationTime = 0;
    public float TotalTime = 0;

    //Inicio
    public void Solve()
    {
        Solution = new List<GameObject>();
        ExploredWaypoints = new List<GameObject>();
        Candidates = new List<AStarNode>();
        Explored = new List<ExploredNode>();
        ExpandIndex = 0;
        IsPossible = true;
        if (true) StartCoroutine(RealtimeSolution());
        else InmediateSolution();
    }

    //Métodos Auxiliares
    private bool IsLegal(Vector2Int coordinate)
    {
        if (coordinate.x >= WorldInfo.Size.x || coordinate.x < 0 || coordinate.y >= WorldInfo.Size.y || coordinate.y < 0)
            return false;
        return true;
    }

    //Solución
    private bool InmediateSolution()
    {
        Vector2Int Coordinates;
        ExploredNode Exploring;
        float StartMark = Time.time;

        Candidates.Add(new AStarNode(WorldInfo.Beginning, 0, WorldInfo.Heuristic.Function(WorldInfo.Beginning, WorldInfo.End)));
        Debug.Log("Starting from: {" + Candidates[ExpandIndex].Position.x + ", " + Candidates[ExpandIndex].Position.y + "}");
        Exploring = new ExploredNode(Candidates[0]);
        NodesGenerated++;

        while (Candidates[ExpandIndex].Position != WorldInfo.End)
        {
            Debug.Log("Exploring Neighbors...");
            for (int i = 0; i < 4; i++)
            {
                Coordinates = Exploring.Position + ConstCoordinates.BasicDirections[i];
                if (IsLegal(Coordinates) && !WorldInfo.Obstacles.Contains(Coordinates) && Explored.Find(x => x.Position == Coordinates) == null)
                {
                    Debug.Log("Neighbor " + (i % 2 == 0 ? i > 2 ? "W" : "E" : i > 2 ? "S" : "N") + " available");
                    UpdateNodeValue(Exploring, Coordinates);
                }
            }
            if (WorldInfo.DoDiagonals)
            {
                for (int i = 0; i < 4; i++)
                {
                    Coordinates = Exploring.Position + ConstCoordinates.Diagonals[i];
                    if (IsLegal(Coordinates) && !WorldInfo.Obstacles.Contains(Coordinates) && Explored.Find(x => x.Position == Coordinates) == null)
                    {
                        Debug.Log("Neighbor " + (i % 2 == 0 ? i > 2 ? "NW" : "NE" : i > 2 ? "SW" : "SE") + " available");
                        UpdateNodeValue(Exploring, Coordinates, true);
                    }
                }
            }

            //Actualizar listas
            Debug.Log("Node Explored");
            Candidates.Remove(Candidates[ExpandIndex]);
            Explored.Add(Exploring);
            NodesExplored++;
            if (Candidates.Count == 0)
            {
                Debug.Log("Last node available explored, there is no solution");
                IsPossible = false;
                break;
            }

            //Buscar nueva Candidata a Explorar
            ExpandIndex = 0; //Aleatorio?
            for (int i = 0; i < Candidates.Count; i++)
            {

                Debug.Log("Comparing {" + Candidates[i].Position.x + ", " + Candidates[i].Position.y + "} and {" + Candidates[ExpandIndex].Position.x + ", " + Candidates[ExpandIndex].Position.y + "}: " + Candidates[i].Value + "<" + Candidates[ExpandIndex].Value);
                if (Candidates[i].Value < Candidates[ExpandIndex].Value || (Candidates[i].Value == Candidates[ExpandIndex].Value && Candidates[i].HeuristicDistance < Candidates[ExpandIndex].HeuristicDistance))
                {
                    Debug.Log("Changing candidate");
                    ExpandIndex = i;
                }
            }
            Exploring = new ExploredNode(Candidates[ExpandIndex]);
            Debug.Log("New node selected: {" + Candidates[ExpandIndex].Position.x + ", " + Candidates[ExpandIndex].Position.y + "}");
            Debug.Log("Cost: " + Candidates[ExpandIndex].Cost + "\nHeuristic:" + Candidates[ExpandIndex].HeuristicDistance + "\nValue:" + Candidates[ExpandIndex].Value);

            //Pause
        }
        Debug.Log("Explored Last Node");
        Explored.Add(Exploring);
        SimulationTime = Time.time - StartMark;
        return IsPossible;
    }

    private IEnumerator RealtimeSolution()
    {
        Vector2Int Coordinates;
        ExploredNode Exploring;
        float StartMark = Time.deltaTime;

        Candidates.Add(new AStarNode(WorldInfo.Beginning, 0, WorldInfo.Heuristic.Function(WorldInfo.Beginning, WorldInfo.End)));
        Debug.Log("Starting from: {" + Candidates[ExpandIndex].Position.x + ", " + Candidates[ExpandIndex].Position.y + "}");
        Exploring = new ExploredNode(Candidates[0]);

        while (Candidates[ExpandIndex].Position != WorldInfo.End)
        {
            Debug.Log("Exploring Neighbors...");
            for (int i = 0; i < 4; i++)
            {
                Coordinates = Exploring.Position + ConstCoordinates.BasicDirections[i];
                if (IsLegal(Coordinates) && !WorldInfo.Obstacles.Contains(Coordinates) && Explored.Find(x => x.Position == Coordinates) == null)
                {
                    Debug.Log("Neighbor " + (i % 2 == 0 ? i > 2 ? "W" : "E" : i > 2 ? "S" : "N") + " available");
                    UpdateNodeValue(Exploring, Coordinates);
                }
            }
            if (WorldInfo.DoDiagonals)
            {
                for (int i = 0; i < 4; i++)
                {
                    Coordinates = Exploring.Position + ConstCoordinates.Diagonals[i];
                    if (IsLegal(Coordinates) && !WorldInfo.Obstacles.Contains(Coordinates) && Explored.Find(x => x.Position == Coordinates) == null)
                    {
                        Debug.Log("Neighbor " + (i % 2 == 0 ? i > 2 ? "NW" : "NE" : i > 2 ? "SW" : "SE") + " available");
                        UpdateNodeValue(Exploring, Coordinates, true);
                    }
                }
            }

            //Actualizar listas
            Debug.Log("Node Explored");
            Candidates.Remove(Candidates[ExpandIndex]);
            Explored.Add(Exploring);
            NodesExplored++;
            ExploredWaypoints.Add(Instantiate(ExploredPrefab, new Vector3(Explored[Explored.Count - 1].Position.x, .3f, Explored[Explored.Count - 1].Position.y), Quaternion.identity, ExploredContainer));
            if (Candidates.Count == 0)
            {
                Debug.Log("Last node available explored, there is no solution");
                IsPossible = false;
                break;
            }

            //Buscar nueva Candidata a Explorar
            ExpandIndex = 0; //Aleatorio?
            for (int i = 0; i < Candidates.Count; i++)
            {

                Debug.Log("Comparing {" + Candidates[i].Position.x + ", " + Candidates[i].Position.y + "} and {" + Candidates[ExpandIndex].Position.x + ", " + Candidates[ExpandIndex].Position.y + "}: " + Candidates[i].Value + "<" + Candidates[ExpandIndex].Value);
                if (Candidates[i].Value < Candidates[ExpandIndex].Value || (Candidates[i].Value == Candidates[ExpandIndex].Value && Candidates[i].HeuristicDistance < Candidates[ExpandIndex].HeuristicDistance))
                {
                    Debug.Log("Changing candidate");
                    ExpandIndex = i;
                }
            }
            Exploring = new ExploredNode(Candidates[ExpandIndex]);
            Debug.Log("New node selected: {" + Candidates[ExpandIndex].Position.x + ", " + Candidates[ExpandIndex].Position.y + "}");
            Debug.Log("Cost: " + Candidates[ExpandIndex].Cost + "\nHeuristic:" + Candidates[ExpandIndex].HeuristicDistance + "\nValue:" + Candidates[ExpandIndex].Value);

            //Pause
            SimulationTime += Time.deltaTime;
            if (Speed > 60 && Time.deltaTime > Threshold) yield return null;
            else if (Speed != 0 && Speed <= 60) yield return new WaitForSeconds(1 / Speed);
            else while (Speed == 0) yield return null;
        }
        Debug.Log("Explored Last Node");
        Explored.Add(Exploring);
        ExploredWaypoints.Add(Instantiate(ExploredPrefab, new Vector3(Explored[Explored.Count - 1].Position.x, .3f, Explored[Explored.Count - 1].Position.y), Quaternion.identity, ExploredContainer));
        SimulationTime += Time.deltaTime;
        yield return IsPossible;
    }
    private void UpdateNodeValue(ExploredNode Origin, Vector2Int To, bool diagonal = false)
    {
        AStarNode Destiny = Candidates.Find(x => x.Position == To);
        float NewCost = Origin.Cost + (diagonal ? WorldInfo.Heuristic.UnitaryDiagonal() : 1);
        if (Destiny == null)
        {
            Candidates.Add(new AStarNode(To, NewCost, WorldInfo.Heuristic.Function(To, WorldInfo.End), Origin));
            NodesGenerated++;
            Debug.Log("New node Added to candidates");
        }
        else if (Destiny.Cost > NewCost)
        {
            Destiny.Cost = NewCost;
            Destiny.Parent = Origin;
            Debug.Log("Candidate node Updated");
        }
    }
    /*
    // Trazas

    // Estado del Algoritmo
    private bool IsSolved = false;
    private bool IsPossible = true;

    // Conjuntos e índices del algoritmo
    public float Speed = 1;

    //Tengo que meter estos en algún lugar estático
    private Vector2Int[] BasicDirections = new Vector2Int[4]
        {new Vector2Int(0, -1), new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(-1, 0)};
    private Vector2Int[] Diagonals = new Vector2Int[4]
        {new Vector2Int(-1, -1), new Vector2Int(1, -1), new Vector2Int(-1, 1), new Vector2Int(1, 1)};

    public void Restart()
    {
        Solution = null;
        Candidates = new List<AStarNode>();
        for (int i = 0; Explored != null && i < Explored.Count; i++)
        {
            Destroy(Explored[i].Instance);
        }
        Explored = new List<ExploredNode>();
        ExpandIndex = 0;
        IsSolved = false;
        IsPossible = true;
    }

    

    

    public IEnumerator GenerateSolution()
    {
        if (IsPossible && IsSolved)
        {
            Solution = new List<GameObject>();
            ExploredNode aux = Explored[Explored.Count - 1];
            while (aux != null)
            {
                Solution.Add(Instantiate(SolutionPrefab, SolutionContainer));
                Solution[Solution.Count-1].transform.position = new Vector3(aux.Position.x, .3f, aux.Position.y);
                Solution[Solution.Count - 1].SetActive(false);
                aux = aux.Parent;
                //if (Time.deltaTime > ) yield return null; //Permite realentizar el reconocimiento de la solución para que no se ejecute en un solo frame
            }
            for (int i = Solution.Count - 1; i >= 0; i--)
            {
                Solution[i].SetActive(true);
                yield return new WaitForSeconds(.2f/Speed);
            }
        }
    }

     /**/
}
