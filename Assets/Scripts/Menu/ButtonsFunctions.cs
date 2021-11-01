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
    bool isString;
    int temp;
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
        isString = int.TryParse(r, out temp);
        if (temp < 1)
        {
            WorldInfo.Size.x = 10;
        }
        else WorldInfo.Size.x = temp;
        Debug.Log("Rows entered: " + WorldInfo.Size.x);
    }
    // Guardar el numero de columnas
    public void SetCols(string c)
    {
        isString = int.TryParse(c, out temp);
        if (temp < 1)
        {
            WorldInfo.Size.y = 10;
        }
        else WorldInfo.Size.y = temp;
        Debug.Log("Rows entered:" + WorldInfo.Size.y);
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
        if (WorldInfo.Size.x > 0 && WorldInfo.Size.y > 0) SceneManager.LoadScene("Simulation");
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
    // Poner el punto de comienzo en un punto random
    // Ver si se puede poner el final
    private void GenerateObjectives(float distance)
    {
        //bool setBegin = false;
        int chebysovDistance = (int)Mathf.Round((WorldInfo.Size.x > WorldInfo.Size.y ? WorldInfo.Size.x : WorldInfo.Size.y) * (int)distance / 100);

        //Dario
        int distanceFromCenter = chebysovDistance - ((WorldInfo.Size.x > WorldInfo.Size.y ? WorldInfo.Size.x : WorldInfo.Size.y));
        if (distanceFromCenter > 0)
        {
            int yDiff, xDiff;
            if (Random.Range(0, 2) % 2 == 0)
            {
                xDiff = Random.Range(WorldInfo.Size.x % 2 == 0 ? 1 : 0, (WorldInfo.Size.x - 1) / 2 + 1);
                yDiff = Random.Range(xDiff < distanceFromCenter ? distanceFromCenter : WorldInfo.Size.y % 2 == 0 ? 1 : 0, (WorldInfo.Size.y - 1) / 2 + 1);
            }
            else
            {
                yDiff = Random.Range(WorldInfo.Size.y % 2 == 0 ? 1 : 0, (WorldInfo.Size.y - 1) / 2 + 1);
                xDiff = Random.Range(yDiff < distanceFromCenter ? distanceFromCenter : WorldInfo.Size.x % 2 == 0 ? 1 : 0, (WorldInfo.Size.x - 1) / 2 + 1);
            }
            WorldInfo.Beginning = new Vector2Int((Random.Range(0, 2) % 2 == 0? 1 : -1) * xDiff + (WorldInfo.Size.x - 1 / 2) + 1, (Random.Range(0, 2) % 2 == 0 ? 1 : -1) * yDiff + (WorldInfo.Size.y - 1 / 2) + 1);
        }
        else
        {
            WorldInfo.Beginning = new Vector2Int(Random.Range(0, WorldInfo.Size.x), Random.Range(0, WorldInfo.Size.y));
        }
        do
        {
            WorldInfo.End = new Vector2Int(Random.Range(0, WorldInfo.Size.x), Random.Range(0, WorldInfo.Size.y));
        } while (Mathf.Max(Mathf.Abs(WorldInfo.End.x - WorldInfo.Beginning.x), Mathf.Abs(WorldInfo.End.y - WorldInfo.Beginning.y)) < chebysovDistance);
    }
}
