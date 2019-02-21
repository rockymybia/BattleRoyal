using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

/// <summary>
/// 로컬 데이터 저장 Json String + AES 암호화
/// 재화, 현재 보유카드, 세팅한 덱정보, 옵션정보
/// </summary>
public class SaveDataManagerScript : Singleton<SaveDataManagerScript> {

    #region 클래스
    /// <summary>자원정보 데이터</summary>
    public class MyDataAsset : CommonPathData
    {        
        private const string Filename = "ES2SD_MyAssets.Dat";
        MyAccountAssetData data = new MyAccountAssetData();

        public MyDataAsset()
        {            
            Path = GetObfuscationPath(Filename);
            Load();
        }

        void AddGold(int _addValue){ data.Gold += _addValue; }        
        public void AddGem(int _addValue) { data.Gem += _addValue; }
        public int GetGold() { return data.Gold; }
        public int GetGem() { return data.Gem; }

        public void RefreshGold(int _RefValue)
        {
            AddGold(_RefValue);
            if (data.Gold < 0)
                data.Gold = 0;

            Save();
        }

        public void RefreshGem(int _RefValue)
        {
            AddGem(_RefValue);
            if (data.Gem < 0)
                data.Gem = 0;

            Save();
        }

        public void Save()
        {
            if (data == null) return;

            string _json = JsonConvert.SerializeObject(data);
            ES2.Save<string>(_json, Path);
        }


        public void Load()
        {
            if (ES2.Exists(Path) == false)
            {
                data = new MyAccountAssetData(100, 0);
                Save();
                return;
            }

            string _LoadJson = ES2.Load<string>(Path);
            data = JsonConvert.DeserializeObject<MyAccountAssetData>(_LoadJson);

            Debug.Log("GameData : " + Filename + _LoadJson);
            //Debug.LogError(Path);
            //Debug.LogError(data.Gold + " / " + data.Gem);
        }
    }

    /// <summary>보유 카드 정보</summary>
    public class MyHaveCards : CommonPathData
    {
        private const string Filename = "ES2SD_HaveCard.Dat";

        List<BattleUnitData> data = new List<BattleUnitData>();
        public List<BattleUnitData> Data
        {
            get { return data; }
            set { data = value; }
        }
        
        public MyHaveCards()
        {            
            Path = GetObfuscationPath(Filename);
            Load();
        }

        /// <summary>테스트 카드 넣기</summary>
        public void InsertCard_Test(int _index, int _id)
        {
            if (Data.Exists(r => r.UnitID == _id))
                return;

            //Debug.LogError("InsertCard_Test : " + _id);

            BattleUnitData _data = new BattleUnitData();
            _data.Index = _index;
            _data.UnitID = _id;
            _data.Level = 1;
            _data.CardCount = 1;
            _data.IsFindCard = true;
            Data.Add(_data);
            Save();
        }
        public void Save()
        {
            if (Data == null) return;

            string _json = JsonConvert.SerializeObject(Data);
            ES2.Save<string>(_json, Path);
        }
        public void Load()
        {
            if (ES2.Exists(Path) == false)
            {
                //테스트용 데이터
                foreach (var item in UnityDataConnector.Instance.m_TableData_Unit.Rows)
                {
                    InsertCard_Test(-1, item.Index);
                }

                Save();
                return;
            }

            string _LoadJson = ES2.Load<string>(Path);
            Data = JsonConvert.DeserializeObject<List<BattleUnitData>>(_LoadJson);

            Debug.Log("GameData : " + Filename + _LoadJson);
            //foreach (var item in data)
            //{
            //    Debug.LogError(item.Index + " / " + item.UnitID);
            //}
        }

        public List<BattleUnitData> GetHaveCardList()
        {
            return data;
        }
    }

    /// <summary>세팅 덱 정보</summary>
    public class MyDeckDatas : CommonPathData
    {
        private const int MAXIMUM_DECKLISTCOUNT = 8;
        private const string Filename = "ES2SD_MyCardDeck.Dat";

        Dictionary<int, List<BattleUnitData>> data = new Dictionary<int, List<BattleUnitData>>();
        
        public MyDeckDatas()
        {
            Path = GetObfuscationPath(Filename);
            Load();
        }

        public void Init()
        {
            Reset();
        }

