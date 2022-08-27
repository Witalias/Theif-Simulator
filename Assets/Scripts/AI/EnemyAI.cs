using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CreatureVision))]
public class EnemyAI : MonoBehaviour
{
    private const string runAnimatorBool = "Run";

    [SerializeField] private float detectionTime = 1.5f;
    [SerializeField] private float detectionRadius = 1f;
    [SerializeField] private float minStayingTime = 1f;
    [SerializeField] private float maxStayingTime = 10f;

    private Animator animator;
    private NavMeshAgent agent;
    private CreatureVision vision;
    private MovementController player;
    private LevelGenerator generator;

    private Transform questionMark = null;
    private Transform targetPatrolPoint = null;
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
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag(Tags.Player.ToString()).GetComponent<MovementController>();
        generator = GameObject.FindGameObjectWithTag(Tags.LevelGenerator.ToString()).GetComponent<LevelGenerator>();
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
            worried = false;
            Destroy(questionMark.gameObject);
        }

        if (isPatrolling && targetPatrolPoint != null && Vector3.Distance(transform.position,
            new Vector3(targetPatrolPoint.position.x, transform.position.y, targetPatrolPoint.position.z)) <= detectionRadius)
        {
            Stop();
            targetPatrolPoint = null;
            StartCoroutine(Wait());
        }

        Patrol();
    }

    private void CreateQuestionMark()
    {
        if (questionMark != null)
            Destroy(questionMark.gameObject);

        questionMark = Instantiate(GameStorage.Instanse.QuestionMarkPrefab, player.QuestionAppearancePoint.position, Quaternion.Euler(90, 0, 0)).transform;
    }

    private IEnumerator DetectTarget()
    {
        yield return new WaitForSeconds(detectionTime);
        inProcessDetection = false;
        if (vision.SeesTarget)
        {
            Debug.Log("Target is detected!");
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
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(Random.Range(minStayingTime, maxStayingTime));
        isPatrolling = false;
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
