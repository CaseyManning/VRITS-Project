using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour
{
    float lastDanceTime = 0;

    void Start()
    {
        
    }

    void Update()
    {
        GetComponent<NavMeshAgent>().SetDestination(GameObject.FindGameObjectWithTag("Player").transform.position);

        //if(Time.time > lastDanceTime + 30)
        //{
        //    GetComponent<Animator>().SetBool("dancey dancey", true);
        //    GetComponent<Animator>().SetTrigger("dancey");
        //    GetComponent<NavMeshAgent>().isStopped = true;
        //}
        //if (Time.time > lastDanceTime + 34.7)
        //{
        //    lastDanceTime = Time.time;
        //    GetComponent<NavMeshAgent>().isStopped = false;
        //}
    }
}