        public void Reset()
        {
            data.Clear();
            data.Add(1, new List<BattleUnitData>());
            data.Add(2, new List<BattleUnitData>());
            data.Add(3, new List<BattleUnitData>());
            Save();
        }

        public void Save()
        {
            if (data == null) return;

            string _json = JsonConvert.SerializeObject(data);
            ES2.Save<string>(_json, Path);
        }

        public void Load()
        {            
            if (ES2.Exists(Path) == false)
            {
                Init();                
                return;
            }

            string _LoadJson = ES2.Load<string>(Path);
            data = JsonConvert.DeserializeObject<Dictionary<int, List<BattleUnitData>>>(_LoadJson);

            if (data.Count == 0)
            {
                Init();
            }

            Debug.Log("GameData : " + Filename + _LoadJson);
            //foreach (var item in data)
            //{
            //    Debug.LogError(item.Key + " / " + item.Value[0].Index + " / " + item.Value[0].UnitID);
            //}
        }

        public BattleUnitData GetMyUnitData(int _DeckIndex, int _Index)
        {
            List<BattleUnitData> list = GetDeckList(_DeckIndex);
            try
            {
                return list.Find(r => r.Index == _Index);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.ToString());
                return null;
            }
            
        }

        public List<BattleUnitData> GetDeckList(int _DeckIndex)
        {
            try
            {
                return data[_DeckIndex];
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.ToString());                
                return null;
            }            
        }

        /// <summary>덱 리스트에 유닛 추가</summary>        
        public void AddDeck(int _Index, BattleUnitData _Data)
        {
            //if (data[_Index].Count >= MAXIMUM_DECKLISTCOUNT)
            //{
            //    Debug.LogError("8개 이상의 덱이 추가됨");
            //    return;
            //}

            //동일 아이디가 리스트에 없을때만 추가
            if (data[_Index].Exists(r=> r.UnitID == _Data.UnitID) == false)
            {
                data[_Index].Add(_Data);
                Save();
            }            
        }

        /// <summary>덱 리스트에 유닛 제거</summary>
        public void RemoveDeck(int _Index, BattleUnitData _Data)
        {
            
            //동일 아이디가 리스트에 있을때만 제거
            //if (data[_Index].Exists(r => r.UnitID == _Data.UnitID))
            if (ExistsDeck(_Index, _Data))
            {
                Debug.LogError("RemoveDeck : " + _Index + " / " + _Data.Index + " / " + _Data.Name);
                BattleUnitData _removeUnit = data[_Index].Find(r => r.UnitID == _Data.UnitID);
                data[_Index].Remove(_removeUnit);
                Save();
            }
        }

        /// <summary>해당 인덱스의 덱에 이미 저장된 덱이 존재하는가?</summary>
        /// <param name="_Index">덱인덱스</param>
        /// <param name="_Data">배틀유닛데이터</param>        
        public bool ExistsDeck(int _Index, BattleUnitData _Data)
        {
            return data[_Index].Exists(r => r.UnitID == _Data.UnitID);
        }


    }

    public class MyPlayHistoryInfo : CommonPathData
    {
        private const string Filename = "ES2SD_MyPlayHistoryInfo.Dat";
        MyPlayHistoryData data = new MyPlayHistoryData();

        public MyPlayHistoryInfo()
        {
            Path = GetObfuscationPath(Filename);
            Load();
        }

        /// <summary>현재 진행할 스테이지 인덱스</summary>        
        public int GetCurrentStageIndex()
        {
            Debug.Log("<color=lime>현재 진행할 스테이지 인덱스</color> : " + data.CurrentStage);
            return data.CurrentStage;
        }

        public void SetNextStage()
        {
            var stageTable = UnityDataConnector.Instance.m_TableData_Stage;
            if (stageTable.ExistsData(data.CurrentStage))
            {
                data.CurrentStage++;
                Save();
                Debug.Log("<color=lime>다음 스테이지 인덱스</color> : " + data.CurrentStage);
            }            
        }

        public void Save()
        {
            if (data == null) return;

            string _json = JsonConvert.SerializeObject(data);
            ES2.Save<string>(_json, Path);
        }

        public void Load()
        {
            if (ES2.Exists(Path) == false)
            {
                data = new MyPlayHistoryData();
                data.CurrentStage = 1;
                Save();
                return;
            }

            string _LoadJson = ES2.Load<string>(Path);
            data = JsonConvert.DeserializeObject<MyPlayHistoryData>(_LoadJson);

            Debug.Log("MyPlayHistoryInfo Load : " + Filename + _LoadJson);
        }

        public void ResetPlayHistory()
        {
            data.CurrentStage = 1;
            Save();
        }
    }


    #endregion



    #region Propertys
    //자원
    MyDataAsset m_MyAssets;
    public SaveDataManagerScript.MyDataAsset MyAssets
    {
        get { return m_MyAssets; }
        set { m_MyAssets = value; }
    }
    //보유카드
    MyHaveCards m_MyCards;
    public SaveDataManagerScript.MyHaveCards MyCards
    {
        get { return m_MyCards; }
        set { m_MyCards = value; }
    }
    //세팅덱
    MyDeckDatas m_MyDecks;
    public SaveDataManagerScript.MyDeckDatas MyDecks
    {
        get { return m_MyDecks; }
        set { m_MyDecks = value; }
    }

    MyPlayHistoryInfo m_MyPlayHistoryInfo;
    public SaveDataManagerScript.MyPlayHistoryInfo MyPlayHistory
    {
        get { return m_MyPlayHistoryInfo; }
        set { m_MyPlayHistoryInfo = value; }
    }
    
