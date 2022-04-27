using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public static class Tools
{
    //Extension method that create bounds with all children from a specific parent
    public static Bounds GetBoundsWithChildren(this GameObject gameObject)
    {

        Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
        Bounds bounds = renderers.Length > 0 ? renderers[0].bounds : new Bounds();

        for (int i = 1; i < renderers.Length; i++)
        {
            if (renderers[i].enabled)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }
        }

        return bounds;
    }

    //Extension method that calculate correct zoom distance from bounds and camera frustrumView
    public static Vector3 FocusDistance(this Camera camera, GameObject focusedObject)
    {
        Bounds bounds = focusedObject.GetBoundsWithChildren();
        
        #if UNITY_EDITOR
        DrawBounds(bounds,1f);
        #endif

        float objectSize = bounds.extents.magnitude;
        float distance = objectSize / Mathf.Sin(Mathf.Deg2Rad * camera.fieldOfView * 0.5f);

        if(camera.orthographic)
            camera.orthographicSize = bounds.max.magnitude;

        camera.nearClipPlane = distance - objectSize;
        camera.farClipPlane = distance + objectSize;
        return bounds.center - distance * camera.transform.forward;
    }

    //helper for drawing custom bounds for an amount of time
    private static void DrawBounds(Bounds b, float delay = 0)
    {
        // bottom
        var p1 = new Vector3(b.min.x, b.min.y, b.min.z);
        var p2 = new Vector3(b.max.x, b.min.y, b.min.z);
        var p3 = new Vector3(b.max.x, b.min.y, b.max.z);
        var p4 = new Vector3(b.min.x, b.min.y, b.max.z);

        Debug.DrawLine(p1, p2, Color.blue, delay);
        Debug.DrawLine(p2, p3, Color.red, delay);
        Debug.DrawLine(p3, p4, Color.yellow, delay);
        Debug.DrawLine(p4, p1, Color.magenta, delay);

        // top
        var p5 = new Vector3(b.min.x, b.max.y, b.min.z);
        var p6 = new Vector3(b.max.x, b.max.y, b.min.z);
        var p7 = new Vector3(b.max.x, b.max.y, b.max.z);
        var p8 = new Vector3(b.min.x, b.max.y, b.max.z);

        Debug.DrawLine(p5, p6, Color.blue, delay);
        Debug.DrawLine(p6, p7, Color.red, delay);
        Debug.DrawLine(p7, p8, Color.yellow, delay);
        Debug.DrawLine(p8, p5, Color.magenta, delay);

        // sides
        Debug.DrawLine(p1, p5, Color.white, delay);
        Debug.DrawLine(p2, p6, Color.gray, delay);
        Debug.DrawLine(p3, p7, Color.green, delay);
        Debug.DrawLine(p4, p8, Color.cyan, delay);
    }
}