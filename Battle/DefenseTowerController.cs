using UnityEngine;
using System.Collections;

public class DefenseTowerController : BattleObject {

    readonly string STR_FIREBALL = "Fire_Fireball";

    /// <summary>투사체 발사 시작 위치</summary>
    public Transform tr_ProjectileStartPos;
    //public string AttackEffectName = "Fire_Fireball";

    TableStructure battleObj_Data;

	void Start () {
        base.Start();
        //InvokeRepeating("SearchTarget", 0f, 0.5f);
	}
    
    void SearchTarget()
    {
        base.FindTarget();
    }

    public void SetData(TableStructure unitdata)
    {
        battleObj_Data = unitdata;
		//Debug.LogError("SetData " + unitdata.Index + " / " + unitdata.Structuretype);
        BaseAbility.Index = unitdata.Index;
		BaseAbility.UnitName = unitdata.StructureName;
		BaseAbility.type = (UnitType)unitdata.Structuretype;
        BaseAbility.Level = 1;
        BaseAbility.HP_Current = unitdata.HP_Max;
        BaseAbility.HP_Max = unitdata.HP_Max;
        BaseAbility.P_Atk = unitdata.P_Atk;
        BaseAbility.P_Def = unitdata.P_Def;
        BaseAbility.M_Atk = unitdata.M_Atk;
        BaseAbility.M_Def = unitdata.M_Def;                
        BaseAbility.AttackSpeed = unitdata.AttackSpeed;        
        BaseAbility.AttackRange = unitdata.AtttackRange;
        SetAttackRange(BaseAbility.AttackRange);//사거리 조절
        BaseAbility.AttackTarget = unitdata.AttackTarget;
        BaseAbility.IsAttackGround = unitdata.IsAttackGround;
        BaseAbility.IsAttackSky = unitdata.IsAttackSky;
        BaseAbility.IsAttackTower = unitdata.IsAttackTower;
        
        hpBar.SetValueMax(TotalMaxHP);
        hpBar_followObject.offset = new Vector2(unitdata.HPBarOffSetX, unitdata.HPBarOffSetY);        
        UpdateHpBar();

        base.SetProjectileData(unitdata.ProjectileIndex);
    }    


    /// <summary>타워 공격</summary>
    public void TowerAttack()
    {
        base.RangeAttack(tr_ProjectileStartPos);
       //공격 불가시에는 리턴
       //if (AbleAttack_Flag == false)
       //    return;
        
       //GameObject _effect;
       //Object eff = AssetLoadScript.Instance.Get(eAssetType.Effect, AttackEffectName);

       // //이펙트 생성후 스폰 스타트 위치가존재하면 해당 위치 월드좌표에 포지션 세팅
       //_effect = GameUtil.CreateObjectToParent(eff, null);
       //_effect.gameObject.transform.position = (tr_ProjectileStartPos != null) ? tr_ProjectileStartPos.position : transform.position;

        
       //TableProjectile tp = UnityDataConnector.Instance.m_TableData_Projectile.GetData(battleObj_Data.ProjectileIndex);
       //if (tp != null)
       //{
       //    Debug.LogError(tp.Name + " / " + tp.Prefab + " / " + tp.Speed);
       //    Vector3 efPos = (tr_ProjectileStartPos != null) ? tr_ProjectileStartPos.position : transform.position;
       //    GameObject _effect = EffectSpawnManagerScript.Instance.Create(tp.Prefab, efPos);

       //    BattleProjectile bp = _effect.AddComponent<BattleProjectile>();
       //    bp.Fire(this, AttackTarget, tp.Speed);
       //}
    }

}
