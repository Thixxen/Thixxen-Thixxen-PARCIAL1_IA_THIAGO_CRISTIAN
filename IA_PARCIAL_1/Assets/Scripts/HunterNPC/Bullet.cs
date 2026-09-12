using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private int damage = 1;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.position +=
            transform.forward *
            speed *
            Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer ==
            LayerMask.NameToLayer("Boid"))
        {
            Debug.Log("¡La bala impactó a un Boid!");

            Boid boid =
                other.GetComponent<Boid>();

            if (boid != null)
            {
                boid.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }
}