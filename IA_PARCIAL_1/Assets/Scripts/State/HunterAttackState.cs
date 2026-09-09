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
    }
}
