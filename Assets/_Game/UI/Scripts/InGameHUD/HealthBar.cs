using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public float currentHealth;
    public float maxHealth;

    public float currentHealthBarAmt;
    public float maxHealthBarAmt;
    [Range(1, 5)]
    public int numberOfHealthBars;

    public Image healthBar;
    public Image healthBarBackground;
    public Color[] healthBarColors;
    public Color[] healthBarBackgroundColors;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;

        maxHealthBarAmt = maxHealth / numberOfHealthBars;
        currentHealthBarAmt = maxHealthBarAmt;

        healthBar.color = healthBarColors[numberOfHealthBars - 1];
        healthBarBackground.color = healthBarBackgroundColors[numberOfHealthBars - 1];
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamageTest(2);
        }
    }

    // decrements player's radial hp bar in counterclockwise fashion
    public void TakeDamageTest(float damage)
    {
        if (currentHealth > 0)
        {
            currentHealth -= damage;

            currentHealthBarAmt -= damage;
            healthBar.fillAmount = currentHealthBarAmt / maxHealthBarAmt;

            if (currentHealthBarAmt < 0)
            {
                float remainder = maxHealthBarAmt + currentHealthBarAmt;
                currentHealthBarAmt = remainder;
                healthBar.fillAmount = currentHealthBarAmt / maxHealthBarAmt;

                numberOfHealthBars -= 1;

                if (numberOfHealthBars > 0)
                {
                    healthBar.color = healthBarColors[numberOfHealthBars - 1];
                    healthBarBackground.color = healthBarBackgroundColors[numberOfHealthBars - 1];
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
