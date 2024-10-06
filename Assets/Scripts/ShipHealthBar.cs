
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Health _health;

    private Slider _slider;

    private void Start()
    {
        _slider = GetComponent<Slider>();
        _health.OnDamage.AddListener(OnDamage);
    }

    private void OnDamage(Health health, int amount, GameObject attacker)
    {
        _slider.value = ((float) _health.CurrentHealth / _health.MaxHealth);
    }
}
