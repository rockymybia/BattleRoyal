using UnityEngine;
using System.Collections;

/// <summary>인게임 배틀카드 슬롯</summary>
public class SlotInGameBattleScript : MonoBehaviour {

    public UITexture tx_unitIcon;
    public UILabel lb_cost;

    BattleUnitData UnitData_;
    public BattleUnitData UnitData
    {
        get { return UnitData_; }
        set { UnitData_ = value; }
    }

    public bool IsSelect = false;
    public bool AbleClick = true;
    public UITexture tx_selected;

	void Start () {
        UIEventListener.Get(tx_unitIcon.gameObject).onClick += OnclickSlot;
        SetSelectOnOff(false);
	}
	
    public void OnclickSlot(GameObject go = null)
    {
        if (AbleClick == false)
            return;

        Debug.LogError("OnclickSlot");        
        //UIManagerScript.Instance.BattleDecks.SelectDeck(this);
    }

    public void SetData(BattleUnitData _Data)
    {
        UnitData = _Data;
        TableUnit UnitBaseData = UnityDataConnector.Instance.m_TableData_Unit.GetUnitData(_Data.UnitID);
        Object res = AssetLoadScript.Instance.Get(eAssetType.Texture, string.Format("Unit_{0}", _Data.UnitID));

        if (res != null)
        {
            //string _IconTexturePath = string.Format("/Texture/UnitCard/Unit_{0}", _data.UnitID);
            tx_unitIcon.mainTexture = (Texture2D)res;
        }

        //lb_cost.text = UnitBaseData.Cost.ToString();
        lb_cost.text = UnitBaseData.Index.ToString();
    }
	
    public void SetSelectOnOff(bool _isselect)
    {
        IsSelect = _isselect;
        tx_selected.enabled = IsSelect;        
    }

    public void SetAbleClick(bool _IsAbleClick)
    {
        AbleClick = _IsAbleClick;        
    }
	
    public void SetStateStay(bool IsStay)
    {
        tx_unitIcon.color = IsStay ? Color.red : Color.white;
    }

}
