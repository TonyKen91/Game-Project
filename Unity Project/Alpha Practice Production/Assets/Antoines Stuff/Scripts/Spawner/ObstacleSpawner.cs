using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour {

    public GameObject obstacle_prefab;
    public GameObject ground;


    private Renderer obstacleRenderer;
    private Renderer groundRenderer;
    private Transform groundTransform;

    public int numberOfObstacles = 5;

	// Use this for initialization
	void Start () {
        //Vector3 spawn_direction
        obstacleRenderer = obstacle_prefab.GetComponent<Renderer>();
        groundRenderer = ground.GetComponent<Renderer>();
        groundTransform = ground.transform;
        Vector3 boundary = groundRenderer.bounds.size;
        Vector3 obstacleSize = obstacleRenderer.bounds.size;
        for (int i = 0; i < numberOfObstacles; i++)
        {
            float randomX = Random.Range(boundary.x / 2, boundary.x / (-2));
            float randomZ = Random.Range(boundary.z / 2, boundary.z / (-2));
            Vector3 spawn_point = groundTransform.position + new Vector3(randomX, obstacleSize.y/2, randomZ);
            Instantiate(obstacle_prefab, spawn_point, Quaternion.identity);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
