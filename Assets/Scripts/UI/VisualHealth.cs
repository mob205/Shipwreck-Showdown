using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Mirror;

public class VisualHealth : MonoBehaviour
{
    private PlayerController controller;

    private Health playerHealth;

    public Sprite filledHeart, unfilledHeart;
    public Image[] hearts;
    public Image damageFlashPanel;

    public float damageFlashDuration;

    public void SetImage(int currentHealth, int maxHealth)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentHealth)
            {
                hearts[i].sprite = filledHeart;
            }
            else
            {
                hearts[i].sprite = unfilledHeart;
            }
        }
    }

    public void TakeDamage(Health health, int amount, GameObject attacker)
    {
        SetImage(health.CurrentHealth, health.MaxHealth);
        damageFlashPanel.enabled = true;

        Invoke(nameof(DisableFlashPanel), damageFlashDuration);
    }

    private void DisableFlashPanel()
    {
        damageFlashPanel.enabled = false;
    }

    private void Update() 
    {
        if(controller == null)
        {
            controller = PlayerController.LocalController;
            if(!controller) { return; }
            playerHealth = controller.GetComponent<Health>();

            playerHealth.OnDamage.AddListener((health, amount, attacker) => TakeDamage(health, amount, attacker));
        }
    }
}

public enum HeartStats
{

    Empty = 0,
    Full = 1,

}
