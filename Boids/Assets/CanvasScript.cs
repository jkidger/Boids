using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasScript : MonoBehaviour
{

    public List<GameObject> lines;
    public List<GameObject> edges;
    bool toggle;
    Vector2 start;
    float x;
    float y;
    Vector2 topLeft;
    Vector2 topRight;
    Vector2 botLeft;
    Vector2 botRight;
    // Start is called before the first frame update
    void Start()
    {
        lines = new List<GameObject>();
        toggle = false;

        // Create Edges
        x = gameObject.transform.localScale.x / 2;
        y = gameObject.transform.localScale.y / 2;
        topLeft = new Vector2(x * -1, y);
        topRight = new Vector2(x, y);
        botLeft = new Vector2(x * -1, y * -1);
        botRight = new Vector2(x, y * -1);
        edges.Add(createObstacle(topLeft, topRight, "Obstacle" + edges.Count));
        edges.Add(createObstacle(topRight, botRight, "Obstacle" + edges.Count));
        edges.Add(createObstacle(botRight, botLeft, "Obstacle" + edges.Count));
        edges.Add(createObstacle(botLeft, topLeft, "Obstacle" + edges.Count));
    }
    private void Update()
    {
        // Update Edges
        x = gameObject.transform.localScale.x / 2;
        y = gameObject.transform.localScale.y / 2;
        topLeft = new Vector2(x * -1, y);
        topRight = new Vector2(x, y);
        botLeft = new Vector2(x * -1, y * -1);
        botRight = new Vector2(x, y * -1);
        edges[0].GetComponent<LineRenderer>().SetPosition(0, topLeft);
        edges[0].GetComponent<LineRenderer>().SetPosition(1, topRight);
        edges[1].GetComponent<LineRenderer>().SetPosition(0, topRight);
        edges[1].GetComponent<LineRenderer>().SetPosition(1, botRight);
        edges[2].GetComponent<LineRenderer>().SetPosition(0, botRight);
        edges[2].GetComponent<LineRenderer>().SetPosition(1, botLeft);
        edges[3].GetComponent<LineRenderer>().SetPosition(0, botLeft);
        edges[3].GetComponent<LineRenderer>().SetPosition(1, topLeft);
    }
    void OnMouseDown()
    {
        if (!toggle)
        {
            start = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            toggle = true;
        } else
        {
            Vector2 end = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            lines.Add(createObstacle(start, end, "Obstacle" + (lines.Count + 4)));
            start = Vector2.zero;
            toggle = false;
        }
    }
    public GameObject createObstacle(Vector2 start, Vector2 end, string name)
    {
        GameObject line = new GameObject();
        line.transform.position = start;
        line.AddComponent<LineRenderer>();
        line.name = name;
        line.tag = "obstacle";
        LineRenderer lr = line.GetComponent<LineRenderer>();
        lr.material = GameObject.Find("Heading").GetComponent<LineRenderer>().material;
        lr.material.color = new Color(255f, 0, 0, 0);
        lr.startWidth = 1f;
        lr.endWidth = 1f;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        GameObject[] boids = GameObject.FindGameObjectsWithTag("drone");
        foreach (GameObject boid in boids)
        {
            var script = boid.GetComponent<BoidScript>();
            script.obstacles = script.findObjects("obstacle");
        }
        return line;
    }
}
