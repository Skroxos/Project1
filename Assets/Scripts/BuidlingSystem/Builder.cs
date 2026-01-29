using System.Collections.Generic;
using UnityEngine;

public class Builder : MonoBehaviour
{
    private WorldGrid _worldGrid;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private BuildPieceSO[] availableBuildPieces;
    [SerializeField] private LayerMask buildableLayerMask;
    [SerializeField] private float gridSnapThreshold;
    [SerializeField] private Camera cam;

    private bool _isBuildingMode;
    private GameObject _ghostObject;
    private Quaternion _rotation = Quaternion.identity;

    private BuildPieceSO _selectedBuildPiece;
    private List<SocketCompatibility> _cachedGhostSockets;
    private List<SocketCompatibility> _cachedTargetSockets;
    private Collider _lastHitCollider;
    private SocketCompatibility _currentSocket;
    private bool _isSnappedToSocket;

    [SerializeField] private int width = 1;
    [SerializeField] private int height = 1;
    [SerializeField] private float cellSize = 1f;

    private void Start()
    {
        Vector3 originPosition = new Vector3(width * cellSize / 2 * -1, 0, height * cellSize / 2 * -1);
        Cursor.lockState = CursorLockMode.Locked;
        _worldGrid = new WorldGrid(width, height, cellSize, originPosition);

        _selectedBuildPiece = availableBuildPieces[0];
    }

    private void Update()
    {
        EnterBuildMode();
        if (_isBuildingMode)
        {
            Vector3 mouseWorldPosition = GetMouseWorldPosition();
            GhostObjectSnapToGrid(mouseWorldPosition);
            RotateGhostObject();
            SellectBuildPiece();
            CheckForSocketConnections();
            if (Input.GetMouseButtonDown(0) && CanBuild())
            {
                BuildBuildPieceAt(_ghostObject.transform.position);
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

        if (_ghostObject != null)
        {
            Vector3 boxCenter = GetCenterOfGhostObject();
            Quaternion boxRotation = _ghostObject.transform.rotation;
  
            Vector3 drawSize = _ghostObject.transform.localScale * 0.9f;
            
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
            if (_isBuildingMode)
            {
                ExitBuildMode();
                return;
            }
            _isBuildingMode = true;
            _ghostObject = ObjectPooler.Instance.GetPooledObject(_selectedBuildPiece.piecePrefab);
            _ghostObject.SetActive(true);
            GetGhostObjectSockets(); 
            _ghostObject.gameObject.GetComponentInChildren<Collider>().enabled = false;
        }
    }

    private void ExitBuildMode()
    {
        _isBuildingMode = false;
        if (_ghostObject != null)
        {
            _ghostObject.SetActive(false);
            _ghostObject = null;
        }
    }

    private void RotateGhostObject()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            _rotation *= Quaternion.Euler(0, 90, 0);
        }
    }

    private void SellectBuildPiece()
    {
        for (int i = 0; i < availableBuildPieces.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                _selectedBuildPiece = availableBuildPieces[i];
                if (_ghostObject != null)
                {
                    _ghostObject.SetActive(false);
                }
                _ghostObject = ObjectPooler.Instance.GetPooledObject(_selectedBuildPiece.piecePrefab);
                if (_ghostObject != null)
                {
                    _ghostObject.SetActive(true);
                    GetGhostObjectSockets();
                    _ghostObject.gameObject.GetComponentInChildren<Collider>().enabled = false;
                }
            }
        }
    }


    private void CheckForSocketConnections()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, buildableLayerMask))
        {
            if (hitInfo.collider != _lastHitCollider)
            {
                _lastHitCollider = hitInfo.collider;
                _cachedTargetSockets = hitInfo.collider.GetComponentInParent<BuildPiece>().mySockets;
            }

            foreach (SocketCompatibility sockets in _cachedTargetSockets)
            {
                float distanceSquared = (sockets.transform.position - hitInfo.point).sqrMagnitude;
                float thresholdSquared = gridSnapThreshold * gridSnapThreshold;
                if (distanceSquared < thresholdSquared)
                {
                    _currentSocket = sockets;
                    foreach (var myGhostSocket in _cachedGhostSockets)
                    {
                        if (_currentSocket.IsCompatible(myGhostSocket))
                        {
                            _ghostObject.transform.position = sockets.transform.position - (myGhostSocket.transform.position - _ghostObject.transform.position);
                            _isSnappedToSocket = true;
                            return;
                        }
                    }
                }
                _isSnappedToSocket = false;
            }
        }
        else
        {
            _lastHitCollider = null;
        }
    }
 
   

    private void GetGhostObjectSockets()
    {
        _cachedGhostSockets = new List<SocketCompatibility>();
        SocketCompatibility[] sockets = _ghostObject.GetComponentsInChildren<SocketCompatibility>();
        foreach (var socket in sockets)
        {
            _cachedGhostSockets.Add(socket);
        }
    }
    
    
    
    private bool CanBuild()
    {
        if (_ghostObject == null) return false;
        Bounds ghostBounds = new Bounds(GetCenterOfGhostObject(), _ghostObject.transform.localScale);
        Vector3 detectionBoxSize = ghostBounds.extents * 0.9f;
        return !Physics.CheckBox(ghostBounds.center, detectionBoxSize, _ghostObject.transform.rotation, buildableLayerMask);
    }
    
    private Vector3 GetCenterOfGhostObject()
    {
        Renderer[] renderers = _ghostObject.GetComponentsInChildren<Renderer>();
        Bounds combinedBounds = new Bounds(_ghostObject.transform.position, Vector3.zero);
        foreach (Renderer renderer in renderers)
        {
            combinedBounds.Encapsulate(renderer.bounds);
        }
        return combinedBounds.center;
    }
    private void GhostObjectSnapToGrid(Vector3 mouseWorldPosition)
    {
        Vector3 snapPosition = _worldGrid.GetClosestEdgeWorldPosition(mouseWorldPosition);
        _ghostObject.transform.position = snapPosition;

        Quaternion basePrefabRotation = _selectedBuildPiece.piecePrefab.transform.rotation;
        _ghostObject.transform.rotation = _rotation * basePrefabRotation;
    }

    private void BuildBuildPieceAt(Vector3 worldPosition)
    {
        Vector3 finalPosition;
        Quaternion finalRotation;
        
        if (_isSnappedToSocket)
        {
            finalPosition = _ghostObject.transform.position;
            finalRotation = _ghostObject.transform.rotation;
        }
        else
        {
            finalPosition = _worldGrid.GetClosestEdgeWorldPosition(worldPosition);
            Quaternion basePrefabRotation = _selectedBuildPiece.piecePrefab.transform.rotation;
            finalRotation = _rotation * basePrefabRotation;
        }
        
        GameObject newBuilding = ObjectPooler.Instance.GetPooledObject(_selectedBuildPiece.piecePrefab);
        if (newBuilding != null)
        {
            newBuilding.transform.position = finalPosition;
            newBuilding.transform.rotation = finalRotation;
            newBuilding.SetActive(true);
        }
        _isSnappedToSocket = false;
    }

    
    
}