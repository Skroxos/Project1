using UnityEngine;

public class HealthPresenter : MonoBehaviour, IDamagable
{
    [SerializeField] private HelathSystemView _view;
    [SerializeField] private int _initialHealth = 100;
    private HealthModel _model;
    
    private void Awake()
    {
        _model = new HealthModel(_initialHealth);
        _model.OnHealthChanged += UpdateView;
        _model.OnDeath += HandleDeath;
        UpdateView(_model.CurrentHealth);
    }

    private void HandleDeath()
    {
        // View can display death screnn or smthing...
        Debug.Log("Character has died.");
    }

    private void UpdateView(int obj)
    {
        float percentage = (float)obj / _model.MaxHealth;
        _view.SetData(percentage);
    }
    
    public void TakeDamage(int damage)
    {
        _model.TakeDamage(damage);
    }
    
}
