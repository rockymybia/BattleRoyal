using UnityEngine;
using System.Collections;

/// <summary>배틀 카드 인터페이스</summary>
public interface IBattleCard{


    int GetCardCount();
    int GetUpgradeCost();
    bool CheckAbleUpgrade();

    
}
