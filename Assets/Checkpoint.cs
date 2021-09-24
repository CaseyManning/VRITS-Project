using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Checkpoint : MonoBehaviour
{
    public static int checkpointsReached = 0;

    RawImage end;

    // Start is called before the first frame update
    void Start()
    {
        end = GameObject.FindGameObjectWithTag("EndScreen").GetComponent<RawImage>();
        end.CrossFadeAlpha(0, 0, false);
        end.transform.GetChild(0).GetComponent<Text>().CrossFadeAlpha(0, 0, false);
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector3(1, 1, 1 + 0.4f*Mathf.Sin(Time.time*2));
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            checkpointsReached++;

            if(checkpointsReached >= 3)
            {
                Logger.currentData.timeToComplete = Time.timeSinceLevelLoad;
                end.CrossFadeAlpha(1, 1, false);
                end.transform.GetChild(0).GetComponent<Text>().CrossFadeAlpha(1, 2, false);
                Logger.writeData();
            }

            Destroy(gameObject);
        }
    }
}
