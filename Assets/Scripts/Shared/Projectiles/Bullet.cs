using UnityEngine;
using Unity.Netcode;

public class Bullet : NetworkBehaviour
{
    private Team team;
    [SerializeField]
    private float damage = 5f;
    [SerializeField]
    private float moveSpeed = 5;
    private Vector3 previousPosition;
    [SerializeField]
    private float destroyingDistance = 3;
    private float coveredDistance = 0;
    private Direction moveDirection;

    private void Start()
    {
        previousPosition = transform.position;
    }

    public void Initialize(Team team, Direction direction)
    {
        this.team = team;
        moveDirection = direction;
    }
    private void FixedUpdate()
    {
        if (!IsServer) return;

        if(coveredDistance >= destroyingDistance)
        {
            Destroy(gameObject);
        }
        

        if (moveDirection == Direction.Right)
            transform.position += new Vector3(1, 0, 0) * moveSpeed * Time.fixedDeltaTime;
        else if (moveDirection == Direction.Left)
            transform.position += new Vector3(-1, 0, 0) * moveSpeed * Time.fixedDeltaTime;

        coveredDistance += (transform.position - previousPosition).magnitude;
        previousPosition = transform.position;
    }

    void OnTriggerEnter2D(Collider2D other) {

        if (!IsServer) return;
        
        if(other.TryGetComponent<Unit>(out Unit unit))
        {
            if(unit.team.Value != team)
            {
                unit.TakeDamage(damage, DamageType.Physical); // Apply damage to the unit
                Destroy(gameObject);
            }
        }
    }   
}