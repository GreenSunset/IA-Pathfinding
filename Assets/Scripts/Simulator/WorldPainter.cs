using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldPainter : MonoBehaviour
{
    //Instancia y controla el mapa, el inicio, el final y los obstáculos

    //Prefabs
    public GameObject MapPrefab;
    public GameObject BeginningPrefab;
    public GameObject EndPrefab;
    public GameObject ObstaclePrefab;

    //Objetos
    private GameObject Map;
    private GameObject Beginning;
    private GameObject End;
    public Transform ObstaclesContainer;
    private List<GameObject> Obstacles = new List<GameObject>();

    //Pincel para selección manual
    private Ray ray => Camera.main.ScreenPointToRay(Input.mousePosition);
    public bool IsBrushActive = false;
    private bool IsBrushLocked = false;
    private bool AreObstaclesSet = !WorldInfo.ManualObstacles;
    private bool IsBeginningSet = !WorldInfo.ManualObjectives;

    //Pintar Mundo
    public void PaintMap()
    {
        Destroy(Map);
        Map = Instantiate(MapPrefab, new Vector3(WorldInfo.Size.x / 2f - .5f, 0, WorldInfo.Size.y / 2f - .5f), Quaternion.identity);
        Map.transform.localScale = new Vector3(WorldInfo.Size.x / 10f, 1, WorldInfo.Size.y / 10f);
        Map.GetComponent<Renderer>().material.mainTextureScale = new Vector2(WorldInfo.Size.x, WorldInfo.Size.y);
    }
    public void PaintObstacles()
    {
        GameObject Aux;
        if (Obstacles != null) for (int i = 0; i < Obstacles.Count; i++) Destroy(Obstacles[i]);
        for (int i = 0; i < WorldInfo.Obstacles.Count; i++)
        {
            Aux = Instantiate(ObstaclePrefab, new Vector3(WorldInfo.Obstacles[i].x, 0.1f, WorldInfo.Obstacles[i].y), Quaternion.identity, ObstaclesContainer.transform);
            Obstacles.Add(Aux);
        }
    }
    public void PaintObjectives()
    {
        Destroy(Beginning);
        Destroy(End);
        Beginning = Instantiate(BeginningPrefab, new Vector3(WorldInfo.Beginning.x, 0.3f, WorldInfo.Beginning.y), Quaternion.identity);
        End = Instantiate(EndPrefab, new Vector3(WorldInfo.End.x, 0.3f, WorldInfo.End.y), Quaternion.identity);
    }

    //Pincel
    private void Update()
    {
        if (IsBrushActive)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("ClickDown");
                IsBrushLocked = true;
                if (!AreObstaclesSet) StartCoroutine(ObstaclesBrush());
                else if (!IsBeginningSet) StartCoroutine(DragBeginning());
                else StartCoroutine(DragEnd());
            }
            else if (Input.GetMouseButtonUp(0))
            {
                Debug.Log("ClickUp");
                IsBrushLocked = false;
            }
            else if (Input.GetKeyDown("return"))
            {
                Debug.Log("Enter");
                Forward();
            }
            else if (Input.GetKeyDown("backspace"))
            {
                Debug.Log("BackSpace");
                GoBack();
            }
        }
    }
    public void Forward()
    {
        IsBrushLocked = false;
        if (!AreObstaclesSet)
        {
            AreObstaclesSet = true;
            if (!WorldInfo.ManualObjectives) IsBrushActive = false;
        }
        else if (!IsBeginningSet && WorldInfo.Beginning != ConstCoordinates.Invalid) IsBeginningSet = true;
        else if (WorldInfo.ManualObjectives && WorldInfo.Beginning != ConstCoordinates.Invalid) IsBrushActive = false;
    }

    public void GoBack()
    {
        IsBrushLocked = false;
        if (IsBeginningSet) IsBeginningSet = false;
        else if (AreObstaclesSet) AreObstaclesSet = false;
    }
    //Selección Manual
    private IEnumerator ObstaclesBrush()
    {
        RaycastHit hit;
        Vector2Int Coordinates;
        GameObject obj;
        if (Physics.Raycast(ray, out hit, 100.0f))
        {
            Debug.Log("{" + hit.point.x + ", " + hit.point.y + ", " + hit.point.z + "}"); //DELETE
            Coordinates = new Vector2Int(Mathf.RoundToInt(hit.point.x), Mathf.RoundToInt(hit.point.z));
            if (WorldInfo.Obstacles.Contains(Coordinates))
            {
                obj = Obstacles[WorldInfo.Obstacles.IndexOf(Coordinates)];
                Destroy(obj);
                Obstacles.Remove(obj);
                WorldInfo.Obstacles.Remove(Coordinates);
                while (IsBrushLocked)
                {
                    yield return null;
                    if (Physics.Raycast(ray, out hit, 100.0f))
                    {
                        Debug.Log("locked removing {" + hit.point.x + ", " + hit.point.y + ", " + hit.point.z + "}"); //DELETE
                        Coordinates = new Vector2Int(Mathf.RoundToInt(hit.point.x), Mathf.RoundToInt(hit.point.z));
                        if (WorldInfo.Obstacles.Contains(Coordinates) && Coordinates != WorldInfo.Beginning && Coordinates != WorldInfo.End)
                        {
                            obj = Obstacles[WorldInfo.Obstacles.IndexOf(Coordinates)];
                            Destroy(obj);
                            Obstacles.Remove(obj);
                            WorldInfo.Obstacles.Remove(Coordinates);
                        }
                    }
                }
            }
            else if (WorldInfo.Obstacles.Count < (WorldInfo.Size.x * WorldInfo.Size.y) - 1 && Coordinates != WorldInfo.Beginning && Coordinates != WorldInfo.End)
            {
                obj = Instantiate(ObstaclePrefab, new Vector3(Coordinates.x, .3f, Coordinates.y), Quaternion.identity, ObstaclesContainer);
                Obstacles.Add(obj);
                WorldInfo.Obstacles.Add(Coordinates);
                while (IsBrushLocked && WorldInfo.Obstacles.Count < (WorldInfo.Size.x * WorldInfo.Size.y) - 1)
                {
                    yield return null;
                    if (Physics.Raycast(ray, out hit, 100.0f))
                    {
                        Debug.Log("locked adding {" + hit.point.x + ", " + hit.point.y + ", " + hit.point.z + "}"); //DELETE
                        Coordinates = new Vector2Int(Mathf.RoundToInt(hit.point.x), Mathf.RoundToInt(hit.point.z));
                        if (!WorldInfo.Obstacles.Contains(Coordinates) && Coordinates != WorldInfo.Beginning && Coordinates != WorldInfo.End)
                        {
                            obj = Instantiate(ObstaclePrefab, new Vector3(Coordinates.x, .3f, Coordinates.y), Quaternion.identity, ObstaclesContainer);
                            Obstacles.Add(obj);
                            WorldInfo.Obstacles.Add(Coordinates);
                        }
                    }
                }
            }
        }
        yield return null;
    }
    private IEnumerator DragBeginning()
    {
        RaycastHit hit;
        Vector2Int Coordinates;
        if (Physics.Raycast(ray, out hit, 100.0f))
        {
            Debug.Log("{" + hit.point.x + ", " + hit.point.y + ", " + hit.point.z + "}"); //DELETE
            Coordinates = new Vector2Int(Mathf.RoundToInt(hit.point.x), Mathf.RoundToInt(hit.point.z));
            if (WorldInfo.Beginning == Coordinates)
            {
                //Delete
                Destroy(Beginning);
                WorldInfo.Beginning = ConstCoordinates.Invalid;
            }
            else if (!WorldInfo.Obstacles.Contains(Coordinates))
            {
                if (Beginning == null) Beginning = Instantiate(BeginningPrefab);
                WorldInfo.Beginning = Coordinates;
                Beginning.transform.position = new Vector3(Coordinates.x, 0.3f, Coordinates.y);
                while (IsBrushLocked)
                {
                    yield return null;
                    if (Physics.Raycast(ray, out hit, 100.0f))
                    {
                        Coordinates = new Vector2Int(Mathf.RoundToInt(hit.point.x), Mathf.RoundToInt(hit.point.z));
                        if (!WorldInfo.Obstacles.Contains(Coordinates))
                        {
                            WorldInfo.Beginning = Coordinates;
                            Beginning.transform.position = new Vector3(Coordinates.x, 0.3f, Coordinates.y);
                        }
                    }
                }
            }
        }
        yield return null;
    }
    private IEnumerator DragEnd()
    {
        RaycastHit hit;
        Vector2Int Coordinates;
        if (Physics.Raycast(ray, out hit, 100.0f))
        {
            Debug.Log("{" + hit.point.x + ", " + hit.point.y + ", " + hit.point.z + "}"); //DELETE
            Coordinates = new Vector2Int(Mathf.RoundToInt(hit.point.x), Mathf.RoundToInt(hit.point.z));
            if (WorldInfo.End == Coordinates)
            {
                //Delete
                Destroy(End);
                WorldInfo.End = ConstCoordinates.Invalid;
            }
            else if (!WorldInfo.Obstacles.Contains(Coordinates))
            {
                if (End == null) End = Instantiate(EndPrefab);
                WorldInfo.End = Coordinates;
                End.transform.position = new Vector3(Coordinates.x, 0.3f, Coordinates.y);
                while (IsBrushLocked)
                {
                    yield return null;
                    if (Physics.Raycast(ray, out hit, 100.0f))
                    {
                        Coordinates = new Vector2Int(Mathf.RoundToInt(hit.point.x), Mathf.RoundToInt(hit.point.z));
                        if (!WorldInfo.Obstacles.Contains(Coordinates))
                        {
                            WorldInfo.End = Coordinates;
                            End.transform.position = new Vector3(Coordinates.x, 0.3f, Coordinates.y);
                        }
                    }
                }
            }
        }
        yield return null;
    }
}
