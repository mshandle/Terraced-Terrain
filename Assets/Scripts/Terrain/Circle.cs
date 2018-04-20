using UnityEngine;
using System;
namespace SimTerrain
{
    
    public class Circle
    {
        public Vector2 center;

        public float radio;

        public Circle(Vector2 point,float _radio)
        {
            center = point;
            radio = _radio;
        }

        static public bool ColliderRect(Circle circle, Rect rect)
        {
            
            if(rect.Contains(circle.center))
            {
                return true;
            }

            Vector2 nearPoint = new Vector2();


            if(circle.center.x > (rect.xMin + rect.width))
            {
                nearPoint.x = rect.xMax;
            }
            else if(circle.center.x < rect.xMin)
            {
                nearPoint.x = rect.xMin;
            }
            else
            {
                nearPoint.x = circle.center.x;
            }

            if (circle.center.y > (rect.yMin + rect.height))
            {
                nearPoint.y = rect.yMax;
            }
            else if (circle.center.y < rect.yMin)
            {
                nearPoint.y = rect.yMin;
            }
            else{
                nearPoint.y = circle.center.y;
            }

            if (Vector2.Distance(circle.center, nearPoint) <= circle.radio)
                return true;

            return false;
        }
        
    }
}
