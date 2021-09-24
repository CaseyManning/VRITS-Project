using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chair : MonoBehaviour
{
    float lastSat = 0;

    bool playerSitting = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time - lastSat < 1)
        {
            return;
        }
        if (playerSitting && Input.GetKeyDown(KeyCode.R))
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>().isKinematic = false;
            playerSitting = false;
            lastSat = Time.time;
            return;
        }

        RaycastHit hit;
        Ray ray = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5f, 0.5f));

        if (Physics.Raycast(ray, out hit, 100f))
        {
            if (hit.collider == GetComponent<BoxCollider>())
            {
                if(Input.GetKey(KeyCode.R))
                {
                    GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>().isKinematic = true;
                    playerSitting = true;
                    lastSat = Time.time;
                }
            }
        }

        if(playerSitting)
        {
            GameObject.FindGameObjectWithTag("Player").transform.position = (transform.position + transform.forward * 1.5f);
        }
    }
}
