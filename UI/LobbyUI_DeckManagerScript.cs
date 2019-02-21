using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum eDeckManagerState
{
    ShowMode,
    Equipmode
}

/// <summary>로비매뉴 - 덱관리</summary>
public class LobbyUI_DeckManagerScript : MonoBehaviour {

    public eDeckManagerState _State = eDeckManagerState.ShowMode;

    public GameObject prefab_UnitInfoSlot;

    public UIScrollView sc;
    public Transform[] tr_DeckSlots;
    public List<SlotUnitInfoScript> list_CurrentDeckCards = new List<SlotUnitInfoScript>();

    public Transform tr_selectmode;
    public SlotUnitInfoScript selectedSlot;

    public UIGrid gr_Havecards;
    public List<SlotUnitInfoScript> list_HaveCards = new List<SlotUnitInfoScript>();

    /// <summary>현재 선택된 덱의 인덱스</summary>
    public int CurrentDeckIndex = 1;

    public UIToggle[] to_Deck;

	void Start () {

        Init();
        
	}
	
    void Init()
    {
        for (int i = 0; i < to_Deck.Length; i++)
        {
            UIEventListener.Get(to_Deck[i].gameObject).onSelect += OnSelectDeckIndex;
        }

        //현재 세팅된 덱 리스트 세팅
        if (SaveDataManagerScript.Instance.MyDecks == null)
        {
            Debug.LogError("덱데이터가 없습니다.");
            SaveDataManagerScript.Instance.MyDecks.Init();
        }

        //최근에 세팅한 덱 인덱스 구하기
        SetDeckIndex(SaveDataManagerScript.Instance.GetDeckIndex_From_PlayerPrefs());

        StartCoroutine(CreateSlots());
    }

    public IEnumerator CreateSlots()
    {
        //빈슬롯 생성
        for (int i = 0; i < tr_DeckSlots.Length; i++)
        {
            GameObject go = GameUtil.CreateObjectToParent(prefab_UnitInfoSlot, tr_DeckSlots[i]);
            SlotUnitInfoScript slot = go.GetComponent<SlotUnitInfoScript>();
            slot.SetEmptySlot(i);
        }
        
        //보유카드 리스트 세팅
        List<BattleUnitData> HaveCardList = SaveDataManagerScript.Instance.MyCards.GetHaveCardList();
        foreach (var item in HaveCardList)
        {
            GameObject go = GameUtil.CreateObjectToParent(prefab_UnitInfoSlot, gr_Havecards.transform);
            SlotUnitInfoScript slot = go.GetComponent<SlotUnitInfoScript>();
            slot.SetData(item, eUnitSlotType.HaveCard);
            list_HaveCards.Add(slot);
        }
        
        //장착 모드 슬롯 세팅
        GameObject go_sel = GameUtil.CreateObjectToParent(prefab_UnitInfoSlot, tr_selectmode);
        selectedSlot = go_sel.GetComponent<SlotUnitInfoScript>();
        selectedSlot.SetEmptySlot();
        ChangeShowMode();

        yield return StartCoroutine(RefreshDeck());
    }

    public void SetDeckIndex(int _Index)
    {
        //Debug.LogError("SetDeckIndex : " + _Index);
        CurrentDeckIndex = _Index;
        
        //토글 세팅
        for (int i = 0; i < to_Deck.Length; i++)
        {
            //Debug.LogError((i + 1) + " / " + _Index);
            if ((i + 1) == _Index)
            {                
                to_Deck[i].value = true;
            }
            else
                to_Deck[i].value = false;
        }
        
        //StartCoroutine(RefreshDeck());
    }

    public void ReturnToHaveCardList(SlotUnitInfoScript _slot)
    {
        _slot.IsEquipedDeck = false;
        _slot._slotType = eUnitSlotType.HaveCard;
        _slot.SetSlotIndex(-1);
        GameUtil.MoveToParent(_slot.gameObject, gr_Havecards.transform);
    }

