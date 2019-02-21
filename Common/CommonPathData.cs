using UnityEngine;
using System.Collections;

interface ICommonDataSave
{
    void Save();
    void Load();
}

/// <summary>공용 데이터 클래스 경로 (세이브 데이터, 라이브 업데이트 테이블)</summary>
public class CommonPathData
{
    /// <summary>완성된 경로</summary>
    string _path = string.Empty;
    public string Path
    {
        get { return _path; }
        set { _path = value; }
    }

    /// <summary>파일 저장 루트 경로</summary>
    public string SavePath
    {
        get
        {
#if UNITY_EDITOR
                return string.Format("{0}", Application.dataPath);
#else
            return string.Format("{0}", Application.persistentDataPath);
#endif
        }
    }

    /// <summary>기본 테이블 데이터 저장 경로</summary>
    public string BaseTableDataPath
    {
        get
        {
#if UNITY_EDITOR
            return string.Format("{0}/Resources/DataTable", Application.dataPath);
#else
            return string.Format("{0}/Resources/DataTable", Application.persistentDataPath);
#endif
        }        
    }

    /// <summary>암호화키</summary>
    string _encryptionKey = "rb13@$";
    public string EncryptionKey
    {
        get { return _encryptionKey; }
        set { _encryptionKey = value; }
    }
    /// <summary>암호화 스트링 포멧</summary>
    string str_Encryption = "encrypt=true&password={0}";
    /// <summary>난독화 스트링 포멧</summary>
    public string str_Obfuscation = "encrypt=true&encryptiontype=obfuscate&password={0}";

    /// <summary>일반 저장 경로 리턴</summary>
    public string GetNormalPath(string _filename)
    {
        return string.Format("{0}/{1}", SavePath, _filename);
    }

    /// <summary>암호화 저장 경로 리턴</summary>
    public string GetObfuscationPath(string _filename)
    {
        string Obfus = string.Format(str_Obfuscation, EncryptionKey);
        return string.Format("{0}/{1}?{2}", SavePath, _filename, Obfus);
    }

    public string GetBaseTableDataPath(string _filename)
    {
        string Obfus = string.Format(str_Obfuscation, EncryptionKey);
        return string.Format("{0}/{1}?{2}", BaseTableDataPath, _filename, Obfus);
    }

}
