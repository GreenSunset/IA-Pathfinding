using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Master : MonoBehaviour
{
    //Organiza el resto de scripts y transiciona entre estados del simulador

    //Scripts
    private UIManager UI;
    private WorldPainter World;
    private AStarSolver Solver;

    //Variables de estado
    private bool IsPainting = false;
    private bool IsSolved = false;

    private void Start()
    {
        UI = GetComponent<UIManager>();
        World = GetComponent<WorldPainter>();
        Solver = GetComponent<AStarSolver>();
        UI.enabled = true;
        World.enabled = true;
        Solver.enabled = false;

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
        Solver.enabled = true;
        UI.SwitchToSolve();
    }

    private void StartReview()
    {
        IsSolved = true;
        UI.SwitchToReview();
    }

}
