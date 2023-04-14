using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    bool toggle = false;
    public GameObject mainBoid;
    Quaternion oringinalRotation;
    // Start is called before the first frame update
    void Start()
    {
        mainBoid = GameObject.Find("Main");
        oringinalRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (mainBoid != null)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                toggle = !toggle;
            }
            if (toggle)
            {
                transform.LookAt(mainBoid.transform.position);
            }
            else
            {
                transform.rotation = oringinalRotation;
            }
        }
    }
}
