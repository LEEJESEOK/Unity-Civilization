using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMapTile : MonoBehaviour
{
    public int width = 50;
    public int length = 50;
    public Vector2 tileSize = new Vector2(1, 1);
    public Vector2 gap = new Vector2(0, 0);
    public GameObject tileFactory;
    Dictionary<string, GameObject> tiles = new Dictionary<string, GameObject>();

    public List<Material> tileMaterial;

    void Start()
    {
        for (int z = 0; z < width; z++)
        {
            for (int x = 0; x < length; x++)
            {
                GameObject tile = Instantiate(tileFactory);

                if(z == 0 || z == length -1)
                {
                    Material[] materials = tile.transform.GetChild(0).GetComponent<MeshRenderer>().materials;
                    materials[0] = tileMaterial[0];
                    tile.transform.GetChild(0).GetComponent<MeshRenderer>().materials = materials;
                }


                tiles.Add((x + z * width).ToString(), tile);
                tile.transform.parent = transform;
                MeshCollider col = tile.transform.GetChild(0).gameObject.AddComponent<MeshCollider>();
                col.convex = true;
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
                GameObject tile = tiles[(x + z * width).ToString()];
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
