using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("Player Stats")]
    [SerializeField] float currentHealth;
    [SerializeField] float maxHealth;

    [Header("Player HP Settings")]
    [SerializeField] float healthBarAmt;
    float maxHealthBarAmt;
    [Range(1, 5)]
    [SerializeField] int numberOfHealthBars;

    [Header("References")]
    [SerializeField] Image healthBar;
    [SerializeField] Image healthBarBackground;
    [SerializeField] Image heartImage;
    [SerializeField] Color[] healthBarColors;
    [SerializeField] Color[] healthBarBackgroundColors;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;

        // healthBarAmt = amount of hp before hp bar changes color
        healthBarAmt = maxHealth / numberOfHealthBars;
        maxHealthBarAmt = healthBarAmt;

        // set health bar color to starting color
        healthBar.color = healthBarColors[numberOfHealthBars - 1];
        healthBarBackground.color = healthBarBackgroundColors[numberOfHealthBars - 1];
        heartImage.color = healthBarBackgroundColors[numberOfHealthBars - 1];
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(3.5f);
        }
    }

    // decrements player's radial hp bar in counterclockwise fashion
    public void TakeDamage(float damage)
    {
        if (currentHealth > 0)
        {
            currentHealth -= damage;

            healthBarAmt -= damage;
            healthBar.fillAmount = currentHealth / maxHealth;

            // if damage causes healthBarAmt to be <= 0, change the hp bar's color
            if (healthBarAmt <= 0)
            {
                float remainder = Mathf.Abs(healthBarAmt);
                healthBarAmt = maxHealthBarAmt - remainder;

                numberOfHealthBars -= 1;

                if (numberOfHealthBars > 0)
                {
                    healthBar.color = healthBarColors[numberOfHealthBars - 1];
                    healthBarBackground.color = healthBarBackgroundColors[numberOfHealthBars - 1];
                    heartImage.color = heartImage.color = healthBarBackgroundColors[numberOfHealthBars - 1];
                }
                else
                {
                    healthBar.enabled = false;
                    this.enabled = false;
                }
            }
        }
    }
}
