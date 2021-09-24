using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activator : MonoBehaviour
{
    public Material noGlow;
    public Material hoverGlow;
    public Material onGlow;

    bool hovering;

    float rocketForce = 5f;

    List<Rigidbody> connectedBlocks;
    Vector3 totalForce = Vector3.zero;

    float lastActivated = 0;

    // Start is called before the first frame update
    void Start()
    {
        BlockData.activated[GetComponent<Rigidbody>()] = false;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Ray ray = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5f, 0.5f));

        if (Physics.Raycast(ray, out hit, 100f))
        {
            if(hit.collider == GetComponent<BoxCollider>())
            {
                if (!BlockData.activated[GetComponent<Rigidbody>()])
                {
                    Material[] foo = GetComponent<MeshRenderer>().materials;
                    foo[1] = hoverGlow;
                    GetComponent<MeshRenderer>().materials = foo;
                }
                hovering = true;
            } else
            {
                if (!BlockData.activated[GetComponent<Rigidbody>()])
                {
                    Material[] foo = GetComponent<MeshRenderer>().materials;
                    foo[1] = noGlow;
                    GetComponent<MeshRenderer>().materials = foo;
                }
                hovering = false;
            }
        }

        if (hovering && Input.GetKey(KeyCode.Q) && Time.time - lastActivated > 1)
        {
            lastActivated = Time.time;
            if (BlockData.activated.ContainsKey(GetComponent<Rigidbody>()) && BlockData.activated[GetComponent<Rigidbody>()])
            {
                deActivateMachine();
        }
        else
            {
                activateMachine();
        }
    }
    }

    private void FixedUpdate()
    {
        if (BlockData.activated.ContainsKey(GetComponent<Rigidbody>()) && BlockData.activated[GetComponent<Rigidbody>()])
        {
            foreach (Rigidbody blockrb in connectedBlocks)
            {
                //blockrb.MovePosition(blockrb.position + totalForce * Time.deltaTime);
                blockrb.velocity = totalForce;
            }
        }
    }

    List<Rigidbody> getConnectedBlocks()
    {
        List<Rigidbody> connected = new List<Rigidbody>();
        List<Rigidbody> toAdd = new List<Rigidbody>();

        Rigidbody rb = GetComponent<Rigidbody>();

        if (BlockData.connected.ContainsKey(rb))
        {
            for (int i = 0; i < BlockData.connected[rb].Count; i++)
            {
                //if (BlockData.connected[rb][i] != null)
                //{
                    toAdd.Add(BlockData.connected[rb][i].gameObject.GetComponent<Rigidbody>());
                //}
            }
        }
        for (int i = 0; i < GetComponents<FixedJoint>().Length; i++)
        {
            toAdd.Add(GetComponents<FixedJoint>()[i].connectedBody);
        }

        while(toAdd.Count > 0)
        {
            Rigidbody other = toAdd[0];
            toAdd.RemoveAt(0);
            connected.Add(other);

            foreach(Joint j in other.GetComponents<Joint>())
            {
                if(!toAdd.Contains(j.connectedBody) && !connected.Contains(j.connectedBody))
                {
                    toAdd.Add(j.connectedBody);
                }
            }
            if(BlockData.connected.ContainsKey(other))
            {
                foreach (Joint j in BlockData.connected[other])
                {
                    if(j == null)
                    {
                        continue;
                    }
                    if (!toAdd.Contains(j.gameObject.GetComponent<Rigidbody>()) && !connected.Contains(j.gameObject.GetComponent<Rigidbody>()))
                    {
                        toAdd.Add(j.gameObject.GetComponent<Rigidbody>());
                    }
                }
            }
        }   

        return connected;
    }


    void activateMachine()
    {
        totalForce = Vector3.zero;
        print("activating");

        BlockData.activated[GetComponent<Rigidbody>()] = true;

        Material[] foo = GetComponent<MeshRenderer>().materials;
        foo[1] = onGlow;
        GetComponent<MeshRenderer>().materials = foo;

        connectedBlocks = getConnectedBlocks();

        foreach (Rigidbody blockrb in connectedBlocks)
        {
            BlockData.activated[blockrb] = true;
            //blockrb.isKinematic = true;
            blockrb.useGravity = false;
            if (blockrb.gameObject.name.Contains("roket"))
            {
                //blockrb.gameObject.AddComponent<ConstantForce>().relativeForce = new Vector3(0, 0, -rocketForce);
                blockrb.gameObject.GetComponent<ParticleSystem>().Play();

                totalForce += blockrb.transform.forward * -rocketForce;
            }
        }
        //foreach (Rigidbody blockrb in connectedBlocks)
        //{
        //    blockrb.gameObject.AddComponent<ConstantForce>().relativeForce = totalForce;
        //}
    }

    void deActivateMachine()
    {
        print("deactivating");
        //Material[] foo = GetComponent<MeshRenderer>().materials;
        //foo[1] = noGlow;
        //GetComponent<MeshRenderer>().materials = foo;

        BlockData.activated[GetComponent<Rigidbody>()] = false;

        List<Rigidbody> connectedBlocks = getConnectedBlocks();
        print(connectedBlocks.Count);

        foreach (Rigidbody blockrb in connectedBlocks)
        {
            BlockData.activated[blockrb] = false;
            //blockrb.isKinematic = false;
            blockrb.useGravity = true;
            if (blockrb.gameObject.name.Contains("roket"))
            {
                blockrb.gameObject.GetComponent<ParticleSystem>().Stop();
            }
            //Destroy(blockrb.gameObject.GetComponent<ConstantForce>());
        }
    }
}
