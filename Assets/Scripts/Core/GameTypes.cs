namespace FogboundMaze
{
    public enum WeaponType
    {
        None,
        SniperRifle = 1,
        LongBlade = 2
    }

    public enum GamePhase
    {
        Title,
        Guide,
        LevelSelect,
        Staging,
        Playing,
        BossIntro,
        BossFight,
        Rescue,
        Paused,
        Won,
        Lost
    }

    public enum DungeonCondition
    {
        Powered,
        Dim,
        Emergency,
        Blackout,
        Contaminated,
        Lockdown,
        DeepCore
    }

    public enum EnemyState
    {
        Inactive,
        Patrol,
        Investigate,
        Chase,
        Search,
        Attack,
        Dead
    }

    public enum BossPhase
    {
        Dormant,
        Intro,
        PhaseOne,
        PhaseTwo,
        PhaseThree,
        Stunned,
        Defeated
    }

    public enum BossActionType
    {
        None,
        Cleave,
        Charge,
        CorruptionShot,
        Summon,
        Shockwave
    }
}
