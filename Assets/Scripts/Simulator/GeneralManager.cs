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
    private AStarSolver2 Solver2;

    //Variables de estado
    private bool IsPainting = false;
    private bool IsSolved = false;

    private void Start()
    {
        UI = GetComponent<UIManager>();
        World = GetComponent<WorldPainter>();
        Solver = GetComponent<AStarSolver>();
        Solver2 = GetComponent<AStarSolver2>();
        UI.enabled = true;
        World.enabled = true;
        Solver.enabled = false;
        Solver2.enabled = false;

        //Establecer Cámara
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
    }
    private void StartSolve()
    {
        IsPainting = false;
        World.enabled = false;
        Solver2.enabled = true;
        UI.SwitchToSolve();
        Solver2.Solve();
    }

    private void StartReview()
    {
        IsSolved = true;
        UI.SwitchToReview();
    }

}
