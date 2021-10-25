using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentHandler : MonoBehaviour
{
    //Camara
    public GameObject MainCamera;

    //World Prefabs
    public GameObject MapPrefab;
    public GameObject StartPrefab;
    public GameObject EndPrefab;
    public GameObject ObstaclePrefab;


    private void Start()
    {
        //Set Up Map
        GameObject Aux = Instantiate(MapPrefab,new Vector3(WorldInfo.Size.x/2f - .5f, 0, WorldInfo.Size.y/2f - .5f), Quaternion.identity);
        Aux.transform.localScale = new Vector3(WorldInfo.Size.x / 10f, 1, WorldInfo.Size.y / 10f);
        Aux.GetComponent<Renderer>().material.mainTextureScale = new Vector2(WorldInfo.Size.x, WorldInfo.Size.y);

        //Setting up Obstacles
        //GetComponent<ObstaclePainter>().enabled = true;
        //if (WorldInfo.ManualObstacles || WorldInfo.ManualObjectives) StartPainter();??
        if (!WorldInfo.ManualObstacles || true)
        {
            for (int i = 0; WorldInfo.Obstacles != null && i < WorldInfo.Obstacles.Count; i++)
            {
                Aux = Instantiate(ObstaclePrefab);
                Aux.transform.position = new Vector3(WorldInfo.Obstacles[i].x, 0.1f, WorldInfo.Obstacles[i].y);
            }
        }
        //Setting up Objectives
        if (!WorldInfo.ManualObjectives || true)
        {
            Aux = Instantiate(StartPrefab);
            Aux.transform.position = new Vector3(WorldInfo.Beginning.x, 0.1f, WorldInfo.Beginning.y);

            Aux = Instantiate(EndPrefab);
            Aux.transform.position = new Vector3(WorldInfo.End.x, 0.1f, WorldInfo.End.y);
        }

        //Setting up Camera
        MainCamera.transform.position = new Vector3(WorldInfo.Size.x / 2f - .5f, 5, WorldInfo.Size.y / 2f - .5f);
        //Scale and margin

        //Start Solver
        AStarSolver Solver = GetComponent<AStarSolver>();
        Solver.Restart();
        IEnumerator coroutine = Solver.SolveCoroutine();
        StartCoroutine(coroutine);
    }
}
