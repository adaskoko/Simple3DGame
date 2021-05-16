using FPSControllerLPFP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

    public bool isHit = false;
    public bool dieing = false;
    public bool hitting = false;

    public FpsControllerLPFP player;

    NavMeshAgent navMeshAgent;

    public ParticleSystem bloodParticles;

    public float attackDistance = 4;

    private void Start()
    {
        navMeshAgent = this.GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isHit && !dieing)
        {
            dieing = true;
            bloodParticles.Emit(10);
            StartCoroutine(Die());
        }

        Vector3 dir = (player.transform.position - transform.position).normalized;
        Ray ray = new Ray(transform.position, dir);
        Debug.DrawRay(transform.position, dir * attackDistance);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, attackDistance) && !hitting)
        {
            if(hit.collider.tag == "Player")
            {
                Debug.Log("ray Hit");
                hitting = true;
                StartCoroutine(Hit());
            }
            
        }

        if(!dieing && !hitting)
        {
            Vector3 destination = player.transform.position;
            navMeshAgent.SetDestination(destination);
        }
    }

    IEnumerator Hit()
    {
        Animator animator = gameObject.GetComponent<Animator>();
        animator.Play("zombie_attack");
        AnimatorClipInfo[] clip = animator.GetCurrentAnimatorClipInfo(0);
        player.Hit();
        yield return new WaitForSeconds(clip[0].clip.length);
        hitting = false;
    }

    IEnumerator Die()
    {
        Animator animator = gameObject.GetComponent<Animator>();
        AnimatorClipInfo[] clip = animator.GetCurrentAnimatorClipInfo(0);
        animator.Play("zombie_death_standing");
        yield return new WaitForSeconds(clip[0].clip.length);
        Destroy(gameObject);
    }
}
