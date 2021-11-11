using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JKH_UnitInfo : MonoBehaviour
{

    #region read
    //��Ȯ���� �ȳ�����
    //�������Ʈ
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
    //�ô�
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

    //����(�ʿ������? �ֳ�? �ƴԸ���)
    enum Militaries
    {
        Army,
        Navy,
        AirForce
            // Warrior
            //Scout
            //
    }

    #region ����

    // EraNum/ Name 

    //�����迭
    enum Recons
    {
        scout, //0 ������
        ranger //4 ������
    }


    //�����迭
    enum Melee
    {
        warrior, //0 ����
        swordsman, //1 �˻�-> �ô�2_����
        manAtArns, //2 �߰��� ->�ô�3_����
        musketman, //3 �ӽ�Ŷ��
        lineInfantry, //4 ��������
        infantry, //5 ����
        merchanizedInfantry //7 ���ȭ����
    }

    //���Ÿ��迭
    enum Range
    {
        slinger, //0 ������
        Archer, //0 �ü�
        Crossbowman, //2 ���ú�
        fieldCannon, //4 ������
        MachineGun //6 �����
    }

    //��(anti) �⺴
    enum AntiCavalry
    {
        spearman, //0 â��
        pikeman, //2 ����ũ��
        ATCrew, //5 ��������
        modernAT //7 ���� ��������
    }

    //��⺴
    enum lightCavalry
    {
        horseMan, //1 �⸶��
        cavalry, //4 �⺴��
        helicopter //6 �︮����
    }

    //�߱⺴
    enum heavyCavalry
    {
        heavyCavalry, //0 ������
        knight, //2 ���
        tank, //5 ��ũ
        modernArmor //7 ���� ����
    }

    //����
    enum Siege
    {
        catapult, //1 ĳ����Ʈ
        trebuchet, //2 Ʈ�����
        bombard, //3 �缮��
        artillery, //5 ����
        rocketArtillery //7 ������
    }
    
    //�Ŵ� �����κ�(GDR) Ȯ����

    //�����ر�
    enum navalMelee
    {
        galley, //0 ����
        caravel, //3 ĳ����
        ironclad, //4 ö����
        destroyer //6 ������
    }

    //���Ÿ��ر�
    enum navalRanged
    {
        quadrireme, //1 ��ܳ뼱
        frigate, //3 ������
        battleship, //5 ����
        missileCruiser //7 �̻��ϼ�����
    }

    //��Ż(�ر� ���̴�)
    enum navalRaider
    {
        privateer, //3 �緫��
        submarine, //5 �����
        nuclearSubmarine //7 �������
    }

    //�װ�����
    enum navalCarrier
    {
        airCraftCarrier //6 �װ�����
    }

    //������
    enum airFighter
    {
        biplane, //5 ������
        fighter, //6 ������
        jetFighter //7 ��Ʈ ������
    }

    //���ݱ�
    enum airBomber
    {
        bomber, //6 ���ݱ�
        jetBomber //7 ��Ʈ ���ݱ�
    }
    
    //����
    enum support
    {
        batteringRam, //0 ������
        SiegeTower, //1 ����ž
        militaryEngineer, //2 ����
        medic, //3 �ǹ���
        observationBalloon, //4 ������ ���ⱸ
        antiAirGun, //5 �����
        mobileSAM //6 ������̻���
    }

    #endregion



    public enum ����
    {
        ����,
        ����,
        ���Ÿ�,
    }

    public enum �ô�
    {
        ���,
        ����,
        �߼�,
    }

   
    
    void Start()
    {
        
    }
    
    void Update()
    {
        
    }       

    
}
