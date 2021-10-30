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

    //Interfaces
    public GameObject UIPainter;
    public GameObject UISolving;
    public GameObject UIReview;

    //Objetos con los que interactúa
    public AStarSolver3 Solver;

    //Objetos Paint UI

    //Objetos Solve UI
    public Slider SpeedSlider;
    public Toggle HideExploredOnSolve;
    public Toggle TurnOffLightOnSolve;

    //Objetos Review UI
    public Slider TimelineSlider;
    public Toggle HideSolution;
    public Toggle HideExplored;
    public Toggle TurnOffLight;

    public GameObject MainLight;
    public Text InfoTextBox;

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
    }


    //Funciones de Solving UI
    public void SwitchToSolve()
    {
        UIPainter.SetActive(false);
        UIReview.SetActive(false);
        UISolving.SetActive(true);

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

        TimelineSlider.onValueChanged.AddListener(delegate { TimelineListener(); });
        HideSolution.onValueChanged.AddListener(delegate { HideSolutionListener(); });
        HideExplored.onValueChanged.AddListener(delegate { HideExploredListener(HideExplored); });
        TurnOffLight.onValueChanged.AddListener(delegate { ToggleLightListener(TurnOffLight); });
        InfoTextBox.text = "Nodos Generados: " + Solver.NodesGenerated + "\nNodos Explorados: " + Solver.NodesExplored + "\nTiempo de simulación: " + Solver.SimulationTime + "\nTiempo total: " + Solver.TotalTime;
    }

    public void ToggleLightListener(Toggle toggle)
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
}
