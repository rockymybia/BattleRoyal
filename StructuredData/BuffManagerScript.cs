using System;
using System.Collections.Generic;
using UnityEngine;

public class BuffManagerScript : MonoBehaviour
{
    public class BuffDataSet
    {
        public BuffType BfType;
        public bool IsActive;
        public int level;
        public float remainTime;
        public float DurationTime;
        public int MaxHpUp;
        public int PAtkUp;
        public int PDefUp;
        public int MAtkUp;
        public int MDefUp;
        public float EvdUp;
        public float CriUp;
        public float AtkSpeedUp;
        public Action BuffActionCalculate;

        public BuffDataSet(int m_index, int m_level, Action funcCalc)
        {
            TableData_Consume ConsumeData = UnityDataConnector.Instance.m_TableData_Consume;
            IsActive = false;
            level = m_level;
            remainTime = 0;
            DurationTime = ConsumeData.GetDuration(m_index, m_level);
            MaxHpUp = ConsumeData.GetMaxHpUp(m_index, m_level);
            PAtkUp = ConsumeData.GetPAtkUp(m_index, m_level);
            PDefUp = ConsumeData.GetPDefUp(m_index, m_level);
            EvdUp = ConsumeData.GetEvdUp(m_index, m_level);
            CriUp = ConsumeData.GetCriUp(m_index, m_level);
            AtkSpeedUp = ConsumeData.GetAtkSpeedUp(m_index, m_level);
            BuffActionCalculate = funcCalc;
        }

        public void BuffActivate(bool isOn)
        {
            IsActive = isOn;
            remainTime = (IsActive == true) ? DurationTime : 0f;

            ExcuteBuffCalc();
        }

        public void ExcuteBuffCalc()
        {
            if (BuffActionCalculate != null)
                BuffActionCalculate();
        }

        public void TimeUpdate()
        {
            //if (IsActive == false)
            //    return;

            remainTime += Time.deltaTime;

            if (remainTime <= 0)
            {
                BuffActivate(false);
            }
        }
    }

    private BattleObject _battleObj;

    private Dictionary<BuffType, BuffDataSet> BuffDatas = new Dictionary<BuffType, BuffDataSet>();
    public Dictionary<BuffType, BuffDataSet> GetBuffs { get { return BuffDatas; } }

    private AbilityData BuffAbility = new AbilityData();

    public int MaxHP { get { return BuffAbility.HP_Max; } }
    public int PAtk { get { return BuffAbility.P_Atk; } }
    public int PDef { get { return BuffAbility.P_Def; } }
    public int MAtk { get { return BuffAbility.M_Atk; } }
    public int MDef { get { return BuffAbility.M_Def; } }
    public float AtkSpeed { get { return BuffAbility.AttackSpeed; } }
    public float CriticalRate { get { return BuffAbility.CriticalRate; } }
    public float EvadeRate { get { return BuffAbility.EvadeRate; } }

    private void Start()
    {
        _battleObj = GetComponent<UnitController>();
        InitBuffDatas();
    }

    public void InitBuffDatas()
    {
        TableData_Consume ConsumeData = UnityDataConnector.Instance.m_TableData_Consume;
        foreach(var item in ConsumeData.list_ConsumeData)
        {
            BuffDatas[item.BfType] = new BuffDataSet(item.Index, 1, BuffCalculate);
        }

        //foreach (BuffType p in Enum.GetValues(typeof(BuffType)))
        //{
        //    BuffDatas[p] = new BuffDataSet(BuffCalculate);
        //}
    }


    public void BuffOn(BuffType onBuffType)
    {
        Debug.Log("<color=red>BuffManagerScript</color> BuffOn : " + onBuffType + " / " + _battleObj.Team);
        GetBuffs[onBuffType].BuffActivate(true);
    }

    /// <summary>현재 적용된 버프 능력치를 합산합니다.</summary>
    public void BuffCalculate()
    {        
        AbilityData calcAbility = new AbilityData();
        foreach (var item in BuffDatas)
        {
            if (item.Value.IsActive)
            {
                Debug.Log("<color=red>BattleObject</color> Activated buff : " + item.Value.BfType);                
                calcAbility.HP_Max += item.Value.MaxHpUp;
                calcAbility.P_Atk += item.Value.PAtkUp;
                calcAbility.P_Def += item.Value.PDefUp;
                calcAbility.M_Atk += item.Value.MAtkUp;
                calcAbility.M_Def += item.Value.MDefUp;
                calcAbility.EvadeRate += item.Value.EvdUp;
                calcAbility.CriticalRate += item.Value.CriUp;
                calcAbility.AttackSpeed += item.Value.AtkSpeedUp;
            }
        }
        Debug.Log("<color=red>BuffManagerScript</color> BuffCalculate : " + calcAbility.AttackSpeed);
        BuffAbility = calcAbility;

    }

    private void Update()
    {
        for (int i = 0; i < (int)BuffType.AttackSpeedUp; i++)
        {
            if (BuffDatas[(BuffType)i].IsActive == false)
                continue;

            BuffDatas[(BuffType)i].TimeUpdate();
        }
    }
}