using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public int playerId;

    public bool isSelected;
    public InGameObjectId unitType;

    //Unit's stat
    public float movePower = 5;
    public float maxMovePower = 5;
    public int hp = 100;

    //unit tilePos
    public GameObject myTilePos;
    public int posX, posY;

    //animator
    public Animator animator;

    protected virtual void Start()
    {
        CheckMyPos();
    }

    protected virtual void Update()
    {

    }

    public void SetObjectColor()
    {
        List<MeshRenderer> mesh = new List<MeshRenderer>(GetComponentsInChildren<MeshRenderer>(true));
        List<SkinnedMeshRenderer> skinnedMesh = new List<SkinnedMeshRenderer>(GetComponentsInChildren<SkinnedMeshRenderer>(true));

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
            myTilePos = hit.transform.gameObject;

            posX = myTilePos.GetComponent<TerrainData>().x;
            posY = myTilePos.GetComponent<TerrainData>().y;
        }
    }

    private void OnDestroy()
    {
        if (GameManager.instance != null)
            GameManager.instance.DestroyUnit(playerId, gameObject);
    }
}
