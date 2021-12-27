using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public float jitter = 0.2f;
    public Transform center;

    public int mapWidth = 100, mapHeight = 100;
    int mapSize;
    public int typeCount = 6;


    // Start is called before the first frame update
    void Start()
    {
        mapSize = mapWidth * mapHeight;

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
            center = hit.transform;
        }

        List<TerrainData> queue = new List<TerrainData>();
        List<Vector2> done = new List<Vector2>();
        queue.Add(center.GetComponent<TerrainData>());
        done.Add(new Vector2(queue[0].x, queue[0].y));

        while (queue.Count > 0 && done.Count == mapSize)
        {
            TerrainData current = queue[0];
            Collider[] neighbors = Physics.OverlapSphere(current.transform.position, 1, mapLayer);

            for (int i = 0; i < neighbors.Length; ++i)
            {
                TerrainData neighbor = neighbors[i].GetComponent<TerrainData>();
                if (done.Contains(new Vector2(neighbor.x, neighbor.y)))
                    continue;

                // jitter 값보다 작을 때 임의의 타일 속성 지정
                TerrainType randFeature = Random.value > jitter ? current.terrainType : (TerrainType)Random.Range(0, typeCount);
                neighbors[i].GetComponent<TerrainData>().terrainType = randFeature;
                done.Add(new Vector2(neighbor.x, neighbor.y));

                // 주변 타일 다음 목록에 추가
                queue.Add(neighbors[i].GetComponent<TerrainData>());
            }

        }
    }
}