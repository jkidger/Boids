using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalScript : MonoBehaviour
{
    public List<GameObject> boids;
    public GameObject canvas;
    System.Random rnd = new System.Random();
    public int numCollisions = 0;
    // Start is called before the first frame update
    void Start()
    {
        boids = new List<GameObject>();

        findBoids();


        // gen random start position
        transform.position = randoPos();
    }
    // Update is called once per frame
    void Update()
    {
        foreach (GameObject b in boids)
        {
            Vector2 me = transform.position;
            Vector2 them = b.transform.position;
            if (dist(me, transform.localScale.x / 2, them, b.transform.localScale.x / 2) <= 0) // if any boid collides with goal
            {
                //Color32 white = new Color32(255, 255, 255, 255);
                //Color32 transparent = new Color32(255, 255, 255, 0);
                numCollisions++;
                // move goal to random position
                transform.position = randoPos();
                //findBoids();
            }
        }
    }
    float dist(Vector3 a, float aR, Vector3 b, float bR)
    {
        float dx = a.x - b.x;
        float dy = a.y - b.y;
        float dz = a.z - b.z;
        float d = Mathf.Sqrt((dx * dx) + (dy * dy) + (dz * dz));
        float final = d - aR - bR;
        return final;
    }
    public Vector3 randoPos()
    {
        float sizeX = canvas.transform.localScale.x / 2;
        float sizeY = canvas.transform.localScale.y / 2;
        float sizeZ = canvas.transform.localScale.z / 2;
        float startX = rnd.Next((int)sizeX * -1, (int)sizeX);
        float startY = rnd.Next((int)sizeY * -1, (int)sizeY);
        float startZ = rnd.Next((int)sizeZ * -1, (int)sizeZ);
        return new Vector3(startX, startY, startZ);
    }
    public void findBoids()
    {
        boids.Clear();
        GameObject[] list = GameObject.FindGameObjectsWithTag("drone");
        foreach (GameObject check in list)
        {
            boids.Add(check);
        }
    }

}
