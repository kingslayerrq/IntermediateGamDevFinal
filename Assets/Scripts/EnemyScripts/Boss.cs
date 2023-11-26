using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : BaseEnemy
{
    protected enum BossState
    {
        Spawn,
        ChasePlayer,
        Attack,
        Transform
    }
}
