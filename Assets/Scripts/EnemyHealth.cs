using UnityEngine;
using System.Collections;

public class EnemyHealth  : MonoBehaviour {

    [SerializeField] private int startingHealth = 20;
    [SerializeField] private float timeSinceLastHit = 0.5f;
    [SerializeField] private float dissapearSpeed = 2f;

    private AudioSource audio;
    private float timer = 0f;
    private Animator anim;
    private NavMeshAgent nav;
    private bool isAlive;
    private Rigidbody rigitBody;
    private CapsuleCollider capsuleCollider;
    private bool dissapearEnemy = false;
    private int currentHealth;
    private ParticleSystem blood;

    public bool IsAlive {
        get { return isAlive; }
    }

    // Use this for initialization
	void Start () {

        GameManager.instance.RegisteredEnemy(this);
        rigitBody = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();
        isAlive = true;
        currentHealth = startingHealth;
        blood = GetComponentInChildren<ParticleSystem>();

    }

    // Update is called once per frame
    void Update () {

        timer += Time.deltaTime;

        if(dissapearEnemy) {
            transform.Translate(-Vector3.up * dissapearSpeed * Time.deltaTime);
        }

	}

    void OnTriggerEnter(Collider other) {
        if (timer >= timeSinceLastHit && !GameManager.instance.GameOver) {
            if (other.tag == "PlayerWeapon") {
                takeHit();
                blood.Play();
                timer = 0f;
            }
        }
    }

    void takeHit() {
        if (currentHealth > 0) {
            audio.PlayOneShot(audio.clip);
            anim.Play("Hurt");
            currentHealth -= 10;
        } else {
            isAlive = false;
            KillEnemy();
        }
    }

    void KillEnemy() {

        GameManager.instance.KilledEnemy(this);
        capsuleCollider.enabled = false;
        nav.enabled = false;
        anim.SetTrigger("EnemyDie");
        rigitBody.isKinematic = true;

        StartCoroutine(removeEnemy());

    }

    IEnumerator removeEnemy() {

        // wait for seconds after enemy dies
        yield return new WaitForSeconds(4f);
        // start to sink the enemy
        dissapearEnemy = true;
        // after 2 seconds
        yield return new WaitForSeconds(2f);
        // destroy the game object 
        Destroy(gameObject);
    }

}
