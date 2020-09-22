using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] float currentHealth;
    [SerializeField] float maxHealth;

    [SerializeField] float currentHealthBarAmt;
    float maxHealthBarAmt;
    [Range(1, 5)]
    [SerializeField] int numberOfHealthBars;

    [SerializeField] Image healthBar;
    [SerializeField] Image healthBarBackground;
    [SerializeField] Color[] healthBarColors;
    [SerializeField] Color[] healthBarBackgroundColors;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;

        // 100 / 5 = 20
        currentHealthBarAmt = maxHealth / numberOfHealthBars;
        maxHealthBarAmt = currentHealthBarAmt;

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

            healthBar.fillAmount = currentHealth / maxHealth;

            if (currentHealthBarAmt <= 0)
            {
                currentHealthBarAmt = maxHealthBarAmt;

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
