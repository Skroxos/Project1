using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBrain : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform player;
    private State currentState;
    public PatrolState patrolState;
    public ChaseState chaseState;
    public AttackState attackState;
    
    private void Start()
    {
        patrolState = new PatrolState(agent, player);
        chaseState = new ChaseState(agent, player);
        attackState = new AttackState(agent, player);
        
        currentState = patrolState;
        currentState.Enter(this);
    }

    private void Update()
    {
        currentState.Execute(this);
    }

   
    public void TransitionToState(State state)
    {
        currentState?.Exit(this);
        currentState = state;
        currentState.Enter(this);
    }
}


public class State
{
    public virtual void Enter(EnemyBrain enemy) { }
    public virtual void Execute(EnemyBrain enemy) { }
    public virtual void Exit(EnemyBrain enemy) { }
}

public class PatrolState : State
{
    private readonly NavMeshAgent agent;
    private readonly Transform player;
    
    private float waitTimer = 0;
    private int maxWaitTime = 3;
    
    public PatrolState(NavMeshAgent agent, Transform player)
    {
        this.agent = agent;
        this.player = player;
    }
    
    public override void Enter(EnemyBrain enemy)
    {
        SetRandomPointToPatrol();
    }
    public override void Execute(EnemyBrain enemy)
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= maxWaitTime)
            {
                SetRandomPointToPatrol();
                waitTimer = 0;
            }
        }
        if (Vector3.Distance(enemy.transform.position, player.position) < 10f)
        {
            enemy.TransitionToState(enemy.chaseState);
        }
       
    }

    public override void Exit(EnemyBrain enemy)
    {
        agent.ResetPath();
    }
    
    private void SetRandomPointToPatrol()
    {
        Vector3 point = Random.insideUnitSphere.normalized * 10f;
        agent.SetDestination(point);
    }
}

public class ChaseState : State
{
    private readonly NavMeshAgent agent;
    private readonly Transform player;

    public ChaseState(NavMeshAgent agent, Transform player)
    {
        this.agent = agent;
        this.player = player;
    }
    
    public override void Enter(EnemyBrain enemy)
    {
        // Optional: Play chase animation or sound
    }
    
    public override void Execute(EnemyBrain enemy)
    {
        agent.SetDestination(player.position);
        if (Vector3.Distance(enemy.transform.position, player.position) > 10f)
        {
            enemy.TransitionToState(enemy.patrolState);
        }

        if (Vector3.Distance(enemy.transform.position, player.position) < 2f)
        {
            enemy.TransitionToState(enemy.attackState);
        }
        
    }

    public override void Exit(EnemyBrain enemy)
    {
        agent.ResetPath();
    }
}

public class AttackState : State
{
    private readonly NavMeshAgent agent;
    private readonly Transform player;
    private float attackCooldown = 1.5f;
    private float lastAttackTime = 0;

    public AttackState(NavMeshAgent agent, Transform player)
    {
        this.agent = agent;
        this.player = player;
    }
    
    public override void Enter(EnemyBrain enemy)
    {
        agent.isStopped = true;
    }
    
    public override void Execute(EnemyBrain enemy)
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            Debug.Log("Enemy attacks the player!");
        }
        if (Vector3.Distance(enemy.transform.position, player.position) > 3f)
        {
            enemy.TransitionToState(enemy.chaseState);
        }
    }

    public override void Exit(EnemyBrain enemy)
    {
        agent.isStopped = false;
    }
}

