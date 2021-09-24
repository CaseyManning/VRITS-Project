using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabGun : MonoBehaviour
{
    [SerializeField] Transform objectHolder;
    [SerializeField] Camera cam;
    [SerializeField] float maxGrabDistance = 10f;

    Rigidbody grabbedRB;

    Rigidbody snappedRB;

    public Material snappedMat;
    Material baseMat;

    Vector3[] rotations = {new Vector3(0,0,0), new Vector3(90, 0, 0) , new Vector3(180, 0, 0) , new Vector3(270, 0, 0), new Vector3(0, 90, 0), new Vector3(0, 270, 0) };
    int currentRotation = 0;

    float timeLastRecordedPosition = 0f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.mouseScrollDelta.y > 0)
        {
            currentRotation += 1;
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            currentRotation -= 1;
        }
        if(currentRotation < 0)
        {
            currentRotation += 6;
        }
        currentRotation = currentRotation % 6;

        if (grabbedRB)
        {
            snappedRB = null;

            grabbedRB.MovePosition(Vector3.Lerp(grabbedRB.position, objectHolder.transform.position, Time.deltaTime * 10f));

            RaycastHit hit;
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));

            if (Physics.Raycast(ray, out hit, maxGrabDistance))
            {
                if (hit.collider.gameObject.CompareTag("Block") && hit.rigidbody != grabbedRB)
                {
                    
                    snappedRB = hit.rigidbody;
                    grabbedRB.MovePosition(hit.collider.transform.position + hit.normal * 2.453f / 2);
                    grabbedRB.gameObject.transform.rotation = Quaternion.LookRotation(hit.normal, hit.rigidbody.transform.up);
                    grabbedRB.gameObject.transform.Rotate(rotations[currentRotation]);

                    grabbedRB.gameObject.GetComponent<MeshRenderer>().material = snappedMat;
                    
                } else
                {
                    grabbedRB.gameObject.GetComponent<MeshRenderer>().material = baseMat;
                }
            } else
            {
                grabbedRB.gameObject.GetComponent<MeshRenderer>().material = baseMat;
            }
        }

        if(Input.GetKeyDown(KeyCode.E))
        {
            if(grabbedRB)
            {
                if (snappedRB != null)
                {
                    placeBlock();
                }
                else
                {
                    dropBlock();
                }

            } else
            {
                grabBlock();
            }
        }

        if(Time.time - timeLastRecordedPosition > 1)
        {
            Logger.currentData.positionData += transform.position + ", ";
            timeLastRecordedPosition = Time.time;
        }
    }

    void grabBlock()
    {
        RaycastHit hit;
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        if (Physics.Raycast(ray, out hit, maxGrabDistance))
        {
            grabbedRB = hit.collider.gameObject.GetComponent<Rigidbody>();

            if (grabbedRB && !(BlockData.activated.ContainsKey(grabbedRB) && BlockData.activated[grabbedRB]))
            {
                grabbedRB.isKinematic = true;
                grabbedRB.detectCollisions = false;

                Joint[] joints = grabbedRB.gameObject.GetComponents<Joint>();
                foreach (Joint j in joints) {
                    BlockData.connected[j.connectedBody].Remove(j);
                    Destroy(j);
                 }


                if (BlockData.connected.ContainsKey(grabbedRB))
                {
                    List<Joint> joints2 = BlockData.connected[grabbedRB];
                    foreach (Joint j in joints2) Destroy(j);

                    BlockData.connected[grabbedRB].Clear();
                }

                baseMat = grabbedRB.gameObject.GetComponent<MeshRenderer>().material;
            }
        }
    }

    void dropBlock()
    {
        grabbedRB.isKinematic = false;
        grabbedRB.detectCollisions = true;
        grabbedRB = null;
    }

    void placeBlock()
    {
        Logger.currentData.blockInteractions++;
        FixedJoint joint = grabbedRB.gameObject.AddComponent<FixedJoint>();
        joint.connectedBody = snappedRB;

        if (!BlockData.connected.ContainsKey(snappedRB))
        {
            BlockData.connected[snappedRB] = new List<Joint>();
            BlockData.connected[snappedRB].Add(joint);
        } else
        {
            BlockData.connected[snappedRB].Add(joint);
        }

        grabbedRB.gameObject.GetComponent<MeshRenderer>().material = baseMat;

        grabbedRB.isKinematic = false;
        grabbedRB.detectCollisions = true;
        grabbedRB = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Logger.currentData.aiInteractions += 1;
            if (grabbedRB)
            {
                grabbedRB.gameObject.GetComponent<MeshRenderer>().material = baseMat;
                dropBlock();
            }
        }
    }
}
