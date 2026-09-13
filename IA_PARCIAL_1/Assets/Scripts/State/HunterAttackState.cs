using UnityEditor.Experimental.GraphView;
using UnityEngine;



public class HunterAttackState : State
{
    private HunterNPC _npc;
    private float _fireTimer = 0f;

    public HunterAttackState(HunterNPC npc, StateMachine stateMachine) : base(stateMachine)
    {
        _npc = npc;
    }

    public override void Enter()
    {
        Debug.Log("Cazador entra en estado Ataque.");

        // Empezamos con el temporizador cargado para que el primer tiro sea inmediato
        _fireTimer = _npc.FireRate;
    }

    public override void Exit()
    {
        Debug.Log("Cazador sale del estado Ataque.");
    }

    public override void Update()
    {
        if (_npc.currentTarget == null)
        {
            StateMachine.ChangeState(HunterStates.Patrol);
            return;
        }

        // ==========================================
        // 1. CHEQUEO TÁCTICO: ¿EL BOID MURIÓ?
        // ==========================================
        if (_npc.currentTarget.gameObject.layer != LayerMask.NameToLayer("Boid"))
        {
            Debug.Log("El objetivo murió. Revisando munición restante...");
            _npc.currentTarget = null;

            // SÓLO recargamos el arma (y el TBA) si nos quedamos en 0.
            if (_npc.currentAmmo <= 0)
            {
                Debug.Log("¡Cargador vacío tras la baja! Iniciando recarga (TBA).");
                _npc.currentAmmo = _npc.MaxAmmo;
                _npc.RestAttackTimer(); // Esto apaga el radar temporalmente
            }
            else
            {
                Debug.Log($"Me sobran {_npc.currentAmmo} balas. Sigo patrullando con el radar activo.");
                // Al NO resetear el AttackTimer, el radar sigue buscando enemigos inmediatamente.
            }

            StateMachine.ChangeState(HunterStates.Patrol);
            return;
        }

        // ==========================================
        // 2. CÁLCULO DE DISTANCIAS
        // ==========================================
        float distance = Vector3.Distance(_npc.transform.position, _npc.currentTarget.position);

        if (distance > _npc.perceptionRadius)
        {
            _npc.currentTarget = null;
            StateMachine.ChangeState(HunterStates.Patrol);
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
        Vector3 direction = (_npc.currentTarget.position - _npc.transform.position).normalized;
        _npc.transform.position += direction * _npc.Speed * Time.deltaTime;

        if (direction != Vector3.zero)
        {
            _npc.transform.forward = direction;
        }
    }

    private void ExecuteRangedAttack()
    {
        // Obliga al Cazador a mirar al boid antes de disparar
        Vector3 direction = (_npc.currentTarget.position - _npc.transform.position).normalized;
        if (direction != Vector3.zero)
        {
            _npc.transform.forward = direction;
        }

        _fireTimer += Time.deltaTime;

        // Espera el "Fire Rate" entre cada disparo individual
        if (_fireTimer >= _npc.FireRate)
        {
            _fireTimer = 0f;
            _npc.currentAmmo--; // Gastamos una bala

            Debug.Log($"¡Disparo! Balas restantes: {_npc.currentAmmo}");

            if (_npc.BulletPrefab != null && _npc.FirePoint != null)
            {
                Vector3 shootDirection = (_npc.currentTarget.position - _npc.FirePoint.position).normalized;
                GameObject bullet = GameObject.Instantiate(_npc.BulletPrefab, _npc.FirePoint.position, Quaternion.identity);
                bullet.transform.forward = shootDirection;
            }

            // Si falló, o si el Boid necesita muchos tiros, y nos quedamos sin balas en pleno tiroteo:
            if (_npc.currentAmmo <= 0)
            {
                Debug.Log("Me quedé sin balas en pleno combate. ¡Recargando (TBA)!");
                _npc.currentAmmo = _npc.MaxAmmo;
                _npc.RestAttackTimer(); // Inicia la recarga larga

                _npc.currentTarget = null;
                StateMachine.ChangeState(HunterStates.Patrol);
            }
        }
    }

    private void ExecuteMeleeAttack()
    {
        Debug.Log("¡Ataque cuerpo a cuerpo exitoso al Boid!");
        _npc.RestAttackTimer();
        _npc.currentTarget = null;
        StateMachine.ChangeState(HunterStates.Patrol);
    }
}
