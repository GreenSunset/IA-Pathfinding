using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralManager : MonoBehaviour
{
    //Organiza el resto de scripts y transiciona entre estados del simulador

    //Scripts
    private UIManager UI;
    private WorldPainter World;
    private AStarSolver Solver;

    //Variables de estado
    private bool IsPainting = false;
    private bool IsSolving = false;

    private void Start()
    {
        UI = GetComponent<UIManager>();
        World = GetComponent<WorldPainter>();
        //Solver1 = GetComponent<AStarSolver>();
        Solver = GetComponent<AStarSolver>();
        UI.enabled = true;
        World.enabled = true;
        Solver.enabled = false;

        //Establecer C�mara
        UI.SetUpCamera();

        //Generar Mundo
        World.PaintMap();
        World.PaintObstacles();
        if (!WorldInfo.ManualObjectives) World.PaintObjectives();
        if (WorldInfo.ManualObjectives || WorldInfo.ManualObstacles)
        {
            IsPainting = true;
            World.IsBrushActive = true;
            UI.SwitchToPaint();
        }
        else StartSolve();
    }

    private void Update()
    {
        if (IsPainting && !World.IsBrushActive) StartSolve();
        else if (IsSolving && Solver.IsSolved) StartReview();
        else if (UI.RestartSignal) Restart();
    }
    private void StartSolve()
    {
        WorldInfo.ManualObjectives = false;
        WorldInfo.ManualObstacles = false;
        IsPainting = false;
        IsSolving = true;
        World.enabled = false;
        Solver.enabled = true;
        UI.SwitchToSolve();
        Solver.Solve();
    }

    private void StartReview()
    {
        IsSolving = false;
        UI.SwitchToReview();
    }

    private void Restart()
    {
        //Generar Mundo
        UI.RestartSignal = false;
        Solver.Restart();
        World.enabled = true;
        Solver.enabled = false;

        if (WorldInfo.ManualObjectives || WorldInfo.ManualObstacles)
        {
            IsPainting = true;
            World.IsBrushActive = true;
            UI.SwitchToPaint();
        }
        else StartSolve();
    }

}
