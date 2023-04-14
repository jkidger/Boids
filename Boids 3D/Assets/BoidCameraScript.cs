using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidCameraScript : MonoBehaviour
{
    public GameObject mainBoid;
    // Start is called before the first frame update
    void Start()
    {
        mainBoid = GameObject.Find("Main");
    }

    // Update is called once per frame
    void Update()
    {
        if (mainBoid != null)
        {
            transform.position = mainBoid.transform.position;
            transform.LookAt(mainBoid.transform.position + mainBoid.GetComponent<BoidScript>().movement);
        }
    }
}