    /// <summary>대상 슬롯을 보유 카드 그리드로 이동</summary>    
    public void RemoveDeck(SlotUnitInfoScript _slot)
    {
        //Debug.LogError("RemoveDeck : " + _slot.Unitdata.Index);
        ReturnToHaveCardList(_slot);
        SaveDataManagerScript.Instance.MyDecks.RemoveDeck(CurrentDeckIndex, _slot.Unitdata);
    }

    /// <summary>덱 추가</summary>
    /// <param name="_AddData">추가할 유닛 데이터</param>
    public void AddDeck(BattleUnitData _AddData)
    {
        Debug.Log("<color=lime>AddDeck</color>  " + _AddData.Index + " / " + _AddData.UnitID + " / " + _AddData.Name);

        //해당 슬롯에 이미 다른 덱이 있는지 체크(UI)
        //SlotUnitInfoScript _BeforeSlot2 = tr_DeckSlots[_AddData.Index].GetComponent<SlotUnitInfoScript>();
        SlotUnitInfoScript _BeforeSlot = list_CurrentDeckCards.Find(r => r.UnitIndex == _AddData.Index);

        if (_BeforeSlot != null)
            RemoveDeck(_BeforeSlot);
        //else
        //    Debug.LogError("RemoveDeck 없음");

        SlotUnitInfoScript _slot = list_HaveCards.Find(r => r.UnitId == _AddData.UnitID);
        _slot.IsEquipedDeck = true;
        _slot._slotType = eUnitSlotType.DeckCard;        
        _slot.SetSlotIndex(_AddData.Index);
        GameUtil.MoveToParent(_slot.gameObject, tr_DeckSlots[_AddData.Index]);
        _slot.lb_Cost.text = _AddData.Index.ToString();

        //현재 등록덱 리스트에 추가
        list_CurrentDeckCards.Add(_slot);

        SaveDataManagerScript.Instance.MyDecks.AddDeck(CurrentDeckIndex, _AddData);
    }    

    /// <summary>현재 설정된 덱 인덱스 기준으로 리스트를 갱신</summary>
    public IEnumerator RefreshDeck()
    {
        //현재 등록되어 있는 카드는 원위치
        list_CurrentDeckCards.ForEach(r => ReturnToHaveCardList(r));
        list_CurrentDeckCards.Clear();

        //현재 설정된 덱 리스트로 채워준다.
        List<BattleUnitData> DeckList = SaveDataManagerScript.Instance.MyDecks.GetDeckList(CurrentDeckIndex);
        if (DeckList != null)
        {
            DeckList.ForEach(r => AddDeck(r));
        }
        
        gr_Havecards.Reposition();

        yield return null;
    }

    public void ChangeUnitEquipMode(BattleUnitData _Unitdata)
    {
        _State = eDeckManagerState.Equipmode;
        sc.ResetPosition();
        gr_Havecards.gameObject.SetActive(false);
        tr_selectmode.gameObject.SetActive(true);
        selectedSlot.SetData(_Unitdata, eUnitSlotType.SelectTargetSlot);
        selectedSlot.IsFreeze = true;
    }

    public void ChangeShowMode()
    {
        _State = eDeckManagerState.ShowMode;
        sc.ResetPosition();
        gr_Havecards.gameObject.SetActive(true);
        gr_Havecards.Reposition();
        tr_selectmode.gameObject.SetActive(false);
    }

	void OnSelectDeckIndex(GameObject go, bool IsSelect)
    {
        //Debug.LogError("OnSelectDeckIndex : " + go.name + " / " + IsSelect);
        if (IsSelect)
        {
            for (int i = 0; i < to_Deck.Length; i++)
            {
                if (to_Deck[i].gameObject.Equals(go))
                {
                    CurrentDeckIndex = (i+1);
                    SaveDataManagerScript.Instance.SaveDeckIndex_To_PlayerPrefs(CurrentDeckIndex);
                    StartCoroutine(RefreshDeck());
                    Debug.LogError("OnSelectDeckIndex : " + CurrentDeckIndex);
                }
            }            
        }
        
    }

    public void ResetAllDeckInfo()
    {
        SaveDataManagerScript.Instance.ResetDeckDatas();
        StartCoroutine(RefreshDeck());
    }

}
