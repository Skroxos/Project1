using UnityEngine;

public class Builder : MonoBehaviour
{
    public WorldGrid worldGrid;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private BuildPieceSO[] availableBuildPieces;

    private bool isBuildingMode;
    private GameObject ghostObject;
    private Quaternion rotation = Quaternion.identity;

    private void Start()
    {
        int width = 50;
        int height = 50;
        float cellSize = 5f;
        Vector3 originPosition = new Vector3(width * cellSize / 2 * -1, 0, height * cellSize / 2 * -1);

        worldGrid = new WorldGrid(width, height, cellSize, originPosition);
    }
    
    private void Update()
    {
        EnterBuildMode();
        if (isBuildingMode)
        {
            Vector3 mouseWorldPosition = GetMouseWorldPosition();
            worldGrid.GetXY(mouseWorldPosition, out int xx, out int zz);
            GhostObjectSnapToGrid(xx, zz);
            RotateGhostObject();
            if (Input.GetMouseButtonDown(0))
            {
                worldGrid.GetXY(mouseWorldPosition, out int x, out int z);
                BuildBuildPieceAt(x, z, availableBuildPieces[0]);
                Debug.Log($"Grid Coordinates: X={x}, Z={z}");
            }
        }
       
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            return hitInfo.point;
        }
        return Vector3.zero;
    }

    private void EnterBuildMode()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            isBuildingMode = true;
            ghostObject = Instantiate(availableBuildPieces[0].piecePrefab);
            ghostObject.gameObject.GetComponent<Collider>().enabled = false;
            Color ghostColor = new Color(1f, 1f, 1f, 0.5f);
        }
    }
    
    private void ExitBuildMode()
    {
        isBuildingMode = false;
        Destroy(ghostObject);
    }
    
    private void RotateGhostObject()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            rotation *= Quaternion.Euler(0, 90, 0);
            ghostObject.transform.rotation = rotation;
        }
    }
    
    private void GhostObjectSnapToGrid(int x, int z)
    {
       Vector3 snapPosition = worldGrid.GetWorldPositionCenter(x, z);
       ghostObject.transform.position = snapPosition;
       ghostObject.transform.rotation = rotation;
    }
    

    private void BuildBuildPieceAt(int x, int z, BuildPieceSO buildPiece)
    {
        Vector3 buildPosition = worldGrid.GetWorldPositionCenter(x, z);
        Instantiate(buildPiece.piecePrefab, buildPosition, rotation);
    }
}