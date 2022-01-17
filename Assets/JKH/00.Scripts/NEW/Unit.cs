using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{

    [SerializeField]
    GameObject plane;
    [SerializeField]
    GameObject body;

    public int playerId;

    public bool isSelected;
    public InGameObjectId unitType;

    //Unit's stat
    public float movePower;
    public float maxMovePower = 2;
    public int hp = 100;

    //unit tilePos
    public GameObject myTilePos;
    public int posX, posY;

    //animator
    public Animator animator;

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();

        CheckMyPos();
    }

    protected virtual void Update()
    {

    }

    public void SetObjectColor()
    {
        Material planeMat = plane.GetComponent<MeshRenderer>().material;
        planeMat.color = ColorManager.instance.playerColor[playerId];
        plane.GetComponent<MeshRenderer>().material = planeMat;

        List<MeshRenderer> mesh = new List<MeshRenderer>(body.GetComponentsInChildren<MeshRenderer>(true));
        List<SkinnedMeshRenderer> skinnedMesh = new List<SkinnedMeshRenderer>(body.GetComponentsInChildren<SkinnedMeshRenderer>(true));

        for (int i = 0; i < mesh.Count; i++)
        {
            Material mat = ColorManager.instance.unitMat[playerId];
            mesh[i].material = mat;
        }

        for (int i = 0; i < skinnedMesh.Count; ++i)
        {
            Material mat = ColorManager.instance.unitMat[playerId];
            skinnedMesh[i].material = mat;
        }
    }

    //이동하면 검사
    public void CheckMyPos()
    {
        int fogLayer = LayerMask.GetMask("HexFog");

        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.up * -1);
        Debug.DrawRay(transform.position, transform.up * -1, Color.red);
        if (Physics.Raycast(ray, out hit, 1, ~fogLayer))
        {
            if (myTilePos != null)
            {
                myTilePos.GetComponent<TerrainData>().objectOn.Remove(gameObject);
            }

            myTilePos = hit.transform.gameObject;
            Vector3 pos = hit.transform.position;
            pos.y = transform.position.y;
            transform.position = pos;

            posX = myTilePos.GetComponent<TerrainData>().x;
            posY = myTilePos.GetComponent<TerrainData>().y;

            //object on 에 유닛 추가
            myTilePos.GetComponent<TerrainData>().AddObjectOn(gameObject, playerId);
        }

    }

    private void OnDestroy()
    {
        if (GameManager.instance != null)
            GameManager.instance.DestroyUnit(playerId, this);
        
        //if(HexFogManager.instance!=null)
        //    HexFogManager.instance.

        if (MapManager.instance != null)
        {
            MapManager.instance.InitMoveArea();

            MapManager.instance.terrainDataMap[posX + (posY * MapManager.instance.mapWidth)].objectOn.Remove(gameObject);
        }

        if (HexFogManager.instance != null)
        {

            while (HexFogManager.instance.prevInFov.Find(x => x == gameObject))
                HexFogManager.instance.prevInFov.Remove(gameObject);
            while (HexFogManager.instance.inFov.Find(x => x == gameObject))
                HexFogManager.instance.inFov.Remove(gameObject);
        }
    }
}
