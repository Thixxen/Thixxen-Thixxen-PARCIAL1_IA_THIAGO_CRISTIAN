using UnityEngine;

public class HunterPatrolState : State
{
    private HunterNPC _npc;

    private int _currentNode = 0;
    private int _direction = 1;

    private float _spawnTimer = 0f;

    public HunterPatrolState(HunterNPC npc, StateMachine stateMachine) : base(stateMachine)
    {
        _npc = npc;
    }

    public override void Enter()
    {
        Debug.Log("Cazador entrea en estado Patrol");
    }

    public override void Exit()
    {
        Debug.Log("Cazador sale del estrado Patrol");
    }

    public override void Update()
    {
        // movimientos por Waipoints
        PatrolMovement();
        // generacion de objeto de interes
        GenerateObjectOfInterest();
    }

    private void PatrolMovement()
    {
        if (_npc.waypoints.Length == 0) return;

        Transform targetWaypoint = _npc.waypoints[_currentNode];
        float distance = Vector3.Distance(_npc.transform.position, targetWaypoint.position);

        if (distance <= _npc.waypointCheckDistance)
        {
            _currentNode += _direction;

            if (_currentNode >= _npc.waypoints.Length)
            {
                _currentNode = _npc.waypoints.Length - 1;
                _direction = -1;
            }
            else if (_currentNode < 0)
            {
                _currentNode = 1;
                _direction = -1;
            }
        }

        Vector3 moveDir = (targetWaypoint.position - _npc.transform.position).normalized;
        _npc.transform.position += moveDir * _npc.Speed * Time.deltaTime;

        if (moveDir != Vector3.zero)
        {
            _npc.transform.forward = moveDir;
        }

    }
    
    private void GenerateObjectOfInterest()
    {
        _spawnTimer += Time.deltaTime;
        if (_spawnTimer >= _npc.spawnInterval)
        {
            _spawnTimer = 0f;

            _npc.activeObjects.RemoveAll(obj => obj == null);

            if(_npc.activeObjects.Count < 5 && _npc.ObjectOfInterestPrefab != null)
            {
                 GameObject newObj = GameObject.Instantiate(_npc.ObjectOfInterestPrefab, _npc.transform.position, Quaternion.identity);
                _npc.activeObjects.Add(newObj);
            }
        }
    }


}