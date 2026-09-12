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
        Debug.Log("Cazador entra en estado Ataque. ¡Objetivo en la mira!");

        // Hacemos que el temporizador comience listo para disparar la primera bala al instante
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

        float distance = Vector3.Distance(_npc.transform.position, _npc.currentTarget.position);

        // Si el boid se escapa de la vista, vuelve a patrullar pero NO recarga mágicamente
        if (distance > _npc.perceptionRadius)
        {
            _npc.currentTarget = null;
            StateMachine.ChangeState(HunterStates.Patrol);
            return;
        }

        // Evaluar distancias de ataque
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
        _fireTimer += Time.deltaTime;

        // Si ya pasó el tiempo para la siguiente bala (Fire Rate)
        if (_fireTimer >= _npc.FireRate)
        {
            _fireTimer = 0f;          // Reiniciamos el tiempo entre balas
            _npc.currentAmmo--;       // Restamos una bala

            Debug.Log($"¡PUM! Balas restantes: {_npc.currentAmmo}");

            // Instanciamos la bala
            if (_npc.BulletPrefab != null && _npc.FirePoint != null)
            {
                Vector3 shootDirection = (_npc.currentTarget.position - _npc.FirePoint.position).normalized;
                GameObject bullet = GameObject.Instantiate(_npc.BulletPrefab, _npc.FirePoint.position, Quaternion.identity);
                bullet.transform.forward = shootDirection;
            }

            // ¿Se quedó sin balas?
            if (_npc.currentAmmo <= 0)
            {
                Debug.Log("Cargador vacío. ¡Iniciando recarga (TBA) y volviendo a Patrullar!");

                _npc.currentAmmo = _npc.MaxAmmo; // Recarga el arma para la próxima vez
                _npc.RestAttackTimer();          // Inicia el cronómetro TBA (Cooldown)

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
