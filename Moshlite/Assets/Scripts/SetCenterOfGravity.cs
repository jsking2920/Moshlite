using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SetCenterOfGravity : MonoBehaviour
{
    [SerializeField] private Transform cog_transform; // Desired position of center of gravity, must be child object of this rigidbody
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = cog_transform.localPosition;
    }
}