#endregion



    //게임옵션

	//로컬 테이블 버전
    LocalTableData_Version m_Ltd_Version;
    public LocalTableData_Version Ltd_Version
    {
        get { return m_Ltd_Version; }
        set { m_Ltd_Version = value; }
    }

    /// <summary>현재 선택된 덱의 인덱스</summary>
    public int CurrentDeckIndex = 1;

    /// <summary>게임 옵션 저장 정보</summary>
    string Filename_GameOption = "ES2SD_GameOption.Dat";

    //IEnumerator Start()
    //{
    //    yield return StartCoroutine(co_Initialize());
    //}

    public void ResetTableVersion()
    {
        m_Ltd_Version = new LocalTableData_Version();
        m_Ltd_Version.ResetVersion();
    }

    public IEnumerator co_LoadTableVersion()
    {
        Debug.Log("GameData : 테이블 버전 로드"); // #GameData
        m_Ltd_Version = new LocalTableData_Version();        
        yield return null;
    }

	public IEnumerator co_Initialize()
    {
        Debug.Log("GameData : 세이브 데이터 로드");        
        MyAssets = new MyDataAsset();
        MyCards = new MyHaveCards();
        MyDecks = new MyDeckDatas();
        MyPlayHistory = new MyPlayHistoryInfo();
        yield return null;        
    }

	IEnumerator LoadMyData_TableVersion()
    {
        if (m_Ltd_Version == null)
            m_Ltd_Version = new LocalTableData_Version();

        yield return null;
    }

    IEnumerator LoadMyData_Asset()
    {
        if (MyAssets == null)
            MyAssets = new MyDataAsset();

        yield return null;
    }

    IEnumerator LoadMyData_HaveCards()
    {
        if (MyCards == null)
            MyCards = new MyHaveCards();
        yield return null;
    }

    IEnumerator LoadMyData_DeckDatas()
    {
        if (MyDecks == null)
            MyDecks = new MyDeckDatas();
        yield return null;
    }

    [ContextMenu("ResetDeckDatas")]
    public void ResetDeckDatas()
    {
        MyDecks.Reset();
    }

    public int GetDeckIndex_From_PlayerPrefs()
    {
        if (PlayerPrefs.HasKey("CurrentDeckIndex") == false)
        {
            //CurrentDeckIndex = 1;
            SaveDeckIndex_To_PlayerPrefs(1);
        }

        return PlayerPrefs.GetInt("CurrentDeckIndex");
    }

    public void SaveDeckIndex_To_PlayerPrefs(int _index)
    {
        CurrentDeckIndex = _index;
        PlayerPrefs.SetInt("CurrentDeckIndex", CurrentDeckIndex);
        PlayerPrefs.Save();
    }

    public List<BattleUnitData> GetCurrentDeckList()
    {
        return m_MyDecks.GetDeckList(CurrentDeckIndex);
    }

    public int GetCurrentDeckCount() { return m_MyDecks.GetDeckList(CurrentDeckIndex).Count; }

    [ContextMenu("ResetPlayHistory")]
    public void ResetPlayHistory()
    {
        m_MyPlayHistoryInfo.ResetPlayHistory();
    }

}
