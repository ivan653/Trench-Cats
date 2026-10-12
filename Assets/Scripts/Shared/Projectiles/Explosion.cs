using Unity.Netcode;
using UnityEngine;

public class Explosion : NetworkBehaviour
{
    [SerializeField] private int damage = 50;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private float destroyingTime = 1f;
    [SerializeField] private Vector2 tileSize = new Vector2(1f, 1f);

    private Team sourceTeam;
    private float timer = 0f;

    public void Initialize(Team team)
    {
        this.sourceTeam = team;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        ApplyDamage();
    }

    private void ApplyDamage()
    {
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(
            transform.position, 
            tileSize, 
            0f
            // targetLayer // Можна розкоментувати при налаштованих шарах
        );

        foreach (var col in hitColliders)
        {
            if (col.TryGetComponent<Unit>(out var target))
            {
                // Наносимо шкоду лише якщо юніт належить ворожій команді
                //if (target.team.Value != sourceTeam)
                //{
                    target.TakeDamage(damage, DamageType.Explosive);
                //}
            }
        }
    }

    private void Update()
    {
        if (!IsServer) return;

        timer += Time.deltaTime;
        if (timer >= destroyingTime)
        {
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, tileSize);
    }
}