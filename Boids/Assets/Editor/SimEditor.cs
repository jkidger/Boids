using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GenBoids))]
public class NewBehaviourScript : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GenBoids genBoids = (GenBoids)target;

        GUILayout.BeginHorizontal();
            if (GUILayout.Button("Delete all Boids"))
            {
                genBoids.deleteAllBoids();
            }
            if (GUILayout.Button("Generate Boids"))
            {
                genBoids.generateBoids();
            }
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Delete all Obstacles"))
        {
            genBoids.deleteAllObstacles();
        }
        if (GUILayout.Button("Reset Metrics"))
        {
            genBoids.resetMetrics();
        }

    }
}
