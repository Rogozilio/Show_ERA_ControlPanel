using System;
using System.Collections.Generic;
using UnityEngine;


public class GizmoDrawer : MonoBehaviour
{
    public static GizmoDrawer Instance { get; private set; }

    private List<Vector3> points1 = new List<Vector3>();
    private List<Vector3> points2 = new List<Vector3>();
    private List<Color> colors = new List<Color>();
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void ClearDrawLine()
    {
        points1.Clear();
        points2.Clear();
        colors.Clear();
    }
    public void AddDrawLine(Vector3 start, Vector3 end, Color color)
    {
        points1.Add(start);
        points2.Add(end);
        colors.Add(color);
    }

    private void OnDrawGizmos()
    {
        for (var i = 0; i < points1.Count; i++)
        {
            Gizmos.color = colors[i];
            Gizmos.DrawLine(points1[i], points2[i]);
        }

    }
}