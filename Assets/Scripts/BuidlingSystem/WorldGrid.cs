using UnityEngine;
public class WorldGrid
{
    private int width;
    private int height;
    private float cellSize;
    private Vector3 originPosition;
    private int[,] gridArray;
    
    public WorldGrid(int width, int height, float cellSize, Vector3 originPosition)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;
        gridArray = new int[width, height];
        
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int z = 0; z < gridArray.GetLength(1); z++)
            {
                Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x, z + 1), Color.white, 100f);
                Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x + 1, z), Color.white, 100f);
            }
        }
    }
    
    public Vector3 GetWorldPosition(int x, int z)
    {
        return new Vector3(x,0,z) * cellSize + originPosition;
    }
    
    public Vector3 GetWorldPositionCenter(int x, int z)
    {
        return new Vector3(x + 0.5f, 0, z + 0.5f) * cellSize + originPosition;
    }
    
    public void GetXY(Vector3 worldPosition, out int x, out int z)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        z = Mathf.FloorToInt((worldPosition - originPosition).z / cellSize);
    }
}
