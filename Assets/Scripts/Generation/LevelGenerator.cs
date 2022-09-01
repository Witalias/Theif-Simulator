using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.AI;

public class LevelGenerator : MonoBehaviour
{
    private const float checkRadius = 0.5f;

    [SerializeField] private bool movePlayer = true;
    [SerializeField] private bool enableFog = true;
    [SerializeField] private int roomsCount = 5;
    [SerializeField] private int frontDoorsCount = 1;
    [SerializeField] private int lockedDoorsAndWindowsCount = 3;
    [SerializeField] private int enemiesCount = 1;
    [SerializeField] private Transform startPoint;
    [SerializeField] private NavMeshRebaker navMeshRebaker;
    [SerializeField] private Transform player;
    [SerializeField] private GameObject fogPrefab;
    [SerializeField] private LayerMask roomMask;
    [SerializeField] private LayerMask doorWallMask;
    [SerializeField] private LayerMask wallMask;
    [SerializeField] private Room[] roomTemplates;
    [SerializeField] private GameObject[] windows;
    [SerializeField] private GameObject[] walls;
    [SerializeField] private Transform[] spawnPoints;

    private int currentRoomsCount = 0;
    private int currentFrontDoorsCount = 0;
    private readonly Queue<Vector3> positionsForNextRoom = new Queue<Vector3>();
    private readonly List<ConnectionWall> allConnectionWalls = new List<ConnectionWall>();
    private readonly List<CenteredPoint> allWindows = new List<CenteredPoint>();
    private readonly List<CenteredPoint> allWalls = new List<CenteredPoint>();
    private readonly List<Transform> patrolPoints = new List<Transform>();
    private readonly List<GameObject> enemies = new List<GameObject>();
    private readonly List<EnemyAI> policemans = new List<EnemyAI>();

    private bool successRoomGenerated = false;
    private ConnectionWall lastUsedConnectionWall = null;

    public bool Generated { get; private set; }

    public List<EnemyAI> GetPolicemans() => policemans;

    public Transform GetRandomPatrolPoint() => patrolPoints[Random.Range(0, patrolPoints.Count)];

    public void LockRandomDoorsAndWindows(int count)
    {
        for (var i = 0; i < count && allWindows.Count > 0 && allConnectionWalls.Count > 0; ++i)
        {
            switch (Random.Range(1, 3))
            {
                case 1:
                    var randomWindow = allWindows[Random.Range(0, allWindows.Count)];
                    allWindows.Remove(randomWindow);
                    randomWindow.GetComponent<Lockable>().Locked = true;
                    break;
                default:
                    var randomDoor = allConnectionWalls[Random.Range(0, allConnectionWalls.Count)];
                    allConnectionWalls.Remove(randomDoor);
                    randomDoor.GetComponent<Lockable>().Locked = true;
                    break;
            }
        }
    }

    public void IncreaseEnemiesSpeed(float value)
    {
        foreach (var enemy in enemies)
        {
            var agent = enemy.GetComponent<NavMeshAgent>();
            if (agent != null)
                agent.speed += GameSettings.Instanse.IncreaseInResidentSpeed;
        }
    }

    public void IncreaseEnemiesViewAngle(float value)
    {
        foreach (var enemy in enemies)
        {
            var vision = enemy.GetComponent<CreatureVision>();
            if (vision != null)
                vision.ViewAngle += GameSettings.Instanse.IncreaseInResidentViewAngle;
        }
    }

    public void IncreaseEnemiesViewDistance(float value)
    {
        foreach (var enemy in enemies)
        {
            var vision = enemy.GetComponent<CreatureVision>();
            if (vision != null)
                vision.ViewDistance += GameSettings.Instanse.IncreaseInResidentViewDistance;
        }
    }

    public void CreateEnemy(int count)
    {
        for (var i = 0; i < count; ++i)
        {
            var spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            var enemy = Instantiate(GameStorage.Instanse.GetRandomEnemyPrefab(), spawnPoint.position, Quaternion.identity);
            enemies.Add(enemy);
        }
    }

