using UnityEngine;
using System.Collections.Generic;

public enum HunterStates { Patrol, Attack, Gather }

public class HunterNPC : MonoBehaviour
{
    
    public StateMachine stateMachine;

    public float TBA = 2.0f;   //tiempo entre ataques
    public float RangeAttackRadius = 10.0f;  // rango de ataque
    public float MeleeAttackRadius = 2.0f;   // rango de ataque cuerpo a cuerpo
    public float speed = 3f;

    public Transform[] waypoints;
    public float waypointCheckDistance = 0.5f;

    public GameObject objectOfInterstPrefab;
    public float spawnInterval = 3f;
    public List<GameObject> activeObjects = new List<GameObject>();

    private void Awake()
    {
        stateMachine = new StateMachine();
        HunterPatrolState patrolState = new HunterPatrolState(this, stateMachine);
        HunterAttackState attackState = new HunterAttackState(this, stateMachine);
        HunterGatherState gatherState = new HunterGatherState(this, stateMachine);

        stateMachine.RegisterState(HunterStates.Patrol, patrolState);
        stateMachine.RegisterState(HunterStates.Attack, attackState);
        stateMachine.RegisterState(HunterStates.Gather, gatherState);

        stateMachine.ChangeState(HunterStates.Patrol);

    }


    private void Update()
    {
        stateMachine.Update();
    }


}
