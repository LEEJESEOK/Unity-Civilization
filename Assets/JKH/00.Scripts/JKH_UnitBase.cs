using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Eras
{
    Ancient, //��� 0
    Classical, //���� 1
    Medieval, //�߼� 2
    Renaissance, //���׻� 3
    Industrial, //��� 4
    Modern, //���� 5
    Atomic, //���� 6
    Information, //���� 7
    Future //�̷� X
}

public enum Militaries
{
    racons, //����
    melee, //����
    range, //���Ÿ�
    heavyCavalry, //�߱⺴
    lightCavalry, //��⺴
    AntiCavalry, //�� �⺴
    Siege, //����
    navalMelee, //�����ر�
    navalRanged, //���Ÿ��ر�
    navalRaider, //��Ż(�ر����̴�)
    navalCarrier, //�װ�����
    airFighter, //������
    airBomber, //���ݱ�
    support //����

}



public class JKH_UnitBase : MonoBehaviour
{
    public Eras era;
    public Militaries type;
    
    //unit's Info
    public int hp;
    public int moveCount;
    public int attackDamage;

    

    // Add Ally and Enemy

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {

    }


}
