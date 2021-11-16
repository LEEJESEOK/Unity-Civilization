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

public enum UnitType
{
    Racons = TypeIdBase.UNIT, //정찰
    Melee, //근접
    Range, //원거리
    HeavyCavalry, //중기병
    LightCavalry, //경기병
    AntiCavalry, //대 기병
    Siege, //공성
    NavalMelee, //근접해군
    NavalRanged, //원거리해군
    NavalRaider, //약탈(해군레이더)
    NavalCarrier, //항공모함
    AirFighter, //전투기
    AirBomber, //폭격기
    Support, //지원

    Builder,
    Settler,
}



public class JKH_UnitBase : MonoBehaviour
{
    public Eras era;
    public UnitType type;

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
