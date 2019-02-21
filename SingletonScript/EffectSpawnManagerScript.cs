using UnityEngine;
using System.Collections;

public class EffectSpawnManagerScript : Singleton<EffectSpawnManagerScript> 
{
    /// <summary>이펙트 생성</summary>
    /// <param name="_name">이펙트 이름</param>
    /// <param name="_pos">생성 위치</param>
    /// <param name="AutoDestroyTime">자동 파괴 시간 , 0f인 경우 파괴하지 않음</param>    
    public GameObject Create(string _name, Vector3 _pos, float AutoDestroyTime = 0f)
    {
       GameObject _effect;
       Object eff = AssetLoadScript.Instance.Get(eAssetType.Effect, _name);        
       _effect = Instantiate(eff) as GameObject;
       _effect.gameObject.transform.position = _pos;

       if (AutoDestroyTime > 0f)
        Destroy(_effect, AutoDestroyTime);

       return _effect;
    }
}
