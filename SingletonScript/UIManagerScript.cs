using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AdvancedInspector;
using UnityEngine.SceneManagement;

[AdvancedInspector(true)]
public class UIManagerScript : Singleton<UIManagerScript>
{
    bool IsInitialized = false;

    [Help(HelpType.Info, "UI 리스트")]    
    public Dictionary<UIType, UIBaseScript> dic_UI = new Dictionary<UIType, UIBaseScript>();

    public LobbyUIManagerScript UI_Lobby
    {
        get
        {
            return dic_UI[UIType.LobbyContents] as LobbyUIManagerScript;
        }
    }

    public BattleDeckSelectUIManagerScript UI_Battle
    {
        get
        {
            return dic_UI[UIType.Battle] as BattleDeckSelectUIManagerScript;
        }
    }

    //[Background("GetBackgroundColor")]
    //[Help(HelpType.Info, "This is an help box")]
    //[Help(HelpType.Warning, HelpPosition.Before, "This box appear before the item.")]
    //[Inspect, Style("ToolbarButton")]

    void Awake()
    {        
        //StartCoroutine(co_LoadLobbyUI());
        StartCoroutine(co_CreateAndAddUI("GameStartManager"));        
    }

	void Start () {
        //GameObject go = (GameObject)Resources.Load("Prefab/UI/GameStartManager");
        //GameUtil.CreateObjectToParent(go, gameObject.transform);
        //SaveDataManagerScript._instance.MyAssets
	}
	
    public void AddUI(UIBaseScript _addui)
    {
        if (dic_UI.ContainsKey(_addui._UIType))
        {
            return;
        }
        else
        {
            GameUtil.MoveToParent(_addui.gameObject, this.transform);
            dic_UI.Add(_addui._UIType, _addui);
        }        
    }

    public IEnumerator co_LoadLobbyUI()
    {   
        //--아웃게임--//

        //로비UI
        yield return StartCoroutine(co_CreateAndAddUI("LobbyUI"));
        //탑UI
        yield return StartCoroutine(co_CreateAndAddUI("TopAssetUI"));
        //바텀UI
        yield return StartCoroutine(co_CreateAndAddUI("BottomeAssetUI"));

        //--인게임--//
        yield return StartCoroutine(co_CreateAndAddUI("Ingame_BattleUI"));
    }

    public void CallBack_CreateUI_GameObject(GameObject go)
    {
        Debug.Log("UI : CreateComplete " + go.name);
    }

    public IEnumerator co_CreateAndAddUI(string _AssetName)
    {
        //Debug.LogError("co_CreateAndAddUI : " + _AssetName);
        //GameObject _go = new GameObject();
        yield return StartCoroutine(AssetLoadScript.Instance.InstantiateGameObjectAsync(eAssetType.UI, _AssetName, CallBack_CreateUI_GameObject));             
        //AddUI(_lobby);
    }

    public UIBaseScript GetUI(UIType _UIType)
    {
        if (dic_UI.ContainsKey(_UIType))
        {
            return dic_UI[_UIType];
        }

        return null;
    }

    
    public void ClearAndDestroyAllUI()
    {
        dic_UI.Clear();
    }


    public void LobbyOn()
    {
        ActivateUI_By_UseType(UI_UseType.OutGame);
        dic_UI[UIType.GameStart].gameObject.SetActive(false);
        UI_Lobby.MoveToContent(2);
    }

    /// <summary>배틀 시작</summary>
    public void StartBattle()
    {
        //StartCoroutine(co_StartBattle());
        //InitBattleDeckSlots        
        ActivateUI_By_UseType(UI_UseType.InGame);
        UI_Battle.ActivateResultLabel(false);
        SceneManager.LoadScene("Battle");
    }

    IEnumerator co_StartBattle()
    {
        yield return null;        
    }

    public void ReturnToLobby()
    {
        //아웃게임UI만 On 해줍니다.
        ActivateUI_By_UseType(UI_UseType.OutGame);
        LobbyOn();
    }

    /// <summary>UseType에 해당하는 UI만 On, 다른 타입은 Off</summary>    
    public void ActivateUI_By_UseType(UI_UseType _OnUseType)
    {
        foreach (var item in dic_UI)
        {
            if (item.Value._UseType == _OnUseType)
            {
                item.Value.gameObject.SetActive(true);
            }
            else
            {
                item.Value.gameObject.SetActive(false);
            }
        }
    }

#if UNITY_EDITOR

    public GameObject CreateAndAddUI(string _AssetName)
    {
        string _path = string.Format("Prefab/UI/{0}", _AssetName);
        GameObject prefab = UnityEditor.PrefabUtility.InstantiatePrefab(Resources.Load(_path)) as GameObject;
        GameUtil.MoveToParent(prefab, transform);
        return prefab;
    }

    [Inspect, Style("Edit_GameStartManager")]
    public void EditGameStartManager() { CreateAndAddUI("GameStartManager"); }

    [Inspect, Style("Edit_LobbyUI")]
    public void EditLobbyUI() { CreateAndAddUI("LobbyUI"); }

    [Inspect, Style("Edit_TopAssetUI")]
    public void EditTopAssetUI() { CreateAndAddUI("TopAssetUI"); }

    [Inspect, Style("Edit_BottomeAssetUI")]
    public void EditBottomeAssetUI() { CreateAndAddUI("BottomeAssetUI"); }

    [Inspect, Style("Ingame_BattleUI")]
    public void EditIngame_BattleUI() { CreateAndAddUI("Ingame_BattleUI"); }
#endif

}
