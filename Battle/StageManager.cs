using AssetBundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using com.ootii.Messages;
using com.ootii.Utilities;

/// <summary>아군과 적군 데이터를 관리하고 게임 승패를 결정합니다.</summary>
public class StageManager : MonoBehaviour
{
    

    public static StageManager _instance;

    /// <summary>타워 3개 파괴시 승리</summary>
    private readonly int WINNERCONDITION_TOWERCOUNT = 3;
    
    private List<TableStageEnemyWave> waveDatas = new List<TableStageEnemyWave>();
    private int CurrentCrowdIndex = 0;

    /// <summary>건물 위치</summary>
    [System.Serializable]
    public class StructureTransform
    {
        public TeamType _Team;
        public Transform tr_Left;
        public Transform tr_Right;
        public Transform tr_Center;
    }

    [Header("아군 타워 위치")]
    public StructureTransform AllyDefenseTowerPosition = new StructureTransform();

    [Header("적군 타워 위치")]
    public StructureTransform EnemyDefenseTowerPosition = new StructureTransform();

    [Header("적군 스폰 위치")]
    public Transform[] tr_EnemySpawnPos;
    [Header("아군 스폰 위치")]
    public Transform[] tr_AllySpawnPos;

    public Dictionary<eBattlePosition, Transform> dic_EnemyspawnPos = new Dictionary<eBattlePosition,Transform>();
    public Dictionary<eBattlePosition, Transform> dic_AllyspawnPos = new Dictionary<eBattlePosition, Transform>();

    private BattleDeckSelectUIManagerScript BattleUI;

    #region BattleObjectLiveData

    [System.Serializable]
    public class BattleObjectLiveData
    {
        public TeamType Team;

        private int teamScore_;

        public int TeamScore
        {
            get { return teamScore_; }
            set { teamScore_ = value; }
        }

        /// <summary>살아남은 타워 카운트</summary>
        public int LiveTowerCount
        {
            get
            {
                int _count = 0;
                for (int i = 0; i < LiveBattleObjects.Count; i++)
                {
                    if (LiveBattleObjects[i].unitType == UnitType.DefenseTower)
                    {
                        _count++;
                    }
                }

                return _count;
            }
        }

        public int LiveUnitCount
        {
            get
            {
                return LiveBattleObjects.Count;                
            }
        }

        /// <summary>타워가 모두 전멸했는가</summary>
        public bool IsAllDestroyedTower { get { return (LiveTowerCount == 0); } }

        /// <summary>모두 제거되었는가</summary>
        public bool IsAllEliminate { get { return (LiveUnitCount == 0); } }

        public List<BattleObject> LiveBattleObjects = new List<BattleObject>();

        /// <summary>공격 대상이 될만한 가장 가까운 타워 대상을 찾을때 리턴해준다</summary>
        /// <param name="_Enemy">공격자 배틀 오브젝트</param>
        public BattleObject FindNearEnemyObject(BattleObject _Enemy)
        {
            if (LiveBattleObjects.Count == 0)
                return null;

            BattleObject target = null;
            List<BattleObject> list = new List<BattleObject>();
            switch (_Enemy.BaseAbility.AttackTarget)
            {
                //case AttackType.GroundUnit:
                //case AttackType.SkyUnit:
                //case AttackType.EveryThing:
                //    break;
                case AttackType.DefenseTower:
                    list = LiveBattleObjects.FindAll(r => r.BaseAbility.type == UnitType.DefenseTower);
                    break;

                default:
                    //공격자가 방어타워라면 적군중 유닛 데이터만 가지고 옵니다.
                    if (_Enemy.IsDefenseTower())
                    {
                        List<BattleObject> UnitList = LiveBattleObjects.FindAll(r => r.IsUnit() && (r.IsUnitDead == false));

                        foreach (var item in UnitList)
                        {
                            //타워이므로 사정거리 내에 들어온 유닛만 공격 대상 리스트에 담는다
                            if (GetDistance(_Enemy, item) <= _Enemy.BaseAbility.AttackRange)
                            {
                                //Debug.LogError("사정거리 내에 들어온 유닛만 리스트에 담는다 " + item.GetBattleObjectName());
                                list.Add(item);
                            }
                        }
                    }
                    else
                    {
                        list.AddRange(LiveBattleObjects);
                    }
                    break;
            }

            if (list.Count == 0)
                return null;

            #region 타겟 가능 대상 중 가장 가까운 적을 찾는다

            //첫번째 배열의 배틀 오브젝트를 1차 타겟으로 잡는다
            target = list[0];
            //타겟과의 거리를 구해보자
            float distance_CurrentTarget = GetDistance(target, _Enemy);

            //공격 가능 대상들과의 거리를 모두 체크하여 가장 가까운 적으로 타겟을 잡는다
            foreach (var item in list)
            {
                if (target == item)
                    continue;

                //다른 타워와의 거리를 구해서 현재 타겟의 거리보다 가깝다면 타겟 교체
                float _Comparedistance = GetDistance(item, _Enemy);
                if (distance_CurrentTarget > _Comparedistance)
                {
                    distance_CurrentTarget = _Comparedistance;
                    target = item;
                }
            }

            #endregion 타겟 가능 대상 중 가장 가까운 적을 찾는다

            return target;
        }

