using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BoidScript : MonoBehaviour
{
    public GameObject canvas;
    float canvasWidth;
    float canvasHeight;

    public float sepLength;
    public float alignLength;
    public float cohesLength;
    public float droneSpeed;

    public float accel;

    public float obstScale;
    public float sepScale;
    public float goalScale;
    public float aliScale;
    public float cohScale;
    public float prevMovScale;

    public List<GameObject> neighbours = new List<GameObject>();
    public GameObject scanCircle;
    public GameObject groupCircle;
    System.Random rnd = new System.Random();
    Vector2 startMovement;
    GameObject heading;
    Vector2 movement;
    public Vector2 goalPos;
    public List<GameObject> obstacles = new List<GameObject>();

    LineRenderer rayRenderer;
    GameObject ray;


    // Start is called before the first frame update
    void Start()
    {
        // add back in to visualize the collision ray
        /*ray = new GameObject();
        ray.transform.position = transform.position;
        ray.AddComponent<LineRenderer>();
        rayRenderer = ray.GetComponent<LineRenderer>();
        rayRenderer.startWidth = 1f;
        rayRenderer.endWidth = 1f;
        ray.name = gameObject.name + " collision ray";*/

        

        heading = new GameObject();
        heading.transform.position = gameObject.transform.position;
        heading.AddComponent<LineRenderer>();
        heading.name = gameObject.name + " Heading";
        heading.tag = "heading";
        LineRenderer lr = heading.GetComponent<LineRenderer>();
        lr.material = GameObject.Find("Heading").GetComponent<LineRenderer>().material;
        lr.material.color = new Color(255f, 0, 0, 0);
        lr.startWidth = 5f;
        lr.endWidth = 0f;

        canvasWidth = canvas.transform.localScale.x;
        canvasHeight = canvas.transform.localScale.y;

        float startX = rnd.Next((int)(((canvasWidth/2) * -1) + 10), (int)((canvasWidth/2) - 10));
        float startY = rnd.Next((int)(((canvasHeight/2) * -1) + 10), (int)((canvasHeight/2) - 10));
        transform.position = new Vector3(startX, startY, 0);

        float dX = (((float)rnd.NextDouble()) - 0.5f) * 2 * droneSpeed;
        float dY = (((float)rnd.NextDouble()) - 0.5f) * 2 * droneSpeed;
        startMovement = capSpeed(new Vector2(dX, dY), droneSpeed);
        movement = startMovement;

        if (gameObject.name == "Main")
        {
            scanCircle.transform.localScale.Set(sepLength * 2, sepLength * 2, 1);
            groupCircle.transform.localScale.Set((alignLength * 2), (alignLength * 2), (1));
        }

        neighbours = findObjects("drone");
        for (int i = 0; i < neighbours.Count; i++)
        {
            neighbours[i].GetComponent<BoidScript>().addNeighbour(gameObject);
        }

        var temp = findObjects("obstacle");
        //print("in boid start, obstacles size: " + obstacles.Count + ", found " + temp.Count + " adding these now...");
        obstacles = temp;
    }
    // Update is called once per frame
    void Update()
    {
        Vector2 prevMove = capSpeed(movement, droneSpeed); // previous movement
        Vector2 goalUrge = capSpeed(goalPos - new Vector2(transform.position.x, transform.position.y), droneSpeed); // boid is urged towards the goal
        Vector2 sepMovement = capSpeed(separation2(), droneSpeed);
        Vector2 aliMovement = capSpeed(alignment(), droneSpeed);
        Vector2 cohMovement = capSpeed(cohesion(), droneSpeed);
        Vector2[] movements = new Vector2[] { scale(sepMovement, sepScale), scale(goalUrge, goalScale), scale(aliMovement, aliScale), scale(cohMovement, cohScale), scale(prevMove, prevMovScale) }; // combine rules by importance

        Vector2 temp = sumUpTo(movements, droneSpeed); // Work out suggested movement
        Vector2 obst = capSpeed(obstCalc(temp, 50, sepLength*2), droneSpeed); // Obstacle avoidance given the suggested movement
        Vector2[] test = new Vector2[] { scale(obst, obstScale), temp };
        Vector2 aaa = capSpeed(sumUpTo(test, droneSpeed)*accel, droneSpeed); // Recombine original suggestion with OA suggestion
        movement = aaa;


        updatePosition(); // Update boid position
        updateHeading(); // Update heading 'line'
        checkEdges(); // Check for out of bounds
        
    }
    void updatePosition()
    {
        Vector2 start = transform.position;
        Vector2 newPos = new Vector2(start.x + movement.x, start.y + movement.y);
        transform.position = newPos;

        if (gameObject.name == "Main")
        {
            scanCircle.transform.position = newPos;
            var test = scanCircle.transform.localScale;
            test.Set(sepLength * 2, sepLength * 2, 1);
            scanCircle.transform.localScale = test;

            groupCircle.transform.position = newPos;
            test.Set(alignLength * 2, alignLength * 2, 1);
            groupCircle.transform.localScale = test;
        }
    }
    void updateHeading()
    {
        Vector2 pos = gameObject.transform.position;
        LineRenderer lr = heading.GetComponent<LineRenderer>();
        lr.SetPosition(0, pos);
        float dx = movement.x;
        float dy = movement.y;
        float speed = Mathf.Sqrt((dx * dx) + (dy * dy));
        float mult = 10 / speed;
        float lineX = dx * mult;
        float lineY = dy * mult;
        lr.SetPosition(1, new Vector2(pos.x + (lineX * 1f), pos.y + (lineY * 1f)));
        heading.SetActive(true);
    }
    void checkEdges()
    {
        canvasWidth = canvas.transform.localScale.x;
        canvasHeight = canvas.transform.localScale.y;
        if (transform.position.x < (canvasWidth / 2) * -1)
        {
            transform.position = new Vector2((canvasWidth / 2), transform.position.y);
        }
        if (transform.position.x > (canvasWidth / 2))
        {
            transform.position = new Vector2((canvasWidth / 2) * -1, transform.position.y);
        }
        if (transform.position.y < (canvasHeight / 2) * -1)
        {
            transform.position = new Vector2(transform.position.x, (canvasHeight / 2));
        }
        if (transform.position.y > (canvasHeight / 2))
        {
            transform.position = new Vector2(transform.position.x, (canvasHeight / 2) * -1);
        }
    }
    Vector2 separation()
    {
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
            float dis = dist(transform.position.x, transform.position.y, sepLength, neighbours[i].transform.position.x, neighbours[i].transform.position.y, (neighbours[i].transform.localScale.x / 2));
            if (dis + sepLength - (neighbours[i].transform.localScale.x / 2) <= 0)
            {
                //gameObject.SetActive(false); // Crash!
                //scanCircle.SetActive(false);
            }
            if (dis <= 0) // if a neighbour is within the detection radius
            {

                if (gameObject.name == "Main")
                {
                    //sr.color = new Color32(0, 0, 0, 255); // if this object is the lead boid then highlight it when it is close to another boid
                }
                // find if neighbour is on the left or right of current trajectory
                float dy = movement.y - transform.position.y;
                float dx = movement.x - transform.position.x;
                float m = dy / dx;
                float c = transform.position.y - (m * transform.position.x);
                // y = mx + c
                float testX = (neighbours[i].transform.position.y - c) / m;
                if (movement.x >= 0)
                {
                    testX *= -1;
                }
                if (neighbours[i].transform.position.x >= testX)
                {
                    // point is on right side so steer to the left
                    float distance = dist(transform.position.x, transform.position.y, transform.localScale.x / 2,
                        neighbours[i].transform.position.x, neighbours[i].transform.position.y, neighbours[i].transform.localScale.x / 2);
                    float distPercent = 1 - (distance / sepLength);
                    float reMapped = distPercent * (2 * Mathf.PI);
                    float turnAngle = reMapped * (0.05f * Mathf.PI);
                    float newX = Mathf.Cos(turnAngle) * movement.x - Mathf.Sin(turnAngle) * movement.y;
                    float newY = Mathf.Sin(turnAngle) * movement.x + Mathf.Cos(turnAngle) * movement.y;
                    return new Vector2(newX, newY);

                }
                else if (neighbours[i].transform.position.x < testX)
                {
                    // point is on left side so steer to the right
                    float distance = dist(transform.position.x, transform.position.y, transform.localScale.x / 2, neighbours[i].transform.position.x, neighbours[i].transform.position.y, neighbours[i].transform.localScale.x / 2);
                    float distPercent = 1 - (distance / sepLength);
                    float reMapped = distPercent * (Mathf.PI);
                    float turnAngle = (2 * Mathf.PI) - (reMapped * (0.05f * Mathf.PI));
                    float newX = Mathf.Cos(turnAngle) * movement.x - Mathf.Sin(turnAngle) * movement.y;
                    float newY = Mathf.Sin(turnAngle) * movement.x + Mathf.Cos(turnAngle) * movement.y;
                    return new Vector2(newX, newY);
                }
            }
        }
        return new Vector2(0, 0);
    }
    Vector2 separation2()
    {
        SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();

        if (gameObject.name == "Main")
        {
            sr.color = new Color32(255, 0, 0, 255); // if this object is the lead boid then colour it red
        }
        else
        {
            sr.color = new Color32(255, 255, 255, 255); // otherwise colour it white
        }

        for (int i = 0; i < neighbours.Count; i++)
        {
            float dis = dist(transform.position.x, transform.position.y, sepLength, neighbours[i].transform.position.x, neighbours[i].transform.position.y, (neighbours[i].transform.localScale.x / 2));
            if (dis + sepLength - (neighbours[i].transform.localScale.x / 2) <= 0)
            {
                //gameObject.SetActive(false); // Crash!
                //scanCircle.SetActive(false);
            }
            if (dis <= 0) // if a neighbour is within the detection radius
            {
                if (gameObject.name == "Main")
                {
                    sr.color = new Color32(0, 0, 0, 255); // if this object is the lead boid then highlight it when it is close to another boid
                }
                GameObject neigh = neighbours[i];
                Vector2 meToNeigh = neigh.transform.position - gameObject.transform.position;
                Vector2 awayFromNeigh = new Vector2(meToNeigh.x*-1, meToNeigh.y*-1);
                return awayFromNeigh;
            }
        }
        return new Vector2(0, 0);
    }
    Vector2 alignment()
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
            float avgDX = totalDX / near.Count;
            float avgDY = totalDY / near.Count;

            Vector2 newMovement = new Vector2(avgDX, avgDY);

            return newMovement;
        } else
        {
            return new Vector2(0, 0);
        }
    }
    Vector2 cohesion()
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
            float totalX = gameObject.transform.position.x;
            float totalY = gameObject.transform.position.y;
            for (int i = 0; i < near.Count; i++)
            {
                totalX += near[i].transform.position.x;
                totalY += near[i].transform.position.y;
            }
            float avgX = totalX / near.Count;
            float avgY = totalY / near.Count;
            float newX = gameObject.transform.position.x - avgX;
            float newY = gameObject.transform.position.y - avgY;
            Vector2 newMovement = new Vector2(newX, newY);

            return newMovement;
        }
        return new Vector2(0, 0);
    }
    Vector2 obstCalc(Vector2 mov, int numRays, float rayLength)
    {
        obstacles = findObjects("obstacle");
        for (int i = 0; i < obstacles.Count; i++) // for each obstacle
        {
            LineRenderer lr = obstacles[i].GetComponent<LineRenderer>();
            Vector2 start = lr.GetPosition(0);
            Vector2 end = lr.GetPosition(lr.positionCount-1);
            Vector2 suggestion = castRays(start, end, rayLength, numRays, mov); // cast rays and test against each edge
            if (suggestion.magnitude > 0)
            {
                return suggestion;
            }

        }
        return new Vector2(0, 0); // otherwise return nothing
    }
    public Vector2 castRays(Vector2 lineA, Vector2 lineB, float rayLength, float numRays, Vector2 mov)
    {
        Vector2 moveNorm = normaliseSpeed(mov, rayLength); // get front ray
        Vector2 rayEnd = new Vector2(transform.position.x, transform.position.y) + moveNorm;

        List<GameObject> rays = new List<GameObject>();

        if (hits(transform.position, rayEnd, lineA, lineB)) // if front ray hits edge
        {
            // add back in to visualize the collision ray
            /*rayRenderer.SetPosition(0, transform.position);
            rayRenderer.SetPosition(1, rayEnd);
            ray.SetActive(true);*/
            float angle = (2 * Mathf.PI / numRays);
            int toggle = -1;
            for (int l = 1; l < numRays / 2; l++) // for each ray to be cast
            {
                if (toggle == 1)
                {
                    l--;
                }
                // cast ray in front +- angle*i
                Vector2 current = rotateVec(moveNorm, toggle * angle * l);

                /*GameObject r = GameObject.Instantiate(ray);
                r.transform.position = transform.position;
                r.AddComponent<LineRenderer>();
                LineRenderer rRenderer = r.GetComponent<LineRenderer>();
                rRenderer.startWidth = 1f;
                rRenderer.endWidth = 1f;
                rRenderer.SetPosition(0, transform.position);
                rRenderer.SetPosition(1, new Vector2(transform.position.x, transform.position.y) + current);
                r.name = gameObject.name + " collision ray";
                r.SetActive(true);
                rays.Add(r);*/

                if (hits(transform.position,new Vector2(transform.position.x, transform.position.y) +
                    normaliseSpeed(current, rayLength), lineA, lineB)) // if new ray hits then carry on
                {
                    toggle *= -1;
                }
                else // otherwise return the unobstructed ray as the new direction to travel
                {
                    for (int i = 0; i < rays.Count; i++)
                    {
                        //Destroy(rays[i]);
                    }
                    return normaliseSpeed(current, droneSpeed);
                }
            }
        }
        return new Vector2(0, 0); // no incoming collisions, so return nothing
    }
    public bool hits(Vector2 A, Vector2 B, Vector2 C, Vector2 D)
    {
        Vector2 E = new Vector2(B.x - A.x, B.y - A.y);
        Vector2 F = new Vector2(D.x - C.x, D.y - C.y);
        Vector2 P = new Vector2(E.y*-1, E.x);
        float h = dotProduct((A - C), P) / dotProduct(F, P);
        float g = dotProduct((A - C), F) / dotProduct(F, P);
        Vector2 hitPoint = C + (F * h);
        float upX = max(A.x, B.x);
        float upY = max(A.y, B.y);
        float lowX = min(A.x, B.x);
        float lowY = min(A.y, B.y);
        if (dotProduct(F, P) != 0 && 0 <= h && h <= 1 && lowX <= hitPoint.x && hitPoint.x <= upX && lowY <= hitPoint.y && hitPoint.y <= upY)
        {
            return true;
        } else
        {
            return false;
        }
    }
    public float max(float a, float b)
    {
        if (a > b)
        {
            return a;
        } else
        {
            return b;
        }
    }
    public float min(float a, float b)
    {
        if (a < b)
        {
            return a;
        }
        else
        {
            return b;
        }
    }
    public float dotProduct(Vector2 A, Vector2 B)
    {
        return ((A.x * B.x) + (A.y * B.y));
    }
    public static Vector2 rotateVec(Vector2 v, float radians)
    {
        var ca = Mathf.Cos(radians);
        var sa = Mathf.Sin(radians);
        return new Vector2(ca * v.x - sa * v.y, sa * v.x + ca * v.y);
    }
    Vector2 normaliseSpeed(Vector2 newSpeed, float speedWanted)
    {
        float newX = newSpeed.x;
        float newY = newSpeed.y;
        float speed = Mathf.Sqrt((newX * newX) + (newY * newY));
        float mult = speedWanted / speed;
        newX *= Mathf.Abs(mult);
        newY *= Mathf.Abs(mult);
        Vector2 newMovement = new Vector2(newX, newY);
        return newMovement;
    }
    Vector2 scale(Vector2 vect, float scale)
    {
        return new Vector2(vect.x * scale, vect.y * scale);
    }
    Vector2 sum(Vector2[] movements)
    {
        Vector2 total = new Vector2(0, 0);
        foreach(Vector2 m in movements)
        {
            total = total + m;
        }
        return total;
    }
    Vector2 sumUpTo(Vector2[] movements, float topSpeed)
    {
        Vector2 total = new Vector2(0, 0);
        float mag = 0;
        foreach (Vector2 m in movements)
        {
            mag = total.magnitude;
            if ((mag + m.magnitude) <= topSpeed)
            {
                total = total + m;
            } else
            {
                float left = topSpeed - mag;
                Vector2 scaled = capSpeed(m, left);
                total = total + scaled;
                return total;
            }
        }
        return total;
    }
    Vector2 capSpeed(Vector2 vect, float speed)
    {
        float mag = vect.magnitude;
        if (mag >= speed)
        {
            return scale(vect, (speed / mag));
        }
        return vect;
    }
    Vector2 avgMove(Vector2[] movements)
    {
        float dx = 0;
        float dy = 0;
        int count = 0;
        for (int i = 0; i < movements.Length; i++)
        {
            if (movements[i].magnitude > 0)
            {
                dx += movements[i].x;
                dy += movements[i].y;
                count++;
            }
        }
        float avgX = dx / count;
        float avgY = dy / count;
        return new Vector2(avgX, avgY);
    }
    float dist(float x1, float y1, float r1, float x2, float y2, float r2)
    {
        float dx = x1 - x2;
        float dy = y1 - y2;
        float d = Mathf.Sqrt((dx * dx) + (dy * dy));
        float final = d - r1 - r2;
        return final;
    }
    public List<GameObject> findObjects(string tag)
    {
        List<GameObject> outList = new List<GameObject>();
        GameObject[] list = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject check in list)
        {
            if (check.name != gameObject.name)
            {
                outList.Add(check);
            }
        }
        return outList;
    }
    public void addNeighbour(GameObject neigh)
    {
        neighbours.Add(neigh);
    }
    public static Vector2[] GetSpriteCorners(SpriteRenderer renderer)
    {
        Vector2 topRight = renderer.transform.TransformPoint(renderer.sprite.bounds.max);
        Vector2 topLeft = renderer.transform.TransformPoint(new Vector2(renderer.sprite.bounds.max.x, renderer.sprite.bounds.min.y));
        Vector2 botLeft = renderer.transform.TransformPoint(renderer.sprite.bounds.min);
        Vector2 botRight = renderer.transform.TransformPoint(new Vector2(renderer.sprite.bounds.min.x, renderer.sprite.bounds.max.y));
        return new Vector2[] { topRight, topLeft, botLeft, botRight };
    } // Credits to Dustin27, found at: https://answers.unity.com/questions/1451688/how-do-i-get-the-positions-of-the-corners-of-a-spr.htm
}
