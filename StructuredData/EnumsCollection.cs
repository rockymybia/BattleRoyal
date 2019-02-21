public enum BattleCardType
{
    Unit,
    Magic
}

//모든 Enumarable Data를 명시함
public enum UnitType
{
    GroundUnit = 0,
    SkyUnit = 1,
    DefenseTower = 2,
    Unknown,
}

public enum AttackType
{
    GroundUnit = 0,//지상 유닛을 우선 타겟
    SkyUnit = 1,//공중 유닛을 우선 타겟
    DefenseTower = 2,//타워 우선 타겟
    EveryThing = 99//타입 구분없이 잡히는대로 타겟
}

/// <summary>공격타입</summary>
public enum AttackAttribute
{
    /// <summary>물리공격</summary>
    Melee,
    /// <summary>마법공격</summary>
    Magic,
    /// <summary>마법공격</summary>
    Range
}

public enum TeamType
{
    Ally,
    Enemy,
    Neutrality
}

/// <summary>UI 종류</summary>
public enum UIType
{
    /// <summary>게임시작 UI</summary>
    GameStart,
    /// <summary>로비 컨텐츠 UI</summary>
    LobbyContents,
    /// <summary>배틀 UI</summary>
    Battle,
    /// <summary>상단 자원정보 UI</summary>
    TopAsset,
    /// <summary>하단 매뉴 UI</summary>
    BottomMenu
}

/// <summary>UI 사용처</summary>
public enum UI_UseType
{
    OutGame,
    InGame
}

public enum TableType
{
    None,
    TableVersion,
    Unit,
    Structure,
    ConsumeItem,
    Projectile,
    Stage,
    StageEnemyWave
    //Equipment,
}

public enum BuffType
{
    Heal = 0,
    DurationHPUp = 1,
    MaxHPUp = 2,
    AttackUp = 3,
    DefenseUp = 4,
    EvadeRateUp = 5,
    CriticalRateUp = 6,
    AttackSpeedUp = 7,
    ReflectAttack = 8,
}

public enum eBattlePosition
{
    Left = 0,
    Right = 1,
    Center = 2
}