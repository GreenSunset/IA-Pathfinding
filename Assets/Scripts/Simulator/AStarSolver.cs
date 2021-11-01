using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;


public class AStarSolver : MonoBehaviour
{
    // Soluciona el problema y controla las instancias de nodos explorados y la solución

    [Header("Prefabs")]
    public GameObject ExploredPrefab;
    public GameObject SolutionPrefab;

    [Space(5)]
    [Header("Containers")]
    public Transform ExploredContainer;
    public Transform SolutionContainer;
    private List<GameObject> Solution;
    private List<GameObject> ExploredWaypoints;

    //Variables de algoritmo
    private List<AStarNode> Candidates;
    private List<ExploredNode> Explored;
    private int ExpandIndex;
    [HideInInspector]
    public bool IsSolved;

    //Variables de opciones
    [HideInInspector]
    public float Speed = 5;
    [HideInInspector]
    public int Step;
    private float Threshold = 0.2f;

    //Datos
    private Stopwatch StopWatch;
    public float Cost { get; private set; }
    public bool IsPossible { get; private set; }
    public int NodesExplored { get; private set; }
    public int NodesGenerated { get; private set; }
    public double SimulationTime { get; private set; }
    public float TotalTime { get; private set; }
    private float TimeMark;

    //Inicio y reinicio
    public void Solve()
    {
        StopWatch = new Stopwatch();
        Solution = new List<GameObject>();
        ExploredWaypoints = new List<GameObject>();
        Candidates = new List<AStarNode>();
        Explored = new List<ExploredNode>();
        ExpandIndex = 0;
        IsPossible = true;
        IsSolved = false;

        NodesExplored = 0;
        NodesGenerated = 0;

        SimulationTime = 0;
        TotalTime = 0;
        TimeMark = Time.time;

        if (WorldInfo.RealtimeSolution) StartCoroutine(RealtimeSolution());
        else InmediateSolution();
    }
    public void Restart()
    {
        StopWatch.Reset();
        for (int i = 0; i < Solution.Count; i++) Destroy(Solution[i]);
        for (int i = 0; i < ExploredWaypoints.Count; i++) Destroy(ExploredWaypoints[i]);
        ExpandIndex = 0;
        IsPossible = true;
        IsSolved = false;
        NodesExplored = 0;
        NodesGenerated = 0;
        SimulationTime = 0;
        TotalTime = 0;
        Speed = 5;
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

        StopWatch.Start();
        Candidates.Add(new AStarNode(WorldInfo.Beginning, 0, WorldInfo.Heuristic.Function(WorldInfo.Beginning, WorldInfo.End)));
        Exploring = new ExploredNode(Candidates[0]);
        NodesGenerated++;

        while (Candidates[ExpandIndex].Position != WorldInfo.End)
        {
            for (int i = 0; i < 4; i++)
            {
                Coordinates = Exploring.Position + ConstCoordinates.BasicDirections[i];
                if (IsLegal(Coordinates) && !WorldInfo.Obstacles.Contains(Coordinates) && Explored.Find(x => x.Position == Coordinates) == null)
                {
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
                        UpdateNodeValue(Exploring, Coordinates, true);
                    }
                }
            }

            //Actualizar listas
            Candidates.Remove(Candidates[ExpandIndex]);
            Explored.Add(Exploring);
            NodesExplored++;
            if (Candidates.Count == 0)
            {
                IsPossible = false;
                break;
            }

            //Buscar nueva Candidata a Explorar
            ExpandIndex = 0; //Aleatorio?
            for (int i = 0; i < Candidates.Count; i++)
            {

                if (Candidates[i].Value < Candidates[ExpandIndex].Value || (Candidates[i].Value == Candidates[ExpandIndex].Value && Candidates[i].HeuristicDistance < Candidates[ExpandIndex].HeuristicDistance))
                {
                    ExpandIndex = i;
                }
            }
            Exploring = new ExploredNode(Candidates[ExpandIndex]);
        }
        if (IsPossible) Explored.Add(Exploring);
        StartCoroutine(GenerateExplored());
        StartCoroutine(GenerateSolution());
        IsSolved = true;
        Step = Explored.Count;
        StopWatch.Stop();
        return IsPossible;
    }
    private IEnumerator RealtimeSolution()
    {
        Vector2Int Coordinates;
        ExploredNode Exploring;
        StopWatch.Start();

        Candidates.Add(new AStarNode(WorldInfo.Beginning, 0, WorldInfo.Heuristic.Function(WorldInfo.Beginning, WorldInfo.End)));
        Exploring = new ExploredNode(Candidates[0]);
        NodesGenerated++;

        while (Candidates[ExpandIndex].Position != WorldInfo.End)
        {
            for (int i = 0; i < 4; i++)
            {
                Coordinates = Exploring.Position + ConstCoordinates.BasicDirections[i];
                if (IsLegal(Coordinates) && !WorldInfo.Obstacles.Contains(Coordinates) && Explored.Find(x => x.Position == Coordinates) == null)
                {
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
                        UpdateNodeValue(Exploring, Coordinates, true);
                    }
                }
            }

            //Actualizar listas
            Candidates.Remove(Candidates[ExpandIndex]);
            Explored.Add(Exploring);
            NodesExplored++;
            ExploredWaypoints.Add(Instantiate(ExploredPrefab, new Vector3(Explored[Explored.Count - 1].Position.x, .3f, Explored[Explored.Count - 1].Position.y), Quaternion.identity, ExploredContainer));
            if (Candidates.Count == 0)
            {
                IsPossible = false;
                break;
            }

            //Buscar nueva Candidata a Explorar
            ExpandIndex = 0; //Aleatorio?
            for (int i = 0; i < Candidates.Count; i++)
            {

                if (Candidates[i].Value < Candidates[ExpandIndex].Value || (Candidates[i].Value == Candidates[ExpandIndex].Value && Candidates[i].HeuristicDistance < Candidates[ExpandIndex].HeuristicDistance))
                {
                    ExpandIndex = i;
                }
            }
            Exploring = new ExploredNode(Candidates[ExpandIndex]);

            //Pause
            StopWatch.Stop();
            if (Speed > 99 && Time.deltaTime > Threshold) yield return null;
            else if (Speed != 0 && Speed <= 99) yield return new WaitForSeconds(1 / Speed);
            else while (Speed == 0) yield return null;
            StopWatch.Start();
        }
        if (IsPossible) Explored.Add(Exploring);
        ExploredWaypoints.Add(Instantiate(ExploredPrefab, new Vector3(Explored[Explored.Count - 1].Position.x, .3f, Explored[Explored.Count - 1].Position.y), Quaternion.identity, ExploredContainer));
        StartCoroutine(GenerateSolution());
        IsSolved = true;
        Step = Explored.Count;
        StopWatch.Stop();
        yield return null;
    }
    private void UpdateNodeValue(ExploredNode Origin, Vector2Int To, bool diagonal = false)
    {
        AStarNode Destiny = Candidates.Find(x => x.Position == To);
        float NewCost = Origin.Cost + (diagonal ? WorldInfo.ConstDiagonalCost ? 2 : WorldInfo.Heuristic.UnitaryDiagonal() : 1);
        if (Destiny == null)
        {
            Candidates.Add(new AStarNode(To, NewCost, WorldInfo.Heuristic.Function(To, WorldInfo.End), Origin));
            NodesGenerated++;
        }
        else if (Destiny.Cost > NewCost)
        {
            Destiny.Cost = NewCost;
            Destiny.Parent = Origin;
        }
    }
    private IEnumerator GenerateSolution()
    {
        SimulationTime = StopWatch.Elapsed.TotalSeconds;
        TotalTime = Time.time - TimeMark;
        if (IsPossible)
        {
            Solution = new List<GameObject>();
            ExploredNode aux = Explored[Explored.Count - 1];
            Cost = aux.Cost;
            while (aux != null)
            {
                Solution.Add(Instantiate(SolutionPrefab, SolutionContainer));
                Solution[Solution.Count - 1].transform.position = new Vector3(aux.Position.x, .3f, aux.Position.y);
                Solution[Solution.Count - 1].SetActive(false);
                aux = aux.Parent;
                //if (Time.deltaTime > ) yield return null; //Permite realentizar el reconocimiento de la solución para que no se ejecute en un solo frame
            }
            for (int i = Solution.Count - 1; i >= 0; i--)
            {
                Solution[i].SetActive(true);
                if (Speed > 99 && Time.deltaTime > Threshold) yield return null;
                else if (Speed != 0 && Speed <= 99) yield return new WaitForSeconds(.2f / Speed);
                else while (Speed == 0) yield return null;
            }
        }

    }
    private IEnumerator GenerateExplored()
    {
        for (int i = 0; i < ExploredWaypoints.Count; i++) Destroy(ExploredWaypoints[i]);
        for (int i = 0; i < Explored.Count; i++)
        {
            ExploredWaypoints.Add(Instantiate(ExploredPrefab, new Vector3(Explored[i].Position.x, .3f, Explored[i].Position.y), Quaternion.identity, ExploredContainer));
            if (Time.deltaTime > Threshold) yield return null;
        }
    }
    //Control de visualización
    public void ShowExplored(bool show)
    {
        ExploredContainer.gameObject.SetActive(show);
    }
    public void ShowSolution(bool show)
    {
        SolutionContainer.gameObject.SetActive(show);
    }
    public void Timeline(float time)
    {

        int GoToStep = (int)(((float)ExploredWaypoints.Count) * time / 100f);
        if (Step < GoToStep)
        {
            for (; Step < GoToStep;) ExploredWaypoints[Step++].SetActive(true);

        }
        else
        {
            for (; Step > GoToStep;) ExploredWaypoints[--Step].SetActive(false);
        }
    }
}
