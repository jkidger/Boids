using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleScript3 : MonoBehaviour
{
    static float rayLength = 10;
    static float droneSpeed = 0.1f;
    List<GameObject> neighbours = new List<GameObject>();
    GameObject scanCircle;
    System.Random rnd = new System.Random();
    Vector2 startMovement;
    // Start is called before the first frame update
    void Start()
    {
        float startX = rnd.Next(-300, 300);
        float startY = rnd.Next(-300, 300);
        transform.position = new Vector3(startX, startY, 0);

        float dX = (float)rnd.NextDouble() * 0.005f;
        float dY = (float)rnd.NextDouble() * 0.005f;
        float speed = Mathf.Sqrt((dX * dX) + (dY * dY));
        float mult = droneSpeed / speed;
        dX *= Mathf.Abs(mult);
        dY *= Mathf.Abs(mult);
        startMovement = new Vector2(dX, dY);

        string radiusSearch = gameObject.name + "Radius";
        scanCircle = GameObject.Find(radiusSearch);

        for (int i = 0; i < 15; i++)
        {
            GameObject newDrone = GameObject.Instantiate(GameObject.Find("Dead"));
            newDrone.name = "Gen" + i;
            newDrone.tag = "drone";
            newDrone.AddComponent<CircleScript4>();
            GameObject newDroneRadius = GameObject.Instantiate(GameObject.Find("DeadRadius"));
            newDroneRadius.name = newDrone.name + "Radius";
            neighbours.Add(newDrone);
        }
        GameObject dead = GameObject.Find("Dead");
        dead.SetActive(false);
        GameObject deadRadius = GameObject.Find("DeadRadius");
        deadRadius.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        Vector2 movement = startMovement;
        movement = separation(movement);
        updateMovement(movement);

        if (transform.position.x < -150)
        {
            transform.position = new Vector2(150, transform.position.y);
        }
        if (transform.position.x > 150)
        {
            transform.position = new Vector2(-150, transform.position.y);
        }
        if (transform.position.y < -150)
        {
            transform.position = new Vector2(transform.position.x, 150);
        }
        if (transform.position.y > 150)
        {
            transform.position = new Vector2(transform.position.x, -150);
        }
        //print(gameObject.name + " pos: " + transform.position.x + ", " + transform.position.y);
    }
    void updateMovement(Vector2 movement)
    {
        Vector2 start = transform.position;
        Vector2 newPos = new Vector2(start.x + movement.x, start.y + movement.y);
        transform.position = newPos;
        scanCircle.transform.position = newPos;
        start = transform.position;
        /*foreach (ray r in rays)
        {
            LineRenderer lr = r.go.GetComponent<LineRenderer>();
            Vector2 newEnd = new Vector2(r.end.x + movement.x, r.end.y + movement.y);
            lr.SetPosition(0, start);
            lr.SetPosition(1, newEnd);
            r.start = start;
            r.end = newEnd;
        }*/
    }
    Vector2 separation(Vector2 movement)
    {
        gameObject.GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 255);

        
        foreach (GameObject neighbour in neighbours)
        {
            float dis = dist(transform.position.x, transform.position.y, rayLength, neighbour.transform.position.x, neighbour.transform.position.y, (neighbour.transform.localScale.x / 2));
            if (dis + rayLength - (neighbour.transform.localScale.x / 2) <= 0)
            {
                //gameObject.SetActive(false); // Crash!
                //scanCircle.SetActive(false);
            }
            if (dis <= 0) // if a neighbour is within the detection radius
            {
                gameObject.GetComponent<Renderer>().material.color = new Color32(0, 0, 0, 255);
                //print("Green: in range!");
                // find if neighbout is on the left or right of current trajectory
                float dy = movement.y - transform.position.y;
                float dx = movement.x - transform.position.x;
                float m = dy / dx;
                float c = transform.position.y - (m * transform.position.x);
                // y = mx + c
                float testX = (neighbour.transform.position.y - c) / m;
                //print("testX: " + testX);
                if (movement.x >= 0)
                {
                    //print("Green flip");
                    testX *= -1;
                }
                //print(neighbour.transform.position.x + " >= " + testX);
                if (neighbour.transform.position.x >= testX)
                {
                    //print("Object is on the right so I'm steering left");
                    // point is on right side so steer to the left
                    float distance = dist(transform.position.x, transform.position.y, transform.localScale.x / 2, neighbour.transform.position.x, neighbour.transform.position.y, neighbour.transform.localScale.x / 2);
                    float distPercent = 1 - (distance / rayLength);
                    float reMapped = distPercent * (2 * Mathf.PI);
                    float turnAngle = reMapped * (0.05f * Mathf.PI);
                    float newX = Mathf.Cos(turnAngle) * movement.x - Mathf.Sin(turnAngle) * movement.y;
                    float newY = Mathf.Sin(turnAngle) * movement.x + Mathf.Cos(turnAngle) * movement.y;
                    movement = new Vector2(newX, newY);
                    
                } else if (neighbour.transform.position.x < testX)
                {
                    //print("Object is on the left so I'm steering right");
                    // point is on left side so steer to the right
                    float distance = dist(transform.position.x, transform.position.y, transform.localScale.x / 2, neighbour.transform.position.x, neighbour.transform.position.y, neighbour.transform.localScale.x / 2);
                    float distPercent = 1 - (distance / rayLength);
                    float reMapped = distPercent * (Mathf.PI);
                    float turnAngle = (2 * Mathf.PI) - (reMapped * (0.05f * Mathf.PI));
                    float newX = Mathf.Cos(turnAngle) * movement.x - Mathf.Sin(turnAngle) * movement.y;
                    float newY = Mathf.Sin(turnAngle) * movement.x + Mathf.Cos(turnAngle) * movement.y;
                    movement = new Vector2(newX, newY);
                }
            }
        }
        return movement;
    }
    float dist(float x1, float y1, float r1, float x2, float y2, float r2)
    {
        float dx = x1 - x2;
        //print("dx: " + dx);
        float dy = y1 - y2;
        //print("dy: " + dy);
        float d = Mathf.Sqrt((dx * dx) + (dy * dy));
        //print("d: " + d);
        float final = d - r1 - r2;
        //print("final: " + final);
        return final;
    }
}