        public float GetDistance(BattleObject obj1, BattleObject obj2)
        {
            Vector3 attackerPos = new Vector3(obj1.transform.position.x, 0f, obj1.transform.position.z);
            Vector3 targetPos = new Vector3(obj2.transform.position.x, 0f, obj2.transform.position.z);
            return Vector3.Distance(attackerPos, targetPos);
        }

        public void DestroyBattleObject(BattleObject _DestroyObject)
        {
            LiveBattleObjects.Remove(_DestroyObject);
        }
    }

    #endregion BattleObjectLiveData

    //public List<BattleObject> list_AllyDefenseTower = new List<BattleObject>();
    //public List<BattleObject> list_EnemyDefenseTower = new List<BattleObject>();
    public BattleObjectLiveData Ally_LiveData = new BattleObjectLiveData();

    public BattleObjectLiveData Enemy_LiveData = new BattleObjectLiveData();

    public Transform tr_Canvas;
    public GameObject prefab_HPBar;

    private void Awake()
    {
        _instance = this;
    }

    // Use this for initialization
    private void Start()
    {
        Ally_LiveData.Team = TeamType.Ally;
        Enemy_LiveData.Team = TeamType.Enemy;

        dic_EnemyspawnPos.Add(eBattlePosition.Left, tr_EnemySpawnPos[(int)eBattlePosition.Left]);
        dic_EnemyspawnPos.Add(eBattlePosition.Right, tr_EnemySpawnPos[(int)eBattlePosition.Right]);
        dic_EnemyspawnPos.Add(eBattlePosition.Center, tr_EnemySpawnPos[(int)eBattlePosition.Center]);

        dic_AllyspawnPos.Add(eBattlePosition.Left, tr_AllySpawnPos[(int)eBattlePosition.Left]);
        dic_AllyspawnPos.Add(eBattlePosition.Right, tr_AllySpawnPos[(int)eBattlePosition.Right]);
        dic_AllyspawnPos.Add(eBattlePosition.Center, tr_AllySpawnPos[(int)eBattlePosition.Center]);

        StartCoroutine(InitStage());
    }

    private void ReSetWaveData()
    {
        int stageIndex = SaveDataManagerScript.Instance.MyPlayHistory.GetCurrentStageIndex();
        waveDatas = UnityDataConnector.Instance.m_TableData_StageEnemyWave.GetWaveListData(stageIndex);
    }

    private IEnumerator InitStage()
    {
        BattleUI = UIManagerScript.Instance.UI_Battle;

        Debug.Log("<color=red>StageManager</color> 전투 준비중");
        ReSetWaveData();
        if (waveDatas.Count == 0)
        {
            Debug.LogError("웨이브 데이터가 없습니다!!");
            StartCoroutine(FinishBattle(TeamType.Enemy));
            yield break;
        }

        CurrentCrowdIndex = 1;
        yield return StartCoroutine(SpawnEnemys());
        yield return StartCoroutine(SpawnMyCharacters());
        //yield return StartCoroutine(InitTower());
        //yield return StartCoroutine(InitBattleMember());
        Debug.Log("<color=red>StageManager</color> 전투 시작!");
    }

    private IEnumerator SpawnEnemys()
    {
        Debug.LogError(CurrentCrowdIndex + " 번 웨이브 생성");
        var Currentwave = waveDatas.FindAll(r => r.CrowdIndex == CurrentCrowdIndex);
        foreach (var item in Currentwave)
        {            
            eBattlePosition epos = (eBattlePosition)item.PosIndex;
            if ((UnitType)item.EnemyType == UnitType.DefenseTower)
	        {
                yield return StartCoroutine(AddTower(item.EnemyIndex, GetTowerTransform(TeamType.Enemy, epos), TeamType.Enemy));
	        }        
            else
            {

                yield return StartCoroutine(SpawnUnit(item.EnemyIndex, dic_EnemyspawnPos[epos], TeamType.Enemy));
            }
        }
    }

