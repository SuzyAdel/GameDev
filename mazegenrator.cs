using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MazeGenerator : MonoBehaviour
{
    public GameObject brickWallPrefab;
    public GameObject metalWallPrefab;
    public GameObject groundPlane;
    public GameObject player;
    public int gridSize = 5; // Must be odd (5,7,9,...)
    public float wallHeight = 2.0f;

    private float cellSize;
    private bool[,] mazeGrid;
    private Vector2Int startPos = new Vector2Int(1, 1); // Defined here

    void Start()
    {
        FixAudioListeners();
        CalculateCellSize();
        InitializeMazeGrid();
        GenerateMazeDFS(startPos.x, startPos.y);
        PlacePlayer();
        PlaceWalls();
    }

    void FixAudioListeners()
    {
        // Ensure only one active AudioListener
        AudioListener[] listeners = FindObjectsOfType<AudioListener>();
        if (listeners.Length > 1)
        {
            // Keep only the first one, disable others
            for (int i = 1; i < listeners.Length; i++)
            {
                listeners[i].enabled = false;
            }
        }
    }

    void CalculateCellSize()
    {
        Renderer groundRenderer = groundPlane.GetComponent<Renderer>();
        cellSize = groundRenderer.bounds.size.x / gridSize;
        Debug.Log($"Cell size: {cellSize}");
    }

    void InitializeMazeGrid()
    {
        mazeGrid = new bool[gridSize, gridSize];
        // Initialize all as walls (false)
        for (int x = 0; x < gridSize; x++)
            for (int y = 0; y < gridSize; y++)
                mazeGrid[x, y] = false;
    }

    void GenerateMazeDFS(int x, int y)
    {
        mazeGrid[x, y] = true; // Mark as path
        Vector2Int[] dirs = { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down };
        dirs = dirs.OrderBy(d => Random.value).ToArray(); // Randomize directions

        foreach (var dir in dirs)
        {
            int nx = x + dir.x * 2, ny = y + dir.y * 2;
            if (nx >= 0 && nx < gridSize && ny >= 0 && ny < gridSize && !mazeGrid[nx, ny])
            {
                mazeGrid[x + dir.x, y + dir.y] = true; // Carve wall between
                GenerateMazeDFS(nx, ny);
            }
        }
    }

    void PlacePlayer()
    {
        Vector3 worldPos = new Vector3(
            groundPlane.transform.position.x - (gridSize * cellSize / 2) + startPos.x * cellSize,
            0.5f, // Player height
            groundPlane.transform.position.z - (gridSize * cellSize / 2) + startPos.y * cellSize
        );
        player.transform.position = worldPos;
    }

    void PlaceWalls()
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                if (!mazeGrid[x, y]) // Only place walls where mazeGrid is false
                {
                    Vector3 position = new Vector3(
                        groundPlane.transform.position.x - (gridSize * cellSize / 2) + x * cellSize,
                        wallHeight / 2,
                        groundPlane.transform.position.z - (gridSize * cellSize / 2) + y * cellSize
                    );

                    // Random rotation (50% chance vertical)
                    Quaternion rotation = Random.value > 0.5f ?
                        Quaternion.Euler(0, -90, 0) :
                        Quaternion.identity;

                    GameObject wallPrefab = Random.value > 0.5f ? brickWallPrefab : metalWallPrefab;
                    Instantiate(wallPrefab, position, rotation, transform);
                }
            }
        }
    }
}