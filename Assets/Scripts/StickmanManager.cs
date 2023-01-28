using System;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class StickmanManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem blood;
    PlayerManager playerManager;
    private Animator StickManAnimator;
    private void Start()
    {
        DOTween.SetTweensCapacity(100000,5000);
        playerManager = FindObjectOfType<PlayerManager>();
        StickManAnimator = GetComponent<Animator>();
        StickManAnimator.SetBool("Run",true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Red") && other.transform.parent.childCount > 0)
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
            Instantiate(blood, transform.position, Quaternion.identity);
        }
        switch (other.tag)
        {
            case "Red":
                if(other.transform.parent.childCount > 0){
                    Destroy(other.gameObject);
                    Destroy(gameObject);
                }
            break;
            case "Jump":
               transform.DOJump(transform.position, 1f, 1, .75f).SetEase(Ease.Flash);
               break;
            case "Format":
                playerManager.FormatStickMan();
                break;
            case "Chest":
                playerManager.gameState = false;
                StickManAnimator.SetBool("Run", false);
                break;
            case "Obstacle":
                transform.parent.parent = null;
                transform.parent = null;    
                playerManager.numberOfStickmans--;
                playerManager.CounterTxt.text = playerManager.numberOfStickmans.ToString();
                Instantiate(blood, transform.position, Quaternion.identity);
                break;
        }
        
        if (other.CompareTag("Stair"))
        {
            transform.parent.parent = null;
            transform.parent = null;
            GetComponent<Rigidbody>().isKinematic = GetComponent<Collider>().isTrigger = false;
            StickManAnimator.SetBool("Run",false);

            if (!PlayerManager.PlayerManagerInstance.moveTheCamera)
                PlayerManager.PlayerManagerInstance.moveTheCamera = true;

            if (PlayerManager.PlayerManagerInstance.player.transform.childCount == 2)
            {
                other.GetComponent<Renderer>().material.DOColor(new Color(0.4f, 0.98f, 0.65f), 0.5f).SetLoops(1000, LoopType.Yoyo)
                    .SetEase(Ease.Flash);
            }
        
        }
    }
}