    private IEnumerator SpawnMyCharacters()
    {
        var myDecks = UIManagerScript.Instance.UI_Lobby._deckManager.list_CurrentDeckCards;
        for (int i = 0; i < myDecks.Count; i++)
        {
            eBattlePosition pos = (eBattlePosition)myDecks[i].UnitIndex;
            yield return StartCoroutine(SpawnUnit(myDecks[i].UnitId, dic_AllyspawnPos[pos], TeamType.Ally));
        }
    }

    private Transform GetTowerTransform(TeamType team, eBattlePosition posIndex)
    {
        StructureTransform SpawnPos = (team == TeamType.Ally) ? AllyDefenseTowerPosition : EnemyDefenseTowerPosition;
        
        switch (posIndex)
	    {
            case eBattlePosition.Left: return SpawnPos.tr_Left;
            case eBattlePosition.Right: return SpawnPos.tr_Right;
            case eBattlePosition.Center: return SpawnPos.tr_Center;
            default: return null;
	    }
    }

    
    private IEnumerator AddTower(int _TowerIndex, Transform _Parent, TeamType _Team)
    {        
        Debug.Log("<color=red>StageManager</color> AddTower : " + _TowerIndex);
        TableStructure Structuredata = UnityDataConnector.Instance.m_TableData_Structure.GetData(_TowerIndex);
        if (Structuredata == null)
            yield break;

        Object prefab;

        //풀버전 빌드시에는 Resources.Load로 리소스를 로드합니다.
        if (AssetLoadScript.Instance.IsFullVersionBuild)
        {
            #region 리소스폴더 로드

            prefab = AssetLoadScript.Instance.Get(eAssetType.Structure, Structuredata.ResourceName);

            #endregion 리소스폴더 로드
        }
        else
        {
            #region 어셋번들 로드

            string BundleName = eAssetType.Structure.ToString().ToLower();
            //string TowerName = string.Format("{0}_DefenseTower", _TowerIndex);
            AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync(BundleName, Structuredata.ResourceName, typeof(GameObject));
            if (request == null)
                yield break;
            yield return StartCoroutine(request);

            // Get the asset.
            prefab = request.GetAsset<GameObject>();

            #endregion 어셋번들 로드
        }

        if (prefab != null)
        {
            GameObject tw = GameUtil.CreateObjectToParent(prefab, _Parent);
            DefenseTowerController con = tw.GetComponent<DefenseTowerController>();
            con.SetTeam(_Team, CreateHpBar());
            con.SetData(Structuredata);

            AddLiveData(con);
            //if (_Team == TeamType.Ally)
            //{
            //    Ally_LiveData.LiveBattleObjects.Add(dc);
            //}
            //else
            //{
            //    Enemy_LiveData.LiveBattleObjects.Add(dc);
            //}
        }
    }

    private IEnumerator SpawnUnit(int _UnitIndex, Transform _SpawnTransform, TeamType _Team)
    {
        Debug.Log("<color=red>StageManager</color> SpawnUnit : " + _UnitIndex);

        //유닛 테이블 데이터를 가져옴
        TableUnit unitdata = UnityDataConnector.Instance.m_TableData_Unit.GetUnitData(_UnitIndex);
        if (unitdata == null)
            yield break;

        GameObject prefab;

        //풀버전 빌드시에는 Resources.Load로 리소스를 로드합니다.
        if (AssetLoadScript.Instance.IsFullVersionBuild)
        {
            #region 리소스폴더 로드

            prefab = (GameObject)AssetLoadScript.Instance.Get(eAssetType.Unit, unitdata.ResourceName);

            #endregion 리소스폴더 로드
        }
        else
        {
            #region 어셋번들 로드

            //번들이름과 리소스이름으로 번들 데이터를 로드
            string BundleName = eAssetType.Unit.ToString().ToLower();
            AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync(BundleName, unitdata.ResourceName, typeof(GameObject));
            if (request == null)
                yield break;
            yield return StartCoroutine(request);
            // Get the asset.
            prefab = request.GetAsset<GameObject>();

            #endregion 어셋번들 로드
        }

        if (prefab != null)
        {            
            GameObject tw = GameUtil.CreateObjectToRoot(prefab, _SpawnTransform.position, _SpawnTransform.rotation.eulerAngles, Vector3.one);
            UnitController con = tw.GetComponent<UnitController>();
            con.SetTeam(_Team, CreateHpBar());            
            con.SetData(unitdata);
            con.tr_SpawnedTransform = _SpawnTransform;
            tw.name = string.Format("{0}_{1}", con.BaseAbility.UnitName, _Team);

            AddLiveData(con);

        }
    }

