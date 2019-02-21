using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>UI - 인게임 배틀 덱 컨트롤 UI</summary>
public class BattleDeckSelectUIManagerScript : UIBaseScript {

    public Transform[] tr_UnitSlots;

    public Transform tr_NextDeck;

    public GameObject prefab_Slot;

    public Transform tr_PoolParent;

    public GameObject go_Loading;

    public TweenAlpha twa_loading;
    private UILabel lb_loading;

    TeamData MyTeamData;

    List<SlotInGameBattleScript> MyDeckslots = new List<SlotInGameBattleScript>();

    //Queue<SlotInGameBattleScript> OrderQueue = new Queue<SlotInGameBattleScript>();

    SlotInGameBattleScript selectedDeck;
    SlotInGameBattleScript NextDeck;



	void Start () {
        AddToUIManager(this);
        lb_loading = twa_loading.GetComponentInChildren<UILabel>();
	}
	
    public void InitBattleDeckSlots()
    {
        //lb_Result.gameObject.SetActive(false);
        //StartCoroutine(co_SetUpBattleDeckPool());
    }

    /// <summary>배틀덱 풀을 세팅합니다.</summary>    
    IEnumerator co_SetUpBattleDeckPool()
    {
        yield return StartCoroutine(TeamManagerScript.Instance.InitTeamData());
        MyTeamData = TeamManagerScript.Instance.GetTeam(TeamType.Ally);

        //난수 리스트생성
        List<int> _list = GetRandomNumbers(0, (MyTeamData.BattleDeckDataList.Count - 1));

        //슬롯 생성
        for (int i = 0; i < _list.Count; i++)
        {
            //Debug.LogError("슬롯 생성 " + _list[i]);
            GameObject go = GameUtil.CreateObjectToParent(prefab_Slot, tr_PoolParent);
            go.name = MyTeamData.BattleDeckDataList[_list[i]].Name;
            SlotInGameBattleScript slot = go.GetComponent<SlotInGameBattleScript>();
            slot.SetData(MyTeamData.BattleDeckDataList[_list[i]]);            
            ReturnToDeckList(slot);
        }

        //먼저 추가된 4개의 데이터를 슬롯에 배치한다.
        int _stayDecksCount = (MyDeckslots.Count > 4) ? 4 : MyDeckslots.Count - 1;

        for (int i = 0; i < _stayDecksCount; i++)
        {
            //슬롯 인덱스에 배치한다.            
            GameUtil.SetWorldPosition(MyDeckslots[0].gameObject, tr_UnitSlots[i].position);
            MyDeckslots[0].gameObject.transform.localScale = Vector3.one;
            MyDeckslots[0].SetAbleClick(true);
            MyDeckslots[0].SetStateStay(false);
            MyDeckslots.RemoveAt(0);
        }

        UpdateNextDeckSlot();

        yield return null;
    }

    public void SelectDeck(SlotInGameBattleScript _slot)
    {
        if (selectedDeck != null)
        {
            selectedDeck.SetSelectOnOff(false);
        }

        _slot.SetSelectOnOff(true);
        selectedDeck = _slot;
        TeamManagerScript.Instance.SelectUnit(_slot.UnitData);

    }

    public void UseDeck()
    {        
        //현재 선택된 덱의 부모 저장        
        Debug.LogError("UseDeck : " + selectedDeck.name);

        //대기에 있던 덱의 클릭을 가능하게한다.
        NextDeck.SetAbleClick(true);
        NextDeck.gameObject.transform.localScale = Vector3.one;
        GameUtil.SetWorldPosition(NextDeck.gameObject, selectedDeck.gameObject.transform.position);        

        UpdateNextDeckSlot();
        ReturnToDeckList(selectedDeck);
        //Debug.LogError("MoveToParent : " + NextDeck.UnitData.UnitID + " / " + selectedDeck.name);        
    }

