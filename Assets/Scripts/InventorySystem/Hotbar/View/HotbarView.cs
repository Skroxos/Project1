using UnityEngine;

public class HotbarView : MonoBehaviour
{
    [SerializeField] private RectTransform selectorTransform;
    [SerializeField] private RectTransform[] hotbarSlots;

    public void MoveSelector(int index)
    {
        if (index < 0 || index >= hotbarSlots.Length) return;
        selectorTransform.position = hotbarSlots[index].position;
    }
}
