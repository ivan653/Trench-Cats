using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public abstract class Unit : NetworkBehaviour
{
    public NetworkVariable<FixedString64Bytes> id = new NetworkVariable<FixedString64Bytes>(
        default,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    public NetworkVariable<float> health = new NetworkVariable<float>(
        20f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    public NetworkVariable<Team> team = new NetworkVariable<Team>(
        Team.Blue,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    
    [field:SerializeField] public int mapLine {protected set; get;}
    [field:SerializeField] public float moveSpeed {private set; get;} = 0.2f;
    [field:SerializeField] public float reloadSpeed {private set; get;} = 1;
    protected float reloadTimer = 0;

    [SerializeField] protected float viewDistance = 3;
    [SerializeField] protected Unit currentTarget;
    public GameObject projectileObject;
    public NetworkVariable<Direction> moveDirection = new NetworkVariable<Direction>(
        Direction.Right,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    public NetworkVariable<Direction> attackDirection = new NetworkVariable<Direction>(
        Direction.Right,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    public NetworkVariable<bool> isInTranch = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    public NetworkVariable<bool> isBuilding = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    public NetworkVariable<bool> isFighting = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    [SerializeField]
    private Effects effects = new ();
    public void Initialize(int mapLine, Team team, Direction moveDir, Direction attackDir)
    {
        id.Value = System.Guid.NewGuid().ToString();
        this.mapLine = mapLine;
        this.team.Value = team;
        moveDirection.Value = moveDir;
        attackDirection.Value = attackDir;
    }

    protected virtual void FixedUpdate()
    {
        if (!IsServer) return;

        SearchForDefeatZone();
        ExecuteBehavior();
        
        reloadTimer -= Time.fixedDeltaTime;
    }

    protected virtual void ExecuteBehavior()
    {
        if (isBuilding.Value) return;

        if (isInTranch.Value)
        {
            SearchForEnemies();
            SearchForBuildings();
        }
        else if (isFighting.Value)
        {
            SearchForEnemies();
            SearchForTrenches();
        }
        else
        {
            Move();
            SearchForTrenches();
            SearchForEnemies();
            SearchForBuildings();
        }
    }

    public virtual void Attack()
    {
        if (reloadTimer <= 0 && projectileObject != null)
        {
            var projectile = Instantiate(projectileObject, transform.position, Quaternion.identity);
            projectile.GetComponent<NetworkObject>().Spawn();

            if (projectile.TryGetComponent<Bullet>(out Bullet bullet))
            {
                bullet.Initialize(team.Value, attackDirection.Value);
            }
            else if (projectile.TryGetComponent<ParabolicProjectile>(out ParabolicProjectile parabolic))
            {
                // Якщо ціль втрачена, стріляємо на максимальну дистанцію огляду
                Vector3 targetPos = currentTarget != null ? 
                    currentTarget.transform.position : 
                    transform.position + ((attackDirection.Value == Direction.Right ? Vector3.right : Vector3.left) * viewDistance);
                
                parabolic.Initialize(team.Value, transform.position, targetPos);
            }

            reloadTimer = reloadSpeed;
        }
    }

    public virtual void Move()
    {
        Vector3 dir = (moveDirection.Value == Direction.Right) ? Vector3.right : Vector3.left;
        transform.position += dir * moveSpeed * Time.fixedDeltaTime;
    }

    public void SwitchMoveDirection()
    {
        if(moveDirection.Value == Direction.Right)
            moveDirection.Value = Direction.Left;
        else
            moveDirection.Value = Direction.Right;
    }

    private void SearchForTrenches()
    {
        RaycastHit2D[] hitColliders = Physics2D.RaycastAll(transform.position, moveDirection.Value == Direction.Right ? Vector2.right : Vector2.left, 0.1f);
        if (hitColliders.Length > 0)
        {
            foreach (var hitCollider in hitColliders)
            {
                Trench trench = hitCollider.collider.GetComponent<Trench>();
                if (trench != null)
                {
                    var temp = trench.HandleTrenchEntrance(this);
                    if(temp) 
                    {
                        isInTranch.Value = true;
                        effects.projectilesResistance = 0.5f;
                        transform.position = trench.transform.position;
                    }
                    break; // Exit the loop after finding the first enemy
                }
            }
        }
    }

    private void SearchForEnemies()
{
    RaycastHit2D[] hitColliders = Physics2D.RaycastAll(transform.position, moveDirection.Value == Direction.Right ? Vector2.right : Vector2.left, viewDistance);
    if (hitColliders.Length > 0)
    {
        foreach (var hitCollider in hitColliders)
        {
            Unit enemyUnit = hitCollider.collider.GetComponent<Unit>();
            if (enemyUnit != null && enemyUnit.team.Value != this.team.Value)
            {
                currentTarget = enemyUnit; // Зберігаємо ціль
                isFighting.Value = true;
                Attack();
                break; 
            }
            else
            {
                isFighting.Value = false;
                currentTarget = null;
            }
        }
    }
    else
    {
        isFighting.Value = false;
        currentTarget = null;
    }
}

    private void SearchForBuildings()
    {
        RaycastHit2D[] hitColliders = Physics2D.RaycastAll(transform.position, moveDirection.Value == Direction.Right ? Vector2.right : Vector2.left, 0.5f);
        if (hitColliders.Length > 0)
        {
            foreach (var hitCollider in hitColliders)
            {
                BuildingProject building = hitCollider.collider.GetComponent<BuildingProject>();
                if (building != null)
                {
                    building.StartBuilding(this);
                    isBuilding.Value = true;
                    break; // Exit the loop after finding the first enemy
                }
            }
        }

    }

    private void SearchForDefeatZone()
    {
        RaycastHit2D[] hitColliders = Physics2D.RaycastAll(transform.position, moveDirection.Value == Direction.Right ? Vector2.right : Vector2.left, 0.01f);
        if (hitColliders.Length > 0)
        {
            foreach (var hitCollider in hitColliders)
            {
                DefeatZone zone = hitCollider.collider.GetComponent<DefeatZone>();
                if (zone != null)
                {
                    if(zone.isLeftSide && team.Value == Team.Red)
                    {
                        MatchManager.Instance.HandleTeamDefeat(Team.Blue);
                        break;
                    }
                    if(!zone.isLeftSide && team.Value == Team.Blue)
                    {
                        MatchManager.Instance.HandleTeamDefeat(Team.Red);
                        break;
                    }
                }
            }
        }
    }

    public void ExitFromTrench()
    {
        isInTranch.Value = false;
    }
    public bool CheckBuildingInFrontPhysics(Transform unitTransform, Direction moveDirection)
    {
        // Визначаємо вектор напрямку (наприклад, вправо або вліво)
        Vector2 direction = (moveDirection == Direction.Right) ? Vector2.right : Vector2.left;
        
        // Позиція на відстані рівно 1 клітинки попереду (припустимо, розмір клітинки = 1 одиниця)
        Vector2 checkPosition = (Vector2)unitTransform.position + direction * 1f;

        // Створюємо маленьку «коробку» або точку перевірки (OverlapPoint / OverlapBox)
        Collider2D hit = Physics2D.OverlapPoint(checkPosition);

        if (hit != null)
        {
            // Перевіряємо, чи це проєкт будівлі або споруда
            if (hit.TryGetComponent<BuildingProject>(out var project))
            {
                return true;
            }
        }

        return false;
    }

    public void TakeDamage(float damage, DamageType damageType)
    {
        if (!IsServer) return;

        float resist = GetResistance(damageType);

        health.Value -= damage * (1-resist);
        if (health.Value <= 0)
        {
            Destroy(gameObject);
        }
    }
    private float GetResistance(DamageType damageType)
    {
        float result = 0;
        if(damageType == DamageType.Physical)
            result = effects.projectilesResistance;
        
        return result;
    }

    public void SetMoveDirection(Direction direction)
    {
        moveDirection.Value = direction;
    }

    public void SetAttackDirection(Direction direction)
    {
        attackDirection.Value = direction;
    }
}