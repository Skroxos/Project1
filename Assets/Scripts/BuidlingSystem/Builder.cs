using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Builder : MonoBehaviour
{
    public WorldGrid worldGrid;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private BuildPieceSO[] availableBuildPieces;
    [SerializeField] private LayerMask buildableLayerMask;
    [SerializeField] private float gridSnapThreshold;
    [SerializeField] private Camera cam;

    private bool isBuildingMode;
    private GameObject ghostObject;
    private Quaternion rotation = Quaternion.identity;

    private BuildPieceSO selectedBuildPiece;
    private List<SocketCompatibility> cachedGhostSockets;
    private List<SocketCompatibility> cachedTargetSockets;
    private Collider lastHitCollider;
    private SocketCompatibility currentSocket;
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

    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            return hitInfo.point;
        }

        return Vector3.zero;
    }

   

    private void OnDrawGizmos()
    {
        if (cam == null) return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(ray.origin, ray.direction * 100f);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(GetMouseWorldPosition(), 0.5f);

        if (ghostObject != null)
        {
            Vector3 boxCenter = GetCenterOfGhostObject();
            Quaternion boxRotation = ghostObject.transform.rotation;
  
            Vector3 drawSize = ghostObject.transform.localScale * 0.9f;
            
            bool canBuild = CanBuild();
            Gizmos.color = canBuild ? Color.green : Color.red;
            
            Matrix4x4 oldMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(boxCenter, boxRotation, Vector3.one);
            
            Gizmos.DrawWireCube(Vector3.zero, drawSize);
            
            Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 0.25f);
            Gizmos.DrawCube(Vector3.zero, drawSize);
            
            Gizmos.matrix = oldMatrix;
        }
    }



    private void EnterBuildMode()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (isBuildingMode)
            {
                ExitBuildMode();
                return;
            }
            isBuildingMode = true;
            ghostObject = ObjectPooler.Instance.GetPooledObject(selectedBuildPiece.piecePrefab);
            ghostObject.SetActive(true);
            GetGhostObjectSockets(); 
            ghostObject.gameObject.GetComponentInChildren<Collider>().enabled = false;
        }
    }

    private void ExitBuildMode()
    {
        isBuildingMode = false;
        if (ghostObject != null)
        {
            ghostObject.SetActive(false);
            ghostObject = null;
        }
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
                if (ghostObject != null)
                {
                    ghostObject.SetActive(false);
                }
                ghostObject = ObjectPooler.Instance.GetPooledObject(selectedBuildPiece.piecePrefab);
                if (ghostObject != null)
                {
                    ghostObject.SetActive(true);
                    GetGhostObjectSockets();
                    ghostObject.gameObject.GetComponentInChildren<Collider>().enabled = false;
                }
            }
        }
    }


    private void CheckForSocketConnections()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, buildableLayerMask))
        {
            if (hitInfo.collider != lastHitCollider)
            {
                lastHitCollider = hitInfo.collider;
                cachedTargetSockets = hitInfo.collider.GetComponentInParent<BuildPiece>().mySockets;
            }

            foreach (SocketCompatibility sockets in cachedTargetSockets)
            {
                float distanceSquared = (sockets.transform.position - hitInfo.point).sqrMagnitude;
                float thresholdSquared = gridSnapThreshold * gridSnapThreshold;
                if (distanceSquared < thresholdSquared)
                {
                    currentSocket = sockets;
                    foreach (var myGhostSocket in cachedGhostSockets)
                    {
                        if (currentSocket.IsCompatible(myGhostSocket))
                        {
                            ghostObject.transform.position = sockets.transform.position - (myGhostSocket.transform.position - ghostObject.transform.position);
                            isSnappedToSocket = true;
                            return;
                        }
                    }
                }
                isSnappedToSocket = false;
            }
        }
        else
        {
            lastHitCollider = null;
        }
    }
 
   

    private void GetGhostObjectSockets()
    {
        cachedGhostSockets = new List<SocketCompatibility>();
        SocketCompatibility[] sockets = ghostObject.GetComponentsInChildren<SocketCompatibility>();
        foreach (var socket in sockets)
        {
            cachedGhostSockets.Add(socket);
        }
    }
    
    
    
    private bool CanBuild()
    {
        if (ghostObject == null) return false;
        Bounds ghostBounds = new Bounds(GetCenterOfGhostObject(), ghostObject.transform.localScale);
        Vector3 detectionBoxSize = ghostBounds.extents * 0.9f;
        return !Physics.CheckBox(ghostBounds.center, detectionBoxSize, ghostObject.transform.rotation, buildableLayerMask);
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
        
        GameObject newBuilding = ObjectPooler.Instance.GetPooledObject(selectedBuildPiece.piecePrefab);
        if (newBuilding != null)
        {
            newBuilding.transform.position = finalPosition;
            newBuilding.transform.rotation = finalRotation;
            newBuilding.SetActive(true);
        }
        isSnappedToSocket = false;
    }

    
    
}