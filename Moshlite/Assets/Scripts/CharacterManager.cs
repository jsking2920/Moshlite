using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [SerializeField] private List<CharacterController> chars;

    [SerializeField] private Vector3 gravity = new Vector3(0.0f, -9.81f, 0.0f);

    private void Start()
    {
        Physics.gravity = gravity;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            foreach (CharacterController c in chars)
            {
                c.BangHead();
            }
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            foreach (CharacterController c in chars)
            {
                c.Jump();
            }
        }
    }
}
