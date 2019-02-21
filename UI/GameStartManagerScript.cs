using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>UI - 게임 시작 매니저</summary>
public class GameStartManagerScript : UIBaseScript {

    public UIProgressBar _progress;

    int Current_LoadingStep = 0;
    int Total_LoadingStep = 0;

    GameObject go_ParentManager;
    List<string> list_StartManagers = new List<string>();

	void Start () {
        base.AddToUIManager(this);
        SetStartManagerList();
        StartCoroutine(InitGameData());        
	}

    void SetStartManagerList()
    {
        go_ParentManager = new GameObject("Managers");
        DontDestroyOnLoad(go_ParentManager);
        list_StartManagers.Add("AssetLoadManager");//어셋로드 매니저
        list_StartManagers.Add("GoogleDriveManager"); //구글플레이 매니저
        list_StartManagers.Add("TeamManager");//팀데이터 매니저 로드
        list_StartManagers.Add("GameDataSaveManager");//세이브데이터 매니저 로드                
    }

    IEnumerator InitGameData()
    {        
        //매니저 로드
        Total_LoadingStep = list_StartManagers.Count;

        foreach (var item in list_StartManagers)
        {
            DontDestroyLoad(item);
            NextStep();
        }

        //구글 드라이브에서 테이블 버전 체크
        yield return StartCoroutine(UnityDataConnector.Instance.co_CheckTableVersion());
        //로컬 기본 테이블 데이터 로드
        yield return StartCoroutine(UnityDataConnector.Instance.LoadBaseTable());
        //세이브 데이터 로드
        yield return StartCoroutine(SaveDataManagerScript.Instance.co_Initialize());
        //로비 UI 초기화
        yield return StartCoroutine(UIManagerScript.Instance.co_LoadLobbyUI()); //UI 로드
        
        //Resources.UnloadUnusedAssets();        
        UIManagerScript.Instance.LobbyOn();//로비켬
        gameObject.SetActive(false);        
        yield return null;
    }

    void DontDestroyLoad(string _name)
    {
        //Debug.LogError(_name);
        GameObject go = Instantiate(Resources.Load(_name)) as GameObject;
        go.transform.SetParent(go_ParentManager.transform);
        DontDestroyOnLoad(go);
    }

    void NextStep()
    {
        Current_LoadingStep++;
        _progress.value = (float)Current_LoadingStep / Total_LoadingStep;
    }
	
}
