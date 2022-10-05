using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidScript : MonoBehaviour
{
    static float rayLength = 10;
    static float alignLength = 20;
    static float cohesLength = 20;
    static float droneSpeed = 0.5f;
    List<GameObject> neighbours = new List<GameObject>();
    GameObject scanCircle;
    GameObject groupCircle;
    System.Random rnd = new System.Random();
    Vector2 startMovement;
    List<GameObject> lines = new List<GameObject>();
    GameObject line;
    public Vector2 movement;
    public class test
    {
        Vector2 movement;
        public test(Vector2 movement)
        {
            this.movement = movement;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        line = new GameObject();
        line.transform.position = gameObject.transform.position;
        line.AddComponent<LineRenderer>();

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
        movement = startMovement;

        string radiusSearch = gameObject.name + "Radius";
        scanCircle = GameObject.Find(radiusSearch);

        string groupSearch = gameObject.name + "GroupRadius";
        groupCircle = GameObject.Find(groupSearch);

        GameObject[] list = GameObject.FindGameObjectsWithTag("drone");
        foreach (GameObject check in list)
        {
            if (check.name != gameObject.name)
            {
                neighbours.Add(check);
                lines.Add(new GameObject());
                lines[lines.Count-1].transform.position = gameObject.transform.position;
                lines[lines.Count-1].AddComponent<LineRenderer>();
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        movement = startMovement;
        Vector2 sepMovement = separation(movement);
        Vector2 aliMovement = alignment(movement);
        Vector2 cohMovement = cohesion(movement);
        //movement = (sepMovement + cohMovement); // TODO: Fix this shite!
        Vector2[] movements = new Vector2[]{sepMovement, aliMovement, cohMovement};
        movement = avgMove(movements);
        updateMovement(movement);

        Vector2 pos = gameObject.transform.position;
        LineRenderer lr = line.GetComponent<LineRenderer>();
        lr.startWidth = 1f;
        lr.endWidth = 1f;
        lr.SetPosition(0, pos);
        //print("pos: " + pos);
        float dx = movement.x;
        float dy = movement.y;
        float speed = Mathf.Sqrt((dx * dx) + (dy * dy));
        //print("speed: " + speed);
        float mult = rayLength / speed;
        //print("mult: " + mult);
        float lineX = dx * mult;
        float lineY = dy * mult;
        lr.SetPosition(1, new Vector2(pos.x + (dx*mult), pos.y + (dy*mult)));

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
        groupCircle.transform.position = newPos;
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
        for (int i = 0; i < lines.Count; i++)
        {
            LineRenderer lr = lines[i].GetComponent<LineRenderer>();
            lr.startWidth = 1f;
            lr.endWidth = 1f;
            lr.SetPosition(0, gameObject.transform.position);
            lr.SetPosition(1, neighbours[i].transform.position);
            lines[i].SetActive(false);
        }
        //gameObject.GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 255);
        SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
        
        if (gameObject.name == "Main")
        {
            sr.color = new Color32(255, 0, 0, 255); // if this object is the lead boid then colour it red
        } else
        {
            sr.color = new Color32(255, 255, 255, 255); // otherwise colour it white
        }

        for (int i = 0; i < neighbours.Count; i++)
        {
            float dis = dist(transform.position.x, transform.position.y, rayLength, neighbours[i].transform.position.x, neighbours[i].transform.position.y, (neighbours[i].transform.localScale.x / 2));
            if (dis + rayLength - (neighbours[i].transform.localScale.x / 2) <= 0)
            {
                //gameObject.SetActive(false); // Crash!
                //scanCircle.SetActive(false);
            }
            if (dis <= 0) // if a neighbour is within the detection radius
            {
                //lines[i].SetActive(true);

                if (gameObject.name == "Main")
                {
                    sr.color = new Color32(0, 0, 0, 255); // if this object is the lead boid then highlight it when it is close to another boid
                }
                //gameObject.GetComponent<Renderer>().material.color = new Color32(0, 0, 0, 255);
                //print("Green: in range!");
                // find if neighbour is on the left or right of current trajectory
                float dy = movement.y - transform.position.y;
                float dx = movement.x - transform.position.x;
                float m = dy / dx;
                float c = transform.position.y - (m * transform.position.x);
                // y = mx + c
                float testX = (neighbours[i].transform.position.y - c) / m;
                //print("testX: " + testX);
                if (movement.x >= 0)
                {
                    //print("Green flip");
                    testX *= -1;
                }
                //print(neighbour.transform.position.x + " >= " + testX);
                if (neighbours[i].transform.position.x >= testX)
                {
                    //print("Object is on the right so I'm steering left");
                    // point is on right side so steer to the left
                    float distance = dist(transform.position.x, transform.position.y, transform.localScale.x / 2, neighbours[i].transform.position.x, neighbours[i].transform.position.y, neighbours[i].transform.localScale.x / 2);
                    float distPercent = 1 - (distance / rayLength);
                    float reMapped = distPercent * (2 * Mathf.PI);
                    float turnAngle = reMapped * (0.05f * Mathf.PI);
                    float newX = Mathf.Cos(turnAngle) * movement.x - Mathf.Sin(turnAngle) * movement.y;
                    float newY = Mathf.Sin(turnAngle) * movement.x + Mathf.Cos(turnAngle) * movement.y;
                    movement = normaliseSpeed(new Vector2(newX, newY));

                }
                else if (neighbours[i].transform.position.x < testX)
                {
                    //print("Object is on the left so I'm steering right");
                    // point is on left side so steer to the right
                    float distance = dist(transform.position.x, transform.position.y, transform.localScale.x / 2, neighbours[i].transform.position.x, neighbours[i].transform.position.y, neighbours[i].transform.localScale.x / 2);
                    float distPercent = 1 - (distance / rayLength);
                    float reMapped = distPercent * (Mathf.PI);
                    float turnAngle = (2 * Mathf.PI) - (reMapped * (0.05f * Mathf.PI));
                    float newX = Mathf.Cos(turnAngle) * movement.x - Mathf.Sin(turnAngle) * movement.y;
                    float newY = Mathf.Sin(turnAngle) * movement.x + Mathf.Cos(turnAngle) * movement.y;
                    movement = new Vector2(newX, newY);
                }
            } else
            {
                lines[i].SetActive(false);
            }
        }
        return movement;
    }
    Vector2 alignment(Vector2 movement)
    {
        List<GameObject> near = new List<GameObject>();
        near.Clear();
        for (int i = 0; i < neighbours.Count; i++)
        {
            float dis = dist(transform.position.x, transform.position.y, alignLength, neighbours[i].transform.position.x, neighbours[i].transform.position.y, (neighbours[i].transform.localScale.x / 2));
            if (dis <= 0) // if a neighbour is within the detection radius
            {
                near.Add(neighbours[i]);
            }
        }
        if (near.Count > 0) {
            float totalDX = movement.x;
            float totalDY = movement.y;
            for (int i = 0; i < near.Count; i++)
            {
                Vector2 neighbourMovement = near[i].GetComponent<BoidScript>().movement;
                totalDX += neighbourMovement.x;
                totalDY += neighbourMovement.y;
            }
            //print("totalDX: " + totalDX);
            //print("totalDY: " + totalDY);
            //print("near.Count: " + near.Count);
            float avgDX = totalDX / near.Count;
            float avgDY = totalDY / near.Count;
            //print("avgDX: " + avgDX);
            //print("avgDY: " + avgDY);
            //print("normalising now...");
            Vector2 newMovement = normaliseSpeed(new Vector2(avgDX, avgDY));
            //print("new x: " + newMovement.x);
            //print("new y: " + newMovement.y);

            return newMovement;
        } else
        {
            return movement;
        }
    }
    Vector2 cohesion(Vector2 movement)
    {
        List<GameObject> near = new List<GameObject>();
        for (int i = 0; i < neighbours.Count; i++)
        {
            float dis = dist(transform.position.x, transform.position.y, cohesLength, neighbours[i].transform.position.x, neighbours[i].transform.position.y, (neighbours[i].transform.localScale.x / 2));
            if (dis <= 0) // if a neighbour is within the detection radius
            {
                near.Add(neighbours[i]);
            }
        }
        if (near.Count > 0) {
            float totalX = movement.x;
            float totalY = movement.y;
            for (int i = 0; i < near.Count; i++)
            {
                totalX += near[i].transform.position.x;
                totalY += near[i].transform.position.y;
            }
            float avgX = totalX / near.Count;
            float avgY = totalY / near.Count;
            float newX = gameObject.transform.position.x - avgX;
            float newY = gameObject.transform.position.y - avgY;
            Vector2 newMovement = normaliseSpeed(new Vector2(newX, newY));
            //print("newX: " + newX);
            //print("newY: " + newY);
            //float speed = Mathf.Sqrt((newX * newX) + (newY * newY));
            //float mult = droneSpeed / speed;
            //newX *= Mathf.Abs(mult);
            //newY *= Mathf.Abs(mult);
            //Vector2 newMovement = new Vector2(newX, newY);

            return newMovement;
        }
        return movement;
    }
    Vector2 normaliseSpeed(Vector2 newSpeed)
    {
        float newX = newSpeed.x;
        float newY = newSpeed.y;
        float speed = Mathf.Sqrt((newX * newX) + (newY * newY));
        float mult = droneSpeed / speed;
        newX *= Mathf.Abs(mult);
        newY *= Mathf.Abs(mult);
        Vector2 newMovement = new Vector2(newX, newY);
        return newMovement;
    }
    Vector2 avgMove(Vector2[] movements)
    {
        float dx = 0;
        float dy = 0;
        for (int i = 0; i < movements.Length; i++)
        {
            dx += movements[i].x;
            dy += movements[i].y;
        }
        float avgX = dx / movements.Length;
        float avgY = dy / movements.Length;
        return new Vector2(avgX, avgY);
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
