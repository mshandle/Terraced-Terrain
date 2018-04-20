using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimTerrain;

[ExecuteInEditMode]
public class LayerTerrain : MonoBehaviour
{

    public LayerTerrainObject configObject;
    private List<SimTerrain.TerrainGroup> ListGroups = new List<SimTerrain.TerrainGroup>();
    public TerrainBrush brush = new TerrainBrush();
    List<TerrainChunk> colliedChunks = new List<TerrainChunk>();

    [SerializeField]
    Material terrainMatrial = null;
    private void Awake()
    {
        gameObject.layer = 8;
        Refresh();
    }

    private void Start()
    {
        // yield return new WaitForSecondsRealtime(3.0f);
        // Debug.LogError("Over");

    }




    private bool Touched = false;
    private Vector2 lastMousePoint;
    private bool FirstTouch = false;
    private Vector3 HitPoint = new Vector3();

    private bool showGim = false;
    private Vector3 LastTouchPoint = new Vector3();
    private Vector3 FirstHit = new Vector3();
    private bool hasCulateBrushType = false;
    private void CheckMouseEvent()
    {
        //Debug.Log("CheckMouseEvent");
        if (Input.GetMouseButtonDown(0) && FirstTouch == false)
        {
            Touched = true;
            FirstTouch = true;
            BrushTerrainBegin();
            hasCulateBrushType = false;
            lastMousePoint = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            lastMousePoint = Input.mousePosition;

            RaycastHit hitinfo;
            Vector3 hitPoint = default(Vector3);
            if (Physics.Raycast(ray, out hitinfo))
            {
                hitPoint = hitinfo.point;
                FirstHit = hitinfo.point;
            }

            brush.brushType = TerrainBrush.BrushType.LOCK;
            int layer = Mathf.RoundToInt (hitinfo.point.y / configObject.GetGroups.chunk.LayerHeight);
            brush.Targetheight = layer + 1;
            brush.Radius = 1.0f;
            brush.Strenght = 0.2f;
            brush.BlurPower = 1.0f;
            brush.BlurRadius = 6.0f;
            return;
        }

        if (Input.GetMouseButtonUp(0))
        {
            Touched = false;
            FirstTouch = false;
            hasCulateBrushType = false;
            BrushTerrainEnd();
            
        }

        if (Touched)
        {

            float distance = Vector3.Distance(Input.mousePosition, lastMousePoint);
            if (distance < 0.2)
            {
                return;
            }

            lastMousePoint = Input.mousePosition;

            Ray ray2 = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (hasCulateBrushType == false)
            {
                RaycastHit hitinfo;
                if (Physics.Raycast(ray2, out hitinfo))
                {
                    Vector3 CurPoint = hitinfo.point;
                    Vector3 dir = CurPoint - FirstHit;
                    dir.y = 0;
                    dir = dir.normalized;
                    Vector3 targetPoint = new Vector3();
                    int curLayer = (int)(brush.Targetheight) - 1;
                    if (dir != Vector3.zero)
                    {
                        for (int lenght = 2; lenght <= 5; lenght++)
                        {
                            targetPoint = CurPoint + (dir * lenght);
                            ray2.direction = targetPoint - Camera.main.transform.position;
                            if (Physics.Raycast(ray2, out hitinfo))
                            {
                                int layer = Mathf.RoundToInt(hitinfo.point.y / configObject.GetGroups.chunk.LayerHeight);
                                if (layer != curLayer)
                                {
                                    if (layer > curLayer)
                                    {
                                        brush.brushType = TerrainBrush.BrushType.BT_DOWN;
                                    }
                                    else {
                                        brush.brushType = TerrainBrush.BrushType.LOCK;
                                    }
                                    hasCulateBrushType = true;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        return;
                    }
                }

            }

            BrushTerrain(ray2);
        }
      

    }


    // Update is called once per frame
    void Update()
    {

        CheckMouseEvent();

    }

    public void Refresh()
    {
        colliedChunks.Clear();
        ListGroups.Clear();
        for (int idx = 0; idx < transform.childCount;)
        {
            GameObject obj = transform.GetChild(idx).gameObject;
            GameObject.DestroyImmediate(obj);
        }
        transform.DetachChildren();
        if (configObject == null)
            Debug.LogError("LayerTerrain Config Object null");
        LayerTerrainGroups groups = configObject.GetGroups;
        for (int idx = 0; idx < groups.datas.Count; ++idx)
        {
            groups.datas[idx].Refresh();
            string name = groups.datas[idx].name;
            if (string.IsNullOrEmpty(name))
            {
                name = "Group" + idx.ToString();
            }
            GameObject group = new GameObject(name);
            group.layer = 8;
            group.transform.parent = transform;
            group.transform.localPosition = groups.datas[idx].worldPositon;
            SimTerrain.TerrainGroup groupCm = group.AddComponent<SimTerrain.TerrainGroup>();
            groupCm.hideFlags = HideFlags.HideAndDontSave;
            groupCm.Init(configObject.GetColors, groups.chunk, groups.datas[idx], terrainMatrial);
            //ListGroups.Add(groupCm);
        }

    }

    private void OnDrawGizmos()
    {
        if (false)
        {
            //Gizmos.color = Color.blue;
            // Gizmos.DrawSphere(HitPoint, brush.Radius);
        }

    }


    public void BrushTerrainBegin()
    {
        //TODO:
        configObject.GetGroups.datas[0].ClearFilterFlag();
        Debug.Log("Brush Begin");
    }

    public void BrushTerrain(Ray ray)
    {
        showGim = true;
        for (int idx = 0; idx < transform.childCount; ++idx)
        {
            // ListGroups[idx].CastGroup(ray);
            SimTerrain.TerrainGroup group = transform.GetChild(idx).GetComponent<SimTerrain.TerrainGroup>();
            group.CastGroup(ray, brush, colliedChunks, out HitPoint);
        }
    }

    public void BrushTerrainEnd()
    {
        //if(colliedChunks)
        for (int idx = 0; idx < colliedChunks.Count; ++idx)
        {
            //BrushChunkEnd
            colliedChunks[idx].BrushChunkEnd();
        }
        showGim = false;
    }

    public bool TouchInTerrain(Ray ray, out int layerIndex)
    {
        for (int idx = 0; idx < transform.childCount; ++idx)
        {
            // ListGroups[idx].CastGroup(ray);
            SimTerrain.TerrainGroup group = transform.GetChild(idx).GetComponent<SimTerrain.TerrainGroup>();
            if (group && group.TouchInGround(ray, out layerIndex))
                return true;
        }
        layerIndex = 0;
        return false;
    }

    public void saveData()
    {
        LayerTerrainGroups groups = configObject.GetGroups;
        for (int idx = 0; idx < groups.datas.Count; ++idx)
        {
            groups.datas[idx].SaveData();
        }
    }
}
