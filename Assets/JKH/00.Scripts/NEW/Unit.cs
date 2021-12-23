using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public int playerId;

    public bool isSelected;

    //Unit's stat
    public int movePower = 2;
    public int hp = 100;

    virtual protected void Start()
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

    void Update()
    {

    }

}
