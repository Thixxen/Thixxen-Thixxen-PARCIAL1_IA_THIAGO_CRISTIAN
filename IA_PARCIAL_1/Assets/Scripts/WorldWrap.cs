using UnityEngine;

public class WorldWrap : MonoBehaviour
{
    [Header("World Bounds")]
    [SerializeField] private float minX = -28.29773f;
    [SerializeField] private float maxX = 61.73773f;
    [SerializeField] private float minZ = -46.5822f;
    [SerializeField] private float maxZ = 55.2534f;

    private void Update()
    {
        Vector3 position = transform.position;

    
        if (position.x < minX)
        {
            position.x = maxX;
        }
        
        else if (position.x > maxX)
        {
            position.x = minX;
        }

    
        if (position.z < minZ)
        {
            position.z = maxZ;
        }
      
        else if (position.z > maxZ)
        {
            position.z = minZ;
        }

        transform.position = position;
    }
}