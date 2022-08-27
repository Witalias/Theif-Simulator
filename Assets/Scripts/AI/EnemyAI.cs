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

    private Animator animator;
    private NavMeshAgent agent;
    private CreatureVision vision;
    private MovementController player;

    private Transform questionMark = null;
    private bool inProcessDetection = false;
    private bool worried = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        vision = GetComponent<CreatureVision>();
        player = GameObject.FindGameObjectWithTag(Tags.Player.ToString()).GetComponent<MovementController>();
    }

    private void Update()
    {
        if (vision.SeesTarget)
        {
            worried = true;
            if (!inProcessDetection)
            {
                inProcessDetection = true;
                Stop();

                if (questionMark != null)
                    Destroy(questionMark.gameObject);

                questionMark = Instantiate(GameStorage.Instanse.QuestionMarkPrefab, player.QuestionAppearancePoint, Quaternion.Euler(90, 0, 0)).transform;
                StartCoroutine(DetectTarget());
            }
        }

        if (questionMark != null && Vector3.Distance(
            transform.position, 
            new Vector3(questionMark.transform.position.x, transform.position.y, questionMark.transform.position.z)
            ) <= detectionRadius)
        {
            Stop();
            worried = false;
            Destroy(questionMark.gameObject);
        }
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
