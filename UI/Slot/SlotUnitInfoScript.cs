using UnityEngine;

public enum eUnitSlotType
{
    EmptySlot,
    DeckCard,
    HaveCard,
    SelectTargetSlot
}

public class SlotUnitInfoScript : MonoBehaviour
{
    private LobbyUI_DeckManagerScript _DeckManager;

    private BattleUnitData _Unitdata;

    public BattleUnitData Unitdata
    {
        get { return _Unitdata; }
        set { _Unitdata = value; }
    }

    private TableUnit UnitBaseData;

    public int UnitIndex
    {
        get
        {
            if (_Unitdata != null)
                return _Unitdata.Index;
            else
                return -1;
        }
    }

    public int UnitId
    {
        get
        {
            if (_Unitdata != null)
                return _Unitdata.UnitID;
            else
                return -1;
        }
    }

    public eUnitSlotType _slotType = eUnitSlotType.HaveCard;

    /// <summary>덱 장착한 상태인가?</summary>
    public bool IsEquipedDeck = false;

    public UITexture tx_UnitIcon;
    public UILabel lb_Unitname;
    public UILabel lb_HaveCount;
    public UILabel lb_Cost;
    public UILabel lb_position;

    /// <summary>버튼 객체를 열지 않는 방식이라면 True</summary>
    public bool IsFreeze = false;

    /// <summary>버튼 객체들이 열려있는 상태인가?</summary>
    private bool IsOnButtons = false;

    public UITable tbl_Buttons;

    /// <summary>업그레이드 버튼</summary>
    public UIButton btn_Upgrade;

    /// <summary>정보 버튼</summary>
    public UIButton btn_ShowInfo;

    /// <summary>장착 버튼</summary>
    public UIButton btn_Equip;

    private void Start()
    {
        UIEventListener.Get(btn_Upgrade.gameObject).onClick += Onclick_Upgrade;
        UIEventListener.Get(btn_ShowInfo.gameObject).onClick += Onclick_ShowInfo;
        UIEventListener.Get(btn_Equip.gameObject).onClick += Onclick_EquipDeck;
        OnController(false);

        _DeckManager = UIManagerScript.Instance.UI_Lobby._deckManager;
    }

    /// <summary>유닛에 빈 정보 세팅</summary>
    public void SetEmptySlot(int _Index = 0)
    {
        _slotType = eUnitSlotType.EmptySlot;
        _Unitdata = new BattleUnitData();
        _Unitdata.Index = _Index;
        lb_position.text = string.Empty;

        IsEquipedDeck = false;
        tx_UnitIcon.mainTexture = null;
        lb_Unitname.text = string.Empty;
        lb_HaveCount.text = string.Empty;
        lb_Cost.text = string.Empty;
        RefreshControllButton();
    }

    /// <summary>슬롯에 유닛 정보 세팅</summary>
    public void SetData(BattleUnitData _data, eUnitSlotType _sType)
    {
        _slotType = _sType;
        IsEquipedDeck = false;
        _Unitdata = _data;
        lb_position.text = (_Unitdata.Index == -1) ? string.Empty : ((eBattlePosition)_Unitdata.Index).ToString();
        UnitBaseData = UnityDataConnector.Instance.m_TableData_Unit.GetUnitData(_data.UnitID);
        Object res = AssetLoadScript.Instance.Get(eAssetType.Texture, string.Format("Unit_{0}", _data.UnitID));

        if (res != null)
        {
            //string _IconTexturePath = string.Format("/Texture/UnitCard/Unit_{0}", _data.UnitID);
            tx_UnitIcon.mainTexture = (Texture2D)res;
        }

        lb_Unitname.text = UnitBaseData.UnitName;
        lb_HaveCount.text = _Unitdata.CardCount.ToString();
        lb_Cost.text = UnitBaseData.Cost.ToString();
        RefreshControllButton();
    }

    /// <summary>버튼 컨트롤 엑티브 갱신</summary>
    public void RefreshControllButton()
    {
        // #TODO 업그레이드 대상이 되는 카드인지 판단하는 부분 향후 추가 예정
        btn_Upgrade.gameObject.SetActive(false);
        btn_ShowInfo.gameObject.SetActive(true);
        btn_Equip.gameObject.SetActive(!IsEquipedDeck);
    }

    public void OnClick_Slot()
    {
        //프리즈로 세팅된 슬롯을 선택하면 쇼모드로 돌아간다.
        if (IsFreeze)
        {
            _DeckManager.ChangeShowMode();
            return;
        }

        if (_DeckManager._State == eDeckManagerState.Equipmode) //장착 모드 일때
        {
            _DeckManager.selectedSlot.Unitdata.Index = UnitIndex;
            _DeckManager.AddDeck(_DeckManager.selectedSlot.Unitdata);
            if (_slotType == eUnitSlotType.DeckCard)
            {
                //현재 장착된 슬롯 데이터는 제거
                _DeckManager.RemoveDeck(this);
            }

            _DeckManager.ChangeShowMode();
        }
        else //보기 모드 일때는 컨트롤러를 보인다.
        {
            if (_slotType != eUnitSlotType.EmptySlot)
            {
                OnController(!IsOnButtons);
            }
        }
    }

    public void OnController(bool IsOn)
    {
        IsOnButtons = IsOn;
        tbl_Buttons.gameObject.SetActive(IsOn);
        if (IsOnButtons)
        {
            RefreshControllButton();
            tbl_Buttons.Reposition();
        }
    }

    /// <summary>업그레이드</summary>
    public void Onclick_Upgrade(GameObject go = null)
    {
        Debug.LogError("Onclick_Upgrade");
    }

    /// <summary>정보 보기</summary>
    public void Onclick_ShowInfo(GameObject go = null)
    {
        Debug.LogError("Onclick_ShowInfo");
    }

    /// <summary>장착</summary>
    public void Onclick_EquipDeck(GameObject go = null)
    {
        Debug.LogError("Onclick_EquipDeck");
        _DeckManager.ChangeUnitEquipMode(this.Unitdata);
        OnController(false);
    }

    public void SetSlotIndex(int index)
    {
        Unitdata.Index = index;
        lb_position.text = (_Unitdata.Index == -1) ? string.Empty : ((eBattlePosition)_Unitdata.Index).ToString();
    }

}