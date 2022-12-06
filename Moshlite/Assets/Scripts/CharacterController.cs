using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField] private float drag = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            rb.drag = drag;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
