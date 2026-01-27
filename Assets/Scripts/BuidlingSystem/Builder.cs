using System;
using System.Collections.Generic;
using UnityEngine;

public class Builder : MonoBehaviour
{
    public WorldGrid worldGrid;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private BuildPieceSO[] availableBuildPieces;
    [SerializeField] private LayerMask buildableLayerMask;
    [SerializeField] private float gridSnapThreshold;

    private bool isBuildingMode;
    private GameObject ghostObject;
    private Quaternion rotation = Quaternion.identity;

    private BuildPieceSO selectedBuildPiece;
    private List<Transform> ghostSocekts;
    private SocketCompatibility currentSocket;
    private SocketCompatibility ghostSocket;
    private bool isSnappedToSocket;

    [SerializeField] private int width = 1;
    [SerializeField] private int height = 1;
    [SerializeField] private float cellSize = 1f;

    private void Start()
    {
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
            CheckForSocketConnections();
            if (Input.GetMouseButtonDown(0) && CanBuild())
            {
                BuildBuildPieceAt(ghostObject.transform.position);
            }
        }

        Debug.Log(CanBuild());

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

   

    private void OnDrawGizmos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(ray.origin, ray.direction * 100f);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(GetMouseWorldPosition(), 0.5f);

        Gizmos.color = Color.blue;
        if (ghostObject != null)
        {
            
            Matrix4x4 oldMatrix = Gizmos.matrix;
            
            Vector3 boxCenter = GetCenterOfGhostObject();
            Quaternion boxRotation = ghostObject.transform.rotation;
            
            Vector3 boxSize = ghostObject.transform.localScale;
            
            Gizmos.matrix = Matrix4x4.TRS(boxCenter, boxRotation, Vector3.one);
            
            Gizmos.DrawWireCube(Vector3.zero, boxSize / 5);
            
            Gizmos.matrix = oldMatrix;
        }
    }


    private void EnterBuildMode()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            isBuildingMode = true;
            ghostObject = Instantiate(selectedBuildPiece.piecePrefab);
            ghostSocekts = GetGhostObjectSockets();
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
                ghostSocekts = GetGhostObjectSockets();
                ghostObject.gameObject.GetComponentInChildren<Collider>().enabled = false;
            }
        }
    }


    private void CheckForSocketConnections()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, buildableLayerMask))
        {
            var hitBuildPiece = hitInfo.collider.GetComponentInParent<BuildPiece>().sockets;
            foreach (Transform socketTransform in hitBuildPiece)
            {
                if (Vector3.Distance(socketTransform.position, hitInfo.point) < gridSnapThreshold)
                {
                    currentSocket = socketTransform.GetComponent<SocketCompatibility>();
                    foreach (Transform ghostSocketTransform in ghostSocekts)
                    {
                        ghostSocket = ghostSocketTransform.GetComponent<SocketCompatibility>();
                        if (currentSocket.IsCompatible(ghostSocket))
                        {
                            ghostObject.transform.position = socketTransform.position - (ghostSocketTransform.position - ghostObject.transform.position);
                            isSnappedToSocket = true;
                            return;
                        }
                    }
                }
                isSnappedToSocket = false;
            }
        }
    }

   

    private List<Transform> GetGhostObjectSockets()
    {
        List<Transform> socketList = new List<Transform>();
        foreach (Transform socketTransform in ghostObject.GetComponent<BuildPiece>().sockets)
        {
            socketList.Add(socketTransform);
        }
        return socketList;
    }
    
    private bool CanBuild()
    {
        if (ghostObject == null) return false;
        return !Physics.CheckBox(GetCenterOfGhostObject(), ghostObject.transform.localScale / 5, ghostObject.transform.rotation, buildableLayerMask);
    }
    
    private Vector3 GetCenterOfGhostObject()
    {
        Renderer[] renderers = ghostObject.GetComponentsInChildren<Renderer>();
        Bounds combinedBounds = new Bounds(ghostObject.transform.position, Vector3.zero);
        foreach (Renderer renderer in renderers)
        {
            combinedBounds.Encapsulate(renderer.bounds);
        }
        return combinedBounds.center;
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
        Vector3 finalPosition;
        Quaternion finalRotation;
        
        if (isSnappedToSocket)
        {
            finalPosition = ghostObject.transform.position;
            finalRotation = ghostObject.transform.rotation;
        }
        else
        {
            finalPosition = worldGrid.GetClosestEdgeWorldPosition(worldPosition);
            Quaternion basePrefabRotation = selectedBuildPiece.piecePrefab.transform.rotation;
            finalRotation = rotation * basePrefabRotation;
        }
        
        GameObject newBuilding = Instantiate(selectedBuildPiece.piecePrefab, finalPosition, finalRotation);
        isSnappedToSocket = false;
    }

    
    
}