using UnityEngine;

using Utilities;

using System.Collections.Generic;
using System;
using System.Linq;

namespace Terrain {
    public class TerrainManager : Singleton<TerrainManager> {
        [SerializeField] private GameObject[] _segments;

        private int _pool;

        private Queue<Transform> _spawnedSegments = new Queue<Transform>();

        [SerializeField] private Vector3 _initialPos;
        [SerializeField] private float _distance = 0.0f;
        [SerializeField] private Transform _player;
        [SerializeField] private float _min;

        private void Start() {
            _min = Helpers.Instance.MainCamera.orthographicSize;
            _player = FindFirstObjectByType<PlayerController>().transform;
            _initialPos = transform.position;
            _pool = _segments.Length;
            List<Transform> segments = FindObjectsOfType<TerrainSegment>().Select((TerrainSegment segment) => segment.transform).ToList();
            segments.Sort((current, other) => other.position.y.CompareTo(current.position.y));
            foreach (Transform segment in segments) {
                _spawnedSegments.Enqueue(segment);
            }
            SpawnSegment();
            SpawnSegment();
        }

        private void FixedUpdate() {
            if (_initialPos.y - _player.position.y >= _distance - _min) {
                SpawnSegment();
            }
            if (_spawnedSegments.Count > 0 && _spawnedSegments.Peek().position.y > _player.position.y + _min * 2.0f) {
                Destroy(_spawnedSegments.Dequeue().gameObject);
            }
        }

        private void SpawnSegment() {
            Vector3 pos = _initialPos;
            Transform segment = Instantiate(_segments[UnityEngine.Random.Range(0, _pool--)]).transform;
            if (_pool < 0) {
                _pool = _segments.Length;
            }
            float height = segment.GetComponent<TerrainSegment>().Height();
            _distance += height;
            segment.position = _initialPos + Vector3.down * _distance;
            _spawnedSegments.Enqueue(segment);
        }
    }
}