using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private int damage = 1; // Valor de daño para pasarle al Boid

    private void Start()
    {
        // Regla de optimización: La bala se autodestruye tras unos segundos
        // para no acumular basura en la memoria si sale volando fuera del mapa.
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        // Movimiento constante hacia adelante (hacia donde fue rotada al instanciarse)
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    // Esta función se activa cuando la bala atraviesa otro objeto
    private void OnTriggerEnter(Collider other)
    {
        // Comprobamos si el objeto con el que chocó está en la capa "Boid"
        if (other.gameObject.layer == LayerMask.NameToLayer("Boid"))
        {
            Debug.Log("¡La bala impactó a un Boid!");

            // ---------------------------------------------------------
            // Aquí es donde se conecta con el código de tu amigo.
            // Si el Boid tiene un script con un método para recibir daño, se llama así:
            //
            // BoidScript boid = other.GetComponent<BoidScript>();
            // if (boid != null) boid.TakeDamage(damage);
            // ---------------------------------------------------------

            // Destruimos la bala porque ya impactó en su objetivo
            Destroy(gameObject);
        }
    }
}