    private void AddLiveData(BattleObject btobj)
    {
        Debug.Log("<color=red>StageManager</color> AddLiveData : " + btobj.Team + " / " + btobj.GetBattleObjectName());
        if (btobj.Team == TeamType.Ally)
        {
            Ally_LiveData.LiveBattleObjects.Add(btobj);
        }
        else
        {
            Enemy_LiveData.LiveBattleObjects.Add(btobj);
        }
    }

    public GameObject CreateHpBar()
    {
        if (prefab_HPBar != null)
        {
            GameObject go = GameUtil.CreateObjectToParent(prefab_HPBar, tr_Canvas);
            return go;
        }

        return null;
    }

    private void Update()
    {
        //CheckOnclickGround();

        if (Input.GetKeyDown(KeyCode.Keypad1))
            Time.timeScale = 1f;

        if (Input.GetKeyDown(KeyCode.Keypad2))
            Time.timeScale = 2f;
    }

    private void CheckOnclickGround()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                //if (hit.transform.name != "Plane")
                //{
                //    Debug.LogError("hit.transform.name != Plane");
                //    return;
                //}

                int _unitid = TeamManagerScript.Instance.GetTeam(TeamType.Ally).GetSelectedUnitId();
                if (_unitid != 0)
                {
                    //덱 사용
                    //UIManagerScript.Instance.BattleDecks.UseDeck();

                    //스폰
                    //StartCoroutine(SpawnUnit(_unitid, hit.point, TeamType.Ally));

                    //선택취소
                    TeamManagerScript.Instance.GetTeam(TeamType.Ally).UnSelect();
                }
            }
        }

    }

    private void CheckAbleSpawnUnit(TeamType _team, Vector3 _SpawnPos)
    {
        TableUnit unitdata = UnityDataConnector.Instance.m_TableData_Unit.GetUnitData(1);
        TeamManagerScript.Instance.CheckAbleSpawn(_team, 1);
    }

    public void RemoveBattleObject(BattleObject _bobj)
    {
        if (_bobj.Team == TeamType.Ally)
        {
            Ally_LiveData.LiveBattleObjects.Remove(_bobj);

            if (Ally_LiveData.IsAllEliminate)
                StartCoroutine(FinishBattle(TeamType.Enemy));
        }
        else
        {
            Enemy_LiveData.LiveBattleObjects.Remove(_bobj);

            if (Enemy_LiveData.IsAllEliminate)
            {
                //다음 웨이브가 있다면 스폰! 없다면 승리!
                if (waveDatas.Exists(r=> r.CrowdIndex == CurrentCrowdIndex + 1))
                {
                    CurrentCrowdIndex++;
                    StartCoroutine(SpawnEnemys());
                }
                else
                {
                    StartCoroutine(FinishBattle(TeamType.Ally));
                }
            }
                
        }
    }



    private IEnumerator FinishBattle(TeamType _winnerTeam)
    {
        Debug.LogError("FinishBattle Winner " + _winnerTeam + " / " + SceneManager.GetActiveScene().buildIndex);

        MessageDispatcher.SendMessage("GameOver");
        //SceneManager.LoadScene("Loading");

        UIManagerScript.Instance.UI_Battle.SetResult(_winnerTeam);
        yield return new WaitForSeconds(1f);

        // #TODO 화면의 모든 터치를 막아야함
        if (_winnerTeam == TeamType.Ally)
        {
            yield return StartCoroutine(NextStage());    
        }
        else
        {
            ReturnTotLobby();
        }        
    }

    public void ReturnTotLobby()
    {
        SceneManager.LoadScene("Loading");
        UIManagerScript.Instance.LobbyOn();
    }

    private IEnumerator NextStage()
    {
        int NextStageIndex = SaveDataManagerScript.Instance.MyPlayHistory.GetCurrentStageIndex() + 1;
        if (UnityDataConnector.Instance.m_TableData_StageEnemyWave.ExistsWaveList(NextStageIndex))
        {
            waveDatas.Clear();
            SaveDataManagerScript.Instance.MyPlayHistory.SetNextStage();
            waveDatas = UnityDataConnector.Instance.m_TableData_StageEnemyWave.GetWaveListData(NextStageIndex);
        }
        else
        {
            Debug.LogError("스테이지 데이터가 더이상 없습니다. " + NextStageIndex);
            ReturnTotLobby();
            yield break;
        }

        BattleUI.ActivateResultLabel(false);
        yield return StartCoroutine(BattleUI.ActivateLoading(true));
        
        //히어로 원위치        
        for (int i = 0; i < Ally_LiveData.LiveBattleObjects.Count; i++)
        {            
            Ally_LiveData.LiveBattleObjects[i].ReturnSpawnedTransform();
        }

        //다음 스테이지 적 스폰
        yield return StartCoroutine(SpawnEnemys());
        yield return StartCoroutine(BattleUI.ActivateLoading(false));
    }


}