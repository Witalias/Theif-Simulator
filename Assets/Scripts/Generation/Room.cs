using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] private ConnectionWall[] connectionWalls;
    [SerializeField] private CenteredPoint[] windows;
    [SerializeField] private CenteredPoint[] walls;
    [SerializeField] private CenteredPoint[] cornerTiles;

    public ConnectionWall[] GetConnectionWalls() => connectionWalls;

    public CenteredPoint[] GetWindows() => windows;

    public CenteredPoint[] GetWalls() => walls;

    public CenteredPoint[] GetCornerTiles() => cornerTiles;

}
