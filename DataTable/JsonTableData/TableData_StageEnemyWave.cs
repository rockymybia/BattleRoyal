using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

public class TableData_StageEnemyWave : CommonPathData
{
    string Filename = "TableData_StageEnemyWave.Dat";
    public List<TableStageEnemyWave> listData = new List<TableStageEnemyWave>();

    public TableData_StageEnemyWave()
    {
        Path = GetObfuscationPath(Filename);
    }

    public void Save(string _Savepath)
    {
        if (listData == null) return;
        string _json = JsonConvert.SerializeObject(listData);
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
        listData = JsonConvert.DeserializeObject<List<TableStageEnemyWave>>(_LoadJson);
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
        listData.Clear();
        listData = JsonConvert.DeserializeObject<List<TableStageEnemyWave>>(jsondata);
    }

    public List<TableStageEnemyWave> GetWaveListData(int _Index)
    {
        if (listData.Count == 0)
            return null;

        return listData.FindAll(r => r.Index == _Index);
    }

    public bool ExistsWaveList(int _Index)
    {
        return listData.Exists(r => r.Index == _Index);
    }

}
