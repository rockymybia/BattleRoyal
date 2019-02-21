using UnityEngine;
using System.Collections;



/// <summary>유닛 테이블</summary>
public class TableUnit
{
    /// <summary>인덱스</summary>
    public int Index;
    /// <summary>이름</summary>
    public string UnitName;
    /// <summary>타입</summary>
    public UnitType type;
    public int HP_Max;
    public int MP_Max;
    /// <summary>물리공격력</summary>
    public int P_Atk;
    /// <summary>물리방어력</summary>
    public int P_Def;
    /// <summary>마법공격력</summary>
    public int M_Atk;
    /// <summary>마법방어력</summary>
    public int M_Def;
    /// <summary>이동속도</summary>
    public float MoveSpeed;
    /// <summary>공격속도</summary>
    public float AttackSpeed;
    /// <summary>소환시간</summary>
    public float SpawnTime;
    /// <summary>공격범위</summary>
    public float AtttackRange;
    /// <summary>공격후딜레이</summary>
    public float AttackDelay;
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
    /// <summary>유닛크기</summary>
    public float UnitScale;
    /// <summary>배틀 코스트</summary>
    public int Cost;
    /// <summary>HpBar Position offset X</summary>
    public float HPBarOffSetX;
    /// <summary>HpBar Position offset Y</summary>
    public float HPBarOffSetY;
    /// <summary>발사체 아이디</summary>
    public int ProjectileIndex;
}