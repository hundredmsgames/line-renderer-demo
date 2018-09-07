using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DrawLine : MonoBehaviour
{
    private Camera mainCamera;
    public GameObject squarePrefab;
    public GameObject circlePrefab;

    private List<GameObject> lineGoList;
    private List<GameObject> circleGoList;
    private List<Vector2> points;

    private float lineWidth = 0.2f;
    private bool resetLine;

    private void Start()
    {
        mainCamera = Camera.main;
        points = new List<Vector2>();
        lineGoList = new List<GameObject>();
        circleGoList = new List<GameObject>();
    }

    private void Update()
    {
        //// If mouse over UI Gameobject, don't draw anything.
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            resetLine = true;
        }

        if (Input.GetMouseButton(0))
            Draw();
    }

    private void Draw()
    {
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        if (points.Count > 0 && resetLine == false)
        {
            mousePosition = GetProperEndPoint(points[points.Count - 1], mousePosition);
        }

        if (!points.Contains(mousePosition))
        {
            points.Add(mousePosition);

            // Draw line.
            if(points.Count > 1 && resetLine == false)
            {
                Vector2 p1 = points[points.Count - 2];
                Vector2 p2 = points[points.Count - 1];
                float distance = Vector2.Distance(p1, p2);
                float angle = GetAngle(p1, p2);

                GameObject line = Instantiate(squarePrefab, p1, Quaternion.identity);
                line.name = "line_" + (points.Count + 1);
                line.tag = "DrawLine";
                line.transform.SetParent(this.transform);
                line.transform.Rotate(0, 0, angle);
                line.transform.localScale = new Vector3(distance, lineWidth, 0);
                line.GetComponent<SpriteRenderer>().sortingLayerName = "Line";
                lineGoList.Add(line);
            }

            // Put a circle end of the line.
            GameObject circle = Instantiate(circlePrefab, points[points.Count - 1], Quaternion.identity);
            circle.name = "circle_" + (circleGoList.Count + 1);
            circle.tag = "DrawLine";
            circle.transform.SetParent(this.transform);
            circle.transform.localScale = new Vector3(lineWidth, lineWidth, 0);
            circle.GetComponent<SpriteRenderer>().sortingLayerName = "Line";
            circleGoList.Add(circle);

            resetLine = false;
        }
    }

    private float GetAngle(Vector2 origin, Vector2 other)
    {
        return Mathf.Atan2(other.y - origin.y, other.x - origin.x) * Mathf.Rad2Deg;
    }


    private Vector2 GetProperEndPoint(Vector2 start, Vector2 end)
    {
        RaycastHit2D[] hits = Physics2D.LinecastAll(start, end);
        float lineRadius = lineWidth / 2;

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.tag != "DrawLine")
            {
                end = hit.point + hit.normal * lineRadius;
                break;
            }
        }

        Collider2D[] cols = Physics2D.OverlapCircleAll(end, lineRadius);
        Collider2D hitCollider = null;
        foreach (Collider2D col in cols)
        {
            if (col.tag != "DrawLine")
            {
                hitCollider = col;
                break;
            }
        }

        if (hitCollider != null)
        {
            RaycastHit2D closest = new RaycastHit2D();
            closest.distance = float.MaxValue;

            RaycastHit2D hit = new RaycastHit2D();
            hits = Physics2D.RaycastAll(end, Vector2.up, lineRadius);
            if (IsThereAnyHit(hits, ref hit) == true && hit.distance < closest.distance)
                closest = hit;

            hits = Physics2D.RaycastAll(end, Vector2.right, lineRadius);
            if (IsThereAnyHit(hits, ref hit) == true && hit.distance < closest.distance)
                closest = hit;

            hits = Physics2D.RaycastAll(end, Vector2.down, lineRadius);
            if (IsThereAnyHit(hits, ref hit) == true && hit.distance < closest.distance)
                closest = hit;

            hits = Physics2D.RaycastAll(end, Vector2.left, lineRadius);
            if (IsThereAnyHit(hits, ref hit) == true && hit.distance < closest.distance)
                closest = hit;

            hits = Physics2D.RaycastAll(end, Vector2.up + Vector2.right, lineRadius);
            if (IsThereAnyHit(hits, ref hit) == true && hit.distance < closest.distance)
                closest = hit;

            hits = Physics2D.RaycastAll(end, Vector2.down + Vector2.right, lineRadius);
            if (IsThereAnyHit(hits, ref hit) == true && hit.distance < closest.distance)
                closest = hit;

            hits = Physics2D.RaycastAll(end, Vector2.up + Vector2.left, lineRadius);
            if (IsThereAnyHit(hits, ref hit) == true && hit.distance < closest.distance)
                closest = hit;

            hits = Physics2D.RaycastAll(end, Vector2.down + Vector2.left, lineRadius);
            if (IsThereAnyHit(hits, ref hit) == true && hit.distance < closest.distance)
                closest = hit;

            // If our rays found the collision.
            if (closest.distance != float.MaxValue)
            {
                end = closest.point + closest.normal * lineRadius;
            }
        }



        return end;
    }

    private bool IsThereAnyHit(RaycastHit2D[] hits, ref RaycastHit2D hitRef)
    {
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.tag != "DrawLine")
            {
                hitRef = hit;
                return true;
            }
        }

        return false;
    }
}