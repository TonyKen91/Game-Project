using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActor : MonoBehaviour {

    public float speed = 5.0f;

    private CharacterController controller;

	// Use this for initialization
	void Start () {
        controller = gameObject.GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 move_direction = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.W))
            move_direction.Set(-speed, 0, 0);
        if (Input.GetKey(KeyCode.S))
            move_direction.Set(speed, 0, 0);
        if (Input.GetKey(KeyCode.A))
            move_direction.Set(0, 0, -speed);
        if (Input.GetKey(KeyCode.D))
            move_direction.Set(0, 0, speed);


        controller.Move(move_direction * Time.deltaTime);

        Vector3 mouse_pos = Input.mousePosition;

        // Use the current camera to convert mouse position to a ray
        Ray mouse_ray = Camera.main.ScreenPointToRay(mouse_pos);

        // Create a plane that faces up at the same position as the player
        Plane player_plane = new Plane(Vector3.up, transform.position);

        // How far along the ray does the interesection with the plane occur?
        float ray_distance = 0;
        player_plane.Raycast(mouse_ray, out ray_distance);

        // Use the ray distance to calculate the point of collision
        Vector3 cast_point = mouse_ray.GetPoint(ray_distance);

        Vector3 to_cast_point = cast_point - transform.position;
        to_cast_point.Normalize();

        Ray fire_ray = new Ray(transform.position, to_cast_point);

        RaycastHit info;
        if(Input.GetMouseButtonDown(0) && Physics.Raycast(fire_ray, out info))
        {
            if(info.collider.tag == "Enemy")
                Destroy(info.collider.gameObject);
        }


	}
}
