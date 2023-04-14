using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BoidScript : MonoBehaviour
{
    public GameObject canvas;
    float canvasWidth;
    float canvasHeight;
    float canvasDepth;

    public float sepLength;
    public float alignLength;
    public float cohesLength;
    public float rayLength;
    public float droneSpeed = 1;

    public float accel;

    public float obstScale;
    public float sepScale;
    public float goalScale;
    public float aliScale;
    public float cohScale;
    public float prevMovScale;
    public float gravity;

    public List<GameObject> neighbours = new List<GameObject>();
    public GameObject scanCircle;
    public GameObject groupCircle;
    System.Random rnd = new System.Random();
    Vector3 startMovement;
    GameObject heading;
    public Vector3 movement;
    public Vector3 goalPos;

    public int numRays;
    public List<Vector3> rayEnds;

    public float speed;


    // Start is called before the first frame update
    void Start()
    {
        canvas = GameObject.Find("Canvas");
        canvasWidth = canvas.transform.localScale.x;
        canvasHeight = canvas.transform.localScale.y;
        canvasDepth = canvas.transform.localScale.z;

        float startX = rnd.Next((int)((canvasWidth / 2) * -1), (int)(canvasWidth / 2));
        float startY = rnd.Next((int)((canvasHeight / 2) * -1), (int)(canvasHeight / 2));
        float startZ = rnd.Next((int)((canvasDepth / 2) * -1), (int)(canvasDepth / 2));
        transform.position = new Vector3(startX, startY, startZ);

        /*float dX = rnd.Next(-(int)droneSpeed * 100, (int)droneSpeed * 100)/100f;
        float dY = rnd.Next(-(int)droneSpeed * 100, (int)droneSpeed * 100)/100f;
        float dZ = rnd.Next(-(int)droneSpeed * 100, (int)droneSpeed * 100)/100f;*/
        float dX = (((float)rnd.NextDouble())-0.5f) * 2  * droneSpeed;
        float dY = (((float)rnd.NextDouble()) - 0.5f) * 2 * droneSpeed;
        float dZ = (((float)rnd.NextDouble()) - 0.5f) * 2 * droneSpeed;
        startMovement = capSpeed(new Vector3(dX, dY, dZ), droneSpeed);
        movement = startMovement;

        rayEnds = rotateAll(rayEnds, new Vector3(0, 0, rayLength), movement); // Rotate fibonacci sphere points to align with suggested direction

        neighbours = findObjects("drone");
        for (int i = 0; i < neighbours.Count; i++)
        {
            neighbours[i].GetComponent<BoidScript>().addNeighbour(gameObject);
        }
    }
    // Update is called once per frame
    void Update()
    {
        speed = movement.magnitude;
        Vector3 prevMove = capSpeed(movement, droneSpeed); // previous movement
        Vector3 goalUrge = capSpeed(goalPos - gameObject.transform.position, droneSpeed); // boid is urged towards the goal
        Vector3 sepMovement = capSpeed(separation2(), droneSpeed);
        Vector3 aliMovement = capSpeed(alignment(), droneSpeed);
        Vector3 cohMovement = capSpeed(cohesion(), droneSpeed);
        Vector3[] movements = new Vector3[] { scale(sepMovement, sepScale), scale(goalUrge, goalScale), scale(aliMovement, aliScale), scale(cohMovement, cohScale), scale(prevMove, prevMovScale) }; // combine rules by importance
        
        Vector3 combinedMovements = sumUpTo(movements, droneSpeed); // Work out suggested movement
        //combinedMovements += (new Vector3(0, -1, 0)) * gravity;

        Vector3 obst = capSpeed(castRays(combinedMovements), droneSpeed); // Obstacle avoidance given the suggested movement

        combinedMovements = sumUpTo(new Vector3[] { scale(obst, obstScale), combinedMovements }, droneSpeed); // Recombine original suggestion with OA suggestion
        combinedMovements *= accel; // Add acceleration
        combinedMovements = capSpeed(combinedMovements, droneSpeed);

        updateRotation(prevMove, combinedMovements);

        movement = combinedMovements; // Set movement from the combination of all suggestions

        updatePosition(); // Update boid position
        checkEdges(); // Check for out of bounds
    }
    void updateRotation(Vector3 oldDir, Vector3 newDir)
    {
        //Quaternion rotation = Quaternion.FromToRotation(new Vector3(oldDir.x, 0, oldDir.z), new Vector3(newDir.x, 0, newDir.z));
        //transform.rotation = rotation * transform.rotation;
        transform.LookAt(transform.position + newDir);
        Vector3 rot = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(0, rot.y, 0);
    }
    void updatePosition()
    {
        Vector3 start = transform.position;
        Vector3 newPos = new Vector3(start.x + movement.x, (start.y + movement.y)-gravity, start.z + movement.z);
        transform.position = newPos;
    }
    void checkEdges()
    {
        canvasWidth = canvas.transform.localScale.x;
        canvasHeight = canvas.transform.localScale.y;
        canvasDepth = canvas.transform.localScale.z;
        if (transform.position.x < (canvasWidth / 2) * -1)
        {
            transform.position = new Vector3((canvasWidth / 2), transform.position.y, transform.position.z);
        }
        if (transform.position.x > (canvasWidth / 2))
        {
            transform.position = new Vector3((canvasWidth / 2) * -1, transform.position.y, transform.position.z);
        }
        if (transform.position.y < (canvasHeight / 2) * -1)
        {
            transform.position = new Vector3(transform.position.x, (canvasHeight / 2), transform.position.z);
        }
        if (transform.position.y > (canvasHeight / 2))
        {
            transform.position = new Vector3(transform.position.x, (canvasHeight / 2) * -1, transform.position.z);
        }
        if (transform.position.z < (canvasDepth / 2) * -1)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, (canvasDepth / 2));
        }
        if (transform.position.z > (canvasDepth / 2))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, (canvasDepth / 2) * -1);
        }
    }
    /*Vector3 separation()
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
                    //print("Green flip");
                    testX *= -1;
                }
                if (neighbours[i].transform.position.x >= testX)
                {
                    // point is on right side so steer to the left
                    float distance = dist(transform.position.x, transform.position.y, transform.localScale.x / 2, neighbours[i].transform.position.x, neighbours[i].transform.position.y, neighbours[i].transform.localScale.x / 2);
                    float distPercent = 1 - (distance / sepLength);
                    float reMapped = distPercent * (2 * Mathf.PI);
                    float turnAngle = reMapped * (0.05f * Mathf.PI);
                    float newX = Mathf.Cos(turnAngle) * movement.x - Mathf.Sin(turnAngle) * movement.y;
                    float newY = Mathf.Sin(turnAngle) * movement.x + Mathf.Cos(turnAngle) * movement.y;
                    return new Vector3(newX, newY);

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
                    return new Vector3(newX, newY);
                }
            }
        }
        return new Vector3(0, 0);
    }*/
    Vector3 separation2()
    {

        for (int i = 0; i < neighbours.Count; i++)
        {
            float dis = dist(transform.position, sepLength, neighbours[i].transform.position, (neighbours[i].transform.localScale.x / 2));
            if (dis + sepLength - (neighbours[i].transform.localScale.x / 2) <= 0)
            {
                //gameObject.SetActive(false); // Crash!
                //scanCircle.SetActive(false);
            }
            if (dis <= 0) // if a neighbour is within the detection radius
            {
                GameObject neigh = neighbours[i];
                Vector3 meToNeigh = neigh.transform.position - gameObject.transform.position;
                Vector3 awayFromNeigh = meToNeigh * -1;
                return awayFromNeigh;
            }
        }
        return new Vector3(0, 0, 0);
    }
    Vector3 alignment()
    {
        List<GameObject> near = new List<GameObject>();
        near.Clear();
        for (int i = 0; i < neighbours.Count; i++)
        {
            float dis = dist(transform.position, alignLength, neighbours[i].transform.position, (neighbours[i].transform.localScale.x / 2));
            if (dis <= 0) // if a neighbour is within the detection radius
            {
                near.Add(neighbours[i]);
            }
        }
        if (near.Count > 0)
        {
            float totalDX = movement.x;
            float totalDY = movement.y;
            float totalDZ = movement.z;
            for (int i = 0; i < near.Count; i++)
            {
                Vector3 neighbourMovement = near[i].GetComponent<BoidScript>().movement;
                totalDX += neighbourMovement.x;
                totalDY += neighbourMovement.y;
                totalDZ += neighbourMovement.z;
            }
            Vector3 avg = new Vector3(totalDX, totalDY, totalDZ) / near.Count;

            Vector3 newMovement = avg;

            return newMovement;
        }
        else
        {
            return new Vector3(0, 0, 0);
        }
    }
    Vector3 cohesion()
    {
        List<GameObject> near = new List<GameObject>();
        for (int i = 0; i < neighbours.Count; i++)
        {
            float dis = dist(transform.position, cohesLength, neighbours[i].transform.position, (neighbours[i].transform.localScale.x / 2));
            if (dis <= 0) // if a neighbour is within the detection radius
            {
                near.Add(neighbours[i]);
            }
        }
        if (near.Count > 0)
        {
            float totalX = gameObject.transform.position.x;
            float totalY = gameObject.transform.position.y;
            float totalZ = gameObject.transform.position.z;
            for (int i = 0; i < near.Count; i++)
            {
                totalX += near[i].transform.position.x;
                totalY += near[i].transform.position.y;
                totalZ += near[i].transform.position.z;
            }
            Vector3 avg = new Vector3(totalX, totalY, totalZ) / near.Count;
            Vector3 newMovement = gameObject.transform.position - avg;

            return newMovement;
        }
        return new Vector3(0, 0, 0);
    }
    public Vector3 castRays(Vector3 mov)
    {
        rayEnds = rotateAll(rayEnds, rayEnds[0], mov); // Rotate fibonacci sphere points to align with suggested direction

        // Cast ray in suggested direction of movement
        bool hits = Physics.Raycast(transform.position, mov, rayLength); // Check if ray collides with an obstacle

        if (hits) // if front ray collides with an obstacle
        {
            foreach (Vector3 ray in rayEnds) // for each ray to be cast
            {
                hits = Physics.Raycast(transform.position, ray, rayLength);
                if (!hits) // if ray does not hit the obstacle then return that ray
                {
                    return normaliseSpeed(ray, droneSpeed); // Could play with this - always returns movement at max speed
                }
            }
        }
        return new Vector3(0, 0, 0); // no incoming collisions, so return nothing
    }
    public bool hits(Vector3 A, Vector3 B, GameObject obstacle)
    {
        // TODO - line + obstacle intersection
        return false;
    }
    public float max(float a, float b)
    {
        if (a > b)
        {
            return a;
        }
        else
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
    public float dotProduct(Vector3 A, Vector3 B)
    {
        return ((A.x * B.x) + (A.y * B.y));
    }
    public static Vector3 rotateVec(Vector3 v, float radians)
    {
        var ca = Mathf.Cos(radians);
        var sa = Mathf.Sin(radians);
        return new Vector3(ca * v.x - sa * v.y, sa * v.x + ca * v.y);
    }
    Vector3 normaliseSpeed(Vector3 newSpeed, float speedWanted)
    {
        float speed = newSpeed.magnitude;
        float mult = speedWanted / speed;
        Vector3 newMovement = newSpeed * mult; //abs?
        return newMovement;
    }
    Vector3 scale(Vector3 vect, float scale)
    {
        return vect * scale;
    }
    Vector3 sum(Vector3[] movements)
    {
        Vector3 total = new Vector3(0, 0);
        foreach (Vector3 m in movements)
        {
            total = total + m;
        }
        return total;
    }
    Vector3 sumUpTo(Vector3[] movements, float topSpeed)
    {
        Vector3 total = new Vector3(0, 0, 0);
        float mag = 0;
        foreach (Vector3 m in movements)
        {
            mag = total.magnitude;
            if ((mag + m.magnitude) <= topSpeed)
            {
                total = total + m;
            }
            else
            {
                float left = topSpeed - mag;
                Vector3 scaled = capSpeed(m, left);
                total = total + scaled;
                return total;
            }
        }
        return total;
    }
    Vector3 capSpeed(Vector3 vect, float speed)
    {
        float mag = vect.magnitude;
        if (mag >= speed)
        {
            return scale(vect, (speed / mag));
        }
        return vect;
    }
    Vector3 avgMove(Vector3[] movements)
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
        return new Vector3(avgX, avgY);
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

    List<Vector3> rotateAll(List<Vector3> points, Vector3 fromDir, Vector3 toDir)
    {
        // Work out rotation angle from fromDir to toDir
        Quaternion rotation = Quaternion.FromToRotation(fromDir, toDir);

        // Loop through all points and rotate them by that angle
        for (int i = 0; i < points.Count; i++)
        {
            points[i] = rotation * points[i];
        }

        return points;
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
}
