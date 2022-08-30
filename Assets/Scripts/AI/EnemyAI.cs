using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CreatureVision))]
[RequireComponent(typeof(Noisy))]
public class EnemyAI : MonoBehaviour
{
    private const string runAnimatorBool = "Run";

    [SerializeField] private float detectionTime = 1.5f;
    [SerializeField] private float detectionRadius = 1f;
    [SerializeField] private float minStayingTime = 1f;
    [SerializeField] private float maxStayingTime = 10f;
    [SerializeField] private float patrolCheckingTime = 20f;

    private Animator animator;
    private NavMeshAgent agent;
    private CreatureVision vision;
    private Noisy noisy;
    private VisibilityScale visibilityScale;
    private MovementController player;
    private LevelGenerator generator;

    private Transform questionMark = null;
    private Transform targetPatrolPoint = null;
    private Coroutine waitCoroutine = null;
    private Coroutine checkPatrolCoroutine = null;
    private bool inProcessDetection = false;
    private bool worried = false;
    private bool isPatrolling = false;

    public void SetTargetPoint()
    {
        if (worried)
            return;

        worried = true;
        isPatrolling = false;
        Stop();
        CreateQuestionMark();
        StartCoroutine(DetectTarget());
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        vision = GetComponent<CreatureVision>();
        noisy = GetComponent<Noisy>();
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag(Tags.Player.ToString()).GetComponent<MovementController>();
        generator = GameObject.FindGameObjectWithTag(Tags.LevelGenerator.ToString()).GetComponent<LevelGenerator>();
        visibilityScale = GameObject.FindGameObjectWithTag(Tags.VisibilityScale.ToString()).GetComponent<VisibilityScale>();
    }

    private void Update()
    {
        if (vision.SeesTarget)
        {
            worried = true;
            isPatrolling = false;

            if (!inProcessDetection)
            {
                inProcessDetection = true;
                Stop();
                CreateQuestionMark();
                StartCoroutine(DetectTarget());
            }
        }

        if (questionMark != null && Vector3.Distance(transform.position, 
            new Vector3(questionMark.transform.position.x, transform.position.y, questionMark.transform.position.z)
            ) <= detectionRadius)
        {
            Stop();
            Destroy(questionMark.gameObject);
            StopCoroutine(checkPatrolCoroutine);
            waitCoroutine = StartCoroutine(Wait());
        }

        if (isPatrolling && targetPatrolPoint != null && Vector3.Distance(transform.position,
            new Vector3(targetPatrolPoint.position.x, transform.position.y, targetPatrolPoint.position.z)) <= detectionRadius)
        {
            Stop();
            targetPatrolPoint = null;
            waitCoroutine = StartCoroutine(Wait());
            StopCoroutine(checkPatrolCoroutine);
        }

        Patrol();
    }

    private void CreateQuestionMark()
    {
        if (questionMark != null)
            Destroy(questionMark.gameObject);

        if (waitCoroutine != null)
            StopCoroutine(Wait());

        if (checkPatrolCoroutine != null)
            StopCoroutine(checkPatrolCoroutine);
        checkPatrolCoroutine = StartCoroutine(CheckPatrol());

        visibilityScale.Add(GameSettings.Instanse.VisibilityValueSuspicion);

        questionMark = Instantiate(GameStorage.Instanse.QuestionMarkPrefab, player.CenterPoint.position, Quaternion.Euler(90, 0, 0)).transform;
    }

    private IEnumerator DetectTarget()
    {
        yield return new WaitForSeconds(detectionTime);
        inProcessDetection = false;
        if (vision.SeesTarget)
        {
            visibilityScale.Add(GameSettings.Instanse.VisibilityValueDetection);
            noisy.Noise(GameSettings.Instanse.HearingRadiusDuringEnemyScream);
        }
        else
        {
            Run(questionMark.position);
        }
    }

    private void Patrol()
    {
        if (worried || isPatrolling || !generator.Generated)
            return;

        isPatrolling = true;
        targetPatrolPoint = generator.GetRandomPatrolPoint();
        Run(targetPatrolPoint.position);
        checkPatrolCoroutine = StartCoroutine(CheckPatrol());
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(Random.Range(minStayingTime, maxStayingTime));
        isPatrolling = false;
        worried = false;
    }

    private IEnumerator CheckPatrol()
    {
        yield return new WaitForSeconds(patrolCheckingTime);
        Stop();

        if (questionMark != null && !inProcessDetection)
            Destroy(questionMark.gameObject);

        waitCoroutine = StartCoroutine(Wait());
    }

    private void Run(Vector3 toPosition)
    {
        agent.SetDestination(toPosition);
        animator.SetBool(runAnimatorBool, true);
    }

    private void Stop()
    {
        agent.isStopped = true;
        agent.ResetPath();
        animator.SetBool(runAnimatorBool, false);
    }
}
