using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInteraction : MonoBehaviour
{
    [SerializeField]
    private TileManager tileManager;

    private Vector3 inputPosition;
    private Vector3 oldInputPosition;

    private Tile selectedTile;
    Vector2Int gridPosition;
    Vector2Int destinationGridPosition;
    bool isRolling = false;
    // The point around which the tile is rolling
    private Vector3 pivot;
    // The vector from the pivot point to the rolling tile 
    private Vector3 pivotToTile;
    // The length in degrees of the route. This should be -90 or 90.
    private float routeLength;
    // The amount of progress made along the route, from 0 to 1.
    private float routeProgress;
    // The rotation of the tile at the start of the movement
    private Quaternion intialRotation;

    private Vector2Int[] directionVectors = { Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down };


    public void StartInput(Vector3 position)
    {
        inputPosition = tileManager.transform.InverseTransformPoint(position);

        gridPosition = new Vector2Int(Mathf.RoundToInt(inputPosition.x), Mathf.RoundToInt(inputPosition.y));
        selectedTile = tileManager.GetTileAtPosition(gridPosition);
    }


    public void ContinueInput(Vector3 position)
    {
        if (selectedTile == null) return;

        oldInputPosition = inputPosition;
        inputPosition = tileManager.transform.InverseTransformPoint(position);

        if ((inputPosition - oldInputPosition).magnitude == 0) return;

        if (!isRolling)
        {
            // The route that is chosen will be the one that the last input gave the highest progress for
            float maxRouteProgess = 0;

            // For each for the 4 directions
            for (int i = 0; i < 4; i++)
            {
                foreach (int j in new int[] { -1, 1 })
                {
                    if (
                        // At least one of the other tiles around the pivot is solid
                        (IsSolid(gridPosition + directionVectors[(i + 3) % 4] * j) ||
                        IsSolid(gridPosition + directionVectors[i] + directionVectors[(i + 3) % 4] * j)) &&
                        // All the tiles that could obstruct the roll are empty
                        !IsSolid(gridPosition + directionVectors[i]) &&
                        !IsSolid(gridPosition + directionVectors[(i + 1) % 4] * j) &&
                        !IsSolid(gridPosition + directionVectors[i] + directionVectors[(i + 1) % 4] * j))
                    {
                        Vector3 newPivot = gridPosition + (Vector2)(directionVectors[i] + directionVectors[(i + 3) % 4] * j) / 2;
                        float angle = Vector2.SignedAngle(oldInputPosition - newPivot, inputPosition - newPivot);
                        //float progress = angle / (-90 * j);
                        float progress = Vector3.Dot((Vector2)directionVectors[i], inputPosition- oldInputPosition);
                        if (progress > maxRouteProgess)
                        {
                            maxRouteProgess = progress;
                            pivot = newPivot;
                            pivotToTile = (Vector3)(Vector2)gridPosition - pivot;
                            routeLength = -90 * j;
                            routeProgress = 0;
                            isRolling = true;
                            intialRotation = selectedTile.transform.localRotation;
                            destinationGridPosition = gridPosition + directionVectors[i];
                        }
                    }
                }
            }
        }

        if (isRolling)
        {
            Debug.DrawLine(tileManager.transform.TransformPoint(pivot), tileManager.transform.TransformPoint(oldInputPosition));
            Debug.DrawLine(tileManager.transform.TransformPoint(pivot), tileManager.transform.TransformPoint(inputPosition), Color.red);
            Debug.DrawLine(
                tileManager.transform.TransformPoint(pivot + pivotToTile),
                tileManager.transform.TransformPoint(pivot + Quaternion.Euler(0, 0, routeLength) * pivotToTile),
                Color.yellow);


            float angle = Vector2.SignedAngle(oldInputPosition - pivot, inputPosition - pivot);
            //float angle = Vector2.SignedAngle(selectedTile.transform.localPosition - pivot, selectedTile.transform.localPosition + inputPosition - oldInputPosition - pivot);
            routeProgress += angle / routeLength;

            //routeProgress = Vector2.SignedAngle(pivotToTile, inputPosition- pivot) / routeLength;

            //routeProgress += Vector3.Dot((Vector2)(destinationGridPosition - gridPosition), inputPosition - oldInputPosition);

            //routeProgress += Vector3.Dot((Vector2)(destinationGridPosition - gridPosition), inputPosition - oldInputPosition);

            //float f = 2 * Vector3.Dot((Vector2)(destinationGridPosition - gridPosition), inputPosition - oldInputPosition);
            //f = Mathf.Clamp(f, -(inputPosition - oldInputPosition).magnitude, (inputPosition - oldInputPosition).magnitude);
            //routeProgress += f;

            if (routeProgress >= 1 || routeProgress <= 0)
            {
                routeProgress = Mathf.Clamp01(routeProgress);
                isRolling = false;
                if (routeProgress == 1)
                {
                    MoveTileToDestination();
                }
            }
            UpdateTileTransform();
        }
    }


    public void EndInput()
    {
        if (selectedTile == null) return;
        if (!isRolling) return;

        routeProgress = Mathf.Round(routeProgress);
        isRolling = false;
        if (routeProgress == 1)
        {
            MoveTileToDestination();
        }
        UpdateTileTransform();
        selectedTile = null;
    }


    private bool IsSolid(Vector2Int position)
    {
        return tileManager.GetTileAtPosition(position) != null;
    }



    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(tileManager.transform.TransformPoint(inputPosition), 0.1f);
    }


    private void UpdateTileTransform()
    {
        selectedTile.transform.localPosition = pivot + Quaternion.Euler(0, 0, routeLength * routeProgress) * pivotToTile;
        selectedTile.transform.localRotation = intialRotation * Quaternion.Euler(0, 0, routeLength * routeProgress);
    }


    private void MoveTileToDestination()
    {
        tileManager.ClearArrayValue(gridPosition);
        tileManager.InsertIntoArray(selectedTile, destinationGridPosition);
        gridPosition = destinationGridPosition;
    }
}
