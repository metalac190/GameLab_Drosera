using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class InGameHUD : MonoBehaviour
{
    [Header("Top Left UI")]
    [SerializeField] Image objectiveImage;
    [SerializeField] Sprite hyperSeedSprite;
    [SerializeField] Sprite dropshipSprite;
    [SerializeField] TextMeshProUGUI objectiveText;

    [Header("Bottom Right UI")]
    [SerializeField] TextMeshProUGUI currentAmmoText;
    [SerializeField] TextMeshProUGUI maxAmmoText;

    [SerializeField] Image selectedAbilityImage;
    [SerializeField] Sprite[] abilitySprites;
    bool firstSelected = true;

    [SerializeField] Image secondaryAttackImage;
    public bool secondaryAttackOnCooldown = false;
    public float secondaryAttackCooldown = 6;
    public float secondaryAttackTimer = 0;

    [SerializeField] Image dodgeImage;
    public bool dodgeOnCooldown = false;
    public float dodgeCooldown = 2;
    public float dodgeTimer = 0;

    // references
    PlayerBase playerHookup;

    // UnityEvent hookups
    HyperSeed hyperSeedHookup;
    OreVein[] oreVeinHookups;

    HealthBar healthBarUI;

    Gunner gunnerHookup;
    GunnerPrimaryFire gunnerPrimaryFireHookup;
    GunnerAltFire gunnerSecondaryFireHookup;

    private void Awake()
    {
        playerHookup = FindObjectOfType<PlayerBase>();

        healthBarUI = GetComponent<HealthBar>();

        gunnerHookup = FindObjectOfType<Gunner>();

        gunnerPrimaryFireHookup = gunnerHookup.GetComponent<GunnerPrimaryFire>();
        gunnerSecondaryFireHookup = gunnerHookup.GetComponent<GunnerAltFire>();

    }

    private void Start()
    {
        gunnerHookup.OnTakeDamage.AddListener(healthBarUI.TakeDamage);
        gunnerHookup.OnHeal.AddListener(healthBarUI.TakeDamage);

        gunnerPrimaryFireHookup.OnFire.AddListener(UpdateAmmoText);
        gunnerSecondaryFireHookup.OnFire.AddListener(DisplaySecondaryAttackCooldown);

        ShowPhaseOneObjectiveText();
        UpdateAmmoText();

        secondaryAttackCooldown = playerHookup.AbilityCooldownTime;
        dodgeCooldown = playerHookup.DodgeCooldownTime;

        GameManager.Instance.OnStateChange += ChangePhaseText;
    }

    void ChangePhaseText()
    {
        if(GameManager.Instance.GameState == DroseraGlobalEnums.GameState.MainOne)
        {
            ShowPhaseOneObjectiveText();

            // wait for ores to spawn before adding ore vein hookup
            oreVeinHookups = FindObjectsOfType<OreVein>();
            foreach (OreVein ore in oreVeinHookups)
            {
                ore.OnInteract.AddListener(UpdateAmmoText);
            }
        }
        else if (GameManager.Instance.GameState == DroseraGlobalEnums.GameState.MainTwo)
        {
            ShowPhaseTwoObjectiveText();
        }
    }

    private void Update()
    {
        // key Q/Y
        if (playerHookup.SwapAbilityButton)
        {
            UpdateAbilityImage();
        }

        // key R/X
        if (playerHookup.ReloadButton)
        {
            UpdateAmmoText();
        }
        
        // key Space/LT
        if (playerHookup.DodgeButtonKey)
        {
            DisplayDodgeCooldown();
        }
        
        if (secondaryAttackOnCooldown)
        {
            if (secondaryAttackTimer < secondaryAttackCooldown)
            {
                secondaryAttackTimer += Time.deltaTime;

                secondaryAttackImage.fillAmount = secondaryAttackTimer / secondaryAttackCooldown;
            }
            else
            {
                secondaryAttackOnCooldown = false;

                secondaryAttackImage.fillAmount = 1;
            }
        }

        if (dodgeOnCooldown)
        {
            if (dodgeTimer < dodgeCooldown)
            {
                dodgeTimer += Time.deltaTime;

                dodgeImage.fillAmount = dodgeTimer / dodgeCooldown;
            }
            else
            {
                dodgeOnCooldown = false;

                dodgeImage.fillAmount = 1;

                dodgeImage.DOFade(0, 0.5f);
            }
        }
    }

    // top-left stuff
    // call at Start()
    public void ShowPhaseOneObjectiveText()
    {
        if (objectiveImage != null)
        {
            objectiveImage.sprite = hyperSeedSprite;
            objectiveImage.transform.localScale = new Vector3(0.65f, 0.65f, 0.65f);
        }

        objectiveText.text = "LOCATE THE HYPERSEED";
    }

    // call when player acquires the Hyperseed
    public void ShowPhaseTwoObjectiveText()
    {
        if (objectiveImage != null)
        {
            objectiveImage.sprite = dropshipSprite;
            objectiveImage.transform.localScale = new Vector3(1, 1, 1);
        }

        objectiveText.text = "ESCAPE TO THE DROPSHIP";
    }

    // bottom-right stuff
    // call for each shot (M1/RT)
    public void UpdateAmmoText()
    {
        currentAmmoText.text = playerHookup.Ammo.ToString();
        maxAmmoText.text = playerHookup.HeldAmmo.ToString();
    }

    // call when ability is toggled (Q/Y)
    public void UpdateAbilityImage()
    {
        firstSelected = !firstSelected;

        // incediary grenade
        if (firstSelected)
        {
            selectedAbilityImage.sprite = abilitySprites[0];
            selectedAbilityImage.SetNativeSize();
        }
        // pesticide grenade- DOT 
        else
        {
            selectedAbilityImage.sprite = abilitySprites[1];
            selectedAbilityImage.SetNativeSize();
        }
    }

    // call when skill is used (M2/RB)
    public void DisplaySecondaryAttackCooldown()
    {
        if (!secondaryAttackOnCooldown)
        {
            secondaryAttackTimer = 0;
            secondaryAttackCooldown = gunnerSecondaryFireHookup.Cooldown;

            secondaryAttackImage.fillAmount = 0;

            secondaryAttackOnCooldown = true;
        }
    }

    // call when dodge is used (Space/LT)
    public void DisplayDodgeCooldown()
    {
        if (!dodgeOnCooldown)
        {
            dodgeTimer = 0;
            dodgeCooldown = playerHookup.DodgeCooldownTime + playerHookup.DodgeTime;

            dodgeImage.fillAmount = 0;

            dodgeImage.DOComplete();
            dodgeImage.DOFade(1, 0);

            dodgeOnCooldown = true;
        }
    }
}
