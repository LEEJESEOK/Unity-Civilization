using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public GameObject tileFab;

    public GameObject center;
    public float jitter = 0.2f;

    public int mapWidth = 100, mapHeight = 100;
    public int typeCount = 6;


    // Start is called before the first frame update
    void Start()
    {
        LayerMask mapLayer = 0;
        mapLayer = mapLayer | LayerMask.GetMask("GrassLand");
        mapLayer = mapLayer | LayerMask.GetMask("Plains");
        mapLayer = mapLayer | LayerMask.GetMask("Desert");
        mapLayer = mapLayer | LayerMask.GetMask("Mountain");
        mapLayer = mapLayer | LayerMask.GetMask("Coast");
        mapLayer = mapLayer | LayerMask.GetMask("Ocean");



        Ray ray = new Ray(transform.position, transform.up);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, float.PositiveInfinity, mapLayer))
        {
            center = hit.transform.gameObject;
        }

        for (int i = 0; i < mapHeight; i++)
        {
            for (int j = 0; i < mapWidth; ++j)
            {
                GameObject tile = Instantiate(tileFab);
                TerrainData terrainData = tile.AddComponent<TerrainData>();


            }
        }
    }

}
