using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Eras
{
    Ancient, //고대 0
    Classical, //고전 1
    Medieval, //중세 2
    Renaissance, //르네상스 3
    Industrial, //산업 4
    Modern, //현대 5
    Atomic, //원자 6
    Information, //정보 7
    Future //미래 X
}

public enum Militaries
{
    racons, //정찰
    melee, //근접
    range, //원거리
    heavyCavalry, //중기병
    lightCavalry, //경기병
    AntiCavalry, //대 기병
    Siege, //공성
    navalMelee, //근접해군
    navalRanged, //원거리해군
    navalRaider, //약탈(해군레이더)
    navalCarrier, //항공모함
    airFighter, //전투기
    airBomber, //폭격기
    support //지원

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
