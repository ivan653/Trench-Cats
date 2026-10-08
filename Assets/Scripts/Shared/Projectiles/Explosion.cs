using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Explosion : NetworkBehaviour
{
    [SerializeField] private int damage = 50;
    [SerializeField] private LayerMask targetLayer; // Шар противників (Enemy / Units)
    [SerializeField] private float destroyingTime = 1;
    private float timer = 0;
    
    // Розмір зони ураження у світових одиницях (наприклад, 1x1 для одного тайла)
    [SerializeField] private Vector2 tileSize = new Vector2(1f, 1f); 

    public override void OnNetworkSpawn()
    {
        // Шкоду наносить тільки сервер одразу при спавні
        if (!IsServer) return;

        ApplyDamage();
        //Destroy(gameObject);
    }


    private void ApplyDamage()
    {
        // Знаходимо всі колайдери в межах тайлу довкола позиції вибуху
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(
            transform.position, 
            tileSize, 
            0f 
            //targetLayer
        );

        foreach (var col in hitColliders)
        {
            if (col.TryGetComponent(out Unit target))
            {
                target.TakeDamage(damage, DamageType.Explosive);
            }
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= destroyingTime)
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