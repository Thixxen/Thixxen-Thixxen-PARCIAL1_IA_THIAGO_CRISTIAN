using UnityEngine;

public class Boid : MonoBehaviour
{
    [Header("Movement")]
    public float maxSpeed = 5f;
    public float maxAcceleration = 10f;

    [Header("Flocking")]
    public float perceptionRadius = 6f;
    public float separationRadius = 2f;

    [Header("Flocking Weights")]
    public float separationWeight = 2f;
    public float alignmentWeight = 1f;
    public float cohesionWeight = 0.5f;

    [Header("Evade")]
    public float evadeRadius = 8f;
    public float evadeWeight = 3f;

    public Vector3 velocity;

    private void Start()
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized;

        velocity = new Vector3(
            randomDirection.x,
            0f,
            randomDirection.y
        ) * maxSpeed;
    }

    private void Update()
    {
        // =========================
        // BUSCAR HUNTER
        // =========================

        GameObject hunter = GameObject.FindGameObjectWithTag("Hunter");

        if (hunter != null)
        {
            float distanceToHunter =
                Vector3.Distance(transform.position, hunter.transform.position);

            // =========================
            // EVADE
            // =========================

            if (distanceToHunter <= evadeRadius)
            {
                Vector3 evade = CalculateEvade(hunter);

                velocity += evade
                    * evadeWeight
                    * Time.deltaTime
                    * maxAcceleration;
            }
            else
            {
                // =========================
                // FLOCKING
                // =========================

                ApplyFlocking();
            }
        }
        else
        {
            // Si no existe el Hunter,
            // los Boids siguen haciendo flocking.

            ApplyFlocking();
        }

        // =========================
        // VELOCIDAD MÁXIMA
        // =========================

        velocity = Vector3.ClampMagnitude(
            velocity,
            maxSpeed
        );

        // =========================
        // MOVIMIENTO
        // =========================

        transform.position += velocity * Time.deltaTime;

        // =========================
        // ORIENTACIÓN
        // =========================

        if (velocity.sqrMagnitude > 0.01f)
        {
            transform.forward = velocity.normalized;
        }
    }

    // =====================================================
    // FLOCKING
    // =====================================================

    private void ApplyFlocking()
    {
        // =========================
        // SEPARATION
        // =========================

        Vector3 separation = CalculateSeparation();

        if (separation != Vector3.zero)
        {
            velocity += separation
                * separationWeight
                * Time.deltaTime
                * maxAcceleration;
        }

        // =========================
        // ALIGNMENT
        // =========================

        Vector3 alignment = CalculateAlignment();

        if (alignment != Vector3.zero)
        {
            velocity += alignment
                * alignmentWeight
                * Time.deltaTime
                * maxAcceleration;
        }

        // =========================
        // COHESION
        // =========================

        Vector3 cohesion = CalculateCohesion();

        if (cohesion != Vector3.zero)
        {
            velocity += cohesion
                * cohesionWeight
                * Time.deltaTime
                * maxAcceleration;
        }
    }

    // =====================================================
    // EVADE
    // =====================================================

    private Vector3 CalculateEvade(GameObject hunter)
    {
        Vector3 directionAway =
            transform.position - hunter.transform.position;

        directionAway.y = 0f;

        if (directionAway.sqrMagnitude < 0.001f)
            return Vector3.zero;

        return directionAway.normalized;
    }

    // =====================================================
    // SEPARATION
    // =====================================================

    private Vector3 CalculateSeparation()
    {
        Collider[] neighbors = Physics.OverlapSphere(
            transform.position,
            separationRadius
        );

        Vector3 separation = Vector3.zero;
        int neighborCount = 0;

        foreach (Collider neighbor in neighbors)
        {
            if (neighbor.gameObject == gameObject)
                continue;

            if (neighbor.gameObject.layer != gameObject.layer)
                continue;

            Vector3 directionAway =
                transform.position - neighbor.transform.position;

            if (directionAway.sqrMagnitude > 0.001f)
            {
                separation += directionAway.normalized;
                neighborCount++;
            }
        }

        if (neighborCount > 0)
        {
            separation /= neighborCount;
            separation.Normalize();
        }

        return separation;
    }

    // =====================================================
    // ALIGNMENT
    // =====================================================

    private Vector3 CalculateAlignment()
    {
        Collider[] neighbors = Physics.OverlapSphere(
            transform.position,
            perceptionRadius
        );

        Vector3 alignment = Vector3.zero;
        int neighborCount = 0;

        foreach (Collider neighbor in neighbors)
        {
            if (neighbor.gameObject == gameObject)
                continue;

            if (neighbor.gameObject.layer != gameObject.layer)
                continue;

            Boid otherBoid =
                neighbor.GetComponent<Boid>();

            if (otherBoid != null)
            {
                alignment += otherBoid.velocity;
                neighborCount++;
            }
        }

        if (neighborCount > 0)
        {
            alignment /= neighborCount;
            alignment.Normalize();
        }

        return alignment;
    }

    // =====================================================
    // COHESION
    // =====================================================

    private Vector3 CalculateCohesion()
    {
        Collider[] neighbors = Physics.OverlapSphere(
            transform.position,
            perceptionRadius
        );

        Vector3 centerOfGroup = Vector3.zero;
        int neighborCount = 0;

        foreach (Collider neighbor in neighbors)
        {
            if (neighbor.gameObject == gameObject)
                continue;

            if (neighbor.gameObject.layer != gameObject.layer)
                continue;

            Boid otherBoid =
                neighbor.GetComponent<Boid>();

            if (otherBoid != null)
            {
                centerOfGroup += neighbor.transform.position;
                neighborCount++;
            }
        }

        if (neighborCount == 0)
            return Vector3.zero;

        centerOfGroup /= neighborCount;

        Vector3 directionToCenter =
            centerOfGroup - transform.position;

        directionToCenter.y = 0f;

        if (directionToCenter.sqrMagnitude < 0.001f)
            return Vector3.zero;

        return directionToCenter.normalized;
    }

    // =====================================================
    // GIZMOS
    // =====================================================

    private void OnDrawGizmosSelected()
    {
        // Perception
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            perceptionRadius
        );

        // Separation
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            separationRadius
        );

        // Evade
        Gizmos.color = Color.blue;

        Gizmos.DrawWireSphere(
            transform.position,
            evadeRadius
        );
    }
}