using System;
using UnityEngine;

public class HotBarModel
{
    public int SelectedIndex { get; private set; }
    public int HotBarSize { get; private set;  }
    
    public event Action<int> OnSelectionChanged;
    
    public HotBarModel(int hotBarSize)
    {
        HotBarSize = hotBarSize;
        SelectedIndex = 0;
    }
    
    public void ChangeSelection(int direction)
    {
        SelectedIndex = (SelectedIndex - direction + HotBarSize) % HotBarSize;
        OnSelectionChanged?.Invoke(SelectedIndex);
    }
    
    public void SetSelection(int index)
    {
        if (index < 0 || index >= HotBarSize) return;
        SelectedIndex = index;
        OnSelectionChanged?.Invoke(SelectedIndex);
    }
    
}
