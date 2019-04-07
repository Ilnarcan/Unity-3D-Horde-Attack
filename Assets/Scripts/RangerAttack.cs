using UnityEngine;
using System.Collections;
using UnityEngine.SocialPlatforms;

public class RangerAttack : MonoBehaviour {

    [SerializeField] private float range = 3f;
    [SerializeField] private float timeBetweenAttacks = 1f;
    [SerializeField] Transform firelocation;

    private Animator anim;
    private GameObject player;
    private bool playerInRange;
    private EnemyHealth enemyHealth;
    private GameObject arrow;

    // Use this for initialization
    void Start() {

        arrow = GameManager.instance.Arrow;
        enemyHealth = GetComponent<EnemyHealth>();
        player = GameManager.instance.Player;
        anim = GetComponent<Animator>();
        StartCoroutine(attack());

    }

    // Update is called once per frame
    void Update() {

        if (Vector3.Distance(transform.position, player.transform.position) < range && enemyHealth.IsAlive) {
            playerInRange = true;
            RotateTowards(player.transform);
        }
        else {
            playerInRange = false;
        }

    }

    IEnumerator attack() {

        if (playerInRange && GameManager.instance.GameOver != true) {
            anim.Play("Attack");
            yield return new WaitForSeconds(timeBetweenAttacks);
        }
        yield return null;
        StartCoroutine(attack());
    }

    private void RotateTowards(Transform player) {

        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
        //transform.rotation = lookRotation;
    }

    public void FireArrow() {

        GameObject newArrow = Instantiate(arrow) as GameObject;
        newArrow.transform.position = firelocation.position;
        newArrow.transform.rotation = transform.rotation;
        newArrow.GetComponent<Rigidbody>().velocity = transform.forward * 25f;

    }

}
