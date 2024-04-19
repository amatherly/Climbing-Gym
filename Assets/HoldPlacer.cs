using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldPlacer : MonoBehaviour
{
    [SerializeField]
    private Grid grid; // The grid system for placement
    [SerializeField]
    private GameObject holdPrefab; // The prefab for the climbing hold

    public GameObject HoldPrefab
    {
        get => holdPrefab;
        set
        {
            if (holdPrefab != value)
            {
                holdPrefab = value;
                if (previewHold != null)
                {
                    UpdatePreviewHold();
                }
            }
        }
    }

    private void UpdatePreviewHold()
    {
        // Destroy the current preview hold if it exists
        if (previewHold != null)
        {
            Destroy(previewHold);
        }

        // Cast a ray from the camera to the mouse position to find the new location
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 nearestPoint = grid.GetNearestPointOnGrid(hit.point);
            // Instantiate a new preview hold with the new prefab
            previewHold = Instantiate(holdPrefab, nearestPoint, Quaternion.identity);
            SetPreviewMode(previewHold, true);
            UpdateRotation(hit.normal); // Call UpdateRotation to set the correct rotation
        }
    }


    private GameObject previewHold = null; // Temporary hold for preview
    private float currentYRotation = 0f; // Store the current Y-axis rotation to adjust based on mouse scroll
    private GameObject selectedHold = null; // To store the currently selected hold



    void Start()
    {
        grid = FindObjectOfType<Grid>(); // Ensure the grid component is found and set correctly
    }
    
 
    void Update()
    {
        UpdatePreviewHold();
        if (Input.GetMouseButtonDown(0) && previewHold != null)
        {
            ConfirmPlacement();
        }

        // Check for right mouse button press
        if (Input.GetMouseButtonDown(1))
        {
            SelectHold();
        }
    
        // Check for right mouse button hold
        if (Input.GetMouseButton(1) && selectedHold != null)
        {
            MoveSelectedHold();
        }

        // Check for right mouse button release
        if (Input.GetMouseButtonUp(1) && selectedHold != null)
        {
            ReleaseSelectedHold();
        }
    }

    private void SelectHold()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            // Check if the hit object is a climbing hold
            if (hit.transform.CompareTag("Hold")) // Make sure your holds have this tag
            {
                selectedHold = hit.transform.gameObject;
                // Optionally, make some visual change to indicate selection
            }
        }
    }

    private void MoveSelectedHold()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 nearestPoint = grid.GetNearestPointOnGrid(hit.point);
            selectedHold.transform.position = nearestPoint;
            // You might want to update the rotation or other properties here as well
        }
    }

    private void ReleaseSelectedHold()
    {
        // Finalize the position of the selected hold and clear the reference
        selectedHold = null;
        // Optionally, revert any visual changes made to indicate selection
    }
    
    
    
    

    private void UpdateRotation(Vector3 normal)
    {
        float baseXRotation = holdPrefab.transform.eulerAngles.x; // Get the base X-axis rotation of the hold prefab
        float baseZRotation = holdPrefab.transform.eulerAngles.z; // Get the base Z-axis rotation of the hold prefab
        float baseYRotation = holdPrefab.transform.eulerAngles.y; // Get the base Y-axis rotation of the hold prefab
        float rotationAmount = Input.GetAxis("Mouse ScrollWheel") * 100f; // Determine rotation change based on scroll wheel movement
        currentYRotation += rotationAmount;

        if (previewHold != null)
        {
            Quaternion normalRotation = Quaternion.LookRotation(-normal, Vector3.up); // Set rotation to align with the surface normal
            Quaternion yRotation = Quaternion.Euler(0, currentYRotation, 0);
            previewHold.transform.rotation = normalRotation * Quaternion.Euler(baseXRotation, baseYRotation, baseZRotation) * yRotation; // Combine initial offset and dynamic rotation
        }
    }

    private void ConfirmPlacement()
    {
        SetPreviewMode(previewHold, false); // Finalize placement by making the material opaque
        previewHold = null; // Reset the preview object for the next placement
        currentYRotation = 0f; // Reset the rotation
    }

    private void SetPreviewMode(GameObject obj, bool isPreview)
    {
        var renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, isPreview ? 0.5f : 1.0f); // Adjust transparency based on preview mode
        }
    }
    
    public void ChangeHoldPrefab(GameObject newPrefab)
    {
        HoldPrefab = newPrefab;
    }
}
