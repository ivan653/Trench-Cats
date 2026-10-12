using UnityEngine;
using Unity.Netcode;

public class ParabolicProjectile : NetworkBehaviour
{
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float flightTime = 1.5f;
    [SerializeField] private float arcHeight = 3f;

    private Team team;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float elapsedTime = 0f;

    public void Initialize(Team team, Vector3 startPos, Vector3 targetPos)
    {
        this.team = team;
        this.startPosition = startPos;
        this.targetPosition = targetPos;
        this.elapsedTime = 0f;
    }

    private void FixedUpdate()
    {
        if (!IsServer) return;

        elapsedTime += Time.fixedDeltaTime;
        float t = elapsedTime / flightTime;

        if (t >= 1f)
        {
            SpawnExplosion();
            return;
        }

        Vector3 currentPos = Vector3.Lerp(startPosition, targetPosition, t);
        float heightOffset = 4f * arcHeight * t * (1f - t);
        currentPos.y += heightOffset;

        transform.position = currentPos;
    }
    
    private void SpawnExplosion()
    {
        if (explosionPrefab != null)
        {
            GameObject explosionInstance = Instantiate(explosionPrefab, targetPosition, Quaternion.identity);
            
            if (explosionInstance.TryGetComponent<Explosion>(out var explosion))
            {
                explosion.Initialize(team);
            }

            explosionInstance.GetComponent<NetworkObject>().Spawn();
        }

        Destroy(gameObject);
    }
}