    public void CreatePoliceman(int count)
    {
        for (var i = 0; i < count; ++i)
        {
            var spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            var policeman = Instantiate(GameStorage.Instanse.PolicemanPrefab, spawnPoint.position, Quaternion.identity);
            enemies.Add(policeman);
            policemans.Add(policeman.GetComponent<EnemyAI>());
        }
    }

    private void Start()
    {
        positionsForNextRoom.Enqueue(startPoint.position);
        StartCoroutine(GenerateNextRoom());
    }

    private IEnumerator GenerateNextRoom()
    {
        if (currentRoomsCount >= roomsCount || positionsForNextRoom.Count == 0)
        {
            StartCoroutine(FinishGeneration());
            yield break;
        }

        var transitionPoint = positionsForNextRoom.Dequeue();
        var roomsList = new List<Room>(roomTemplates);
        for (var i = 0; i < roomTemplates.Length; ++i)
        {
            var randomRoomTemplate = roomTemplates[Random.Range(0, roomsList.Count)];
            roomsList.Remove(randomRoomTemplate);

            var room = Instantiate(randomRoomTemplate);
            StartCoroutine(SetRoom(room, transitionPoint));

            yield return new WaitForSeconds(0.5f);

            if (successRoomGenerated)
            {
                room.GetComponent<BoxCollider>().enabled = true;

                var connectionWalls = new List<ConnectionWall>(room.GetConnectionWalls());
                ComplementRoomElements(room);

                if (++currentRoomsCount < roomsCount)
                {
                    connectionWalls.Remove(lastUsedConnectionWall);
                    while (connectionWalls.Count > 0)
                    {
                        var randomConnectionWall = connectionWalls[Random.Range(0, connectionWalls.Count)];
                        connectionWalls.Remove(randomConnectionWall);
                        positionsForNextRoom.Enqueue(randomConnectionWall.PassagePointPosition);
                    }
                }
                break;
            }
            else
            {
                Destroy(room.gameObject);
            }
        }
        StartCoroutine(GenerateNextRoom());
    }

    private IEnumerator SetRoom(Room room, Vector3 transitionPoint)
    {
        successRoomGenerated = false;
        var connectionWalls = room.GetConnectionWalls();
        for (var i = 0; i < connectionWalls.Length; ++i)
        {
            for (var angle = 0; angle < 360; angle += 90)
            {
                yield return new WaitForSeconds(0.01f);

                room.transform.rotation = Quaternion.Euler(0, angle, 0);
                var passagePoint = connectionWalls[i].PassagePointPosition;
                var delta = transitionPoint - passagePoint;
                room.transform.position += delta;

                if (IsIntersectOtherRoom(room))
                    continue;

                successRoomGenerated = true;
                lastUsedConnectionWall = connectionWalls[i];
                CreateFog(room);

                yield break;
            }
        }
        successRoomGenerated = false;
        lastUsedConnectionWall = null;
    }

    private bool IsIntersectOtherRoom(Room room)
    {
        foreach (var cornerTile in room.GetCornerTiles())
        {
            var colliders = Physics.OverlapSphere(cornerTile.CenterPoint, checkRadius, roomMask);
            if (colliders.Length > 0)
                return true;
        }
        return false;
    }

    private void ComplementRoomElements(Room room)
    {
        allConnectionWalls.AddRange(room.GetConnectionWalls());
        allWindows.AddRange(room.GetWindows());
        allWalls.AddRange(room.GetWalls());
        patrolPoints.AddRange(room.GetPatrolPoints());
    }

