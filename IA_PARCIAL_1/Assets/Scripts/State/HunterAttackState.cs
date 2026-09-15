using UnityEngine;

public class HunterAttackState : State
{
    private HunterNPC _npc;
    private float _fireTimer = 0f;

    private WorldSpaceIndicator _indicator;

    public HunterAttackState(
        HunterNPC npc,
        StateMachine stateMachine
    ) : base(stateMachine)
    {
        _npc = npc;
    }

    public override void Enter()
    {
        Debug.Log("Cazador entra en estado Ataque.");

       
        _fireTimer = _npc.FireRate;

        _indicator =
            _npc.GetComponent<WorldSpaceIndicator>();
    }

    public override void Exit()
    {
        Debug.Log("Cazador sale del estado Ataque.");

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

     

        if (
            _npc.currentTarget.gameObject.layer !=
            LayerMask.NameToLayer("Boid")
        )
        {
            Debug.Log(
                "El objetivo murió. Revisando munición restante..."
            );

            _npc.currentTarget = null;

            if (_npc.currentAmmo <= 0)
            {
                Debug.Log(
                    "¡Cargador vacío tras la baja! " +
                    "Iniciando recarga (TBA)."
                );

                _npc.currentAmmo =
                    _npc.MaxAmmo;

                _npc.RestAttackTimer();
            }
            else
            {
                Debug.Log(
                    $"Me sobran {_npc.currentAmmo} balas. " +
                    "Sigo patrullando con el radar activo."
                );
            }

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

        if (distance > _npc.perceptionRadius)
        {
            _npc.currentTarget = null;

            StateMachine.ChangeState(
                HunterStates.Patrol
            );

            return;
        }

      

        if (distance <= _npc.meleeAttackRadius)
        {
            ExecuteMeleeAttack();
        }
        else if (distance <= _npc.rangeAttackRadius)
        {
            ExecuteRangedAttack();
        }
        else
        {
            PursueTarget();
        }
    }

    private void PursueTarget()
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

    private void ExecuteRangedAttack()
    {
       

        Vector3 direction =
            (
                _npc.currentTarget.position -
                _npc.transform.position
            ).normalized;

        if (direction != Vector3.zero)
        {
            _npc.transform.forward =
                direction;
        }

        _fireTimer += Time.deltaTime;

        

        if (_fireTimer >= _npc.FireRate)
        {
            _fireTimer = 0f;

            _npc.currentAmmo--;

            Debug.Log(
                $"¡Disparo! Balas restantes: " +
                $"{_npc.currentAmmo}"
            );

          

            if (_indicator != null)
            {
                _indicator.Show(
                    "X",
                    Color.red
                );
            }

        

            if (
                _npc.BulletPrefab != null &&
                _npc.FirePoint != null
            )
            {
                Vector3 shootDirection =
                    (
                        _npc.currentTarget.position -
                        _npc.FirePoint.position
                    ).normalized;

                GameObject bullet =
                    GameObject.Instantiate(
                        _npc.BulletPrefab,
                        _npc.FirePoint.position,
                        Quaternion.identity
                    );

                bullet.transform.forward =
                    shootDirection;
            }

         

            if (_npc.currentAmmo <= 0)
            {
                Debug.Log(
                    "Me quedé sin balas en pleno combate. " +
                    "¡Recargando (TBA)!"
                );

                _npc.currentAmmo =
                    _npc.MaxAmmo;

                _npc.RestAttackTimer();

                _npc.currentTarget = null;

                StateMachine.ChangeState(
                    HunterStates.Patrol
                );
            }
        }
    }

    private void ExecuteMeleeAttack()
    {
        Debug.Log(
            "¡Ataque cuerpo a cuerpo exitoso al Boid!"
        );

       
        if (_indicator != null)
        {
            _indicator.Show(
                "X",
                Color.red
            );
        }

        _npc.RestAttackTimer();

        _npc.currentTarget = null;

        StateMachine.ChangeState(
            HunterStates.Patrol
        );
    }
}