using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _speed = 10f; // Bajamos un poco la velocidad para verla viajar
    [SerializeField] private float _lifeTime = 3f;
    [SerializeField] private int _damage = 1;

    // ¡ACÁ ESTÁ LA VARIABLE! Declarada arriba para que exista en todo el código.
    private bool _hasImpacted = false;

    private void Start()
    {
        // Se destruirá sola a los 3 segundos si no choca con nada
        Destroy(gameObject, _lifeTime);
    }

    private void Update()
    {
        // El motor de movimiento de la bala
        transform.position += transform.forward * _speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        // ¡El chismoso! Esto nos dirá en la consola contra qué está chocando apenas nace
        Debug.Log($"La bala chocó contra: {other.gameObject.name} (Capa: {LayerMask.LayerToName(other.gameObject.layer)})");

        // El seguro antibugs: si ya impactó en este frame, ignoramos el resto
        if (_hasImpacted) return;

        // Si choca contra la capa del Boid...
        if (other.gameObject.layer == LayerMask.NameToLayer("Boid"))
        {
            Boid scriptDelBoid = other.GetComponent<Boid>();

            if (scriptDelBoid != null)
            {
                scriptDelBoid.TakeDamage(_damage);
                _hasImpacted = true; // ¡Activamos el seguro!
            }

            // Destruimos la bala al final
            Destroy(gameObject);
        }
    }
}