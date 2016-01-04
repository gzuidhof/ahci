using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Painter : MonoBehaviour {

    private List<Vector3> linePoints;
    private LineRenderer lineRenderer;
    private float newPointDelta = 0.02f;



    // Use this for initialization
    void Start () {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
        lineRenderer.SetVertexCount(0);
        lineRenderer.SetWidth(0.1f, 0.1f);
        lineRenderer.SetColors(Color.red, Color.red);
        lineRenderer.useWorldSpace = true;
        linePoints = new List<Vector3>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}



    public void addPoint(Vector3 point)
    {
        // Using named temp variables like this helps me think more clearly about the code
        Vector3 previousPoint = (linePoints.Count > 0) ? linePoints[linePoints.Count - 1] : new Vector3(-1000, -1000, -1000); // If you've never seen this before, it's called a ternary expression.
                                                                                                                              // It's just an if/else collapsed into a single line of code. 
                                                                                                                              // Also, the crazy out of bounds initial value here ensures the starting point will always draw.

        if (Vector3.Distance(point, previousPoint) > newPointDelta)
        {
            linePoints.Add(point);
            lineRenderer.SetVertexCount(linePoints.Count);
            lineRenderer.SetPosition(linePoints.Count - 1, (Vector3)linePoints[linePoints.Count - 1]);
        }
    }

    public void removeLine()
    {
        linePoints.Clear();
    }
}
