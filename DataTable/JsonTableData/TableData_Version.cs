using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

public class TableVersion
{
    public string TableType;
    public int version;
}

public class TableData_Version
{
    List<TableVersion> m_list_VersionData = new List<TableVersion>();
    public List<TableVersion> Data
    {
        get { return m_list_VersionData; }
        set { m_list_VersionData = value; }
    }
    public void LoadJsonData(string jsondata)
    {
        m_list_VersionData = JsonConvert.DeserializeObject<List<TableVersion>>(jsondata);
    }
}

#region 로컬 테이블 버전
public class LocalTableData_Version : CommonPathData
{
    List<TableVersion> _data = new List<TableVersion>();
    public List<TableVersion> Data
    {
        get { return _data; }
        set { _data = value; }
    }
    string Filename = "ES2SD_LT_Version.Dat";
    public LocalTableData_Version()
    {
        Path = GetObfuscationPath(Filename);
        Load();
    }

    public void SetVersion(TableType _type, int _value)
    {
        TableVersion _tv = Data.Find(r => r.TableType == _type.ToString());
        if (_tv != null)
            _tv.version = _value;
        
    }

    public int GetVersion(TableType _type)
    {
        return GetVersion(_type.ToString());
    }

    public int GetVersion(string _type)
    {        
        try
        {
            TableVersion _TvData = Data.Find(r => r.TableType == _type);
            return _TvData.version;
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.ToString());
            return -1;
        }           
    }

    public void ResetVersion()
    {
        _data.ForEach(r=> r.version = 0);
        Save();
    }

    public int CompareVersion(TableVersion _CompareData)
    {
        bool ExistRowdata = Data.Exists(r=> r.TableType == _CompareData.TableType);
        if (ExistRowdata == false)
        {
            Debug.LogError(string.Format("{0} 테이블 데이터가 없음", _CompareData.TableType));
            return -1;
        }
                    
        Debug.Log("GameData : " + string.Format("{0} 테이블 버전 체크", _CompareData.TableType));
        int localversion = GetVersion(_CompareData.TableType);
        if (localversion == _CompareData.version)
        {
            Debug.LogError(string.Format("{0} 테이블 버전이 일치", _CompareData.TableType));
            return 1;
        }
        else
        {
            if (localversion > _CompareData.version) //
            {
                Debug.Log("GameData : 로컬 버전이 높다. 서버 테이블이 버전을 올려라");
                return -2;
            }
            else
            {
                Debug.Log("GameData : 로컬 버전이 낮다. 로컬 테이블 업데이트해라");
                return -1;
            }            
        }         
    }

    public void RefreshData(List<TableVersion> _data) 
    {
        this._data.Clear();
        this._data = GameUtil.Clone<TableVersion>(_data);
        Save();
    }

    public void Save()
    {
        if (_data == null) return;

        string _json = JsonConvert.SerializeObject(_data);
        ES2.Save<string>(_json, Path);
    }

    public void Load()
    {
        if (ES2.Exists(Path) == false)
        {
            Save();
            return;
        }

        string _LoadJson = ES2.Load<string>(Path);
        _data = JsonConvert.DeserializeObject<List<TableVersion>>(_LoadJson);

        Debug.Log("ES2 " + Filename + _LoadJson);        
    }
}
#endregion