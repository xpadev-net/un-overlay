using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace lib
{
    public static class Raycast
    {
        public static List<GameObject> Get(Canvas canvas, Camera eventCamera, Ray ray)
        {
            if (!canvas.enabled)
            {
                return null;
            }

            if (!canvas.gameObject.activeInHierarchy)
            {
                return null;
            }

            IList<Graphic> graphics = GraphicRegistry.GetGraphicsForCanvas(canvas);
            List<GameObject> result = new List<GameObject>();
            for (int i = 0; i < graphics.Count; i++)
            {
                Graphic graphic = graphics[i];

                if (graphic.depth == -1 || !graphic.raycastTarget)
                {
                    continue;
                }

                Transform graphicTransform = graphic.transform;
                Vector3 graphicForward = graphicTransform.forward;

                float dir = Vector3.Dot(graphicForward, ray.direction);

                // Return immediately if direction is negative.
                if (dir <= 0)
                {
                    continue;
                }

                float distance = Vector3.Dot(graphicForward, graphicTransform.position - ray.origin) / dir;

                Vector3 position = ray.GetPoint(distance);
                Vector2 pointerPosition = eventCamera.WorldToScreenPoint(position);
                var rect = graphic.rectTransform;
                // To continue if the graphic doesn't include the point.
                if (!RectTransformUtility.RectangleContainsScreenPoint(rect, pointerPosition, eventCamera))
                {
                    continue;
                }

                // To continue if graphic raycast has failed.
                if (!graphic.Raycast(pointerPosition, eventCamera))
                {
                    continue;
                }
                result.Add(graphic.gameObject);
                //Debug.Log($"Raycast hit at {graphic.name}", graphic.gameObject);
            }

            return result;
        }
    }
}