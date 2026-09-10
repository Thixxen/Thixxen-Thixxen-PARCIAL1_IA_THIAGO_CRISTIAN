using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class HunterAttackState : State
{
    private HunterNPC _npc;

    public HunterAttackState(HunterNPC npc, StateMachine stateMachine) : base(stateMachine)
    {
        _npc = npc;
    }

    public override void Enter()
    {
        Debug.Log("Cazador entrea en estado ataque");
    }

    public override void Exit()
    {
        Debug.Log("Cazador sale del estrado ataque");
    }

    public override void Update()
    {
        // evaluacion de la distancias
        // ataque a los Boids (melee o distancia)
        if (_npc.currentTarget == null)
        {
            StateMachine.ChangeState(HunterStates.Patrol);
            return;
        }

        float distance = Vector3.Distance(_npc.transform.position, _npc.currentTarget.position);

        if (distance > _npc.perceptionRadius)
        {
            _npc.currentTarget = null;
            StateMachine.ChangeState(HunterStates.Patrol);
            return;
        }
        if (distance <= _npc.meleeAttackRadius)
        {
            ExecuteAttack("cuerpo a cuerpo");
        }
        else if (distance <= _npc.rangeAttackRadius)
        {
            ExecuteAttack("A Distancia");
        }
        else
        {
            PursueTarget();
        }
    }
    private void PursueTarget()
    {
        Vector3 direction = (_npc.currentTarget.position - _npc.transform.position).normalized;
        _npc.transform.position += direction * _npc.Speed * Time.deltaTime;

        if (direction != Vector3.zero)
        {
            _npc.transform.forward = direction;
        }
    }

    private void ExecuteAttack(string attackType)
    {
        Debug.Log($"¡Ataque {attackType} exitoso al Boid!");
        _npc.RestAttackTimer();

        _npc.currentTarget = null;
        StateMachine.ChangeState(HunterStates.Patrol);
    }
}
