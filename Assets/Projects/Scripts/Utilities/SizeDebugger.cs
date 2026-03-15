using UnityEngine;

public class SizeDebugger : MonoBehaviour
{
    private void Start()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        Bounds totalBounds = renderers[0].bounds;

        foreach (Renderer r in renderers)
            totalBounds.Encapsulate(r.bounds);

        Debug.Log("Width (X): " + totalBounds.size.x);
        Debug.Log("Height (Y): " + totalBounds.size.y);
        Debug.Log("Depth (Z): " + totalBounds.size.z);
    }
}