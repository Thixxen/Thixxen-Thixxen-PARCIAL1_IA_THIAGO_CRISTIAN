using UnityEngine;

public class HunterGatherState : State
{
    private HunterNPC _npc;

    public HunterGatherState(HunterNPC npc, StateMachine stateMachine) : base(stateMachine)
    {
        _npc = npc;
    }

    public override void Enter()
    {
        Debug.Log("Cazador entrea en estado gather");
    }

    public override void Exit()
    {
        Debug.Log("Cazador sale del estrado gather");
    }

    public override void Update()
    {
        // Recoleccion de los Boids eliminados
    }
}
