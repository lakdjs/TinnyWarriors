using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string animationName;

    public void ShowDamage()
    {
        animator.Play(animationName);
    }
}
