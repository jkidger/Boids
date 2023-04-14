using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class GenBoids : MonoBehaviour
{
    GameObject goal;

    public List<long> timeToGoal = new List<long>();
    long goalStartTime;

    public float percentFlocked;

    public long timeToAllFlock;
    long flockStartTime;

    public int goalCollisions = 0;

    public float flockMaxDist;

    GameObject canvas;
    public float canvasWidth;
    public float canvasHeight;

    public float sepLength = 10;
    public float alignLength = 20;
    public float cohesLength = 20;
    public float droneSpeed = 0.1f;

    public float accel;

    Stopwatch timer = new Stopwatch();

    List<GameObject> boids;

    public float obstScale = 1;
    public float sepScale = 0.25f;
    public float goalScale = 0;
    public float aliScale = 0.125f;
    public float cohScale = 0.125f;
    public float prevMovScale = 1;

    public int boidsToGenerate = 1;

    GameObject flockCentre;


    Color32 red = new Color32(255, 0, 0, 255);
    Color32 white = new Color32(255, 255, 255, 255);
    Color32 gray = new Color32(255, 255, 255, 30);
    Color32 lightGray = new Color32(255, 255, 255, 20);
    Color32 transparent = new Color32(255, 255, 255, 0);
    // Start is called before the first frame update
    void Start()
    {
        canvas = GameObject.Find("Canvas");
        canvasWidth = canvas.transform.localScale.x;
        canvasHeight = canvas.transform.localScale.y;
        boids = new List<GameObject>();

        goal = GameObject.Instantiate(GameObject.Find("DeadGoal"));
        SpriteRenderer gr = goal.GetComponent<SpriteRenderer>();
        gr.color = new Color32(0, 255, 0, 255);
        goal.name = "Goal";
        goal.tag = "goal";
        goal.transform.position = new Vector2(0, 0);
        goal.AddComponent<GoalScript>();
        goal.GetComponent<GoalScript>().canvas = canvas;
        goal.GetComponent<GoalScript>().randoPos();

        flockCentre = GameObject.Find("Flock Circle");


        // add back in for auto run
        /*boidsToGenerate = 50;
        generateBoids();*/
    }
    // Update is called once per frame
    void Update()
    {
        canvas.transform.localScale = new Vector3(canvasWidth, canvasHeight, 1f);

        getMetrics(); // Get current metrics for testing
        
        foreach (GameObject boid in boids)
        {
            var boidScript = boid.GetComponent<BoidScript>();
            // Param assignment
                if (goal != null)
                {
                    boidScript.goalPos = goal.transform.position;
                }

                boidScript.sepLength = sepLength;
                boidScript.alignLength = alignLength;
                boidScript.cohesLength = cohesLength;
                boidScript.droneSpeed = droneSpeed;

                boidScript.accel = accel;

                boidScript.obstScale = obstScale;
                boidScript.sepScale = sepScale;
                boidScript.goalScale = goalScale;
                boidScript.aliScale = aliScale;
                boidScript.cohScale = cohScale;
                boidScript.prevMovScale = prevMovScale;
            // Param assignment
        }
    }
    public void generateBoids()
    {
        timer.Start();
        goalStartTime = timer.ElapsedMilliseconds;


        int genNum = boidsToGenerate;

        //UnityEngine.Debug.Log("Generating " + genNum + " Boids...");
        if (GameObject.Find("Main"))
        {
            //UnityEngine.Debug.Log("Main Boid already exists!");
            genNum++;
        } else
        {
            //UnityEngine.Debug.Log("Generating main Boid...");
            createBoid("Main", red, gray, lightGray);
        }
        for (int i = 0; i < genNum-1; i++)
        {
            createBoid(("Gen" + (boids.Count-1)), white, transparent, transparent);
        }
        goal.GetComponent<GoalScript>().findBoids();
    }
    public void deleteAllBoids()
    {
        goal.GetComponent<GoalScript>().boids.Clear();
        GameObject[] allRadii = GameObject.FindGameObjectsWithTag("radius");
        GameObject[] allHeadings = GameObject.FindGameObjectsWithTag("heading");
        int num = boids.Count;
        foreach (GameObject b in boids)
        {
            Destroy(b);
        }
        foreach (GameObject r in allRadii)
        {
            Destroy(r);
        }
        foreach (GameObject h in allHeadings)
        {
            Destroy(h);
        }
        UnityEngine.Debug.Log("Deleted " + num + " Boids");
        boids.Clear();
    }
    public void createBoid(string name, Color32 boidColour, Color32 sepColour, Color32 groupColour)
    {
        GameObject newDrone = GameObject.Instantiate(GameObject.Find("Dead")); // Create new boid from template
        SpriteRenderer sr = newDrone.GetComponent<SpriteRenderer>();
        newDrone.name = name;
        sr.color = boidColour;
        newDrone.tag = "drone";
        newDrone.AddComponent<BoidScript>(); // Assign script to boid
        var boidScript = newDrone.GetComponent<BoidScript>();

        if (name == "Main") // If main boid, then create radii
        {
            GameObject newDroneRadius = GameObject.Instantiate(GameObject.Find("DeadRadius"));
            SpriteRenderer rsr = newDroneRadius.GetComponent<SpriteRenderer>();
            rsr.color = sepColour;
            newDroneRadius.tag = "radius";
            newDroneRadius.name = newDrone.name + "Radius";
            boidScript.scanCircle = newDroneRadius;

            GameObject newDroneGroupRadius = GameObject.Instantiate(GameObject.Find("DeadGroupRadius"));
            SpriteRenderer grsr = newDroneGroupRadius.GetComponent<SpriteRenderer>();
            grsr.color = groupColour;
            newDroneGroupRadius.tag = "radius";
            newDroneGroupRadius.name = newDrone.name + "GroupRadius";
            boidScript.groupCircle = newDroneGroupRadius;
        }

        
        
        
        boidScript.canvas = canvas;
        // Param assignment
            boidScript.sepLength = sepLength;
            boidScript.alignLength = alignLength;
            boidScript.cohesLength = cohesLength;
            boidScript.droneSpeed = droneSpeed;

            boidScript.accel = accel;

            boidScript.obstScale = obstScale;
            boidScript.sepScale = sepScale;
            boidScript.goalScale = goalScale;
            boidScript.aliScale = aliScale;
            boidScript.cohScale = cohScale;
            boidScript.prevMovScale = prevMovScale;
        // Param assignment

        newDrone.GetComponent<BoidScript>().goalPos = goal.GetComponent<GoalScript>().transform.position; // Tell boid where the goal is

        foreach (GameObject b in boids) // Tell all boids to search for neighbours
        {
            b.GetComponent<BoidScript>().findObjects("drone");
        }
        boids.Add(newDrone);
    }
    public void deleteAllObstacles()
    {
        GameObject canvas = GameObject.Find("Canvas");
        var script = canvas.GetComponent<CanvasScript>();
        foreach (GameObject line in script.lines)
        {
            Destroy(line);
        }
        script.lines.Clear();
    }
    public void getMetrics()
    {
        long time = timer.ElapsedMilliseconds;

        // Flock centre visualization
        flockCentre.transform.position = avgPos();
        flockCentre.transform.localScale = new Vector2(flockMaxDist, flockMaxDist);

        // Time to goal metric
        if (goalScale == 0)
        {
            goalStartTime = time;
        } else
        {
            goal = GameObject.Find("Goal");
            var goalScript = goal.GetComponent<GoalScript>();
            if (goalScript.numCollisions > goalCollisions)
            {
                goalCollisions = goalScript.numCollisions;
                
                //UnityEngine.Debug.Log("Time to Goal: " + time + "ms");
                timeToGoal.Add(time - goalStartTime);
                goalStartTime = time;
            }
        }

        // Percent flocked metric
        percentFlocked = calcPercentFlocked();

        // Time to flock metric
        if (timeToAllFlock == 0)
        {
            if (percentFlocked == 1)
            {
                timeToAllFlock = time - flockStartTime;
            }
        }
    }
    public float calcPercentFlocked()
    {
        float numFlocked = 0;
        foreach(GameObject boid in boids)
        {
            float dist = (boid.transform.position - flockCentre.transform.position).magnitude;
            if (dist < (flockMaxDist/2))
            {
                numFlocked++;
            }
        }
        numFlocked /= boids.Count;
        return numFlocked;
    }
    public bool isFlocked()
    {
        float max = 0;
        for (int i = 0; i < boids.Count-1; i++)
        {
            for (int j = i+1; j < boids.Count; j++)
            {
                float dist = (boids[j].transform.position - boids[i].transform.position).magnitude;
                if (dist > max)
                {
                    max = dist;
                    if (max > flockMaxDist)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }
    public Vector3 avgPos()
    {
        Vector3 centreOfMass = new Vector3(0, 0, 0);
        foreach (GameObject boid in boids)
        {
            centreOfMass += boid.transform.position;
        }
        if (boids.Count > 0)
        {
            centreOfMass /= boids.Count;
        }
        return centreOfMass;
    }
    public void resetMetrics()
    {
        long time = timer.ElapsedMilliseconds;

        timeToGoal.Clear();
        goalStartTime = time;

        timeToAllFlock = 0;
        flockStartTime = time;

        percentFlocked = 0;
    }
}
