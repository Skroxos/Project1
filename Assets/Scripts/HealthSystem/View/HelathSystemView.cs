using UnityEngine;
using UnityEngine.UI;

public class HelathSystemView : MonoBehaviour
{
    [SerializeField] private Slider healthBar;

    public void SetData(float precentage)
    {
        healthBar.value = precentage;
    }
}
