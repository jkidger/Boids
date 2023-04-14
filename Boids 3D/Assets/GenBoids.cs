using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class GenBoids : MonoBehaviour
{
    public GameObject canvas;
    public float canvasWidth;
    public float canvasHeight;
    public float canvasDepth;

    public GameObject goal;

    public List<long> timeToGoal = new List<long>();

    public float sepLength = 10;
    public float alignLength = 20;
    public float cohesLength = 20;
    public float rayLength = 20;
    public float droneSpeed = 0.1f;

    public float accel;

    public Stopwatch timer = new Stopwatch();

    public List<GameObject> boids;

    public float obstScale = 1;
    public float sepScale = 0.25f;
    public float goalScale = 0;
    public float aliScale = 0.125f;
    public float cohScale = 0.125f;
    public float prevMovScale = 1;
    public float gravity;

    public int boidsToGenerate = 1;

    public int numCollisions = 0;

    public int numFibPoints = 1000;
    public List<Vector3> fibSphere;

    Material red;
    Material white;

    Camera[] cams;

    // Start is called before the first frame update
    void Start()
    {
        canvas = GameObject.Find("Canvas");
        canvasWidth = canvas.transform.localScale.x;
        canvasHeight = canvas.transform.localScale.y;
        canvasDepth = canvas.transform.localScale.z;
        boids = new List<GameObject>();


        /*goal = GameObject.Instantiate(GameObject.Find("DeadGoal"));
        SpriteRenderer gr = goal.GetComponent<SpriteRenderer>();
        gr.color = new Color32(0, 255, 0, 255);*/
        goal = GameObject.Find("Goal");
        goal.name = "Goal";
        goal.tag = "goal";
        goal.transform.position = new Vector2(0, 0);
        goal.AddComponent<GoalScript>();
        goal.GetComponent<GoalScript>().canvas = canvas;
        goal.GetComponent<GoalScript>().randoPos();

        red = (Material)Resources.Load("Red", typeof(Material));
        white = (Material)Resources.Load("White", typeof(Material));

        cams = Camera.allCameras;


        fibSphere = fibonacciSphere(numFibPoints); // Generate points around a sphere for ray-casting
        for (int i = 0; i < fibSphere.Count; i++)
        {
            fibSphere[i] = (fibSphere[i] * rayLength); // Transform points to be rayLength away from origin
        }


        // add back in for auto run
        /*boidsToGenerate = 20;
        generateBoids();*/

    }
    // Update is called once per frame
    void Update()
    {
        canvas.transform.localScale = new Vector3(canvasWidth, canvasHeight, canvasDepth);

        goal = GameObject.Find("Goal");
        var goalScript = goal.GetComponent<GoalScript>();
        if (goalScript.numCollisions > numCollisions)
        {
            numCollisions = goalScript.numCollisions;
            long time = timer.ElapsedMilliseconds;
            //UnityEngine.Debug.Log("Time to Goal: " + time + "ms");
            timeToGoal.Add(time);
            timer.Reset();
            timer.Start();
        }
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
                boidScript.rayLength = rayLength;
                boidScript.droneSpeed = droneSpeed;

                boidScript.accel = accel;

                boidScript.obstScale = obstScale;
                boidScript.sepScale = sepScale;
                boidScript.goalScale = goalScale;
                boidScript.aliScale = aliScale;
                boidScript.cohScale = cohScale;
                boidScript.prevMovScale = prevMovScale;
                boidScript.gravity = gravity;
            // Param assignment
        }

        // Camera switching
        for (int i = 1; i < cams.Length; i++)
        {
            if (Input.GetKeyDown("[" + i + "]"))
            {
                print(cams[i].name);
                foreach (Camera cam in cams)
                {
                    cam.enabled = false;
                }
                cams[i].enabled = true;
            }
        }
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
    public void generateBoids()
    {
        timer.Start();
        int genNum = boidsToGenerate;
        //deleteAllBoids();
        UnityEngine.Debug.Log("Generating " + genNum + " Boids...");
        if (GameObject.Find("Main"))
        {
            UnityEngine.Debug.Log("Main Boid already exists!");
            genNum++;
        }
        else
        {
            UnityEngine.Debug.Log("Generating main Boid...");
            createBoid("Main", "Drone_Red");
        }
        for (int i = 0; i < genNum - 1; i++)
        {
            createBoid(("Gen" + (boids.Count - 1)), "Drone_White");
        }
        goal.GetComponent<GoalScript>().findBoids();
    }
    public void deleteAllBoids()
    {
        goal.GetComponent<GoalScript>().boids.Clear();
        /*GameObject[] allRadii = GameObject.FindGameObjectsWithTag("radius");
        GameObject[] allHeadings = GameObject.FindGameObjectsWithTag("heading");*/
        int num = boids.Count;
        foreach (GameObject b in boids)
        {
            Destroy(b);
        }
        /*foreach (GameObject r in allRadii)
        {
            Destroy(r);
        }
        foreach (GameObject h in allHeadings)
        {
            Destroy(h);
        }*/
        UnityEngine.Debug.Log("Deleted " + num + " Boids");
        boids.Clear();
    }
    public void createBoid(string name, string modelName)
    {
        GameObject newDrone = GameObject.Instantiate(GameObject.Find(modelName)); // Create new boid from template
        newDrone.name = name;
        //newDrone.GetComponent<Renderer>().material = boidColour;
        newDrone.tag = "drone";
        newDrone.AddComponent<BoidScript>(); // Assign script to boid
        var boidScript = newDrone.GetComponent<BoidScript>();

        if (name == "Main") // If main boid, then create radii
        {
            for (int i = 1; i < cams.Length-1; i++)
            {
                cams[i].GetComponent<CameraScript>().mainBoid = newDrone;
            }
            cams[cams.Length - 1].GetComponent<BoidCameraScript>().mainBoid = newDrone;
        }

        boidScript.canvas = canvas;
        // Param assignment
            boidScript.sepLength = sepLength;
            boidScript.alignLength = alignLength;
            boidScript.cohesLength = cohesLength;
            boidScript.rayLength = rayLength;
            boidScript.droneSpeed = droneSpeed;

            boidScript.accel = accel;

            boidScript.obstScale = obstScale;
            boidScript.sepScale = sepScale;
            boidScript.goalScale = goalScale;
            boidScript.aliScale = aliScale;
            boidScript.cohScale = cohScale;
            boidScript.prevMovScale = prevMovScale;
            boidScript.gravity = gravity;

            boidScript.rayEnds = fibSphere;
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
        /*GameObject canvas = GameObject.Find("Canvas");
        var script = canvas.GetComponent<CanvasScript>();
        foreach (GameObject line in script.lines)
        {
            Destroy(line);
        }
        script.lines.Clear();*/
    }
    public List<Vector3> fibonacciSphere(int samples)
    {
        float[] indices = new float[samples];
        List<Vector3> points = new List<Vector3>();
        for (int i = 0; i < samples; i++)
        {
            indices[i] = i + 0.5f;
            float phi = Mathf.Acos(1 - (2 * indices[i] / samples));
            float theta = Mathf.PI * (1 + Mathf.Pow(5, 0.5f)) * indices[i];
            float x = Mathf.Cos(theta) * Mathf.Sin(phi);
            float y = Mathf.Sin(theta) * Mathf.Sin(phi);
            float z = Mathf.Cos(phi);
            points.Add(new Vector3(x, y, z));
        }
        return points;
    }
    public void randoGoal()
    {
        goal.transform.position = goal.GetComponent<GoalScript>().randoPos();
    }
}
