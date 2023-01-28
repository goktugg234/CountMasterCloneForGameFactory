using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
   PlayerManager playerManager;
   [SerializeField] public GameObject startMenuObj;
   [SerializeField] public GameObject finishMenuObj;

    void Awake()
    {
        playerManager = FindObjectOfType<PlayerManager>();
    }
    public void StartGame(){
        playerManager.gameState = true;
        startMenuObj.SetActive(false);
        PlayerManager.PlayerManagerInstance.player.GetChild(1).GetComponent<Animator>().SetBool("Run",true);
    }
    public void StartMenu(){
        startMenuObj.SetActive(true);
    }
    public void FinishGame(){
        SceneManager.LoadScene(1);
    }
    public void FinishMenu(){
        finishMenuObj.SetActive(true);
    }
    public void SpawnStickman(){
        playerManager.MakeStickMan(50);
    }
}
