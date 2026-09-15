using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _speed = 10f; 
    [SerializeField] private float _lifeTime = 3f;
    [SerializeField] private int _damage = 1;

    
    private bool _hasImpacted = false;

    private void Start()
    {
       
        Destroy(gameObject, _lifeTime);
    }

    private void Update()
    {
        
        transform.position += transform.forward * _speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
    
        Debug.Log($"La bala chocó contra: {other.gameObject.name} (Capa: {LayerMask.LayerToName(other.gameObject.layer)})");

       
        if (_hasImpacted) return;

        
        if (other.gameObject.layer == LayerMask.NameToLayer("Boid"))
        {
            Boid scriptDelBoid = other.GetComponent<Boid>();

            if (scriptDelBoid != null)
            {
                scriptDelBoid.TakeDamage(_damage);
                _hasImpacted = true; 
            }

           
            Destroy(gameObject);
        }
    }
}