using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Game.Interface;
using Game.Extensions;

namespace Game.Controllers
{
    public class SelectionController : MonoBehaviour
    {
        private IRayProvider RayProvider;
        private ITransformProvider TransformProvider;

        private ISelectable Selectable;
        private IDraggable Draggable;
        //private ISoundable Soundable;
        private IRotatable Rotatable; 
        private Transform Target;

        void Awake()
        {
            RayProvider = GetComponent<IRayProvider>();
            TransformProvider = GetComponent<ITransformProvider>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0) && !ExtensionMethods.IsPointerOverUIObject())
            {
                CheckSelection();
            }
            if (Input.GetMouseButton(0))
            {
                var Pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (Draggable != null)
                {
                    Draggable.OnDragging(new Vector2(Pos.x, Pos.y));
                }
                if (Rotatable != null)
                {
                    Rotatable.OnRotating(new Vector2(Pos.x, Pos.y));
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                if (Draggable != null)
                {
                    Draggable.OnDragEnd();
                }
                if (Selectable != null)
                {
                    Selectable.OnDeSelect();
                }
                //if (Soundable != null)
                //{
                //    Soundable.StopToolSelectSound();
                //    Soundable.PlayToolDeSelectSound();
                //}
                if (Rotatable != null)
                {
                    Rotatable.OnRotateEnd();
                }
                Target = null;
                Draggable = null;
                Selectable = null;
                //Soundable = null;
                Rotatable = null;
            }
        }

        void CheckSelection()
        {
            var Ray = RayProvider.CreateRay();
            var Transform = TransformProvider.Check(Ray);

            if (Transform != null)
            {
                Selectable = Transform.GetComponent<ISelectable>();
                if (Selectable != null)
                {
                    Selectable.OnSelect();
                }

                Draggable = Transform.GetComponent<IDraggable>();
                if (Draggable != null)
                {
                    var Pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Draggable.OnDragStart(new Vector2(Pos.x, Pos.y));
                    Target = Transform;
                }

                //Soundable = Transform.GetComponent<ISoundable>();
                //if (Soundable != null)
                //{
                //    Soundable.PlayToolSelectSound();
                //}

                Rotatable = Transform.GetComponent<IRotatable>();
                if(Rotatable != null)
                {
                    var Pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Rotatable.OnRotateStart(new Vector2(Pos.x, Pos.y));
                }
            }
        }
        
    }
}