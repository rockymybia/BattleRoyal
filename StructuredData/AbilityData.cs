using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;

//유닛 능력치 클래스 Obscured
public class AbilityData
{
    public ObscuredInt Index;
    public string UnitName;
    public UnitType type;
    public ObscuredInt Level;
    public ObscuredInt HP_Current;
    public ObscuredInt HP_Max;
    //public ObscuredInt MP_Current;
    //public ObscuredInt MP_Max;
    public ObscuredInt P_Atk;
    public ObscuredInt P_Def;
    public ObscuredInt M_Atk;
    public ObscuredInt M_Def;
    public ObscuredFloat CriticalRate;
    public ObscuredFloat EvadeRate;
    public ObscuredFloat MoveSpeed;
    public ObscuredFloat AttackSpeed;
    public ObscuredFloat SpawnTime;
    public ObscuredFloat AttackRange;
    public ObscuredFloat AttackDelay;
    public AttackType AttackTarget;
    /// <summary>지상유닛 공격가능 여부</summary>
    public ObscuredBool IsAttackGround;
    /// <summary>공중유닛 공격가능 여부</summary>
    public ObscuredBool IsAttackSky;
    /// <summary>타워 공격 가능 여부</summary>
    public ObscuredBool IsAttackTower;

}