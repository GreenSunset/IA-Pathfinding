using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentHandler : MonoBehaviour
{
    //Camara
    public GameObject MainCamera;
    public ObstaclePainter Painter;
    public float targetaspect = 16.0f / 9.0f;

    //World Prefabs
    public GameObject MapPrefab;
    public GameObject StartPrefab;
    public GameObject EndPrefab;
    public GameObject ObstaclePrefab;

    private void Awake()
    {
        WorldInfo.Size = new Vector2Int(1000, 100);
    }

    private void Start()
    {
        Painter.enabled = false;

        //Set Up Map
        GameObject Aux = Instantiate(MapPrefab,new Vector3(WorldInfo.Size.x/2f - .5f, 0, WorldInfo.Size.y/2f - .5f), Quaternion.identity);
        Aux.transform.localScale = new Vector3(WorldInfo.Size.x / 10f, 1, WorldInfo.Size.y / 10f);
        Aux.GetComponent<Renderer>().material.mainTextureScale = new Vector2(WorldInfo.Size.x, WorldInfo.Size.y);

        //Setting up Obstacles
        
        if (WorldInfo.ManualObstacles || WorldInfo.ManualObjectives) Painter.enabled = true;
        for (int i = 0; WorldInfo.Obstacles != null && i < WorldInfo.Obstacles.Count; i++)
        {
            Aux = Instantiate(ObstaclePrefab);
            Aux.transform.position = new Vector3(WorldInfo.Obstacles[i].x, 0.1f, WorldInfo.Obstacles[i].y);
        }
        //Setting up Objectives
        if (!WorldInfo.ManualObjectives)
        {
            Aux = Instantiate(StartPrefab);
            Aux.transform.position = new Vector3(WorldInfo.Beginning.x, 0.1f, WorldInfo.Beginning.y);

            Aux = Instantiate(EndPrefab);
            Aux.transform.position = new Vector3(WorldInfo.End.x, 0.1f, WorldInfo.End.y);
        }

        //Setting up Camera
        SetUpCamera();
        //Scale and margin
        
    }
    private void Update()
    {
        if (Painter.AllDone)
        {
            Painter.enabled = false;
            StartSolving();
        }
    }

    private void StartSolving()
    {
        //Start Solver
        AStarSolver Solver = GetComponent<AStarSolver>();
        Solver.Restart();
        IEnumerator coroutine = Solver.SolveCoroutine();
        StartCoroutine(coroutine);
    }

    private void SetUpCamera()
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
        //camera.orthographicSize = Mathf.Max(WorldInfo.Size.x, WorldInfo.Size.y) / 2;
    }
}
