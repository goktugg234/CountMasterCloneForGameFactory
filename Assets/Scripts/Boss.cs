using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using TMPro;

public class Boss : MonoBehaviour
{
    public bool attack;
    public TextMeshPro CounterTxt;
    public Transform enemy;
    void Start()
    {
        CounterTxt.text = 50.ToString();
    }
    void Update()
    {
        if (attack)
        {
            var enemyDirection = enemy.position - transform.position;
                transform.rotation = Quaternion.Slerp(transform.rotation, quaternion.LookRotation(enemyDirection,Vector3.up),
                    Time.deltaTime * 3f);
                if (enemy.childCount > 1)
                {
                    var distance = enemy.GetChild(1).position - transform.position;
                    if (distance.magnitude < 1.5f)
                    {
                        transform.position = Vector3.Lerp(transform.position,
                            enemy.GetChild(1).position,Time.deltaTime * 2f);
                    } 
                }
        }
    }

    public void AttackThem(Transform enemyForce)
    {
        enemy = enemyForce;
        attack = true;
    }

    public void StopAttacking()
    {
        PlayerManager.PlayerManagerInstance.gameState =  attack = false;
    }
}
