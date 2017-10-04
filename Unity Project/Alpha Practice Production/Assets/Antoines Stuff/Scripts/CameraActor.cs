using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraActor : MonoBehaviour {

    public Transform target;

    private Vector3 view;

    //private Vector3 isometricView;
    //private Transform isometricViewTransform;

    //private Vector3 topView;
    //private Transform topViewTransform;

    private bool isometricView = true;

	// Use this for initialization
	void Start () {
        //isometricViewTransform = new Transform;
        //isometricViewTransform.position = target.position + new Vector3(10, 4, 10);
        //isometricViewTransform.rotation.Set(20, 220, 0, topViewTransform.rotation.w);

        //topViewTransform.position = target.position + new Vector3(0, 10, 0);
        //topViewTransform.rotation.Set(90, 0, 0, topViewTransform.rotation.w);
        view = this.transform.position - target.position;
    }

    // Update is called once per frame
    void Update () {
        //Vector3 topView = target.position + new Vector3(0, 5, 0);
        if (Input.GetKeyDown(KeyCode.M))
        {
            SwitchView();
        }

        Vector3 target_pos = target.position + view;
        this.transform.position = target_pos;
        

        // This is used to ignore collisions with CharacterCollider
	}

    void SwitchView()
    {
        if (isometricView == false)
        {
            this.transform.position = target.position + new Vector3(10, 4, 0);
            this.transform.rotation = Quaternion.Euler(20, -90, 0);
            isometricView = true;
        }
        else
        {
            this.transform.position = target.position + new Vector3(0, 15, 0);
            this.transform.rotation = Quaternion.Euler(90, 0, 90);
                
                //rotation.Set(90, 0, 90, topViewTransform.rotation.w);
            isometricView = false;
        }
        view = transform.position - target.position;

    }
}
