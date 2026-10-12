using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class MortarUnit : Unit
{
    [Header("Mortar Settings")]
    [SerializeField] private float aimDelay = 0.8f;
    [SerializeField] private bool lockMovementWhileAiming = true;

    private bool isAiming = false;

    public override void Attack()
    {
        if (!IsServer || reloadTimer > 0 || isAiming || projectileObject == null) return;

        StartCoroutine(AimAndShootRoutine());
    }

    private IEnumerator AimAndShootRoutine()
    {
        isAiming = true;

        reloadTimer = reloadSpeed + aimDelay;

        Vector3 targetCoords;
        if (currentTarget != null)
        {
            targetCoords = currentTarget.transform.position;
        }
        else
        {
            Vector3 direction = attackDirection.Value == Direction.Right ? Vector3.right : Vector3.left;
            targetCoords = transform.position + direction * viewDistance;
        }

        yield return new WaitForSeconds(aimDelay);

        if (projectileObject != null)
        {
            GameObject projectile = Instantiate(projectileObject, transform.position, Quaternion.identity);
            projectile.GetComponent<NetworkObject>().Spawn();

            if (projectile.TryGetComponent<ParabolicProjectile>(out var parabolic))
            {
                parabolic.Initialize(team.Value, transform.position, targetCoords);
            }
        }

        reloadTimer = reloadSpeed;
        isAiming = false;
    }

    public override void Move()
    {
        if (lockMovementWhileAiming && isAiming) return;

        base.Move();
    }
}