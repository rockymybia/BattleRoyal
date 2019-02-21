using System.Collections;
using UnityEngine;

//using LitJson;

/// <summary>구글 드라이브 연결 클라스</summary>
public class UnityDataConnector : Singleton<UnityDataConnector>
{
    /// <summary>웹 경로</summary>
    public string webServiceUrl = "";

    /// <summary>스프레드 시트 아이디</summary>
    public string spreadsheetId = "";

    /// <summary>시트이름</summary>
    public string worksheetName = "";

    /// <summary>문서에 암호 걸린경우 암호</summary>
    public string password = "";

    public float maxWaitTime = 10f;

    //public GameObject dataDestinationObject;
    public string statisticsWorksheetName = "Statistics";

    private bool updating;
    private string currentStatus;
    private bool saveToGS;

    public TableData_Version m_LocalTableData_Version = new TableData_Version();
    public TableData_Unit m_TableData_Unit = new TableData_Unit();
    public TableData_Structure m_TableData_Structure = new TableData_Structure();
    public TableData_Consume m_TableData_Consume = new TableData_Consume();
    public TableData_Projectile m_TableData_Projectile = new TableData_Projectile();
    public TableData_Stage m_TableData_Stage = new TableData_Stage();
    public TableData_StageEnemyWave m_TableData_StageEnemyWave = new TableData_StageEnemyWave();

    private void Awake()
    {
    }

    private void Start()
    {
        updating = false;
        currentStatus = "Offline";
        saveToGS = false;

        //StartCoroutine(Connect_TableLoad());
    }

    public IEnumerator Connect_TableLoad()
    {
        if (updating)
            yield break;

        updating = true;

        yield return StartCoroutine(RequestTableData(TableType.Unit));
        yield return StartCoroutine(RequestTableData(TableType.ConsumeItem));
        yield return StartCoroutine(RequestTableData(TableType.Stage));
        yield return StartCoroutine(RequestTableData(TableType.StageEnemyWave));
        //yield return StartCoroutine(RequestTableData(TableType.Structure));

        //StageManager._instance.InitStage();
    }

    #region jsondata 파싱

    private void ParseJsonTabledata(TableType _type, string response)
    {
        Debug.Log("GameData " + response);

        switch (_type)
        {
            case TableType.TableVersion: m_LocalTableData_Version.LoadJsonData(response); break;
            case TableType.Unit: m_TableData_Unit.UpdateTable(response); break;
            case TableType.Structure: m_TableData_Structure.UpdateTable(response); break;
            case TableType.ConsumeItem: m_TableData_Consume.UpdateTable(response); break;
            case TableType.Projectile: m_TableData_Projectile.UpdateTable(response); break;
            case TableType.Stage: m_TableData_Stage.UpdateTable(response); break;
            case TableType.StageEnemyWave: m_TableData_StageEnemyWave.UpdateTable(response); break;
        }

        updating = false;
    }

    private IEnumerator RequestTableData(TableType _type)
    {
        string connectionString = webServiceUrl + "?ssid=" + spreadsheetId + "&sheet=" + _type.ToString() + "&pass=" + password + "&action=GetData";
        //if (debugMode)
        //    Debug.LogError("Request TableData " + _type);

        WWW www = new WWW(connectionString);

        float elapsedTime = 0.0f;
        //currentStatus = "Stablishing Connection... ";

        while (!www.isDone)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= maxWaitTime)
            {
                currentStatus = "Max wait time reached, connection aborted.";
                Debug.Log(currentStatus);
                updating = false;
                break;
            }