    private IEnumerator FinishGeneration()
    {
        positionsForNextRoom.Clear();

        DeleteExcessWindows(allWindows);
        DeleteExcessDoorWalls(allConnectionWalls);
        DeleteExcessWalls(allWalls);

        for (var i = allConnectionWalls.Count - 1; i >= 0; --i)
        {
            if (allConnectionWalls[i] == null)
                continue;

            if (allConnectionWalls[i].RemoveAfterGeneration)
            {
                allConnectionWalls[i].Delete();
                allConnectionWalls.RemoveAt(i);
            }
            else
                allConnectionWalls[i].RotateDoor(120);
        }
        LockRandomDoorsAndWindows(lockedDoorsAndWindowsCount);
        CreateEnemies();

        yield return new WaitForSeconds(0.01f);
        navMeshRebaker.Bake();

        foreach (var connectionWall in allConnectionWalls)
        {
            if (connectionWall == null) continue;
            connectionWall.RotateDoor(0);
        }

        Generated = true;
    }

    private void DeleteExcessDoorWalls(List<ConnectionWall> allConnectionWalls)
    {
        for (var i = allConnectionWalls.Count - 1; i >= 0; --i)
        {
            var doorWallsColliders = Physics.OverlapSphere(allConnectionWalls[i].PassagePointPosition, checkRadius, doorWallMask);
            if (doorWallsColliders.Length > 1)
            {
                var connectionWall1 = doorWallsColliders[0].GetComponent<ConnectionWall>();
                var connectionWall2 = doorWallsColliders[1].GetComponent<ConnectionWall>();
                if (!connectionWall1.RemoveAfterGeneration && !connectionWall2.RemoveAfterGeneration)
                {
                    connectionWall1.RemoveAfterGeneration = true;
                }
                continue;
            }

            var wallsColliders = Physics.OverlapSphere(allConnectionWalls[i].PassagePointPosition, checkRadius, wallMask);
            if (wallsColliders.Length == 0)
            {
                if (currentFrontDoorsCount >= frontDoorsCount)
                {
                    var window = Instantiate(windows[Random.Range(0, windows.Length)], allConnectionWalls[i].transform.position, allConnectionWalls[i].transform.rotation);
                    allWindows.Add(window.GetComponent<CenteredPoint>());
                    Destroy(allConnectionWalls[i].gameObject);
                }
                else
                {
                    StartCoroutine(MovePlayer(allConnectionWalls[i]));
                    ++currentFrontDoorsCount;
                }
            }
            else
            {
                Destroy(allConnectionWalls[i].gameObject);
            }
        }
    }

    private void DeleteExcessWindows(List<CenteredPoint> allWindows)
    {
        for (var i = allWindows.Count - 1; i >= 0; --i)
        {
            var wallsColliders = Physics.OverlapSphere(allWindows[i].CenterPoint, checkRadius, wallMask);
            if (wallsColliders.Length > 0)
            {
                Destroy(allWindows[i].gameObject);
                allWindows.RemoveAt(i);
            }
        }
    }

    private void DeleteExcessWalls(List<CenteredPoint> allWalls)
    {
        for (var i = allWalls.Count - 1; i >= 0; --i)
        {
            var colliders = Physics.OverlapSphere(allWalls[i].CenterPoint, checkRadius, wallMask);
            if (colliders.Length > 1)
            {
                Destroy(colliders[1].gameObject);
                allWalls.RemoveAt(i);
            }
        }
    }

    private void CreateFog(Room room)
    {
        var fogs = room.GetFogs();
        foreach (var fog in fogs)
            fog.SetActive(enableFog);
    }

    private IEnumerator MovePlayer(ConnectionWall toWall)
    {
        yield return new WaitForSeconds(0.1f);
        if (movePlayer)
            player.position = toWall.OutsidePoint;
    }

    private void CreateEnemies()
    {
        for (var i = 0; i < enemiesCount; ++i)
        {
            var spawnPosition = GetRandomPatrolPoint().position;
            var enemy = Instantiate(GameStorage.Instanse.GetRandomEnemyPrefab(), spawnPosition, Quaternion.identity);
            enemies.Add(enemy);
        }
    }
}
