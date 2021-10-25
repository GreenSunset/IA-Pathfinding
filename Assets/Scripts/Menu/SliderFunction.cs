using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderFunction : MonoBehaviour
{
    public Text myTextObstacle;
    public Text myTextDistance;
    public Slider mySliderObstacle;
    public Slider mySliderDistance;
    public Slider mySliderSelection;
    // Cuando comienza la escena el slider estara escondido
    private void Start()
    {
        mySliderObstacle.gameObject.SetActive(false);
        mySliderDistance.gameObject.SetActive(false);
    }
    // En cada "frame" de juego el texto se actualizará con el valor del slider
    void Update()
    {
        myTextObstacle.text = mySliderObstacle.value.ToString("0") + "%";
        TemporalData.obstacles = (int)mySliderObstacle.value;
        myTextDistance.text = mySliderDistance.value.ToString("0") + "%";
        TemporalData.distance = (int)mySliderDistance.value; 
    }
    public void SetHeuristicAlgorithm()
    {
        Debug.Log("Heurictic Saved: " + (int)mySliderSelection.value);
        switch ((int)mySliderSelection.value)
        {
            case 0:
                WorldInfo.Heuristic = new ManhatanHeuristic();
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
}

