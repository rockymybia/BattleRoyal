using EnergyBarToolkit;
using ICode;
using UnityEngine;
using com.ootii.Messages;
using com.ootii.Utilities;

/// <summary>
/// 전투 객체 BaseClass
/// 타워, 유닛 등...
/// </summary>
public class BattleObject : MonoBehaviour
{
    /// <summary>팀타입</summary>
    public TeamType Team;

    #region 스탯과 관련된 맴버
    /// <summary>기본 능력치</summary>
    private AbilityData _baseAbility = new AbilityData();
    public AbilityData BaseAbility
    {
        get { return _baseAbility; }
        set { _baseAbility = value; }
    }

    //합산 능력치 기본, 장비, 버프
    public int TotalMaxHP { get { return BaseAbility.HP_Max + _buffManager.MaxHP; } }
    public int TotalPAtk { get { return BaseAbility.P_Atk + _buffManager.PAtk; } }
    public int TotalPDef { get { return BaseAbility.P_Def + _buffManager.PAtk; } }
    public int TotalMAtk { get { return BaseAbility.M_Atk + _buffManager.MAtk; } }
    public int TotalMDef { get { return BaseAbility.M_Def + _buffManager.MDef; } }
    public float TotalAttackSpeed { get { return BaseAbility.AttackSpeed + _buffManager.AtkSpeed; } }
    public float TotalCritical { get { return BaseAbility.CriticalRate + _buffManager.CriticalRate; } }
    public float TotalEvade { get { return BaseAbility.EvadeRate + _buffManager.EvadeRate; } }

    protected Animator animator;
    protected BuffManagerScript _buffManager;

    /// <summary>투사체 데이터 정보(원거리 공격을 가진 캐릭터만 세팅됨)</summary>
    protected TableProjectile projectileData;
#endregion

    /// <summary>공격대상</summary>
    public BattleObject AttackTarget;

    public UnitType unitType { get { return BaseAbility.type; } }

    private bool IsLOCKAttack_ = false;

    /// <summary>공격 잠금
    /// 디버프로 인한 공격 불가시에 True로 설정한다
    /// </summary>
    public bool IsLockAttack
    {
        get { return IsLOCKAttack_; }
        set { IsLOCKAttack_ = value; }
    }

    public bool IsUnitDead { get { return BaseAbility.HP_Current <= 0; } }

    /// <summary>전투 AI</summary>
    private ICodeBehaviour _icode;

    /// <summary>공격할 수 있는 상태인가?</summary>
    public bool AbleAttack_Flag
    {
        get
        {
            //공격할 대상이 없다면 False
            if (AttackTarget == null)
                return false;

            //공격이 차단된 상태라면 False
            if (IsLockAttack)
                return false;

            return true;
        }
    }

    public EnergyBar hpBar;
    public EnergyBarFollowObject hpBar_followObject;

    public Transform tr_SpawnedTransform;
    
    public void ReturnSpawnedTransform()
    {
        this.transform.position = tr_SpawnedTransform.position;
        this.transform.eulerAngles = tr_SpawnedTransform.eulerAngles;
    }


    protected void Start()
    {        
        InvokeRepeating("FindTarget", 0f, 0.5f);
        MessageDispatcher.AddListener("GameOver", OnGameOverMessageReceived);      
    }

    protected void OnDestroy()
    {
        Debug.Log("<color=red>BattleObject</color> OnDestroy : " + GetBattleObjectName());
        MessageDispatcher.RemoveListener("GameOver", OnGameOverMessageReceived);
    }

    public void OnGameOverMessageReceived(IMessage rMessage)
    {
        if (hpBar != null)
        {
            hpBar.gameObject.SetActive(false);    
        }        
    }

    public void SetTeam(TeamType _Team, GameObject go_hpBar)
    {        
        _icode = gameObject.GetBehaviour();
        Team = _Team;

        _buffManager = this.GetOrAddComponent<BuffManagerScript>();  
        SetHpBar(go_hpBar);
    }

    /// <summary>타겟을 찾는다
    /// 1. 내 시야에 들어온 적이 있는지 먼저 체크한다.
    /// 2. 시야에 들어왔다면 공격 가능한 대상인지 체크 후 가능하다면 타겟으로 잡는다.
    /// 3. 공격 가능 대상이 아니라면 타워를 타겟으로 잡고 이동한다.
    /// </summary>
    public void FindTarget()
    {
        //가까운 위치의 적을 찾음
        if (Team == TeamType.Ally)
        {
            AttackTarget = StageManager._instance.Enemy_LiveData.FindNearEnemyObject(this);
        }
        else
        {
            AttackTarget = StageManager._instance.Ally_LiveData.FindNearEnemyObject(this);
        }

        if (AttackTarget != null)
            _icode.stateMachine.GetVariable("AttackTarget").SetValue(AttackTarget.gameObject);
    }

    /// <summary>공격 사정 거리 세팅</summary>
    protected void SetAttackRange(float _value)
    {
        _icode.stateMachine.GetVariable("AttackRange").SetValue(_value);
    }

    /// <summary>공격 후 딜레이 세팅</summary>    
    protected void SetAttackDelay(float _value)
    {
        _icode.stateMachine.GetVariable("AttackDelay").SetValue(_value);
    }

    public void ExcuteDeath()
    {
        Debug.Log("<color=red>BattleObject</color> ExcuteDeath " + GetBattleObjectName());

        //스테이지 매니저에서 해당 오브젝트를 생존 객체 리스트에서 제거
        StageManager._instance.RemoveBattleObject(this);

        //타워인 경우 파괴 이펙트 출력
        if (BaseAbility.type == UnitType.DefenseTower)
        {
            EffectSpawnManagerScript.Instance.Create("Destroy_Tower", this.transform.position, 1f);
        }

        //Fsm에 사망 플래그 세팅
        _icode.stateMachine.SetVariable("IsDead", true);

        // #TODO 객체 파괴처리 필요함
        GameObject.Destroy(hpBar.gameObject);
        GameObject.Destroy(gameObject, 3f);

        
    }

