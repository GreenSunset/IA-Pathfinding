using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    //Ajusta Cámara al inicio, Controla interacciones con botones
    //Camara
    public GameObject MainCamera;
    public float targetaspect = 16f / 9f;

    //Comunicación
    [HideInInspector]
    public bool RestartSignal = false;

    //Objetos con los que interactúa
    [Space(5)]
    [Header("Script Symbiosis")]
    public AStarSolver Solver;
    public WorldPainter Painter;

    //Objetos Paint UI
    [Space(5)]
    [Header("Painter UI")]
    public GameObject UIPainter;
    public Text CurrentState;
    public GameObject ForwardButton;
    public GameObject BackButton;
    //Colores
    private Color InactiveRed = new Color(.4f, .4f, .4f);
    private Color Red = new Color(1, .2f, 0);
    private Color InactiveGreen = new Color(.6f, .6f, .6f);
    private Color Green = new Color(.2f, 1, 0);

    //Objetos Solve UI
    [Space(5)]
    [Header("Solve UI")]
    public GameObject UISolving;
    public Slider SpeedSlider;
    public Toggle HideExploredOnSolve;
    public Toggle TurnOffLightOnSolve;

    //Objetos Review UI
    [Space(5)]
    [Header("Review UI")]
    public GameObject UIReview;
    public Slider TimelineSlider;
    public Toggle HideSolution;
    public Toggle HideExplored;
    public Toggle TurnOffLight;

    public GameObject MainLight;
    public Text InfoTextBox;

    //Objetos Redo UI
    [Space(5)]
    [Header("Redo UI")]
    public GameObject UIRedo;
    public Slider HeuristicSlider;
    public Toggle DiagonalsToggle;
    public Toggle EditWorldToggle;
    public Toggle RealTimeToggle;

    //Iniciar Cámara
    public void SetUpCamera()
    {
        // Ratio de cámara fijo
        float windowaspect = Screen.width / (float)Screen.height;
        float scaleheight = windowaspect / targetaspect;
        Camera camera = MainCamera.GetComponent<Camera>();
        Rect rect = camera.rect;
        if (scaleheight < 1.0f)
        {
            rect.width = 1.0f;
            rect.height = scaleheight;
            rect.x = 0;
            rect.y = (1.0f - scaleheight) / 2.0f;
            camera.rect = rect;
        }
        else
        {
            float scalewidth = 1.0f / scaleheight;
            rect.width = scalewidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scalewidth) / 2.0f;
            rect.y = 0;
            camera.rect = rect;
        }

        MainCamera.transform.position = new Vector3(WorldInfo.Size.x / 2f - .5f, 5, WorldInfo.Size.y / 2f - .5f);
        float WorldRatio = WorldInfo.Size.x / (float)WorldInfo.Size.y;
        camera.orthographicSize = Mathf.Max(WorldInfo.Size.x, WorldInfo.Size.y) / 2;
    }

    //Funciones de Paint UI
    public void SwitchToPaint()
    {
        UISolving.SetActive(false);
        UIReview.SetActive(false);
        UIPainter.SetActive(true);
        UIRedo.SetActive(false);

        Painter.AreObstaclesSet = !WorldInfo.ManualObstacles;
        Painter.IsBeginningSet = !WorldInfo.ManualObjectives;
        if (WorldInfo.ManualObstacles)
        {
            CurrentState.text = "Pintando Obstáculos";
            if (WorldInfo.ManualObjectives)
            {
                ForwardButton.GetComponent<Image>().color = Green;
                ForwardButton.GetComponentInChildren<Text>().text = "Pintar inicio";
                ForwardButton.GetComponent<Button>().enabled = true;
                BackButton.GetComponentInChildren<Text>().text = "...";
                BackButton.GetComponent<Image>().color = InactiveRed;
                BackButton.GetComponent<Button>().enabled = false;
            }
            else
            {
                ForwardButton.GetComponent<Image>().color = Green;
                ForwardButton.GetComponentInChildren<Text>().text = "Simular";
                ForwardButton.GetComponent<Button>().enabled = true;
                BackButton.GetComponentInChildren<Text>().text = "...";
                BackButton.GetComponent<Image>().color = InactiveRed;
                BackButton.GetComponent<Button>().enabled = false;
            }
        }
        else if (WorldInfo.ManualObjectives)
        {
            CurrentState.text = "Pintando Inicio";

            ForwardButton.GetComponent<Image>().color = Green;
            ForwardButton.GetComponentInChildren<Text>().text = "Pintar final";
            ForwardButton.GetComponent<Button>().enabled = true;
            BackButton.GetComponent<Image>().color = Red;
            BackButton.GetComponentInChildren<Text>().text = "Modificar Obstáculos";
            BackButton.GetComponent<Button>().enabled = true;
        }
        StartCoroutine(PaintInput());
    }
    private IEnumerator PaintInput()
    {
        while (Painter.IsBrushActive)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("ClickDown");
                Painter.IsBrushLocked = true;
                if (!Painter.AreObstaclesSet) StartCoroutine(Painter.ObstaclesBrush());
                else if (!Painter.IsBeginningSet) StartCoroutine(Painter.DragBeginning());
                else StartCoroutine(Painter.DragEnd());
            }
            else if (Input.GetMouseButtonUp(0))
            {
                Debug.Log("ClickUp");
                Painter.IsBrushLocked = false;
            }
            else if (Input.GetKeyDown("return"))
            {
                Debug.Log("Enter");
                Forward();
            }
            else if (Input.GetKeyDown("backspace"))
            {
                Debug.Log("BackSpace");
                Back();
            }
            yield return null;
        }
    }
    public void Forward()
    {

        Painter.IsBrushLocked = false;
        if (!Painter.AreObstaclesSet)
        {
            CurrentState.text = "Pintando Inicio";

            ForwardButton.GetComponent<Image>().color = Green;
            ForwardButton.GetComponentInChildren<Text>().text = "Pintar final";
            ForwardButton.GetComponent<Button>().enabled = true;

            BackButton.GetComponent<Image>().color = Red;
            BackButton.GetComponentInChildren<Text>().text = "Modificar Obstáculos";
            BackButton.GetComponent<Button>().enabled = true;

            Painter.AreObstaclesSet = true;
            if (!WorldInfo.ManualObjectives) Painter.IsBrushActive = false;
        }
        else if (!Painter.IsBeginningSet && WorldInfo.Beginning != ConstCoordinates.Invalid)
        {
            CurrentState.text = "Pintando Final";

            ForwardButton.GetComponent<Image>().color = Green;
            ForwardButton.GetComponentInChildren<Text>().text = "Empezar Simulación";
            ForwardButton.GetComponent<Button>().enabled = true;

            BackButton.GetComponent<Image>().color = Red;
            BackButton.GetComponentInChildren<Text>().text = "Modificar Inicio";
            BackButton.GetComponent<Button>().enabled = true;

            Painter.IsBeginningSet = true;
        }
        else if (WorldInfo.ManualObjectives && WorldInfo.Beginning != ConstCoordinates.Invalid && WorldInfo.End != ConstCoordinates.Invalid)
        {
            Painter.IsBrushActive = false;
        }
    }
    public void Back()
    {
        Painter.IsBrushLocked = false;

        if (Painter.IsBeginningSet)
        {
            CurrentState.text = "Pintando Inicio";

            ForwardButton.GetComponent<Image>().color = Green;
            ForwardButton.GetComponentInChildren<Text>().text = "Pintar final";
            ForwardButton.GetComponent<Button>().enabled = true;

            BackButton.GetComponent<Image>().color = Red;
            BackButton.GetComponentInChildren<Text>().text = "Modificar Obstáculos";
            BackButton.GetComponent<Button>().enabled = true;

            Painter.IsBeginningSet = false;
        }
        else if (Painter.AreObstaclesSet)
        {
            CurrentState.text = "Pintando Obstáculos";

            ForwardButton.GetComponent<Image>().color = Green;
            ForwardButton.GetComponentInChildren<Text>().text = "Pintar inicio";
            ForwardButton.GetComponent<Button>().enabled = true;

            BackButton.GetComponent<Image>().color = InactiveRed;
            BackButton.GetComponentInChildren<Text>().text = "...";
            BackButton.GetComponent<Button>().enabled = false;

            Painter.AreObstaclesSet = false;
        }

    }

    //Funciones de Solving UI
    public void SwitchToSolve()
    {
        UIPainter.SetActive(false);
        UIReview.SetActive(false);
        UISolving.SetActive(true);
        UIRedo.SetActive(false);

        SpeedSlider.value = Solver.Speed;
        HideExploredOnSolve.isOn = Solver.ExploredContainer.gameObject.activeSelf;
        TurnOffLightOnSolve.isOn = !MainLight.activeSelf;

        SpeedSlider.onValueChanged.RemoveAllListeners();
        HideExploredOnSolve.onValueChanged.RemoveAllListeners();
        TurnOffLightOnSolve.onValueChanged.RemoveAllListeners();

        SpeedSlider.onValueChanged.AddListener(delegate { SpeedListener(); });
        HideExploredOnSolve.onValueChanged.AddListener(delegate { HideExploredListener(HideExploredOnSolve); });
        TurnOffLightOnSolve.onValueChanged.AddListener(delegate { ToggleLightListener(TurnOffLightOnSolve); });
    }
    private void SpeedListener()
    {
        Solver.Speed = SpeedSlider.value;
    }

    //Funciones de Review UI
    public void SwitchToReview()
    {
        UIPainter.SetActive(false);
        UISolving.SetActive(false);
        UIReview.SetActive(true);
        UIRedo.SetActive(false);

        TimelineSlider.value = Solver.Step;
        HideSolution.isOn = Solver.SolutionContainer.gameObject.activeSelf;
        HideExplored.isOn = Solver.ExploredContainer.gameObject.activeSelf;
        TurnOffLight.isOn = !MainLight.activeSelf;

        TimelineSlider.onValueChanged.RemoveAllListeners();
        HideSolution.onValueChanged.RemoveAllListeners();
        HideExplored.onValueChanged.RemoveAllListeners();
        TurnOffLight.onValueChanged.RemoveAllListeners();

        TimelineSlider.onValueChanged.AddListener(delegate { TimelineListener(); });
        HideSolution.onValueChanged.AddListener(delegate { HideSolutionListener(); });
        HideExplored.onValueChanged.AddListener(delegate { HideExploredListener(HideExplored); });
        TurnOffLight.onValueChanged.AddListener(delegate { ToggleLightListener(TurnOffLight); });
        if (Solver.IsPossible)
        {
            InfoTextBox.text = "Solución encontrada.\nCoste final: " + Solver.Cost + "\nNodos Generados: " + Solver.NodesGenerated + "\nNodos Explorados: " + Solver.NodesExplored + "\nTiempo de simulación: " + Solver.SimulationTime + "\nTiempo total: " + Solver.TotalTime;
        }
        else
        {
            InfoTextBox.text = "No se ha encontrado solución.\nNodos Generados: " + Solver.NodesGenerated + "\nNodos Explorados: " + Solver.NodesExplored + "\nTiempo de simulación: " + Solver.SimulationTime + "\nTiempo total: " + Solver.TotalTime;
            HideSolution.gameObject.SetActive(false);
        }
    }
    private void ToggleLightListener(Toggle toggle)
    {
        MainLight.SetActive(!toggle.isOn);
    }
    private void TimelineListener()
    {
        Solver.Timeline(TimelineSlider.value);
    }
    private void HideSolutionListener()
    {
        Solver.ShowSolution(HideSolution.isOn);
    }
    private void HideExploredListener(Toggle toggle)
    {
        Solver.ShowExplored(toggle.isOn);
    }
    public void ExitApp()
    {
        Application.Quit();
    }
    //Funciones de Redo UI

    public void SwitchToRedo()
    {
        UIPainter.SetActive(false);
        UISolving.SetActive(false);
        UIReview.SetActive(false);
        UIRedo.SetActive(true);

        if (WorldInfo.Heuristic.Equals(new ManhattanHeuristic())) HeuristicSlider.value = 0;
        else if (WorldInfo.Heuristic.Equals(new EuclideanHeuristic())) HeuristicSlider.value = 1;
        else if (WorldInfo.Heuristic.Equals(new ChebyshovHeuristic())) HeuristicSlider.value = 2;
        DiagonalsToggle.isOn = WorldInfo.DoDiagonals;
        EditWorldToggle.isOn = false;
        RealTimeToggle.isOn = WorldInfo.RealtimeSolution;

        HeuristicSlider.onValueChanged.RemoveAllListeners();
        DiagonalsToggle.onValueChanged.RemoveAllListeners();
        EditWorldToggle.onValueChanged.RemoveAllListeners();
        RealTimeToggle.onValueChanged.RemoveAllListeners();

        HeuristicSlider.onValueChanged.AddListener(delegate { HeuristicSliderListener(); });
        DiagonalsToggle.onValueChanged.AddListener(delegate { DiagonalsToggleListener(); });
        EditWorldToggle.onValueChanged.AddListener(delegate { EditWorldToggleListener(); });
        RealTimeToggle.onValueChanged.AddListener(delegate { RealTimeToggleListener(); });
    }
    private void HeuristicSliderListener()
    {
        Debug.Log("Heurictic Saved: " + (int)HeuristicSlider.value);
        switch ((int)HeuristicSlider.value)
        {
            case 0:
                WorldInfo.Heuristic = new ManhattanHeuristic();
                break;
            case 1:
                WorldInfo.Heuristic = new EuclideanHeuristic();
                break;
            case 2:
                WorldInfo.Heuristic = new ChebyshovHeuristic();
                break;
            default:
                WorldInfo.Heuristic = new EuclideanHeuristic();
                break;
        }
    }
    private void DiagonalsToggleListener()
    {
       WorldInfo.DoDiagonals = DiagonalsToggle.isOn;
    }
    private void EditWorldToggleListener()
    {
        WorldInfo.ManualObjectives = EditWorldToggle.isOn;
        WorldInfo.ManualObstacles = EditWorldToggle.isOn;
    }
    private void RealTimeToggleListener()
    {
        WorldInfo.RealtimeSolution = RealTimeToggle.isOn;
    }
    public void Restart()
    {
        RestartSignal = true;
    }
}
