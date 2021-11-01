using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleFunctions : MonoBehaviour
{
    public Slider mySliderObstacle;
    public Slider mySliderDistance;
    public void ObstacleSlider(Toggle myToggle)
    {
        if (!myToggle.isOn)
        {
            mySliderObstacle.gameObject.SetActive(true);
            WorldInfo.ManualObstacles = false;
            Debug.Log("Obstaculos aleatorios.");
        }
        else
        {
            mySliderObstacle.gameObject.SetActive(false);
            WorldInfo.ManualObstacles = true;
            Debug.Log("Obstaculos manuales.");
        }
    }
    // Guarda si queremos poner la distancia del comienzo al final manualmente o no
    public void DistanceSlider(Toggle myToggle)
    {
        if (!myToggle.isOn)
        {
            mySliderDistance.gameObject.SetActive(true);
            WorldInfo.ManualObjectives = false;
            Debug.Log("Distancia aleatoria.");
        }
        else
        {
            mySliderDistance.gameObject.SetActive(false);
            WorldInfo.ManualObjectives = true;
            Debug.Log("Distancia manual.");
        }
    }
    // Guarda si queremos poner la distancia del comienzo al final manualmente o no
    public void MoveToggle(Toggle myToggle)
    {
        if (myToggle.isOn)
        {
            WorldInfo.DoDiagonals = true;
            Debug.Log("8 movimientos.");
        }
        else
        {
            WorldInfo.DoDiagonals = false;
            Debug.Log("4 movimientos.");
        }
    }
    public void RealTimeToggle(Toggle myToggle)
    {
        WorldInfo.RealtimeSolution = myToggle.isOn;
    }
    public void DiagonalCostToggle(Toggle myToggle)
    {
        WorldInfo.ConstDiagonalCost = !myToggle.isOn;
    }
}
