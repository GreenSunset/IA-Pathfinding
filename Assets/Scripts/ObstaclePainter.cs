using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObstaclePainter : MonoBehaviour
{
    //Objetos
    public GameObject Map;
    public GameObject MainCamera;

    //prefabs
    public GameObject BeginningPrefab;
    public GameObject EndPrefab;
    public GameObject ObstaclePrefab;

    //Redundant if I merge Scenes 1 and 2
    public List<GameObject> ProvisionalObstacles;
    private GameObject ProvisionalEnd;
    private GameObject ProvisionalBeginning;

    // Class Variables
    private bool locked = false;
    private bool PaintingObstacles = false;
    private bool SettingBeginning = false;
    private bool AllDone = false;
    private RaycastHit hit;
    private Ray ray => Camera.main.ScreenPointToRay(Input.mousePosition);
    private Vector2Int Coordinates;


    private void Start()
    {
        WorldInfo.Size = new Vector2Int(10, 10);
        Map.transform.position = new Vector3(WorldInfo.Size.x / 2 - .5f, 0, WorldInfo.Size.y / 2 - .5f);
        Map.transform.localScale = new Vector3(WorldInfo.Size.x / 10f, 1, WorldInfo.Size.y / 10f);
        MainCamera.transform.position = new Vector3(WorldInfo.Size.x / 2 - .5f, 5, WorldInfo.Size.y / 2 - .5f);
        MainCamera.GetComponent<Camera>().orthographicSize = Mathf.Max(WorldInfo.Size.x, WorldInfo.Size.y)/2f;
        Map.GetComponent<Renderer>().material.mainTextureScale = new Vector2(WorldInfo.Size.x, WorldInfo.Size.y);
        //Escalar Cámara
        if (WorldInfo.ManualObstacles) PaintingObstacles = true;
        if (WorldInfo.ManualObjectives) SettingBeginning = true;
    }

    public void Update()
    {
        if (AllDone) SceneManager.LoadScene("Simulation");
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("ClickDown");
            locked = true;
            if (PaintingObstacles) StartCoroutine(PaintObstacles());
            else if (SettingBeginning) StartCoroutine(DragStart());
            else StartCoroutine(DragEnd());
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("ClickUp");
            locked = false;
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
        }/**/
    }
    public void Forward()
    {
        locked = false;
        if (PaintingObstacles)
        {
            PaintingObstacles = false;
            if (!WorldInfo.ManualObjectives) AllDone = true;
        }
        else if (SettingBeginning && WorldInfo.Beginning != new Vector2Int(-1, -1)) SettingBeginning = false;
        else if (WorldInfo.ManualObjectives && WorldInfo.Beginning != new Vector2Int(-1, -1)) AllDone = true;
    }

    public void GoBack()
    {
        locked = false;
        if (SettingBeginning) PaintingObstacles = true;
        else SettingBeginning = true;
    }

    public IEnumerator PaintObstacles()
    {
        GameObject obj;
        if (Physics.Raycast(ray, out hit, 100.0f))
        {
            Debug.Log("{" + hit.point.x + ", " + hit.point.y + ", " + hit.point.z + "}");
            Coordinates = new Vector2Int(Mathf.RoundToInt(hit.point.x), Mathf.RoundToInt(hit.point.z));
            if (WorldInfo.Obstacles.Contains(Coordinates))
            {
                obj = ProvisionalObstacles[WorldInfo.Obstacles.IndexOf(Coordinates)];
                Destroy(obj);
                ProvisionalObstacles.Remove(obj);
                WorldInfo.Obstacles.Remove(Coordinates);
                while (locked)
                {
                    yield return null;
                    if (Physics.Raycast(ray, out hit, 100.0f))
                    {
                        Debug.Log("locked removing {" + hit.point.x + ", " + hit.point.y + ", " + hit.point.z + "}");
                        Coordinates = new Vector2Int(Mathf.RoundToInt(hit.point.x), Mathf.RoundToInt(hit.point.z));
                        if (WorldInfo.Obstacles.Contains(Coordinates) && Coordinates != WorldInfo.Beginning && Coordinates != WorldInfo.End)
                        {
                            obj = ProvisionalObstacles[WorldInfo.Obstacles.IndexOf(Coordinates)];
                            Destroy(obj);
                            ProvisionalObstacles.Remove(obj);
                            WorldInfo.Obstacles.Remove(Coordinates);
                        }
                    }
                }
            }
            else if (WorldInfo.Obstacles.Count < (WorldInfo.Size.x * WorldInfo.Size.y) - 1 && Coordinates != WorldInfo.Beginning && Coordinates != WorldInfo.End)
            {
                obj = Instantiate(ObstaclePrefab, new Vector3(Coordinates.x, .3f, Coordinates.y), Quaternion.identity);
                ProvisionalObstacles.Add(obj);
                WorldInfo.Obstacles.Add(Coordinates);
                while (locked && WorldInfo.Obstacles.Count < (WorldInfo.Size.x * WorldInfo.Size.y) - 1)
                {
                    yield return null;
                    if (Physics.Raycast(ray, out hit, 100.0f))
                    {
                        Debug.Log("locked adding {" + hit.point.x + ", " + hit.point.y + ", " + hit.point.z + "}");
                        Coordinates = new Vector2Int(Mathf.RoundToInt(hit.point.x), Mathf.RoundToInt(hit.point.z));
                        if (!WorldInfo.Obstacles.Contains(Coordinates) && Coordinates != WorldInfo.Beginning && Coordinates != WorldInfo.End)
                        {
                            obj = Instantiate(ObstaclePrefab, new Vector3(Coordinates.x, .3f, Coordinates.y), Quaternion.identity);
                            ProvisionalObstacles.Add(obj);
                            WorldInfo.Obstacles.Add(Coordinates);
                        }
                    }
                }
            }
        }
        yield return null;
    }
    public IEnumerator DragStart()
    {
        if (Physics.Raycast(ray, out hit, 100.0f))
        {
            Debug.Log("{" + hit.point.x + ", " + hit.point.y + ", " + hit.point.z + "}");
            Coordinates = new Vector2Int(Mathf.RoundToInt(hit.point.x), Mathf.RoundToInt(hit.point.z));
            if (WorldInfo.Beginning == Coordinates)
            {
                //Delete
                Destroy(ProvisionalBeginning);
                WorldInfo.Beginning = new Vector2Int(-1, -1);
            }
            else if (!WorldInfo.Obstacles.Contains(Coordinates))
            {
                if (ProvisionalBeginning == null) ProvisionalBeginning = Instantiate(BeginningPrefab);
                WorldInfo.Beginning = Coordinates;
                ProvisionalBeginning.transform.position = new Vector3(Coordinates.x, 0.3f, Coordinates.y);
                while (locked)
                {
                    yield return null;
                    if (Physics.Raycast(ray, out hit, 100.0f))
                    {
                        Coordinates = new Vector2Int(Mathf.RoundToInt(hit.point.x), Mathf.RoundToInt(hit.point.z));
                        if (!WorldInfo.Obstacles.Contains(Coordinates))
                        {
                            WorldInfo.Beginning = Coordinates;
                            ProvisionalBeginning.transform.position = new Vector3(Coordinates.x, 0.3f, Coordinates.y);
                        }
                    }
                    //else locked = false;
                }
            }
        }
        yield return null;
    }
    public IEnumerator DragEnd()
    {
        if (Physics.Raycast(ray, out hit, 100.0f))
        {
            Debug.Log("{" + hit.point.x + ", " + hit.point.y + ", " + hit.point.z + "}");
            Coordinates = new Vector2Int(Mathf.RoundToInt(hit.point.x), Mathf.RoundToInt(hit.point.z));
            if (WorldInfo.End == Coordinates)
            {
                //Delete
                Destroy(ProvisionalEnd);
                WorldInfo.End = new Vector2Int(-1, -1);
            }
            else if (!WorldInfo.Obstacles.Contains(Coordinates))
            {
                if (ProvisionalEnd == null) ProvisionalEnd = Instantiate(EndPrefab);
                WorldInfo.End = Coordinates;
                ProvisionalEnd.transform.position = new Vector3(Coordinates.x, 0.3f, Coordinates.y);
                while (locked)
                {
                    yield return null;
                    if (Physics.Raycast(ray, out hit, 100.0f))
                    {
                        Coordinates = new Vector2Int(Mathf.RoundToInt(hit.point.x), Mathf.RoundToInt(hit.point.z));
                        if (!WorldInfo.Obstacles.Contains(Coordinates))
                        {
                            WorldInfo.End = Coordinates;
                            ProvisionalEnd.transform.position = new Vector3(Coordinates.x, 0.3f, Coordinates.y);
                        }
                    }
                    //else locked = false;
                }
            }
        }
        yield return null;
    }
}
