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
        if (WorldInfo.Size.x > 0 && WorldInfo.Size.y > 0) SceneManager.LoadScene("ManualSelection");
    }

    // Generar Obstaculos aleatorios

    // Generar la distancia del inicio y final
}
