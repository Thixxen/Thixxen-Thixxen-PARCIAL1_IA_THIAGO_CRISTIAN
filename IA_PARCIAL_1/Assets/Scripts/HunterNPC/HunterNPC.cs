using UnityEngine;
using System.Collections.Generic;

public enum HunterStates { Patrol, Attack, Gather }

public class HunterNPC : MonoBehaviour
{
    
    public StateMachine stateMachine;

    [SerializeField] private float _tba = 2.0f;   //tiempo entre ataques
    [SerializeField] private float _rangeAttackRadius = 10.0f;  // rango de ataque
    [SerializeField] private float _meleeAttackRadius = 2.0f;   // rango de ataque cuerpo a cuerpo
    [SerializeField] private float _speed = 3f;

    public float TBA => _tba;
    public float rangeAttackRadius => _rangeAttackRadius;
    public float meleeAttackRadius => _meleeAttackRadius;
    public float Speed => _speed;

    [SerializeField] private Transform[] _waypoints;
    [SerializeField] private float _waypointCheckDistance = 0.5f;

    public Transform[] waypoints => _waypoints;
    public float waypointCheckDistance => _waypointCheckDistance;

    [SerializeField] private GameObject _objectOfInterstPrefab;
    [SerializeField] private float _spawnInterval = 3f;
    [SerializeField] private List<GameObject> _activeObjects = new List<GameObject>();

    public GameObject ObjectOfInterestPrefab => _objectOfInterstPrefab;
    public float spawnInterval => _spawnInterval;
    public List<GameObject> activeObjects => _activeObjects;

    [SerializeField] private float _perceptionRadius = 15f;
    [SerializeField] private LayerMask _boid;

    public float perceptionRadius => _perceptionRadius;
    public LayerMask boid => _boid;

    public float attackTimer { get; private set; }
    public Transform currentTarget { get; set; }

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
        if (attackTimer < _tba)
        {
            attackTimer = Time.deltaTime;
        }

        stateMachine.Update();
    }
    public void RestAttackTimer()
    {
        attackTimer = 0f;
    }
}
