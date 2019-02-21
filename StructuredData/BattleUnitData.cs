using UnityEngine;
using System.Collections;
using CodeStage.AntiCheat.ObscuredTypes;

[System.Serializable]
/// <summary>전투 유닛 데이터</summary>
public class BattleUnitData
{
    /// <summary>덱인덱스</summary>
    public int Index;
    /// <summary>배틀 유닛 ID</summary>
    public int UnitID;
    /// <summary>배틀 유닛 레벨</summary>
    int _Level;
    public int Level
    {
        get{ return _Level; }
        set{ _Level = value;}
    }

    public string Name
    {
        get{ return UnityDataConnector.Instance.m_TableData_Unit.GetUnitName(UnitID); }
    }

    /// <summary>카드수량</summary>
    public int CardCount;

    public bool IsFindCard = false;

}

