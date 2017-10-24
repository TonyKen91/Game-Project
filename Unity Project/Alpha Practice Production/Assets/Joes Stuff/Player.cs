using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    public float health = 100f;
    private float hunger = 100f;
    private float hydration = 100f;

    public float hungerDep = 0.5f;
    public float hydrationDep = 0.5f;
    private float healthDep = 0.5f;

    public Text healthTxt;
    public Text hungerTxt;
    public Text hydrationTxt;

	void Update () {

        healthTxt.text = ("Health: " + health);
        hungerTxt.text = ("Hunger: " + hunger);
        hydrationTxt.text = ("Hydration: " + hydration);
         
        Depleation();
    }

    void Depleation()
    {
        if (hydration >= 1)
        {
            hydration -= hydrationDep * Time.deltaTime;
        }

        if (hunger >= 1)
        {
            hunger -= hungerDep * Time.deltaTime;
        }

        //if running hunger depleats faster
        if (Input.GetButtonDown("Fire3"))
        {
            hungerDep = hungerDep * 2;
        }

        if (Input.GetButtonUp("Fire3"))
        {
            hungerDep /= 2;
        }

        //For Both hunger and hydration under 50
        if (hydration <= 50 && hunger <= 50)
        {
            health -= healthDep * 2 * Time.deltaTime;
        }

        //For seperate hunger and hydration under 50
        if (hunger <= 50)
        {
            health -= healthDep * Time.deltaTime;
        }

        if (hydration <= 50)
        {
            health -= healthDep * Time.deltaTime;
        }
    }
}
