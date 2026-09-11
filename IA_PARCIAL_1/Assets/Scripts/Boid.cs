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

    [Header("Danger Memory")]
    public float dangerMemoryTime = 5f;

    [Header("Arrive")]
    public float arriveRadius = 30f;
    public float arriveSlowRadius = 4f;
    public float arriveWeight = 2f;

    public Vector3 velocity;

    private float dangerTimer = 0f;
    private Vector3 lastHunterPosition;

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
        GameObject hunter =
            GameObject.FindGameObjectWithTag("Hunter");

        bool hunterDetected = false;

        if (hunter != null)
        {
            float distanceToHunter = Vector3.Distance(
                transform.position,
                hunter.transform.position
            );

            if (distanceToHunter <= evadeRadius)
            {
                hunterDetected = true;

                lastHunterPosition =
                    hunter.transform.position;

                dangerTimer = dangerMemoryTime;
            }
        }

        if (dangerTimer > 0f)
        {
            dangerTimer -= Time.deltaTime;
        }

        // ==========================================
        // 1. HUNTER DETECTADO
        // ==========================================

        if (hunterDetected)
        {
            Vector3 escapeDirection =
                transform.position - hunter.transform.position;

            escapeDirection.y = 0f;

            if (escapeDirection.sqrMagnitude > 0.001f)
            {
                escapeDirection.Normalize();

                velocity = Vector3.Lerp(
                    velocity,
                    escapeDirection * maxSpeed,
                    evadeWeight * Time.deltaTime
                );
            }
        }

        // ==========================================
        // 2. MEMORIA DE PELIGRO
        // ==========================================

        else if (dangerTimer > 0f)
        {
            Vector3 escapeDirection =
                transform.position - lastHunterPosition;

            escapeDirection.y = 0f;

            if (escapeDirection.sqrMagnitude > 0.001f)
            {
                escapeDirection.Normalize();

                velocity = Vector3.Lerp(
                    velocity,
                    escapeDirection * maxSpeed,
                    evadeWeight * Time.deltaTime
                );
            }
        }

        // ==========================================
        // 3. COMPORTAMIENTO NORMAL
        // ==========================================

        else
        {
            GameObject interestPoint =
                FindClosestInterestPoint();

            if (interestPoint != null)
            {
                float distanceToInterest =
                    Vector3.Distance(
                        transform.position,
                        interestPoint.transform.position
                    );

                if (distanceToInterest <= arriveRadius)
                {
                    Vector3 arrive =
                        CalculateArrive(interestPoint);

                    velocity += arrive
                        * arriveWeight
                        * Time.deltaTime
                        * maxAcceleration;
                }
                else
                {
                    ApplyFlocking();
                }
            }
            else
            {
                ApplyFlocking();
            }
        }

        velocity = Vector3.ClampMagnitude(
            velocity,
            maxSpeed
        );

        transform.position +=
            velocity * Time.deltaTime;

        if (velocity.sqrMagnitude > 0.01f)
        {
            transform.forward =
                velocity.normalized;
        }
    }

    private GameObject FindClosestInterestPoint()
    {
        GameObject[] interestPoints =
            GameObject.FindGameObjectsWithTag(
                "ObjectOfInterest"
            );

        if (interestPoints.Length == 0)
            return null;

        GameObject closest = null;

        float closestDistance =
            Mathf.Infinity;

        foreach (GameObject interestPoint in interestPoints)
        {
            if (interestPoint == null)
                continue;

            float distance =
                Vector3.Distance(
                    transform.position,
                    interestPoint.transform.position
                );

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = interestPoint;
            }
        }

        return closest;
    }

    private Vector3 CalculateArrive(
        GameObject target
    )
    {
        Vector3 directionToTarget =
            target.transform.position -
            transform.position;

        directionToTarget.y = 0f;

        float distance =
            directionToTarget.magnitude;

        if (distance < 0.1f)
        {
            return -velocity.normalized;
        }

        Vector3 desiredDirection =
            directionToTarget.normalized;

        float speed = maxSpeed;

        if (distance < arriveSlowRadius)
        {
            speed =
                maxSpeed *
                (distance / arriveSlowRadius);
        }

        Vector3 desiredVelocity =
            desiredDirection * speed;

        Vector3 steering =
            desiredVelocity - velocity;

        if (steering.sqrMagnitude > 0.001f)
        {
            steering.Normalize();
        }

        return steering;
    }

    private void ApplyFlocking()
    {
        Vector3 separation =
            CalculateSeparation();

        if (separation != Vector3.zero)
        {
            velocity +=
                separation *
                separationWeight *
                Time.deltaTime *
                maxAcceleration;
        }

        Vector3 alignment =
            CalculateAlignment();

        if (alignment != Vector3.zero)
        {
            velocity +=
                alignment *
                alignmentWeight *
                Time.deltaTime *
                maxAcceleration;
        }

        Vector3 cohesion =
            CalculateCohesion();

        if (cohesion != Vector3.zero)
        {
            velocity +=
                cohesion *
                cohesionWeight *
                Time.deltaTime *
                maxAcceleration;
        }
    }

    private Vector3 CalculateSeparation()
    {
        Collider[] neighbors =
            Physics.OverlapSphere(
                transform.position,
                separationRadius
            );

        Vector3 separation =
            Vector3.zero;

        int neighborCount = 0;

        foreach (Collider neighbor in neighbors)
        {
            if (neighbor.gameObject == gameObject)
                continue;

            if (neighbor.gameObject.layer != gameObject.layer)
                continue;

            Vector3 directionAway =
                transform.position -
                neighbor.transform.position;

            if (directionAway.sqrMagnitude > 0.001f)
            {
                separation +=
                    directionAway.normalized;

                neighborCount++;
            }
        }

        if (neighborCount > 0)
        {
            separation /=
                neighborCount;

            separation.Normalize();
        }

        return separation;
    }

    private Vector3 CalculateAlignment()
    {
        Collider[] neighbors =
            Physics.OverlapSphere(
                transform.position,
                perceptionRadius
            );

        Vector3 alignment =
            Vector3.zero;

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
                alignment +=
                    otherBoid.velocity;

                neighborCount++;
            }
        }

        if (neighborCount > 0)
        {
            alignment /=
                neighborCount;

            alignment.Normalize();
        }

        return alignment;
    }

    private Vector3 CalculateCohesion()
    {
        Collider[] neighbors =
            Physics.OverlapSphere(
                transform.position,
                perceptionRadius
            );

        Vector3 centerOfGroup =
            Vector3.zero;

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
                centerOfGroup +=
                    neighbor.transform.position;

                neighborCount++;
            }
        }

        if (neighborCount == 0)
            return Vector3.zero;

        centerOfGroup /=
            neighborCount;

        Vector3 directionToCenter =
            centerOfGroup -
            transform.position;

        directionToCenter.y = 0f;

        if (directionToCenter.sqrMagnitude < 0.001f)
            return Vector3.zero;

        return directionToCenter.normalized;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            perceptionRadius
        );

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            separationRadius
        );

        Gizmos.color = Color.blue;

        Gizmos.DrawWireSphere(
            transform.position,
            evadeRadius
        );

        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(
            transform.position,
            arriveRadius
        );

        Gizmos.color = Color.cyan;

        Gizmos.DrawWireSphere(
            transform.position,
            arriveSlowRadius
        );
    }
}