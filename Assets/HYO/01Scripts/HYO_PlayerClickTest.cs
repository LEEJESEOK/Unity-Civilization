using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HYO_PlayerClickTest : MonoBehaviour
{
    HYO_ConstructManager constrMng;

    private void Start()
    {
        constrMng = HYO_ConstructManager.instance;
    }

    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject empty = Instantiate(constrMng.emptyPre);
                int chooseIndex = -1;
                for (int i = 0; i < constrMng.districtOn_.Length; i++)
                {
                    if (constrMng.districtOn_[i] = null)
                    {
                        constrMng.districtOn_[i] = empty;
                        chooseIndex = i;
                        break;
                    }
                }
                if (chooseIndex == -1)
                {
                    return;
                }
                empty.transform.parent = hit.transform;
                empty.transform.position = hit.transform.position;
                empty.transform.localPosition = constrMng.iconPos[chooseIndex];
                empty.transform.localEulerAngles = new Vector3(90, 0, 0);
                empty.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                FacilityData fd = hit.transform.GetComponent<FacilityData>();
                empty.GetComponent<SpriteRenderer>().sprite = HYO_ConstructManager.instance.icons[fd.iconNum];
            }
        }
    }
}
