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
        float sizeX = canvas.transform.localScale.x/2;
        float sizeY = canvas.transform.localScale.y/2;
        float startX = rnd.Next((int)sizeX * -1, (int)sizeX);
        float startY = rnd.Next((int)sizeY * -1, (int)sizeY);
        transform.position = new Vector3(startX, startY, 0);
    }
        // Update is called once per frame
    void Update()
    {
        foreach (GameObject b in boids)
        {
            Vector2 me = transform.position;
            Vector2 them = b.transform.position;
            if (dist(me, transform.localScale.x /2, them, b.transform.localScale.x / 2) <= 0) // if any boid collides with goal
            {
                Color32 white = new Color32(255, 255, 255, 255);
                Color32 transparent = new Color32(255, 255, 255, 0);
                numCollisions++;
                // move goal to random position
                randoPos();
            }
        }
    }
    float dist(Vector2 a, float aR, Vector2 b, float bR)
        {
            float dx = a.x - b.x;
            float dy = a.y - b.y;
            float d = Mathf.Sqrt((dx * dx) + (dy * dy));
            float final = d - aR - bR;
            return final;
        }
    public void randoPos()
    {
        float sizeX = canvas.transform.localScale.x/2;
        float sizeY = canvas.transform.localScale.y/2;
        float startX = rnd.Next((int)sizeX * -1, (int)sizeX);
        float startY = rnd.Next((int)sizeY * -1, (int)sizeY);
        transform.position = new Vector3(startX, startY, 0);
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
