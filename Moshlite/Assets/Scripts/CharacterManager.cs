using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [SerializeField] private List<CharacterController> chars;

    [SerializeField] private Vector3 defaultGravity = new Vector3(0.0f, -9.81f, 0.0f);
    private bool isGravityOn = true;

    private void Start()
    {
        Physics.gravity = defaultGravity;
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
    public void TurnGravityOn()
    {
        Physics.gravity = defaultGravity;
        isGravityOn = true;
    }

    public void TurnGravityOff()
    {
        Physics.gravity = Vector3.zero;
        isGravityOn = false;
    }

    public void ToggleGravity()
    {
        if (isGravityOn)
        {
            Physics.gravity = Vector3.zero;
        }
        else
        {
            Physics.gravity = defaultGravity;
        }
        isGravityOn = !isGravityOn;
    }

    // Add randomness to forces
    // figure out mapping of effects
    // set up some coroutine animations for specific effects
    // Set up different classes of character (jumpers/head bangers/etc)
}
