using UnityEngine;
using System.Collections;

/// <summary>건축물 테이블</summary>
public class TableStructure
{
    /// <summary>인덱스</summary>
    public int Index;
    /// <summary>이름</summary>
    public string StructureName;
    /// <summary>타입</summary>
    public UnitType Structuretype;
    public int HP_Max;
    /// <summary>물리공격력</summary>
    public int P_Atk;
    /// <summary>물리방어력</summary>
    public int P_Def;
    /// <summary>마법공격력</summary>
    public int M_Atk;
    /// <summary>마법방어력</summary>
    public int M_Def;
    /// <summary>공격속도</summary>
    public float AttackSpeed;
    /// <summary>공격범위</summary>
    public float AtttackRange;
    /// <summary>공격대상</summary>
    public AttackType AttackTarget;
    /// <summary>지상유닛 공격가능 여부</summary>
    public bool IsAttackGround;
    /// <summary>공중유닛 공격가능 여부</summary>
    public bool IsAttackSky;
    /// <summary>타워 공격 가능 여부</summary>
    public bool IsAttackTower;
    /// <summary>리소스이름</summary>
    public string ResourceName;
    /// <summary>HpBar Position offset X</summary>
    public float HPBarOffSetX;
    /// <summary>HpBar Position offset Y</summary>
    public float HPBarOffSetY;
    /// <summary>투사체 인덱스</summary>
    public int ProjectileIndex;
}