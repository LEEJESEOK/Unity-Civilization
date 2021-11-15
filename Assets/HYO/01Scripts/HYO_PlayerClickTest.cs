using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HYO_PlayerClickTest : MonoBehaviour
{
    public GameObject emptyPre;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (emptyPre == null)
        {
            return;
        }
        // ���� ���콺 ��ư�� ������
        if (Input.GetMouseButtonDown(0))
        {
            // �������� ã�Ƴ���ʹ�.
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // ã�� ��ü�� TerrainData������Ʈ�� �����ͼ� 
                //arrayX = hit.transform.GetComponent<TerrainData>().x;
                //arrayY = hit.transform.GetComponent<TerrainData>().y;
                //terrainData.SetIndex(arrayX, arrayY);

                GameObject empty = Instantiate(emptyPre);
                empty.transform.parent = hit.transform;
                empty.transform.position = hit.transform.position;
                empty.transform.localPosition = new Vector3(0.208f, 0.135f, 0.255f);
                empty.transform.localEulerAngles = new Vector3(90, 0, 0);
                empty.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                FacilityData fd = hit.transform.GetComponent<FacilityData>();
                empty.GetComponent<SpriteRenderer>().sprite = HYO_ConstructManager.instance.icons[fd.iconNum];
            }
        }
    }
}
