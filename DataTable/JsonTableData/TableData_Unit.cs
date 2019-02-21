using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

/// <summary>유닛 테이블 데이터
/// 기본 테이블 정보는 Resource 폴더에 저장되어 있습니다. 실제 사용은 라이브 데이터로 사용되며 테이블 패치시에도 라이브 데이터만 갱신됩니다.
/// 유니티에서 테이블 업데이트하면 기본 테이블과 라이브 테이블이 모두 갱신됩니다.
/// </summary>
[System.Serializable]
public class TableData_Unit : CommonPathData
{
    string Filename = "TableData_Unit.Dat";
    List<TableUnit> list_UnitData = new List<TableUnit>();

    public List<TableUnit> Rows
    {
        get { return list_UnitData; }
        set { list_UnitData = value; }
    }
    public void LoadJsonData(string jsondata)
    {
        list_UnitData.Clear();
        list_UnitData = JsonConvert.DeserializeObject<List<TableUnit>>(jsondata);
    }

    public TableData_Unit()
    {
        Path = GetObfuscationPath(Filename);        
    }

    public void Save(string _Savepath)
    {
        if (list_UnitData == null) return;
        string _json = JsonConvert.SerializeObject(list_UnitData);
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
        list_UnitData = JsonConvert.DeserializeObject<List<TableUnit>>(_LoadJson);

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

    public TableUnit GetUnitData(int _Index)
    {
        if (list_UnitData.Count == 0)
            return null;

        return list_UnitData.Find(r => r.Index == _Index);
    }

    public string GetUnitName(int _Index)
    {
        if (list_UnitData.Count == 0)
            return string.Empty;

        return list_UnitData.Find(r => r.Index == _Index).UnitName;
    }

    public string GetResourceName(int _Index)
    {
        string resname = string.Empty;
        TableUnit _data = GetUnitData(_Index);
        if (_data != null)
            resname = _data.ResourceName;

        return resname;
    }

    public int GetLength()
    {
        return list_UnitData.Count;
    }

}
