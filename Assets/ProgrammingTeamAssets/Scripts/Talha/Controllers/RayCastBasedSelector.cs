using UnityEngine;
using Game.Interface;
using System.Linq;

public class RayCastBasedSelector : MonoBehaviour,ITransformProvider
{
    public Transform Check(Ray Ray)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(Ray.origin, Ray.direction);
        if (hits.Length > 0)
        {
            var hit = hits.FirstOrDefault(x => x.transform.GetComponent<IDraggable>() != null 
            || x.transform.GetComponent<ISelectable>() != null 
            || x.transform.GetComponent<IRotatable>() != null);
            return hit.transform;
        }
        else
        {
            return null;
        }
    }
}
