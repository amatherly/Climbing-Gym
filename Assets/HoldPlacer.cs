using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldPlacer : MonoBehaviour
{
    
    private Grid grid;
    public GameObject holdPrefab;
    // Start is called before the first frame update
    void Start()
    {
        grid = FindObjectOfType<Grid>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                PlaceHold(hit.point);
            }
        }
    }

    private void PlaceHold(Vector3 point)
    {
        var finalPosition = grid.GetNearestPointOnGrid(point);
        // GameObject.CreatePrimitive(PrimitiveType.Cube).transform.position = finalPosition;
        Instantiate(holdPrefab, finalPosition, Quaternion.identity);
    }
    
}
