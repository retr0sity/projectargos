using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EdgeColliderCreatorComponent : MonoBehaviour
{
    public GameObject Target;

    public List<Transform> Reference;
    private EdgeCollider2D Component;
    private Vector2 FirstPoint = new Vector2();
    private Vector2 LastPoint =new Vector2();

    [ContextMenu("Apply")]
    public void Apply()
    {
        Reference = new List<Transform>();
        Reference.Clear();

        Component = Target.GetComponent<EdgeCollider2D>();
        if (Component == null)
        {
            Component = Target.AddComponent<EdgeCollider2D>();
        }

        var ChildCount = Target.transform.childCount;
        var Points = new List<Vector2>();
        for (int i = 0; i < ChildCount; i++)
        {
            var Child = Target.transform.GetChild(i);
            if (Child.gameObject.activeInHierarchy)
            {
                Reference.Add(Child);
                Points.Add(new Vector2(Child.localPosition.x, Child.localPosition.y));
            }
        }
        Component.SetPoints(Points);
    }
    public void Start()
    {
        Component = Target.GetComponent<EdgeCollider2D>();
        FirstPoint = Component.points.FirstOrDefault();
        LastPoint = Component.points.LastOrDefault();
    }

    public void Update()
    {
        UpdateCollider();
    }

    private void UpdateCollider()
    {
        List<Vector2> Points = new List<Vector2>();
        Points.Add(FirstPoint);
        for (int i = 1; i < Reference.Count - 1; i++)
        {
            var Vector = new Vector2(Reference[i].localPosition.x, Reference[i].localPosition.y);
            Points.Add(Vector);

        }
        Points.Add(LastPoint);
        Component.SetPoints(Points);
    }
}