            yield return null;
        }

        if (!www.isDone || !string.IsNullOrEmpty(www.error))
        {
            currentStatus = "Connection error after" + elapsedTime.ToString() + "seconds: " + www.error;
            Debug.LogError(currentStatus);
            updating = false;
            yield break;
        }

        string response = www.text;
        //Debug.Log(elapsedTime + " : " + response);
        //currentStatus = "Connection stablished, parsing data...";

        if (response == "\"Incorrect Password.\"")
        {
            //currentStatus = "Connection error: Incorrect Password.";
            Debug.LogError("Connection error: Incorrect Password.");
            updating = false;
            yield break;
        }

        //JsonData Parsing
        ParseJsonTabledata(_type, response);

        //Debug.LogError(response);
        //      try
        //{
        //          List<TableUnit> data = JsonConvert.DeserializeObject<List<TableUnit>>(response);
        //          Debug.LogError(data.Count);
        //	//ssObjects = JsonMapper.ToObject<JsonData[]>(response);
        //}
        //catch
        //{
        //	currentStatus = "Data error: could not parse retrieved data as json.";
        //	Debug.LogError(currentStatus);
        //	updating = false;
        //	yield break;
        //}

        //currentStatus = "Data Successfully Retrieved!";
        //updating = false;

        // Finally use the retrieved data as you wish.
        //dataDestinationObject.SendMessage("DoSomethingWithTheData", ssObjects);
    }

    #endregion jsondata 파싱

    public void Start_DataUpdate()
    {
        StartCoroutine(co_CheckTableVersion());
    }

    private TableType GetTableType(string _tablename)
    {
        foreach (TableType en in System.Enum.GetValues(typeof(TableType)))
        {
            if (en.ToString() == _tablename)
            {
                return en;
            }
        }

        return TableType.None;
    }

    public IEnumerator co_CheckTableVersion()
    {
        Debug.Log("GameData load co_CheckTableVersion");
        yield return StartCoroutine(SaveDataManagerScript.Instance.co_LoadTableVersion());

        SaveDataManagerScript.Instance.ResetTableVersion();

        //테이블 버전 정보를 먼저 체크하고 버전이 높은 테이블 데이터를 갱신해준다.
        yield return StartCoroutine(RequestTableData(TableType.TableVersion));

        //버전 비교
        var _savedData = SaveDataManagerScript.Instance.Ltd_Version;
        for (int i = 0; i < m_LocalTableData_Version.Data.Count; i++)
        {
            var _target = m_LocalTableData_Version.Data[i];
            int _CompareResult = _savedData.CompareVersion(_target);
            if (_CompareResult == -1)
            {
                //대상 테이블 패치
                TableType _TbType = GetTableType(_target.TableType);
                if (_TbType != TableType.None)
                {
                    Debug.Log("GameData : " + string.Format("{0} table update", _TbType));
                    yield return StartCoroutine(RequestTableData(_TbType));
                    _savedData.SetVersion(_TbType, _target.version);
                }
            }
        }

        //버전 테이블 갱신
        SaveDataManagerScript.Instance.Ltd_Version.RefreshData(m_LocalTableData_Version.Data);

        Debug.Log("GameData : CheckTableVersion Complete");
    }

    public IEnumerator LoadBaseTable()
    {
        m_TableData_Unit.Load();
        //m_TableData_Structure.Load();
        m_TableData_Consume.Load();
        yield return null;
    }

    public void Showlog_TableData_Unit()
    {
        m_TableData_Unit.Load();
    }

    #region Hide Source Code

    //public void SaveDataOnTheCloud(string ballName, float collisionMagnitude)
    //{
    //    if (saveToGS)
    //        StartCoroutine(SendData(ballName, collisionMagnitude));
    //}

    //IEnumerator SendData(string ballName, float collisionMagnitude)
    //{
    //    if (!saveToGS)
    //        yield break;

    //    string connectionString = 	webServiceUrl +
    //                                "?ssid=" + spreadsheetId +
    //                                "&sheet=" + statisticsWorksheetName +
    //                                "&pass=" + password +
    //                                "&val1=" + ballName +
    //                                "&val2=" + collisionMagnitude.ToString() +
    //                                "&action=SetData";

    //    if (debugMode)
    //        Debug.Log("Connection String: " + connectionString);
    //    WWW www = new WWW(connectionString);
    //    float elapsedTime = 0.0f;

    //    while (!www.isDone)
    //    {
    //        elapsedTime += Time.deltaTime;
    //        if (elapsedTime >= maxWaitTime)
    //        {
    //            // Error handling here.
    //            break;
    //        }

    //        yield return null;
    //    }

    //    if (!www.isDone || !string.IsNullOrEmpty(www.error))
    //    {
    //        // Error handling here.
    //        yield break;
    //    }

    //    string response = www.text;

    //    if (response.Contains("Incorrect Password"))
    //    {
    //        // Error handling here.
    //        yield break;
    //    }

    //    if (response.Contains("RCVD OK"))
    //    {
    //        // Data correctly sent!
    //        yield break;
    //    }
    //}

    #endregion Hide Source Code
}