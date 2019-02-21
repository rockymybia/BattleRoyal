using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

[System.Serializable]
public class TableData_Consume : CommonPathData
{
    string Filename = "TableData_Consume.Dat";
    public List<TableConsume> list_ConsumeData = new List<TableConsume>();

    public TableData_Consume()
    {
        Path = GetObfuscationPath(Filename);        
    }

    public void Save(string _Savepath)
    {
        if (list_ConsumeData == null) return;
        string _json = JsonConvert.SerializeObject(list_ConsumeData);
        ES2.Save<string>(_json, _Savepath);
    }

    public void Load()
    {
        if (ES2.Exists(Path) == false)
        {
            Save(Path);
            return;
        }

        string _LoadJson = ES2.Load<string>(Path);
        list_ConsumeData = JsonConvert.DeserializeObject<List<TableConsume>>(_LoadJson);

        Debug.Log("ES2 : " + Filename + _LoadJson);

    }

    public void UpdateTable(string jsondata)
    {
        LoadJsonData(jsondata);
        //에디터인 경우 베이스 테이블을 갱신해줍니다. 빌드시 포함
#if UNITY_EDITOR
        Save(GetBaseTableDataPath(Filename));
#endif
        //라이브 테이블 업데이트합니다.
        Save(Path);
    }

    public void LoadJsonData(string jsondata)
    {
        list_ConsumeData.Clear();
        list_ConsumeData = JsonConvert.DeserializeObject<List<TableConsume>>(jsondata);
    }

    public TableConsume GetData(int _Index)
    {
        if (list_ConsumeData.Count == 0)
            return null;

        return list_ConsumeData.Find(r => r.Index == _Index);
    }

    #region 스탯 정보 로드 메소드
    public float GetDuration(int m_index, int m_level)
    {
        TableConsume tc = GetData(m_index);
        if (tc == null)
            return 0f;

        return tc.Duration + (tc.Duration_Inc * (m_level - 1));
    }

    public int GetHealHp(int m_index, int m_level)
    {
        TableConsume tc = GetData(m_index);
        if (tc == null)
            return 0;

        return tc.HealHp + (tc.HealHp_Inc * (m_level - 1));
    }

    public int GetMaxHpUp(int m_index, int m_level)
    {
        TableConsume tc = GetData(m_index);
        if (tc == null)
            return 0;

        return tc.MaxHpUp + (tc.MaxHpUp_Inc * (m_level - 1));
    }

    public int GetPAtkUp(int m_index, int m_level)
    {
        TableConsume tc = GetData(m_index);
        if (tc == null)
            return 0;

        return tc.PAtkUp + (tc.PAtkUp_Inc * (m_level - 1));
    }

    public int GetPDefUp(int m_index, int m_level)
    {
        TableConsume tc = GetData(m_index);
        if (tc == null)
            return 0;

        return tc.PDefUp + (tc.PDefUp_Inc * (m_level - 1));
    }

    public float GetEvdUp(int m_index, int m_level)
    {
        TableConsume tc = GetData(m_index);
        if (tc == null)
            return 0;

        return tc.EvdUp + (tc.EvdUp_Inc * (m_level - 1));
    }

    public float GetCriUp(int m_index, int m_level)
    {
        TableConsume tc = GetData(m_index);
        if (tc == null)
            return 0;

        return tc.CriUp + (tc.CriUp_Inc * (m_level - 1));
    }

    public float GetAtkSpeedUp(int m_index, int m_level)
    {
        TableConsume tc = GetData(m_index);
        if (tc == null)
            return 0;

        return tc.AtkSpeedUp + (tc.AtkSpeedUp_Inc * (m_level - 1));
    }
#endregion

}
