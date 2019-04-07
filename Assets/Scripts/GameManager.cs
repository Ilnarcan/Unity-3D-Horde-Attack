using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    [SerializeField] GameObject player;
    [SerializeField] GameObject[] spawnPoints;
    [SerializeField] GameObject[] powerUpSpawns;
    [SerializeField] GameObject tanker;
    [SerializeField] GameObject ranger;
    [SerializeField] GameObject soldier;
    [SerializeField] GameObject arrow;
    [SerializeField] GameObject healthPowerUp;
    [SerializeField] GameObject speedPowerUp;
    [SerializeField] Text levelText;
    [SerializeField] Text endGameText;
    [SerializeField] int maxPowerUps;
    [SerializeField] int finalLevel = 20;

    private bool gameOver = false;
    private int currentLevel;
    private float generatedSpawnTime = 1;
    private float currentSpawnTime = 0;
    private float powerUpSpawnTime = 5;
    private float currentPowerUpSpawnTime = 0;
    private int powerups = 0;

    private GameObject newEnemy;
    private GameObject newSpawn;

    private List<EnemyHealth> enemies = new List<EnemyHealth>();
    private List<EnemyHealth> killedEnemies = new List<EnemyHealth>();

    public static GameManager instance = null;

    public void RegisteredEnemy(EnemyHealth enemy) {
        enemies.Add(enemy);
    }

    public void KilledEnemy(EnemyHealth enemy) {
        killedEnemies.Add(enemy);
    }

    public void RegisteredPowerUp() {
        powerups++;
    }

    public bool GameOver {
        get { return gameOver; }
    }

    public GameObject Player {
        get { return player; }
    }

    public GameObject Arrow {
        get { return arrow; }
    }

    private void Awake() {

        if (instance == null) {
            instance = this;
        }
        else if (instance != this) {
            Destroy(gameObject);
        }

    }

    // Use this for initialization
    void Start() {

        endGameText.GetComponent<Text>().enabled = false;
        StartCoroutine(spawn());
        StartCoroutine(PowerUpSpawn());
        currentLevel = 1;

    }

    // Update is called once per frame
    void Update() {

        currentSpawnTime += Time.deltaTime;
        currentPowerUpSpawnTime += Time.deltaTime;
    }

    public void PlayerHit(int currentHP) {

        if (currentHP > 0) {
            gameOver = false;
        }
        else {
            gameOver = true;
            StartCoroutine(endGame("Defeat!"));
        }
    }

    IEnumerator spawn() {

        // check that spawn time is greater than the current
        //if there are less enemies on screen than the current level, randomly select a spawn point
        // and spawn a random enemy.
        // if we have killed the same number of enemies as the  current level, clear out the enemies and killed
        // killed enemies arrays, increment the current level by 1, and start over.

        if (currentSpawnTime > generatedSpawnTime) {
            currentSpawnTime = 0;

            if (enemies.Count < currentLevel) {

                int randomNumber = Random.Range(0, spawnPoints.Length - 1);
                GameObject spawnLocation = spawnPoints[randomNumber];

                int randomEnemy = Random.Range(0, 3);

                if (randomEnemy == 0) {
                    newEnemy = Instantiate(soldier) as GameObject;
                }
                else if (randomEnemy == 1) {
                    newEnemy = Instantiate(ranger) as GameObject;
                }
                else if (randomEnemy == 2) {
                    newEnemy = Instantiate(tanker) as GameObject;
                }

                newEnemy.transform.position = spawnLocation.transform.position;

            }
            if (killedEnemies.Count == currentLevel && currentLevel != finalLevel) {

                enemies.Clear();
                killedEnemies.Clear();

                yield return new WaitForSeconds(3);
                currentLevel++;
                levelText.text = "Level " + currentLevel;
            }

            if (killedEnemies.Count == finalLevel) {
                StartCoroutine(endGame("Victory!"));
            }
        }

        yield return null;
        StartCoroutine(spawn());

    }

    IEnumerator PowerUpSpawn() {

        if (currentPowerUpSpawnTime > powerUpSpawnTime) {
            currentPowerUpSpawnTime = 0;
            if (powerups < maxPowerUps) {

                int randomNumber = Random.Range(0, powerUpSpawns.Length - 1);
                GameObject powerUpSpawnLocation = powerUpSpawns[randomNumber];

                int randomPowerUp = Random.Range(0, 2);
                if (randomPowerUp == 0) {
                    newSpawn = Instantiate(healthPowerUp) as GameObject;
                }
                else if (randomPowerUp == 1) {
                    newSpawn = Instantiate(speedPowerUp) as GameObject;
                }
                newSpawn.transform.position = powerUpSpawnLocation.transform.position;
            }

        }

        yield return null;
        StartCoroutine(PowerUpSpawn());

    }

    IEnumerator endGame(string outcome) {

        endGameText.text = outcome;
        endGameText.GetComponent<Text>().enabled = true;
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("Main");


    }
}
