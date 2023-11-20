using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class EnemyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Damage;

    private Enemy enemy;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    private void OnEnable()
    {
        enemy.OnTakeDamage += Enemy_OnTakeDamage;
    }

    private void OnDisable()
    {
        enemy.OnTakeDamage -= Enemy_OnTakeDamage;
    }

    private void Enemy_OnTakeDamage(int damage)
    {
        Damage.text = damage.ToString();
        Damage.gameObject.SetActive(true);
        Damage.transform.localPosition = new Vector3(0, -0.75f, 0);

        Damage.transform
            .DOLocalMoveY(0.5f, 0.2f)
            .OnComplete(() =>
            {
                Damage.gameObject.SetActive(false);
            });
    }
}
