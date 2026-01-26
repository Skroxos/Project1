using UnityEngine;

[CreateAssetMenu(fileName = "BuildPieceSO", menuName = "ScriptableObjects/BuildPieceSO", order = 1)]
public class BuildPieceSO : ScriptableObject
{
    public string pieceName;
    public GameObject piecePrefab;
}