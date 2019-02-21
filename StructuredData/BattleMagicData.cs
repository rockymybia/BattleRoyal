using UnityEngine;
using System.Collections;
using CodeStage.AntiCheat.ObscuredTypes;

[System.Serializable]
/// <summary>전투 마법 데이터</summary>
public class BattleMagicData
{
    /// <summary>인덱스</summary>
    public ObscuredInt Index;
    /// <summary>마법 ID</summary>
    public ObscuredInt MagicID;
    /// <summary>레벨</summary>
    public ObscuredInt Level;
    /// <summary>카드수량</summary>
    public ObscuredInt CardCount;
}
