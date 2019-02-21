using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;



/// <summary>팀데이터와 유닛 스폰을 관리합니다. (로비에서도 팀 데이터를 이용합니다.)</summary>
public class TeamManagerScript : Singleton<TeamManagerScript> 
{    
    //public List<TeamData> TeamDatas2 = new List<TeamData>();
    
    /// <summary>팀 데이터 딕셔너리 (아군, 적군)</summary>
    public Dictionary<int, TeamData> TeamDatas = new Dictionary<int, TeamData>();
    
    /// <summary>전투 시작했는가?</summary>
    public bool IsBattleStart = false;

    void Awake()
    {
        
    }

	void Start () {
        IsBattleStart = false;
	}
	
    public IEnumerator InitTeamData()
    {        
        //아군 세팅
        TeamData myteam = new TeamData(1);
        myteam.InitDeckinfo(1, SaveDataManagerScript.Instance.GetCurrentDeckList());
        TeamDatas.Add(1, myteam);

        //적군 세팅
        TeamData enemyteam = new TeamData(2);
        enemyteam.InitDeckinfo(2, SaveDataManagerScript.Instance.GetCurrentDeckList());
        TeamDatas.Add(2, enemyteam);

        yield return null;
    }
	
	void Update () {

        if (IsBattleStart)
        {
            TeamDatas[1].ChargingCost();
            TeamDatas[2].ChargingCost();    
        }
        
        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    TeamDatas[1].SelectUnit(0);
        //}

        //if (Input.GetKeyDown(KeyCode.Alpha2))
        //{
        //    TeamDatas[1].SelectUnit(1);
        //}
	}

    public void SelectUnit(BattleUnitData _Data)
    {
        TeamDatas[1].SelectUnit(_Data);
    }

    /// <summary>팀데이터 가져오기</summary>
    /// <param name="_type">팀 타입</param>    
    public TeamData GetTeam(TeamType _type)
    {
        if (_type == TeamType.Ally)
        {
            return TeamDatas[1];
        }
        else
        {
            return TeamDatas[2];
        }
    }

    /// <summary>스폰 전 배틀 코스트 충분한가?</summary>    
    public bool CheckAbleSpawn(TeamType _type, int NeedBC)
    {
        TeamData _tdata = GetTeam(_type);
        return _tdata.CheckBattleCost(NeedBC);
    }

}
