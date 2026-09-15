using UnityEngine;

public class HunterGatherState : State
{
    private HunterNPC _npc;
    private float _gatherTimer = 0f;

    private WorldSpaceIndicator _indicator;

    public HunterGatherState(
        HunterNPC npc,
        StateMachine stateMachine
    ) : base(stateMachine)
    {
        _npc = npc;
    }

    public override void Enter()
    {
        Debug.Log(
            "Cazador entra en estado gather. Recolectando"
        );

        _gatherTimer = 0f;

        _indicator =
            _npc.GetComponent<WorldSpaceIndicator>();

       
        if (_indicator != null)
        {
            _indicator.Show(
                "+",
                Color.green
            );
        }
    }

    public override void Exit()
    {
        Debug.Log(
            "Cazador sale del estado gather"
        );

        if (_indicator != null)
        {
            _indicator.Hide();
        }
    }

    public override void Update()
    {
      

        if (_npc.currentTarget == null)
        {
            StateMachine.ChangeState(
                HunterStates.Patrol
            );

            return;
        }

     
        float distance =
            Vector3.Distance(
                _npc.transform.position,
                _npc.currentTarget.position
            );

      

        if (distance > _npc.meleeAttackRadius)
        {
            Vector3 direction =
                (
                    _npc.currentTarget.position -
                    _npc.transform.position
                ).normalized;

            _npc.transform.position +=
                direction *
                _npc.Speed *
                Time.deltaTime;

            if (direction != Vector3.zero)
            {
                _npc.transform.forward =
                    direction;
            }
        }

        

        else
        {
            _gatherTimer +=
                Time.deltaTime;

            Debug.Log(
                $"Recolectando... " +
                $"{_gatherTimer:F1}s / " +
                $"{_npc.gatherTime}s"
            );

            if (
                _gatherTimer >
                _npc.gatherTime
            )
            {
                

                Boid boid =
                    _npc.currentTarget.GetComponent<Boid>();

                if (boid != null)
                {
                   
                    float randomX =
                        Random.Range(
                            -28.29773f,
                            61.73773f
                        );

                    float randomZ =
                        Random.Range(
                            -46.5822f,
                            55.2534f
                        );

                    Vector3 respawnPosition =
                        new Vector3(
                            randomX,
                            1f,
                            randomZ
                        );

                    boid.ReviveAt(
                        respawnPosition
                    );

                    Debug.Log(
                        "¡Boid recolectado y revivido!"
                    );
                }

                _npc.currentTarget = null;

                StateMachine.ChangeState(
                    HunterStates.Patrol
                );
            }
        }
    }
}