using UnityEngine;
using System.Collections;
using AdvancedInspector;
using UnityEngine.SceneManagement;

/// <summary>UI - 로비</summary>
public class LobbyUIManagerScript : UIBaseScript {

    public UIScrollView sc;
    public UIGrid gr;
    public UICenterOnChild coc;
    public UILabel lb_currentStage;

    public LobbyUI_DeckManagerScript _deckManager;    

	void Start () {
        base.AddToUIManager(this);
	}

    void OnEnable()
    {
        int index = SaveDataManagerScript.Instance.MyPlayHistory.GetCurrentStageIndex();
        lb_currentStage.text = string.Format("Stage {0}", index);
    }

    [Inspect, Style("MoveToContent")]
    public void MoveToContent(int MenuIndex)
    {
        coc.CenterOn(gr.GetChild(MenuIndex));
    }

    public void OnClick_StartBattle()
    {
        //현재 세팅된 덱이 2개 이상일때만 전투 시작 가능
        if (SaveDataManagerScript.Instance.GetCurrentDeckCount() < 2)
        {
            Debug.LogError("2개 이상의 카드가 장착된 상태에만 전투에 진입할 수 있습니다.");
            return;
        }

        UIManagerScript.Instance.StartBattle();
    }

    public void ResetPlayHistory()
    {
        SaveDataManagerScript.Instance.ResetPlayHistory();
        OnEnable();
    }

}
