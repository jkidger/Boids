using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleScript : MonoBehaviour
{
    public class ray
    {
        public GameObject go { get; set; }
        public Vector2 start { get; set; }
        public Vector2 end { get; set; }
        public float length { get; set; }
        public float angle { get; set; }
        public ray(GameObject go, Vector2 start, Vector2 end, float length, float angle)
        {
            this.go = go;
            this.start = start;
            this.end = end;
        }
    }

    static int numRays = 32;
    static float rayLength = 10;
    static float rayWidth = 0.1f;
    static float droneSpeed = -0.005f;
    ray[] rays = new ray[numRays];
    GameObject collisionCircle;
    // Start is called before the first frame update
    void Start()
    {
        collisionCircle = GameObject.Find("collision");
        //print("test: " + (collisionCircle == null));
        //GameObject circle = new GameObject();
        //circle.transform.position = new Vector2(50, 0);
        //circle.AddComponent<LineRenderer>();
        //LineRenderer cr = circle.GetComponent<LineRenderer>();


        float subAngle = 2*Mathf.PI / numRays;

        for (int i = 0; i < numRays; i++)
        {
            Vector2 start = transform.position;
            GameObject line = new GameObject();
            line.transform.position = start;
            line.AddComponent<LineRenderer>();
            LineRenderer lrr = line.GetComponent<LineRenderer>();
            lrr.startWidth = rayWidth;
            lrr.endWidth = rayWidth;
            lrr.SetPosition(0, start);
            float angle = i * subAngle;
            Vector2 e = rayEnd(start, rayLength, angle);
            lrr.SetPosition(1, e);
            rays[i] = new ray(line, start, e, rayLength, angle);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 movement = new Vector2(droneSpeed, 0);
        movement = checkRays(movement);
        updateMovement(movement);
        
    }

    Vector2 checkRays(Vector2 movement)
    {
        gameObject.GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 255);
        foreach (ray r in rays)
        {
            r.go.SetActive(false);
            Vector2 end = r.end;
            if (pDistance(collisionCircle.transform.position.x, collisionCircle.transform.position.y, r.start.x, r.start.y, r.end.x, r.end.y)<=(collisionCircle.transform.localScale.x/2))
            {
                //print("going to collide!");
                gameObject.GetComponent<Renderer>().material.color = new Color32(0, 0, 0, 255);
                r.go.SetActive(true);
                /*for (int i = 1; i < rays.Length; i++)
                {
                    ray ra = rays[i];
                    Vector2 e = ra.end;
                    if (pDistance(collisionCircle.transform.position.x, collisionCircle.transform.position.y, ra.start.x, ra.start.y, ra.end.x, ra.end.y) > (collisionCircle.transform.localScale.x / 2))
                    {
                        //r.go.SetActive(true);
                        float dx = ra.end.x - ra.start.x;
                        float dy = ra.end.y - ra.start.y;
                        float speed = Mathf.Sqrt((dx * dx) + (dy * dy));
                        float scale = droneSpeed / speed;
                        Vector2 newCourse = new Vector2(dx*scale, dy*scale);
                        return newCourse;
                    }
                }*/
            }

        }
        return movement;

    }

    float pDistance(float x, float y, float x1, float y1, float x2, float y2)
    {

        float A = x - x1;
        float B = y - y1;
        float C = x2 - x1;
        float D = y2 - y1;

        float dot = A * C + B * D;
        float len_sq = C * C + D * D;
        float param = -1;
        if (len_sq != 0) //in case of 0 length line
            param = dot / len_sq;

        float xx;
        float yy;

        if (param < 0)
        {
            xx = x1;
            yy = y1;
        }
        else if (param > 1)
        {
            xx = x2;
            yy = y2;
        }
        else
        {
            xx = x1 + param * C;
            yy = y1 + param * D;
        }

        var dx = x - xx;
        var dy = y - yy;
        return Mathf.Sqrt(dx * dx + dy * dy);
    }

    bool inCircle(Vector2 point, GameObject circle)
    {
        float pointX = point.x;
        float pointY = point.y;
        float circX = circle.transform.position.x;
        float circY = circle.transform.position.y;
        //print("circX " + circX);
        float dx = circX - pointX;
        float dy = circY - pointY;
        float dist = (Mathf.Sqrt((dx*dx)+(dy*dy)));
        //print("circle scale: " + circle.transform.localScale.x);
        if (-1*dist >= (circle.transform.localScale.x / 2) || dist <= (circle.transform.localScale.x/2))
        {
            return true;
        }
        return false;
    }

    void updateMovement(Vector2 movement)
    {
        Vector2 start = transform.position;
        Vector2 newPos = new Vector2(start.x + movement.x, start.y + movement.y);
        transform.position = newPos;
        start = transform.position;
        foreach (ray r in rays)
        {
            LineRenderer lr = r.go.GetComponent<LineRenderer>();
            Vector2 newEnd = new Vector2(r.end.x + movement.x, r.end.y + movement.y);
            lr.SetPosition(0, start);
            lr.SetPosition(1, newEnd);
            r.start = start;
            r.end = newEnd;
        }
    }

    Vector2 rayEnd(Vector2 startPos, float length, float angle)
    {
        //GameObject ray = new GameObject();
        //ray.transform.position = startPos;
        //ray.AddComponent<LineRenderer>();
        //LineRenderer lr = ray.GetComponent<LineRenderer>();
        //lr.SetWidth(width, width);
        //lr.SetPosition(0, startPos);
        float x = startPos.x + (length * Mathf.Cos(angle));
        //print("x length: " + (length * Mathf.Cos(angle)));
        float y = startPos.y + (length * Mathf.Sin(angle));
        //print("y length: " + (length * Mathf.Sin(angle)));
        //print("h length: " + Mathf.Sqrt((x * x) + (y * y)));
        Vector2 end = new Vector2(x, y);
        //lr.SetPosition(1, end);
        //return ray;
        return end;
    }
}
