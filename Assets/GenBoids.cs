using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenBoids : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 30; i++)
        {
            GameObject newDrone = GameObject.Instantiate(GameObject.Find("Dead"));
            GameObject newDroneRadius = GameObject.Instantiate(GameObject.Find("DeadRadius"));
            GameObject newDroneGroupRadius = GameObject.Instantiate(GameObject.Find("DeadGroupRadius"));
            SpriteRenderer sr = newDrone.GetComponent<SpriteRenderer>();
            SpriteRenderer rsr = newDroneRadius.GetComponent<SpriteRenderer>();
            SpriteRenderer grsr = newDroneGroupRadius.GetComponent<SpriteRenderer>();
            if (i == 0)
            {
                newDrone.name = "Main";
                sr.color = new Color32(255, 0, 0, 255);
                rsr.color = new Color32(255, 255, 255, 30);
                grsr.color = new Color32(255, 255, 255, 20);
            } else
            {
                newDrone.name = "Gen" + i;
                sr.color = new Color32(255, 255, 255, 255);
                rsr.color = new Color32(255, 255, 255, 0);
                grsr.color = new Color32(255, 255, 255, 0);
            }
            newDrone.tag = "drone";
            newDroneRadius.tag = "radius";
            newDroneRadius.tag = "radius";
            newDrone.AddComponent<BoidScript>();
            newDroneRadius.name = newDrone.name + "Radius";
            newDroneGroupRadius.name = newDrone.name + "GroupRadius";
        }
        GameObject dead = GameObject.Find("Dead");
        dead.SetActive(false);
        GameObject deadRadius = GameObject.Find("DeadRadius");
        deadRadius.SetActive(false);
        GameObject deadGroupRadius = GameObject.Find("DeadGroupRadius");
        deadGroupRadius.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
