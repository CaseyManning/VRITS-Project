using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockData : MonoBehaviour
{
    public static Dictionary<Rigidbody, List<Joint>> connected = new Dictionary<Rigidbody, List<Joint>>();
    public static Dictionary<Rigidbody, bool> activated = new Dictionary<Rigidbody, bool>();
}
