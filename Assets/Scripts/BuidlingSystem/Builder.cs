using System;
using UnityEngine;

public class Builder : MonoBehaviour
{
    public WorldGrid worldGrid;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private BuildPieceSO[] availableBuildPieces;

    private bool isBuildingMode;
    private GameObject ghostObject;
    private Quaternion rotation = Quaternion.identity;
    
    private BuildPieceSO selectedBuildPiece;

    private void Start()
    {
        int width = 1;
        int height = 1;
        float cellSize = 1f;
        Vector3 originPosition = new Vector3(width * cellSize / 2 * -1, 0, height * cellSize / 2 * -1);
        Cursor.lockState = CursorLockMode.Locked;
        worldGrid = new WorldGrid(width, height, cellSize, originPosition);
        
        selectedBuildPiece = availableBuildPieces[0];
    }
    
    private void Update()
    {
        EnterBuildMode();
        if (isBuildingMode)
        {
            Vector3 mouseWorldPosition = GetMouseWorldPosition();
            GhostObjectSnapToGrid(mouseWorldPosition); 
            RotateGhostObject();
            SellectBuildPiece();
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 buildPosition = worldGrid.GetClosestEdgeWorldPosition(mouseWorldPosition);
                BuildBuildPieceAt(buildPosition);
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
            ghostObject = Instantiate(selectedBuildPiece.piecePrefab);
            ghostObject.gameObject.GetComponentInChildren<Collider>().enabled = false;
            Color ghostColor = new Color(1f, 1f, 1f, 0.5f);
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitBuildMode();
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
        }
    }
    
    private void SellectBuildPiece()
    {
       for (int i = 0; i < availableBuildPieces.Length; i++)
       {
           if (Input.GetKeyDown(KeyCode.Alpha1 + i))
           {
               selectedBuildPiece = availableBuildPieces[i];
               Destroy(ghostObject);
               ghostObject = Instantiate(selectedBuildPiece.piecePrefab);
               ghostObject.gameObject.GetComponentInChildren<Collider>().enabled = false;
           }
       }
    }
    
    
    private void GhostObjectSnapToGrid(Vector3 mouseWorldPosition)
    {
        Vector3 snapPosition = worldGrid.GetClosestEdgeWorldPosition(mouseWorldPosition);
        ghostObject.transform.position = snapPosition;
        
        Quaternion basePrefabRotation = selectedBuildPiece.piecePrefab.transform.rotation;
        ghostObject.transform.rotation = rotation * basePrefabRotation;
    }
    
    

  
    private void BuildBuildPieceAt(Vector3 worldPosition)
    {
        Vector3 buildPosition = worldGrid.GetClosestEdgeWorldPosition(worldPosition);
        
        Quaternion basePrefabRotation = selectedBuildPiece.piecePrefab.transform.rotation;
        Quaternion finalRotation = rotation * basePrefabRotation;

        Instantiate(selectedBuildPiece.piecePrefab, buildPosition, finalRotation);
    }
}