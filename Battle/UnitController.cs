using com.ootii.Messages;
using UnityEngine;

public class UnitController : BattleObject
{
    //public string AttackEffectName = "Fire_Fireball";

    private NavMeshAgent NMA;

    private void Start()
    {
        //NMA = GetComponent<NavMeshAgent>();        
        //_buffManager = GetComponent<BuffManagerScript>();
        base.Start();
        //InvokeRepeating("SearchTarget", 0f, 0.5f);
        //MessageDispatcher.AddListener("GameOver", OnGameOverMessageReceived);
    }

    private void SearchTarget()
    {
        base.FindTarget();
    }

    public void SetNavMeshAgent_Speed(float _speed)
    {
        NMA.speed = _speed * 2;
        base.animator.speed = _speed * 1.5f;
    }

    /// <summary>유닛 능력치 데이터 세팅</summary>
    public void SetData(TableUnit unitdata)
    {
        NMA = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        BaseAbility.Index = unitdata.Index;
        BaseAbility.UnitName = unitdata.UnitName;
        BaseAbility.type = unitdata.type;
        BaseAbility.Level = 1;
        BaseAbility.HP_Current = unitdata.HP_Max;
        BaseAbility.HP_Max = unitdata.HP_Max;
        BaseAbility.P_Atk = unitdata.P_Atk;
        BaseAbility.P_Def = unitdata.P_Def;
        BaseAbility.M_Atk = unitdata.M_Atk;
        BaseAbility.M_Def = unitdata.M_Def;
        BaseAbility.MoveSpeed = unitdata.MoveSpeed;
        SetNavMeshAgent_Speed(BaseAbility.MoveSpeed);
        BaseAbility.AttackSpeed = unitdata.AttackSpeed;
        
        BaseAbility.SpawnTime = unitdata.SpawnTime;
        BaseAbility.AttackRange = unitdata.AtttackRange;
        SetAttackRange(BaseAbility.AttackRange);
        BaseAbility.AttackDelay = unitdata.AttackDelay;
        SetAttackDelay(BaseAbility.AttackDelay);
        BaseAbility.AttackTarget = unitdata.AttackTarget;
        BaseAbility.IsAttackGround = unitdata.IsAttackGround;
        BaseAbility.IsAttackSky = unitdata.IsAttackSky;
        BaseAbility.IsAttackTower = unitdata.IsAttackTower;
        CustumizeUnit(unitdata);

        hpBar.SetValueMax(TotalMaxHP);
        hpBar_followObject.offset = new Vector2(unitdata.HPBarOffSetX, unitdata.HPBarOffSetY);
        UpdateHpBar();

        base.SetProjectileData(unitdata.ProjectileIndex);

    }

    /// <summary>모델링 커스터마이즈</summary>
    public void CustumizeUnit(TableUnit unitdata)
    {
        //테이블 데이터에 있는 크기에 맞게 조절
        transform.localScale = new Vector3(unitdata.UnitScale, unitdata.UnitScale, unitdata.UnitScale);
    }

    private void Update()
    {
        base.UpdateDistance();        
        //if (Input.GetKeyDown(KeyCode.Space) && Team == TeamType.Ally)
        //{
        //    UseBuff(BuffType.AttackSpeedUp);
        //}
    }

    //private void UseBuff(BuffType bfType)
    //{
    //    _buffManager.BuffOn(bfType);
    //    RefreshAbility();
    //}

    //public void RefreshAbility()
    //{
    //    Debug.LogError("RefreshAbility : " + TotalAttackSpeed);
    //    animator.speed = TotalAttackSpeed;
    //}

    

    public void RangeAttack() //원거리 공격용
    {
        base.RangeAttack();
        //공격 불가시에는 리턴
        //if (AbleAttack_Flag == false)
        //    return;

        //if (base.projectileData != null)
        //{
        //    Debug.LogError(base.projectileData.Name + " / " + base.projectileData.Prefab + " / " + base.projectileData.Speed);
        //    //Vector3 efPos = (tr_ProjectileStartPos != null) ? tr_ProjectileStartPos.position : transform.position;
        //    Vector3 efPos = transform.position;
        //    GameObject _effect = EffectSpawnManagerScript.Instance.Create(base.projectileData.Prefab, efPos);

        //    BattleProjectile bp = _effect.AddComponent<BattleProjectile>();
        //    bp.Fire(this, AttackTarget, base.projectileData.Speed);
        //}

    }

}