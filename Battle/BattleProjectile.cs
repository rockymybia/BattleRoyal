using UnityEngine;
using System.Collections;
using DG.Tweening.Plugins;

public class BattleProjectile : MonoBehaviour
{
    public bool IsFlying = false;

    [SerializeField]
    private BattleObject Attacker;

    [SerializeField]
    private BattleObject Defender;

    [SerializeField]
    private int _power = 0;
    [SerializeField]
    private float _speed = 0;

    private readonly string DEFAULT_DestroyEffectName = "Destroy_Fireball";
    private string STR_DestroyEffectName = "";

    public void Fire(BattleObject _Attacker, BattleObject _Defender, float ProjectileSpeed, string destroyEffectName = "")
    {
        IsFlying = true;
        Attacker = _Attacker;
        Defender = _Defender;
        _speed = ProjectileSpeed;

        _power = _Attacker.BaseAbility.P_Atk;

        //STR_DestroyEffectName = (destroyEffectName != "") ? destroyEffectName : DEFAULT_DestroyEffectName;
        STR_DestroyEffectName = destroyEffectName;

        this.transform.LookAt(_Defender.transform);

        Hashtable _hash = new Hashtable();
        _hash.Add("position", Defender.transform.position + new Vector3(0f, 1f, 0f));
        _hash.Add("speed", ProjectileSpeed);
        _hash.Add("easetype", iTween.EaseType.linear);
        _hash.Add("oncomplete", "EndFire");
        iTween.MoveTo(gameObject, _hash);
    }



    private bool ValidCheck(Collider other)
    {
        if (Attacker == null || other.gameObject == null)
        {
            EndFire();
            return false;
        }
        
        if (other.gameObject.Equals(Attacker.gameObject))
            return false;

        return true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (ValidCheck(other) == false)
            return;           
        
        BattleObject _script = other.GetComponent<BattleObject>();
        if (_script != null)
        {
            if (_script == Defender)
            {
                Attacker.TryAttack();
                EndFire();
            }
        }
    }

    private void EndFire()
    {
        IsFlying = false;

        if (Attacker != null)
            Debug.Log("EndFire " + Attacker.name);
        
        //충돌 이펙트 생성
        EffectSpawnManagerScript.Instance.Create(STR_DestroyEffectName, transform.position, 1f);
        //파괴
        Destroy(gameObject);
    }

    private void Update()
    {
        if (IsFlying)
        {
            if (Defender == null)
                EndFire();            
        }
    }
}