using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonsFunctions : MonoBehaviour
{
    // ATRIBUTOS 
    int distance;
    public Canvas MenuPrincipal;
    public Canvas AjusteTablero;
    // METODOS PUBLICOS
    private void Start()
    {
        MenuPrincipal.enabled = true;
        AjusteTablero.enabled = false;
    }
    // Cambiar Canvas
    public void SwitchCanvas()
    {
        MenuPrincipal.enabled = !MenuPrincipal.enabled;
        AjusteTablero.enabled = !AjusteTablero.enabled;
    }
    // Guardar el numero de filas
    public void SetRows(string r) {
        WorldInfo.Size.x = int.Parse(r); 
        Debug.Log("Rows entered");
    }
    // Guardar el numero de columnas
    public void SetCols(string c)
    {
        WorldInfo.Size.y = int.Parse(c); 
        Debug.Log("Cols entered");
    }
    // Guardar la distancia entre el comienzo y  el final
    public void SetDistance(string d)
    {
        distance = int.Parse(d);
        Debug.Log("Distance entered");
    }
    // Salir del juego
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game is exiting");
    }
    // Empezar la simulacion
    public void StartSimulation()
    {
        if (!WorldInfo.ManualObjectives) GenerateObjectives(TemporalData.distance);
        if (!WorldInfo.ManualObstacles) GenerateObstacles(TemporalData.obstacles);
        if (WorldInfo.Size.x > 0 && WorldInfo.Size.y > 0) SceneManager.LoadScene("ManualSelection");
    }

    // Generar Obstaculos aleatorios
    private void GenerateObstacles(float density)
    {
        //Brute force
        Vector2Int AuxCoordinates;
        for (int i = Mathf.RoundToInt((WorldInfo.Size.x * WorldInfo.Size.y - (WorldInfo.Beginning == WorldInfo.End? 1:2)) * density / 100f); i > 0;)
        {
            AuxCoordinates = new Vector2Int(Random.Range(0, WorldInfo.Size.x), Random.Range(0, WorldInfo.Size.y));
            if (WorldInfo.Beginning != AuxCoordinates && WorldInfo.End != AuxCoordinates && !WorldInfo.Obstacles.Contains(AuxCoordinates))
            {
                WorldInfo.Obstacles.Add(AuxCoordinates);
                i--;
            }
        }
    }


    // Generar la distancia del inicio y final
    private void GenerateObjectives(float distance)
    {
        //No hace nada actualmente
        WorldInfo.ManualObjectives = true;
    }
}