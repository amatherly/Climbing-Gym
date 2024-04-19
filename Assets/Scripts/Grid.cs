using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField]
    private float size = 1f;  // Size of each grid cell

    public int width = 10;    // Width of the grid (number of cells in X direction)
    public int height = 10;   // Height of the grid (number of cells in Z direction)
    public Color gizmoColor = Color.green;  // Color of the grid points in the scene view

    

    public Vector3 GetNearestPointOnGrid(Vector3 position)
    {
        position -= transform.position;  // Adjusting to grid's local space

        int xCount = Mathf.RoundToInt(position.x / size);
        int yCount = Mathf.RoundToInt(position.y / size);
        int zCount = Mathf.RoundToInt(position.z / size);

        Vector3 result = new Vector3(
            xCount * size,
            yCount * size,
            zCount * size
        );

        result += transform.position;  // Convert back to world space

        return result;
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        for (float x = 0; x < width * size; x += size)
        {
            for (float z = 0; z < height * size; z += size)
            {
                var point = GetNearestPointOnGrid(new Vector3(x, 0f, z));
                Gizmos.DrawSphere(point, 0.1f);
            }
        }
    }
    
}