    /// <summary>다음 덱 슬롯을 세팅</summary>
    void UpdateNextDeckSlot()
    {
        Debug.LogError("UpdateNextDeckSlot : " + MyDeckslots[0].UnitData.Name);
        NextDeck = MyDeckslots[0];        
        NextDeck.SetAbleClick(false);
        NextDeck.SetStateStay(false);
        MyDeckslots.RemoveAt(0);
        GameUtil.SetWorldPosition(NextDeck.gameObject, tr_NextDeck.position);
        NextDeck.gameObject.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
    }

    void ReturnToDeckList(SlotInGameBattleScript _slot)
    {
        //Debug.LogError("ReturnToDeckList : " + _slot.UnitData.UnitID);
        GameUtil.SetWorldPosition(_slot.gameObject, tr_PoolParent.position);        
        _slot.gameObject.SetActive(true);
        _slot.SetSelectOnOff(false);
        _slot.SetAbleClick(false);
        _slot.SetStateStay(true);
        //_slot.SetStateStay(true);
        MyDeckslots.Add(_slot);
    }

    /// <summary>
    /// 지정된 숫자의 범위 갯수만큼 중복되지 않는 난수 배열을 생성합니다.<br />
    /// 난수 범위: start ~ end (* end - 1 이 아님에 유의)
    /// </summary>
    /// <param name="start">난수의 시작 값 입니다.</param>
    /// <param name="end">난수의 끝 값 입니다.</param>
    public static List<int> GetRandomNumbers(int start, int end)
    {
        // 시작 인덱스, 종료 인덱스, 숫자 갯수, 반복 조건식 값
        // 그리고 원본 숫자 및 결과 숫자를 저장할 목록(List)을 초기화한다.
        int startIndex = start > end ? end : start;
        int endIndex = start > end ? start : end;
        int nCount = endIndex - startIndex + 1;
        int nLoopCount = startIndex + endIndex + 1;
        List<int> numberList = new List<int>(nCount);
        List<int> resultList = new List<int>(nCount);

        // 원본 목록에 start 부터 end 까지의 값을 집어넣는다.
        for (int i = startIndex; i < nLoopCount; i++)
            numberList.Add(i);

        // 원본 목록에 값이 있을 경우 계속 반복한다.
        while (numberList.Count > 0)
        {            
            // 0 부터 원본 목록의 항목 갯수까지의 난수를 생성한다.
            int pickedIndex = Random.Range(0, numberList.Count);

            // 원본 목록에서 값을 가져온 다음 결과 목록에 추가하고
            // 가져온 값을 제거한다.
            resultList.Add(numberList[pickedIndex]);
            numberList.RemoveAt(pickedIndex);
        }

        // 결과 목록을 배열로 만들어서 반환한다.
        return resultList;
    }

    #region Result
    public UILabel lb_Result;
    public void SetResult(TeamType _type)
    {
        
        if (_type == TeamType.Ally)
        {
            lb_Result.text = "VICTORY";
        }
        else
        {
            lb_Result.text = "DEFEAT";
        }
        ActivateResultLabel(true);
        lb_Result.GetComponent<TweenAlpha>().PlayForward();
    }
    #endregion

    public void ActivateResultLabel(bool isActivate)
    {
        lb_Result.gameObject.SetActive(isActivate);
    }

    public IEnumerator ActivateLoading(bool isLoading)
    {
        twa_loading.enabled = true;
        if (isLoading)
        {
            int index = SaveDataManagerScript.Instance.MyPlayHistory.GetCurrentStageIndex();
            lb_loading.text = string.Format("Stage {0}", index);
            twa_loading.PlayForward();    
        }
        else
        {
            twa_loading.PlayReverse();
        }

        yield return new WaitForSeconds(twa_loading.duration + 1f);
        twa_loading.enabled = false;
    }

    public void ReturnTotLobby()
    {
        StageManager._instance.ReturnTotLobby();
    }

}
