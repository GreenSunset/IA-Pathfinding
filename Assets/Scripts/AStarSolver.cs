using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AStarSolver : MonoBehaviour
{
    // Trazas
    public GameObject ExploredPrefab;
    public GameObject SolutionPrefab;
    public Transform ExploredContainer;
    public Transform SolutionContainer;

    // Estado del Algoritmo
    private bool IsSolved = false;
    private bool IsPossible = true;

    // Conjuntos e índices del algoritmo
    private List<AStarNode> Candidates;
    private List<ExploredNode> Explored;
    private List<GameObject> Solution;
    private int ExpandIndex = 0;
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

    private void UpdateNodeValue(ExploredNode Origin, Vector2Int To, bool diagonal = false)
    {
        AStarNode Destiny = Candidates.Find(x => x.Position == To);
        float NewCost = Origin.Cost + (diagonal ? WorldInfo.Heuristic.UnitaryDiagonal() : 1);
        if (Destiny == null)
        {
            Candidates.Add(new AStarNode(To, NewCost, WorldInfo.Heuristic.Function(To, WorldInfo.End), Origin));
            Debug.Log("New node Added to candidates");
        }
        else if (Destiny.Cost > NewCost)
        {
            Destiny.Cost = NewCost;
            Destiny.Parent = Origin;
            Debug.Log("Candidate node Updated");
        }
    }

    public IEnumerator SolveCoroutine()
    {
        if (IsSolved) yield return IsPossible;
        Debug.Log("Started Solving");
        //Establecer Listas de Candidatos y Explorados
        Vector2Int Auxiliar;
        //Meter Inicio en Candidatos
        Candidates.Add(new AStarNode(WorldInfo.Beginning, 0, WorldInfo.Heuristic.Function(WorldInfo.Beginning, WorldInfo.End)));
        Debug.Log("Starting from: {" + Candidates[ExpandIndex].Position.x + ", " + Candidates[ExpandIndex].Position.y + "}");
        ExploredNode Exploring = new ExploredNode(Candidates[ExpandIndex]);
        while (Candidates[ExpandIndex].Position != WorldInfo.End)
        {
            //Comprobar Vecinos
            Debug.Log("Exploring Neighbors...");
            for (int i = 0; i < 4; i++)
            {
                Auxiliar = Exploring.Position + BasicDirections[i];
                if (IsLegal(Auxiliar) && !WorldInfo.Obstacles.Contains(Auxiliar) && Explored.Find(x => x.Position == Auxiliar) == null)
                {
                    Debug.Log("Neighbor " + (i % 2 == 0 ? i > 2 ? "W" : "E" : i > 2 ? "S" : "N") + " available");
                    UpdateNodeValue(Exploring, Auxiliar);
                }
            }
            if (WorldInfo.DoDiagonals)
            {
                for (int i = 0; i < 4; i++)
                {
                    Auxiliar = Exploring.Position + Diagonals[i];
                    if (IsLegal(Auxiliar) && !WorldInfo.Obstacles.Contains(Auxiliar) && Explored.Find(x => x.Position == Auxiliar) == null)
                    {
                        Debug.Log("Neighbor " + (i % 2 == 0 ? i > 2 ? "NW" : "NE" : i > 2 ? "SW" : "SE") + " available");
                        UpdateNodeValue(Exploring, Auxiliar, true);
                    }
                }
            }
            //Actualizar listas
            Debug.Log("Node Explored");
            Candidates.Remove(Candidates[ExpandIndex]);
            //Si no hay candidatas, No es posible
            if (Candidates.Count == 0)
            {
                Debug.Log("Last node available explored, there is no solution");
                IsPossible = false;
                break;
            }
            Explored.Add(Exploring);
            Explored[Explored.Count - 1].Instance = Instantiate(ExploredPrefab, ExploredContainer);
            Explored[Explored.Count - 1].Instance.transform.position = new Vector3(Explored[Explored.Count - 1].Position.x, .3f, Explored[Explored.Count - 1].Position.y);
            //Buscar nueva Candidata a Explorar
            ExpandIndex = 0;

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
            yield return new WaitForSeconds(1/Speed);
        }
        Debug.Log("Explored Last Node");
        Explored.Add(Exploring);
        Explored[Explored.Count - 1].Instance = Instantiate(ExploredPrefab, ExploredContainer);
        Explored[Explored.Count - 1].Instance.transform.position = new Vector3(Explored[Explored.Count - 1].Position.x, .3f, Explored[Explored.Count - 1].Position.y);
        IsSolved = true;
        StartCoroutine(GenerateSolution());
        yield return IsPossible;
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

    private bool IsLegal(Vector2Int coordinate)
    {
        if (coordinate.x >= WorldInfo.Size.x || coordinate.x < 0 || coordinate.y >= WorldInfo.Size.y || coordinate.y < 0)
            return false;
        return true;
    } /**/
}
