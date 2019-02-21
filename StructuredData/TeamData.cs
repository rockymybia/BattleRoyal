using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;


[System.Serializable]
/// <summary>팀 데이터</summary>
public class TeamData
{
    #region Members
    /// <summary>팀 인덱스</summary>
    public int TeamIndex;

    /// <summary>전투중? True</summary>
    public bool BattleState = false;

    bool IsChargingBattleCost = false;
    ObscuredFloat ChargingDelayTime = 3f;
    ObscuredFloat Elapsedtime = 0f;

    public ObscuredInt BattleCost;
    ObscuredInt DEFAULT_BATTLECOST = 1;
    ObscuredInt ADD_BATTLECOST = 1;
    ObscuredInt MAXIMUM_BATTLECOST = 10;
    ObscuredInt MAXIMUM_DeckLength = 10;

    
    //Dictionary<int, List<BattleUnitData>> Dic_BattleDecks = new Dictionary<int, List<BattleUnitData>>();

    List<BattleUnitData> BattleDeckDataList_ = new List<BattleUnitData>();

    public List<BattleUnitData> BattleDeckDataList
    {
        get { return BattleDeckDataList_; }
        set { BattleDeckDataList_ = value; }
    }

    //현재 선택된 유닛의 덱 인덱스
    public int SelectedUnitID = 0;
    public BattleUnitData Current_SelectedBattleDeck;
    #endregion

    #region Functions
    public TeamData(int _index)
    {
        TeamIndex = _index;
    }

    public bool CheckBattleCost(int _consumeCost)
    {
        return _consumeCost <= BattleCost ? true : false;
    }

    public void InitBattle()
    {
        BattleState = true;
        BattleCost = DEFAULT_BATTLECOST;
    }

    public void InitLobby()
    {
        BattleState = false;
    }

    public void AddBattleCost()
    {
        if (BattleCost >= MAXIMUM_BATTLECOST)
        {
            Debug.LogError(string.Format("{0}팀 {1}코스트 완충! ", TeamIndex, BattleCost));
            IsChargingBattleCost = false;
            Elapsedtime = 0f;
            return;
        }
        else
        {
            BattleCost += ADD_BATTLECOST;
            //Debug.LogError(string.Format("{0}팀 {1}코스트 충전! ", TeamIndex, BattleCost));
        }
    }

    public void ConsumeBattleCost(int _consumeCost)
    {
        if (CheckBattleCost(_consumeCost))
        {
            BattleCost -= _consumeCost;
        }
    }

    public void ChargingCost()
    {
        if (BattleCost < MAXIMUM_BATTLECOST)
            IsChargingBattleCost = true;

        if (IsChargingBattleCost == false)
            return;

        Elapsedtime += Time.deltaTime;
        if (Elapsedtime > ChargingDelayTime)
        {
            AddBattleCost();
            Elapsedtime = 0f;
        }
    }

    public void InitDeckinfo(int _DeckIndex, List<BattleUnitData> _unitList)
    {        
        //Dic_BattleDecks.Add(_DeckIndex, _unitList);
        BattleDeckDataList_.Clear();
        TeamIndex = _DeckIndex;
        BattleDeckDataList_ = _unitList;
    }

    public void SelectUnit(BattleUnitData _DeckData)
    {        
        //Current_SelectedBattleDeck = Dic_BattleDecks[SelectedBattleDeckIndex][_DeckIndex];
        try
        {
            //Current_SelectedBattleDeck = BattleDeckDataList_.Find(r => r.Index == _DeckIndex);
            Current_SelectedBattleDeck = _DeckData;
            SelectedUnitID = Current_SelectedBattleDeck.UnitID;
            Debug.LogError("SelectUnit : " + Current_SelectedBattleDeck.UnitID);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.ToString());            
            throw;
        }
    }

    public void UnSelect()
    {
        //Current_SelectedBattleDeck = null;
        SelectedUnitID = 0;
    }

    public int GetSelectedUnitId()
    {
        if (SelectedUnitID > 0)
        {
            return SelectedUnitID;
        }

        return 0;
    }
    #endregion
}
