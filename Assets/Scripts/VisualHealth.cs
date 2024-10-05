using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class VisualHealth : MonoBehaviour
{
    private PlayerController controller;

    private Health playerHealth;

    public Sprite filledHeart, unfilledHeart;
    public Image[] hearts;

    private void Awake() 
    {

        if (hearts == null || hearts.Length == 0)
        {
            Debug.LogError("feet");
        }
        
    }

    private void Start()
    {

    }

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

    public void TakeDamage(Health health, int amount)
    {

        SetImage(health.CurrentHealth, health.MaxHealth);

    }

    private void Update() 
    {

        if(controller == null)
        {

            controller = PlayerController.LocalController;
            if(!controller) { return; }
            playerHealth = controller.GetComponent<Health>();

            playerHealth.OnDamage.AddListener((health, amount) => TakeDamage(health, amount));

        }

        SetImage(playerHealth.CurrentHealth, playerHealth.MaxHealth);
        
    }

}

public enum HeartStats
{

    Empty = 0,
    Full = 1,

}
