using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMapTile : MonoBehaviour
{
    public bool makeMap;
    public int width = 50;
    public int length = 50;
    public Vector2 tileSize = new Vector2(1, 1);
    public Vector2 gap = new Vector2(0, 0);
    public GameObject tileFactory;
    //Dictionary<string, GameObject> tiles = new Dictionary<string, GameObject>();
    List<GameObject> tiles = new List<GameObject>();

    public List<Material> tileMaterial;

    void Start()
    {
        if (makeMap)
        {
            for (int z = 0; z < width; z++)
            {
                for (int x = 0; x < length; x++)
                {
                    GameObject tile = Instantiate(tileFactory);

                    if (z == 0 || z == length - 1)
                    {
                        Material[] materials = tile.transform.GetComponent<MeshRenderer>().materials;
                        materials[0] = tileMaterial[0];
                        tile.GetComponent<MeshRenderer>().materials = materials;
                    }
                    tile.AddComponent<TerrainData>();
                    tile.GetComponent<TerrainData>().SetIndex(x, z);

                    tiles.Add(tile);
                    tile.transform.parent = transform;
                    MeshCollider col = tile.transform.gameObject.AddComponent<MeshCollider>();
                    col.convex = true;
                }
            }
        }
        else
        {
            for (int i = 0; i < transform.childCount; ++i)
            {
                tiles.Add(transform.GetChild(i).gameObject);
            }

        }
    }


    void Update()
    {
        float cx = -width * tileSize.x * 0.5f;
        float cz = -length * tileSize.y * 0.5f;
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < length; z++)
            {
                GameObject tile = tiles[(x + z * width)];
                float fz = z;
                if (x % 2 == 1)
                {
                    fz += tileSize.y * 0.5f;
                }
                tile.transform.localPosition = new Vector3(cx + x + gap.x * x, 0, cz + fz + gap.y * z);

            }
        }
    }
}
