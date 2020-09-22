using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CharacterButtonAnim : MonoBehaviour
{
    public void AnimateCharacterBorderOut()
    {
        gameObject.transform.DOMoveX(150, 0.5f);
    }

    public void AnimateCharacterBorderIn()
    {
        gameObject.transform.DOMoveX(0, 0.5f);
    }
}
