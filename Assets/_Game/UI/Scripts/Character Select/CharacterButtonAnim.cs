using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CharacterButtonAnim : MonoBehaviour
{
    [SerializeField] float animateInX;
    [SerializeField] float animateOutX;

    public void AnimateCharacterBorderOut()
    {
        gameObject.transform.DOMoveX(animateOutX, 0.5f);
    }

    public void AnimateCharacterBorderIn()
    {
        gameObject.transform.DOMoveX(animateInX, 0.5f);
    }
}
