using System;
using UnityEngine;

public class HotbarPresenter : MonoBehaviour
{
    [SerializeField] private HotbarView hotBarView;
    [SerializeField] private int hotBarSize = 5;
    private HotBarModel hotBarModel;

    private void Start()
    {
        hotBarModel = new HotBarModel(hotBarSize);
        hotBarModel.OnSelectionChanged += UpdateHotBarSelection;
    }

    private void OnDestroy()
    {
        hotBarModel.OnSelectionChanged -= UpdateHotBarSelection;
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            int direction = scroll > 0 ? 1 : -1;
            hotBarModel.ChangeSelection(direction);
        }
        for (int i = 0; i < hotBarSize; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                hotBarModel.SetSelection(i);
            }
        }
    }
    private void UpdateHotBarSelection(int obj)
    {
        hotBarView.MoveSelector(obj);
    }

   
}
