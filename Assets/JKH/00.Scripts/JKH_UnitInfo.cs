using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JKH_UnitInfo : MonoBehaviour
{

    #region read
    //※확장팩 안넣음※
    //참고사이트
    //https://namu.wiki/w/%EB%AC%B8%EB%AA%85%206/%EC%9C%A0%EB%8B%9B#s-3
    //https://civilization.fandom.com/wiki/List_of_units_in_Civ6
    #endregion

    //unit's Info
    public int hp; 
    public int moveCount; 
    public int attackDamage;

    //resource
    public int gold;
    public int wood;

    public Eras eras;
    //시대
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

    //군대(필요없을듯? 있나? 아님말고)
    enum Militaries
    {
        Army,
        Navy,
        AirForce
        
    }

    #region 병종

    // EraNum/ Name 

    //정찰계열
    enum Recons
    {
        scout, //0 정찰병
        ranger //4 레인저
    }


    //근접계열
    enum Melee
    {
        warrior, //0 전사
        swordsman, //1 검사-> 시대2_전사
        manAtArns, //2 중갑병 ->시대3_전사
        musketman, //3 머스킷병
        lineInfantry, //4 전열보병
        infantry, //5 보병
        merchanizedInfantry //7 기계화보병
    }

    //원거리계열
    enum Range
    {
        slinger, //0 투석병
        Archer, //0 궁수
        Crossbowman, //2 석궁병
        fieldCannon, //4 전장포
        MachineGun //6 기관총
    }

    //대(anti) 기병
    enum AntiCavalry
    {
        spearman, //0 창병
        pikeman, //2 파이크병
        ATCrew, //5 대전차병
        modernAT //7 현대 대전차병
    }

    //경기병
    enum lightCavalry
    {
        horseMan, //1 기마병
        cavalry, //4 기병대
        helicopter //6 헬리콥터
    }

    //중기병
    enum heavyCavalry
    {
        heavyCavalry, //0 중전차
        knight, //2 기사
        tank, //5 탱크
        modernArmor //7 현대 전차
    }

    //공성
    enum Siege
    {
        catapult, //1 캐터펄트
        trebuchet, //2 트레뷰셋
        bombard, //3 사석포
        artillery, //5 야포
        rocketArtillery //7 로켓포
    }
    
    //거대 전투로봇(GDR) 확장팩

    //근접해군
    enum navalMelee
    {
        galley, //0 갤리
        caravel, //3 캐러벨
        ironclad, //4 철갑함
        destroyer //6 구축함
    }

    //원거리해군
    enum navalRanged
    {
        quadrireme, //1 사단노선
        frigate, //3 프리깃
        battleship, //5 전함
        missileCruiser //7 미사일순양함
    }

    //약탈(해군 레이더)
    enum navalRaider
    {
        privateer, //3 사략선
        submarine, //5 잠수함
        nuclearSubmarine //7 핵잠수함
    }

    //항공모함
    enum navalCarrier
    {
        airCraftCarrier //6 항공모함
    }

    //전투기
    enum airFighter
    {
        biplane, //5 복엽기
        fighter, //6 전투기
        jetFighter //7 제트 전투기
    }

    //폭격기
    enum airBomber
    {
        bomber, //6 폭격기
        jetBomber //7 제트 폭격기
    }
    
    //지원
    enum support
    {
        batteringRam, //0 공성추
        SiegeTower, //1 공성탑
        militaryEngineer, //2 공병
        medic, //3 의무병
        observationBalloon, //4 관측용 열기구
        antiAirGun, //5 대공포
        mobileSAM //6 지대공미사일
    }

    #endregion



    public enum 병종
    {
        정찰,
        근접,
        원거리,
    }

    public enum 시대
    {
        고대,
        고전,
        중세,
    }

   
    //@@@@@@@@@@@@@@@@@@
    void Start()
    {
        
    }
    
    void Update()
    {
        
    }       

    
}
