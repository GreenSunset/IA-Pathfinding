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
            TemporalData.manualObstacles = false;
            Debug.Log("Obstaculos aleatorios.");
        }
        else
        {
            mySliderObstacle.gameObject.SetActive(false);
            TemporalData.manualObstacles = true;
            Debug.Log("Obstaculos manuales.");
        }
    }
    // Guarda si queremos poner la distancia del comienzo al final manualmente o no
    public void DistanceSlider(Toggle myToggle)
    {
        if (!myToggle.isOn)
        {
            mySliderDistance.gameObject.SetActive(true);
            TemporalData.manualDistance = false;
            Debug.Log("Distancia aleatoria.");
        }
        else
        {
            mySliderDistance.gameObject.SetActive(false);
            TemporalData.manualDistance = true;
            Debug.Log("Distancia manual.");
        }
    }
}
