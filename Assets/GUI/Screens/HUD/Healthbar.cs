﻿using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Healthbar : MonoBehaviour 
{
    public enum HealthType
    {
        Shield,
        Armor
    }

    public HealthType type;
    public int shieldSector;

    public string stateName = "Healthbar";
    public int healthStatusLayer = 0;

    [Tooltip("Leave blank for player")]
    public Hitpoints ship;
    
    private Animator animator;
    private int healthbarStatehash;

    void Start()
    {
        animator = GetComponent<Animator>();
        healthbarStatehash = Animator.StringToHash(stateName); 
    }

    void Update()
    {
        var player = PlayerShip.LocalPlayer;
        var target = ship;
        if (target == null && player != null)
        {
            target = player.GetComponent<Hitpoints>();
        }
        
        if (target)
        {
            float currentHealth;
            float maxHealth;

            if (type == HealthType.Armor)
            {
                currentHealth = target.GetArmor();
                maxHealth = target.GetMaxArmor();
            }
            else
            {
                currentHealth = target.GetShield(shieldSector);
                maxHealth = target.GetMaxShields(shieldSector);
            }

            animator.Play(healthbarStatehash, healthStatusLayer, currentHealth / maxHealth);
        }
        else
        {
            Debug.Log("Tried to update a healthbar with no ship ref");
        }
    }
}
