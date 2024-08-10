using UnityEngine;


namespace DiceBreakerUtility
{
    public static class Utilty
    { 
        private static float screenHalfWidth, screenHalfHeight;
        public static float GetScreenHalfWidth()
        {
            screenHalfWidth = Camera.main.aspect * Camera.main.orthographicSize;
            return screenHalfWidth;
        }
        public static float GetScreenHalfHeight()
        {
            screenHalfHeight = (1 / Camera.main.aspect) * screenHalfWidth;
            return screenHalfHeight;
        }

        public static Vector2 ReflectVector(Vector2 dir, Collision2D collision)
        {
            Vector2 inNormal = collision.contacts[0].normal;
            Debug.Log($"Normal : {inNormal}");

            Vector2 refVec = Vector2.Reflect(dir, inNormal);
            Debug.Log($"RefVec = {refVec}");
            return refVec;
        }

    }
}
