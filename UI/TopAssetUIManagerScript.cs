using UnityEngine;
using System.Collections;

/// <summary>UI - 탑 자원 정보</summary>
public class TopAssetUIManagerScript : UIBaseScript
{
    public UISlider sl_exp;
    public UILabel lb_exp;
    public UILabel lb_gold;
    public UILabel lb_gem;

	void Start () {
        AddToUIManager(this);
        UpdateAssetInfo();
	}
	
    public void UpdateAssetInfo()
    {
        sl_exp.value = 0;
        lb_exp.text = "0/100";
        lb_gold.text = SaveDataManagerScript.Instance.MyAssets.GetGold().ToString();
        lb_gem.text = SaveDataManagerScript.Instance.MyAssets.GetGem().ToString();
    }

}
