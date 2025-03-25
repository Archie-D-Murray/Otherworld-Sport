using System;

using UnityEngine;

namespace Utilities {
    public class Helpers : Singleton<Helpers> {

        public readonly Vector2 Offset = new Vector2(0.5f, 0.5f);

        protected override void Awake() {
            base.Awake();
            MainCamera = Camera.main;
        }
        public Camera MainCamera;
        private Vector2 _mousePosition;
        public Vector2 TileMapMousePosition => Vector2Int.FloorToInt(_mousePosition) + Offset;
        public static Vector2 FromRadians(float radians) {
            return new Vector2(Mathf.Sin(radians), Mathf.Cos(radians));
        }

        public float AngleToMouse(Transform obj) {
            return Vector2.SignedAngle(
                Vector2.up,
                ((Vector2)MainCamera.ScreenToWorldPoint(Input.mousePosition) - (Vector2)obj.position).normalized
            );
        }

        public float AngleToMouseOpposite(Transform obj) {
            return Vector2.SignedAngle(
                ((Vector2)MainCamera.ScreenToWorldPoint(Input.mousePosition) - (Vector2)obj.position).normalized,
                Vector2.up
            );
        }

        public Vector2 GetWorldMousePosition() {
            return MainCamera.ScreenToWorldPoint(Input.mousePosition);
        }

        public Vector2 VectorToMouse(Vector2 position) {
            return (GetWorldMousePosition() - position).normalized;
        }

        private void FixedUpdate() {
            _mousePosition = GetWorldMousePosition();
        }
    }
}