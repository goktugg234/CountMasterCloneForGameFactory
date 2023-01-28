using System.Collections;
using Cinemachine;
using DG.Tweening;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public Transform player;
    public GameManager gameManager;
    public int numberOfStickmans,numberOfEnemyStickmans,numberOfBossHealt;
    [SerializeField] public TextMeshPro CounterTxt;
    [SerializeField] private GameObject stickMan;

    [Range(0f,1f)] [SerializeField] private float DistanceFactor, Radius;
   
   
    public bool moveByTouch,gameState;
    private Vector3 mouseStartPos,playerStartPos;
    public float playerSpeed,roadSpeed;
    private new Camera camera;

    [SerializeField] private Transform road;
    [SerializeField] private Transform enemy;
    [SerializeField] private Transform boss;
    private bool attackToEnemy,attackToBoss;
    public static PlayerManager PlayerManagerInstance;
    public ParticleSystem blood;
    public GameObject SecondCam;
    public Animator animator;
    public bool FinishLine,moveTheCamera;
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        gameManager.StartMenu();
        player = transform;
        
        numberOfStickmans = transform.childCount - 1;

        CounterTxt.text = numberOfStickmans.ToString();
        
        camera = Camera.main;

        PlayerManagerInstance = this;

        gameState = false;
    }
    void Update()
    {
        if (attackToEnemy)
        {
            var enemyDirection = new Vector3(enemy.position.x,transform.position.y,enemy.position.z) - transform.position;
            for (int i = 1; i < transform.childCount; i++)
            {
                transform.GetChild(i).rotation = 
                    Quaternion.Slerp( transform.GetChild(i).rotation,Quaternion.LookRotation(enemyDirection,Vector3.up), Time.deltaTime * 3f );
            }
            if (enemy.GetChild(1).childCount > 1)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    var Distance = enemy.GetChild(1).GetChild(0).position - transform.GetChild(i).position;
                    if (Distance.magnitude < 1.5f)
                    {
                        transform.GetChild(i).position = Vector3.Lerp(transform.GetChild(i).position, 
                            new Vector3(enemy.GetChild(1).GetChild(0).position.x,transform.GetChild(i).position.y,
                                enemy.GetChild(1).GetChild(0).position.z), Time.deltaTime * 1f );
                    }
                }
            }
            else
            {
                    attackToEnemy = false;
                    roadSpeed = 2f;
                    FormatStickMan();
                    for (int i = 1; i < transform.childCount; i++){
                        transform.GetChild(i).rotation = Quaternion.identity;
                    }
                    enemy.gameObject.SetActive(false); 
            }
            if (transform.childCount == 1)
            {
                enemy.transform.GetChild(1).GetComponent<EnemyManager>().StopAttacking();

                gameObject.SetActive(false);
            }
        }
        if(attackToBoss){
            var bossDirection = new Vector3(boss.position.x, transform.position.y, boss.position.z) - transform.position;
            for (int i = 1; i < transform.childCount; i++)
            {
                transform.GetChild(i).rotation = 
                    Quaternion.Slerp( transform.GetChild(i).rotation,Quaternion.LookRotation(bossDirection,Vector3.up), Time.deltaTime * 3f );
            }
            for (int i = 0; i < transform.childCount; i++)
            {
                var Distance = boss.GetChild(0).position - transform.GetChild(i).position;
                if (Distance.magnitude < 1.5f)
                {
                    transform.GetChild(i).position = Vector3.Lerp(transform.GetChild(i).position, 
                        new Vector3(boss.GetChild(0).position.x,transform.GetChild(i).position.y,
                            boss.GetChild(0).position.z), Time.deltaTime * 1f );
                }
            }
            
            attackToBoss = false;
            roadSpeed = 2f;
            FormatStickMan();
            for (int i = 1; i < transform.childCount; i++){
                transform.GetChild(i).rotation = Quaternion.identity;
            }
            boss.gameObject.SetActive(false); 
            
            if (transform.childCount == 1)
            {
                boss.transform.GetChild(0).GetComponent<Boss>().StopAttacking();

                gameObject.SetActive(false);
            }
        }
        else
        {
            if(!FinishLine)
                MoveThePlayer();
        }
        if (transform.childCount == 1 && FinishLine)
        {
            gameState = false;
            gameManager.FinishMenu();
        }
        if (gameState)
        {
            road.Translate(road.forward * Time.deltaTime * roadSpeed);
        }
        if (moveTheCamera && transform.childCount > 1)
        {
            var cinemachineTransposer = SecondCam.GetComponent<CinemachineVirtualCamera>()
            .GetCinemachineComponent<CinemachineTransposer>();

            var cinemachineComposer = SecondCam.GetComponent<CinemachineVirtualCamera>()
            .GetCinemachineComponent<CinemachineComposer>();

            cinemachineTransposer.m_FollowOffset = new Vector3(4.5f, Mathf.Lerp(cinemachineTransposer.m_FollowOffset.y,
            transform.GetChild(1).position.y + 6f, Time.deltaTime * 1f), -5f);
                
            cinemachineComposer.m_TrackedObjectOffset = new Vector3(0f,Mathf.Lerp(cinemachineComposer.m_TrackedObjectOffset.y,
            6f,Time.deltaTime * 1f),0f);
        }
    }
    void MoveThePlayer()
    {
        if (Input.GetMouseButtonDown(0) && gameState)
            {
                moveByTouch = true;
                
                var plane = new Plane(Vector3.up, 0f);

                var ray = camera.ScreenPointToRay(Input.mousePosition);
                
                if (plane.Raycast(ray,out var distance))
                {
                    mouseStartPos = ray.GetPoint(distance + 1f);
                    playerStartPos = transform.position;
                }

            }
        if (Input.GetMouseButtonUp(0))
            {
                moveByTouch = false;
                
            }
            
        if (moveByTouch)
        { 
                var plane = new Plane(Vector3.up, 0f);
                var ray = camera.ScreenPointToRay(Input.mousePosition);
                
            if (plane.Raycast(ray,out var distance))
            {
                var mousePos = ray.GetPoint(distance +  1f);
                    
                var move = mousePos - mouseStartPos;
                    
                var control = playerStartPos + move;


                if (numberOfStickmans > 50)
                    control.x = Mathf.Clamp(control.x, -0.7f, 0.7f);
                else
                    control.x = Mathf.Clamp(control.x, -1.5f, 1.5f);

                transform.position = new Vector3(Mathf.Lerp(transform.position.x,control.x,Time.deltaTime * playerSpeed),transform.position.y,transform.position.z);
            }
        }
    }
    public void FormatStickMan()
    {
        for (int i = 1; i < player.childCount; i++)
        {
            var x = DistanceFactor * Mathf.Sqrt(i) * Mathf.Cos(i * Radius);
            var z = DistanceFactor * Mathf.Sqrt(i) * Mathf.Sin(i * Radius);
                
            var NewPos = new Vector3(x, 0,z);

            player.transform.GetChild(i).DOLocalMove(NewPos, 0.5f).SetEase(Ease.OutBack);
        }
    }
    public void MakeStickMan(int number)
    {
        for (int i = numberOfStickmans; i < number; i++)
        {
            Instantiate(stickMan, transform.position, quaternion.identity, transform);
        }

        numberOfStickmans = transform.childCount - 1;
        CounterTxt.text = numberOfStickmans.ToString();
        FormatStickMan();
    }
    private void OnTriggerEnter(Collider other)
    {  
        if (other.CompareTag("Gate"))
        {
            other.transform.parent.GetChild(0).GetComponent<BoxCollider>().enabled = false; 
            other.transform.parent.GetChild(1).GetComponent<BoxCollider>().enabled = false; 
            var gateManager = other.GetComponent<GateManager>();
            numberOfStickmans = transform.childCount - 1;
            if (gateManager.multiplyToStickmans)
            {
                MakeStickMan(numberOfStickmans * gateManager.randomNumber);
            }
            else
            {
                MakeStickMan(numberOfStickmans + gateManager.randomNumber);
            }
        }
        if (other.CompareTag("Enemy"))
        { 
            enemy = other.transform;
            attackToEnemy = true;
            roadSpeed = 0.5f;
            other.transform.GetChild(1).GetComponent<EnemyManager>().AttackThem(transform);
            StartCoroutine(UpdateTheEnemyAndPlayerStickMansNumbers());
        }
        if(other.CompareTag("Boss")){
            boss = other.transform;
            attackToBoss = true;
            roadSpeed = 0.5f;
            other.transform.GetChild(1).GetComponent<Boss>().AttackThem(transform);
            StartCoroutine(BossFight());
        }
        if (other.CompareTag("Finish"))
        {
            moveByTouch= false;
            SecondCam.SetActive(true);
            FinishLine = true;
            Tower.TowerInstance.CreateTower(transform.childCount - 1);
            transform.GetChild(0).gameObject.SetActive(false);
        }
        if(other.CompareTag("Chest"))
        {
            gameState = false;
            road.Translate(road.forward * 0);
            animator.SetBool("run", false);
        }
        if(other.CompareTag("Obstacle")){
            StartCoroutine(UpdatePlayerStickman());
        }
        
    }
    IEnumerator UpdateTheEnemyAndPlayerStickMansNumbers()
    {
        numberOfEnemyStickmans = enemy.transform.GetChild(1).childCount - 1;
        numberOfStickmans = transform.childCount - 1;
        while (numberOfEnemyStickmans > 0 && numberOfStickmans > 0)
        {
            numberOfEnemyStickmans--;
            numberOfStickmans--;
            enemy.transform.GetChild(1).GetComponent<EnemyManager>().CounterTxt.text = numberOfEnemyStickmans.ToString();
            CounterTxt.text = numberOfStickmans.ToString();
            yield return null;
        }
        if (numberOfEnemyStickmans == 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).rotation = Quaternion.identity;
            }
        }
    }
    IEnumerator UpdatePlayerStickman(){
        numberOfStickmans = transform.childCount - 1;
        while(numberOfStickmans > 0){
            numberOfStickmans--;
            CounterTxt.text = numberOfStickmans.ToString();
            yield return null;
        }
    }
    IEnumerator BossFight(){
        numberOfBossHealt = 50;
        numberOfStickmans = transform.childCount-1;
        while(numberOfStickmans > 0 && numberOfBossHealt > 0){
            numberOfStickmans--;
            numberOfBossHealt--;
            boss.transform.GetChild(1).GetComponent<Boss>().CounterTxt.text = numberOfBossHealt.ToString();
            CounterTxt.text = numberOfStickmans.ToString();
            yield return null;
        }
        if(numberOfBossHealt == 0){
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).rotation = Quaternion.identity;
            }
            gameState = false;
            gameManager.FinishMenu();
        }
    }
    
}