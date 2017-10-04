using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class AmmoSpawnActor : MonoBehaviour {

    public GameObject ammo_prefab;
    public float spawn_time = 2.0f; // seconds between spawns
    public float spawn_radius = 30.0f; // distance from player to spawn

    private FPSController player;
    //private PlayerActor player;
    private float spawn_timer; // The timer that counts and controls spawning

    private Renderer ammo_renderer;

    // Use this for initialization
    void Start()
    {
        spawn_timer = spawn_time;
        player = GameObject.FindObjectOfType<FPSController>();
        ammo_renderer = ammo_prefab.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 ammo_size = ammo_renderer.bounds.size;
        // Count our timer down each frame
        spawn_timer -= Time.fixedDeltaTime;

        if (spawn_timer < 0)
        {

            // reset the timer
            spawn_timer = spawn_time;

            // Pick a random angle in radians and set the spawn point
            float spawn_angle = Random.Range(0, 2 * Mathf.PI);

            Vector3 spawn_direction = new Vector3(Mathf.Sin(spawn_angle), 0, Mathf.Cos(spawn_angle));
            spawn_direction *= spawn_radius;

            Vector3 spawn_point = player.transform.position + spawn_direction;
            spawn_point.y = 0 + ammo_size.y/2;

            // Spawn the enemy at the desired location
            Instantiate(ammo_prefab, spawn_point, Quaternion.identity);
        }
    }
}
