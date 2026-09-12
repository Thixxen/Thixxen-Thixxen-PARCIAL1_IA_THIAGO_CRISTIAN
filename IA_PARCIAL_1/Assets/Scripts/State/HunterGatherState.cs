using UnityEngine;

public class HunterGatherState : State
{
    private HunterNPC _npc;
    private float _gatherTimer = 0f;

    public HunterGatherState(HunterNPC npc, StateMachine stateMachine) : base(stateMachine)
    {
        _npc = npc;
    }

    public override void Enter()
    {
        Debug.Log("Cazador entrea en estado gather. Recolectando");
        _gatherTimer = 0f;
    }

    public override void Exit()
    {
        Debug.Log("Cazador sale del estrado gather");
    }

    public override void Update()
    {
        // Recoleccion de los Boids eliminados
        if (_npc.currentTarget == null)
        {
            StateMachine.ChangeState(HunterStates.Patrol);
            return;
        }
        float distance = Vector3.Distance(_npc.transform.position, _npc.currentTarget.position);

        if (distance > _npc.meleeAttackRadius)
        {
            Vector3 direction = (_npc.currentTarget.position - _npc.transform.position).normalized;
            _npc.transform.position += direction * _npc.Speed * Time.deltaTime;

            if(direction != Vector3.zero)
            {
                _npc.transform.forward = direction;
            }
        }
        else
        {
            _gatherTimer += Time.deltaTime;
            Debug.Log($"Recolectando... {_gatherTimer:F1}s / {_npc.gatherTime}s");

            if ( _gatherTimer > _npc.gatherTime )
            {
                GameObject.Destroy(_npc.currentTarget.gameObject);

                _npc.currentTarget = null;
                StateMachine.ChangeState(HunterStates.Patrol);
            }
        }
    }
}
