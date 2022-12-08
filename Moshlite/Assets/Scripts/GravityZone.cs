using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class GravityZone : MonoBehaviour
{
    public Vector3 gravity = new Vector3(0.0f, -9.81f, 0.0f);
    private BoxCollider box;

    private void Start()
    {
        box = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Make sure trigger has the tag "BulbHead" and is in the bulbhead layer
        if (other.tag == "BulbHead")
        {
            GravityZoneTrigger t = other.GetComponent<GravityZoneTrigger>();
            if (t != null)
            {
                t.bulbHeadController.SetRealGravity(gravity);
            }
        }
    }

    public void ToggleGravityZone()
    {
        gravity *= -1.0f;

        foreach (Collider coll in Physics.OverlapBox(box.center, box.size * 0.5f, Quaternion.identity, LayerMask.GetMask("BulbHead")))
        {
            BulbHeadController c = coll.GetComponent<GravityZoneTrigger>().bulbHeadController;
            if (c != null)
            {
                c.SetRealGravity(gravity);
            }
        }
    }
}