    public void TryAttack()
    {
        if (AttackTarget == null)
            return;

        Debug.Log("<color=lime>TryAttack </color>" + GetBattleObjectName() + " -> " + BaseAbility.P_Atk + " / " + AttackTarget.gameObject.name);
        //_icode.stateMachine.SetVariable("AttackType", Random.Range(0, 2));
        AttackTarget.HitByEnemy(TotalPAtk);

        //if (Ability.type == UnitType.DefenseTower) AttackTarget.HitByEnemy(1);
        //else AttackTarget.HitByEnemy(100);
    }

    public void AttackEnd()
    {
        Debug.Log("<color=orange>BattleObject</color> AttackEnd " + GetBattleObjectName());
        _icode.stateMachine.SetVariable("AttackStart", false);
    }

    public void HitByEnemy(int _Power, AttackAttribute _atype = AttackAttribute.Melee)
    {
        ReduceHP(_Power);
    }

    private void ReduceHP(int _Power)
    {        
        BaseAbility.HP_Current -= _Power;

        Debug.Log("<color=red>BattleObject</color> ReduceHP - " + GetBattleObjectName() + " ReduceHP : " + BaseAbility.HP_Current);

        if (BaseAbility.HP_Current <= 0)
        {
            BaseAbility.HP_Current = 0;
            ExcuteDeath();
        }

        hpBar.SetValueCurrent(BaseAbility.HP_Current);        
        //UpdateHpBar();
    }

    public void UpdateHpBar()
    {
        hpBar.ChangeValueCurrent(BaseAbility.HP_Current);
    }

    public void SetLockAttack(bool _isLock)
    {
        IsLockAttack = _isLock;
    }

    #region Ability 데이터 리턴 메소드

    public string GetBattleObjectName()
    {
        if (BaseAbility != null)
        {
            return BaseAbility.UnitName;
        }

        return string.Empty;
    }

    /// <summary>배틀오브젝트 타입 가져오기</summary>
    public UnitType GetBattleObjectType()
    {
        if (BaseAbility != null)
        {
            return BaseAbility.type;
        }

        return UnitType.Unknown;
    }

    /// <summary>방어타워 인가?</summary>
    public bool IsDefenseTower()
    {
        return (GetBattleObjectType() == UnitType.DefenseTower) ? true : false;
    }

    /// <summary>유닛인가?</summary>
    public bool IsUnit()
    {
        return (IsGroundUnit() || IsSkyUnit()) ? true : false;
    }

    /// <summary>지상 유닛인가?</summary>
    public bool IsGroundUnit()
    {
        return (GetBattleObjectType() == UnitType.GroundUnit) ? true : false;
    }

    /// <summary>공중 유닛인가?</summary>
    public bool IsSkyUnit()
    {
        return (GetBattleObjectType() == UnitType.SkyUnit) ? true : false;
    }

    #endregion Ability 데이터 리턴 메소드

    public void UpdateDistance()
    {
        if (AttackTarget != null)
        {
            if (AttackTarget.IsUnitDead)
            {
                AttackTarget = null;
                _icode.stateMachine.GetVariable("AttackTarget").SetValue(null);
                return;
            }

            Vector3 attackerPos = new Vector3(transform.position.x, 0f, transform.position.z);
            Vector3 targetPos = new Vector3(AttackTarget.transform.position.x, 0f, AttackTarget.transform.position.z);
            float dist = Vector3.Distance(attackerPos, targetPos);
            _icode.stateMachine.SetVariable("Distance", dist);
        }
    }

    protected void SetHpBar(GameObject m_hp_bar)
    {
        m_hp_bar.transform.localScale = new Vector3(0.1f, 0.1f, 1f);
        hpBar = m_hp_bar.GetComponent<EnergyBar>();
        hpBar_followObject = hpBar.GetComponent<EnergyBarFollowObject>();
        hpBar_followObject.followObject = this.gameObject;
        //hpBar_followObject. = Camera.main;        
    }

    protected void UseBuff(BuffType bfType)
    {
        _buffManager.BuffOn(bfType);
        RefreshAbility();
    }

    protected void RefreshAbility()
    {
        Debug.Log("<color=red>BattleObject</color> RefreshAbility : " + TotalAttackSpeed);
        animator.speed = TotalAttackSpeed;
    }
    
    protected void SetProjectileData(int projectileIndex)
    {
        if (projectileIndex > 0)
        {
            var pjData = UnityDataConnector.Instance.m_TableData_Projectile.GetData(projectileIndex);
            if (pjData != null)
	        {
                projectileData = pjData;
	        }
        }            
    }

    protected void RangeAttack(Transform tr_ProjectileStartPos = null) //원거리 공격용
    {
        //공격 불가시에는 리턴
        if (AbleAttack_Flag == false)
            return;

        if (projectileData != null)
        {
            //Debug.LogError(projectileData.Name + " / " + projectileData.Prefab + " / " + projectileData.Speed);
            Vector3 efPos = (tr_ProjectileStartPos != null) ? tr_ProjectileStartPos.position : transform.position;
            //Vector3 efPos = transform.position;
            GameObject _effect = EffectSpawnManagerScript.Instance.Create(projectileData.Prefab, efPos);

            BattleProjectile bp = _effect.AddComponent<BattleProjectile>();
            bp.Fire(this, AttackTarget, projectileData.Speed, projectileData.DestroyEffectName);
        }

    }

}