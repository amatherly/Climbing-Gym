using UnityEngine;

public class CubeSideGrid : MonoBehaviour
{
    public GameObject spherePrefab;  // Reference to the sphere prefab
    public LayerMask meshLayer;      // Layer on which the mesh resides
    public float gridSpacing = 0.5f; // Spacing between grid points
    public int raysPerRow = 20;      // Number of rays per row
    public int numberOfRows = 20;    // Number of rows of rays

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        Vector3 boundsMin = GetComponent<Renderer>().bounds.min;
        Vector3 boundsMax = GetComponent<Renderer>().bounds.max;

        for (int i = 0; i < numberOfRows; i++)
        {
            for (int j = 0; j < raysPerRow; j++)
            {
                Vector3 rayStart = new Vector3(
                    Mathf.Lerp(boundsMin.x, boundsMax.x, (float)j / (raysPerRow - 1)),
                    boundsMax.y + 1.0f,  // Start a bit above the object
                    Mathf.Lerp(boundsMin.z, boundsMax.z, (float)i / (numberOfRows - 1))
                );

                if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, 2.0f, meshLayer))
                {
                    Instantiate(spherePrefab, hit.point, Quaternion.identity, transform);
                }
            }
        }
    }
}