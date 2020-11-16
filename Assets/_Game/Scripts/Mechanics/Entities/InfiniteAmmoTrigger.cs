using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteAmmoTrigger : MonoBehaviour
{
    //This trigger has sort of become the tutorial manager, whoops
    [SerializeField] OreVein oreVein;
    [SerializeField] GameObject oreInteractUI;
    [SerializeField] GameObject tutorialPrompt;

    private static bool firstTime = true;
    private bool hasBeenEnabled = false;
    private bool closedInPause = false;

    private void Start()
    {
        if(firstTime)
        {
            tutorialPrompt.SetActive(true);
            firstTime = false;
        }
        else
        {
            oreVein.isInfinite = false;
            oreInteractUI.SetActive(false);
            tutorialPrompt.SetActive(false);
            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if(tutorialPrompt.activeInHierarchy && Time.timeScale < 1)
        {
            tutorialPrompt.SetActive(false);
            closedInPause = true;
        }

        if(closedInPause && Time.timeScale >= 1)
        {
            tutorialPrompt.SetActive(true);
            closedInPause = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Gunner player = other.GetComponent<Gunner>();
        if(firstTime && player && !hasBeenEnabled)
        {
            hasBeenEnabled = true;
            player.SetInfiniteAmmo(true);
            print("Player has infinite ammo.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Gunner player = other.GetComponent<Gunner>();
        if (player)
        {
            player.SetInfiniteAmmo(false);
            print("Player does not have infinite ammo.");
            oreVein.isInfinite = false;
            oreInteractUI.SetActive(false);
            tutorialPrompt.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}
