using UnityEngine;

namespace Utilities {

    public class Singleton<T> : MonoBehaviour where T : Component {
        protected static T _internalInstance;
        public static bool HasInstance => _internalInstance != null;
        public static T TryGetInstance() => HasInstance ? _internalInstance : null;
        public static T Current => _internalInstance;

        public static T Instance {
            get {
                if (_internalInstance == null) {
                    _internalInstance = FindFirstObjectByType<T>();
                    if (_internalInstance == null) {
                        GameObject obj = new GameObject();
                        obj.name = $"{typeof(T).Name} - AutoCreated";
                        _internalInstance = obj.AddComponent<T>();
                    }
                }

                return _internalInstance;
            }
        }

        public static void StartSingleton() {
            if (_internalInstance == null) {
                _internalInstance = FindFirstObjectByType<T>();
                if (_internalInstance == null) {
                    GameObject obj = new GameObject();
                    obj.name = $"{typeof(T).Name} - AutoCreated";
                    _internalInstance = obj.AddComponent<T>();
                }
            }
        }

        protected virtual void Awake() => InitialiseSingleton();

        protected virtual void InitialiseSingleton() {
            if (!Application.isPlaying) {
                return;
            }
            _internalInstance = this as T;
        }
    }
}