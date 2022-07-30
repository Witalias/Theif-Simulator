using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class LevelGenerator : MonoBehaviour
{
    private const float checkRadius = 0.5f;

    [SerializeField] private int roomsCount = 5;
    [SerializeField] private int frontDoorsCount = 1;
    [SerializeField] private Transform startPoint;
    [SerializeField] private NavMeshRebaker navMeshRebaker;
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask roomMask;
    [SerializeField] private LayerMask doorWallMask;
    [SerializeField] private LayerMask wallMask;
    [SerializeField] private Room[] roomTemplates;
    [SerializeField] private GameObject[] windows;
    [SerializeField] private GameObject[] walls;

    private bool generated = false;
    private int currentRoomsCount = 0;
    private int currentFrontDoorsCount = 0;
    private readonly Queue<Vector3> positionsForNextRoom = new Queue<Vector3>();
    private readonly List<ConnectionWall> allConnectionWalls = new List<ConnectionWall>();
    private readonly List<CenteredPoint> allWindows = new List<CenteredPoint>();
    private readonly List<CenteredPoint> allWalls = new List<CenteredPoint>();

    private bool successRoomGenerated = false;
    private ConnectionWall lastUsedConnectionWall = null;

    public bool Generated { get => generated; }

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
                allConnectionWalls.AddRange(connectionWalls);
                allWindows.AddRange(room.GetWindows());
                allWalls.AddRange(room.GetWalls());

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

    private IEnumerator FinishGeneration()
    {
        positionsForNextRoom.Clear();

        DeleteExcessWindows(allWindows);
        DeleteExcessDoorWalls(allConnectionWalls);
        DeleteExcessWalls(allWalls);


        foreach (var connectionWall in allConnectionWalls)
        {
            if (connectionWall == null) continue;
            connectionWall.RotateDoor(120);
            connectionWall.Delete();
        }

        yield return new WaitForSeconds(0.01f);
        navMeshRebaker.Bake();

        foreach (var connectionWall in allConnectionWalls)
        {
            if (connectionWall == null) continue;
            connectionWall.RotateDoor(0);
        }

        generated = true;
    }

    private void DeleteExcessDoorWalls(List<ConnectionWall> allConnectionWalls)
    {
        foreach (var connectionWall in allConnectionWalls)
        {
            var doorWallsColliders = Physics.OverlapSphere(connectionWall.PassagePointPosition, checkRadius, doorWallMask);
            if (doorWallsColliders.Length > 1)
            {
                var connectionWall1 = doorWallsColliders[0].GetComponent<ConnectionWall>();
                var connectionWall2 = doorWallsColliders[1].GetComponent<ConnectionWall>();
                if (!connectionWall1.RemoveAfterGeneration && !connectionWall2.RemoveAfterGeneration)
                    connectionWall1.RemoveAfterGeneration = true;
                continue;
            }

            var wallsColliders = Physics.OverlapSphere(connectionWall.PassagePointPosition, checkRadius, wallMask);
            if (wallsColliders.Length == 0)
            {
                if (currentFrontDoorsCount >= frontDoorsCount)
                {
                    Instantiate(windows[Random.Range(0, windows.Length)], connectionWall.transform.position, connectionWall.transform.rotation);
                    Destroy(connectionWall.gameObject);
                }
                else
                {
                    ++currentFrontDoorsCount;
                    player.position = connectionWall.OutsidePoint;
                }
            }
            else
                Destroy(connectionWall.gameObject);
        }
    }

    private void DeleteExcessWindows(List<CenteredPoint> allWindows)
    {
        foreach (var window in allWindows)
        {
            var wallsColliders = Physics.OverlapSphere(window.CenterPoint, checkRadius, wallMask);
            if (wallsColliders.Length > 0)
                Destroy(window.gameObject);
        }
    }

    private void DeleteExcessWalls(List<CenteredPoint> allWalls)
    {
        foreach (var wall in allWalls)
        {
            var colliders = Physics.OverlapSphere(wall.CenterPoint, checkRadius, wallMask);
            if (colliders.Length > 1)
                Destroy(colliders[1].gameObject);
        }
    }
}
