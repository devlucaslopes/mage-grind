using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAnimator : MonoBehaviour
{
    [SerializeField] private Boss Boss;

    public void CreateImpact() => Boss.CreateImpact();
}
