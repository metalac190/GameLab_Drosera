using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InGameHUD : MonoBehaviour
{
    // top-left stuff
    [SerializeField] TextMeshProUGUI objectiveText;

    // top-right stuff
    [SerializeField] TextMeshProUGUI ammoText;

    [SerializeField] Image selectedAbilityImage;
    [SerializeField] Sprite[] abilitySprites;

    [SerializeField] Image skillImage;
    public bool skillOnCooldown = false;
    public float skillCooldown = 0;
    public float skillTimer = 0;

    [SerializeField] Image dodgeImage;
    public bool dodgeOnCooldown = false;
    public float dodgeCooldown = 0;
    public float dodgeTimer = 0;

    private void Start()
    {
        ShowPhaseOneObjectiveText();
    }

    private void Update()
    {
        if (skillOnCooldown)
        {
            float maxTimer = skillCooldown;

            if (skillTimer < skillCooldown)
            {
                skillTimer += Time.deltaTime;

                skillImage.fillAmount = skillTimer / maxTimer;
            }
            else
            {
                skillTimer = 1;
                skillOnCooldown = false;

                skillImage.fillAmount = 1;
            }
        }

        if (dodgeOnCooldown)
        {
            float maxTimer = dodgeCooldown;

            if (dodgeTimer < dodgeCooldown)
            {
                dodgeTimer += Time.deltaTime;

                dodgeImage.fillAmount = dodgeTimer / maxTimer;
            }
            else
            {
                dodgeTimer = 1;
                dodgeOnCooldown = false;

                dodgeImage.fillAmount = 1;
            }
        }
    }

    // top-left stuff
    // call at Start()
    public void ShowPhaseOneObjectiveText()
    {
        objectiveText.text = "LOCATE THE HYPERSEED";
    }

    // HOOKUP- call when player acquires the Hyperseed
    public void ShowPhaseTwoObjectiveText()
    {
        objectiveText.text = "ESCAPE THE LEVEL";
    }

    // bottom-right stuff
    // HOOKUP- call for each shot (M1/RT)
    public void UpdateAmmoText(int currentAmmo, int maxAmmo)
    {
        ammoText.text = currentAmmo.ToString() + " / " + maxAmmo.ToString();
    }

    // HOOKUP- call when ability is toggled (Q/Y)
    public void UpdateAbilityImage(int index)
    {
        selectedAbilityImage.sprite = abilitySprites[index];
        selectedAbilityImage.SetNativeSize();
    }

    // HOOKUP- call when skill is used (LShift/LB)
    public void DisplaySkillImageCooldown(float cooldown)
    {
        skillTimer = 0;
        skillCooldown = cooldown;
        skillOnCooldown = true;

        skillImage.fillAmount = 0;
    }

    // HOOKUP- call when dodge is used (Space/LT)
    public void DisplayDodgeCooldown(float cooldown)
    {
        dodgeTimer = 0;
        dodgeCooldown = cooldown;
        dodgeOnCooldown = true;

        dodgeImage.fillAmount = 0;
    }
}
