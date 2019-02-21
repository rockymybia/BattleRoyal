using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;

/// <summary>보유 자원 정보</summary>
public class MyAccountAssetData{

    public MyAccountAssetData(){}
    /// <summary>생성자</summary>
    /// <param name="_gold">골드 수량</param>
    /// <param name="_gem">잼 수량</param>
    public MyAccountAssetData(int _gold, int _gem)
    {
        Gold = _gold;
        Gem = _gem;
    }
   
    public int Gold;
    public int Gem;    

}
