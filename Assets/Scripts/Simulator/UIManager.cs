using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    //Alternar entre interfaces
    public void SwitchToPaint()
    {
        UISolving.SetActive(false);
        UIReview.SetActive(false);
        UIPainter.SetActive(true);
    }
    public void SwitchToSolve()
    {
        UIPainter.SetActive(false);
        UIReview.SetActive(false);
        UISolving.SetActive(true);
    }
    public void SwitchToReview()
    {
        UIPainter.SetActive(false);
        UISolving.SetActive(false);
        UIReview.SetActive(true);
    }

}
