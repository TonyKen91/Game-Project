using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class CompanionActor : MonoBehaviour
{
    public float companionSpawnHeight = 5.0f;

    // When we want to make a prefab, we can't reference any specific object in a scene
    // This is because it could be used in multiple scenes
    //private PlayerActor player;
    private FPSController player;

    // Use this for initialization
    void Start()
    {
        // To overcome specifying a reference, this is used instead to find the object
        //player = GameObject.FindObjectOfType<PlayerActor>();
        player = GameObject.FindObjectOfType<FPSController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 dirToPlayer;
        dirToPlayer = player.transform.position - this.transform.position;
        dirToPlayer.Normalize();

        transform.position += dirToPlayer * player.OutputPlayerMovement().magnitude * Time.fixedDeltaTime;
    }
}
