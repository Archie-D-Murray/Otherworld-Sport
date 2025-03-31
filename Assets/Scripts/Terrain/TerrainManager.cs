using UnityEngine;

using Utilities;

using System.Collections.Generic;
using System;
using System.Linq;

using Tags;
using TMPro;

namespace Terrain {
    public enum TerrainType { Forest, Cyber, Steampunk }

    [DefaultExecutionOrder(-99)]
    public class TerrainManager : Singleton<TerrainManager> {
        [SerializeField] private GameObject _targetPrefab;
        [SerializeField] private GameObject _worldTargetPrefab;
        [SerializeField] private GameObject _updraftPrefab;
        [SerializeField] private GameObject _hitPrefab;
        [SerializeField] private Transform _worldCanvas;
        [SerializeField] private TerrainData[] _terrainData;
        [SerializeField] private SpriteRenderer _background;
        [SerializeField] private List<Target> _targets;
        [SerializeField] private List<Updraft> _updrafts;
        [SerializeField] private Vector3[] _targetPoints;
        [SerializeField] private Vector3[] _updraftPoints;
        [SerializeField] private float _worldTargetChance = 0.2f;
        [SerializeField] private BGMEmitter _bgm;
        [SerializeField] private int _minTargets = 3;
        [SerializeField] private int _maxTargets = 5;
        [SerializeField] private int _updraftCount = 2;

        private Dictionary<TerrainType, int> _lookup = new Dictionary<TerrainType, int>();
        private TerrainData _currentData => _terrainData[_lookup[_type]];

        [SerializeField] private PlayerController _player;
        [SerializeField] private TerrainType _type = TerrainType.Forest;

        public readonly HashSet<TerrainType> DiscoveredWorlds = new HashSet<TerrainType>();
        public readonly int MaxWorldTypes = Enum.GetValues(typeof(TerrainType)).Length;
        public Action OnDiscoveredAllWorlds;
        public TerrainType WorldType => _type;

        private void Start() {
            _bgm = GetComponent<BGMEmitter>();
            _bgm.PlayNone(0.0f);
            _bgm.PlayBGM(BGMType.Forest);
            for (int i = 0; i < _terrainData.Length; i++) {
                _lookup.Add(_terrainData[i].Type, i);
            }
            _player = FindFirstObjectByType<PlayerController>();
            Transform targetPoints = GetComponentInChildren<TargetPoints>().transform;
            _targetPoints = new Vector3[targetPoints.childCount];
            for (int i = 0; i < targetPoints.childCount; i++) {
                _targetPoints[i] = targetPoints.GetChild(i).position;
            }
            Transform updraftPoints = GetComponentInChildren<UpdraftPoints>().transform;
            _updraftPoints = new Vector3[updraftPoints.childCount];
            for (int i = 0; i < updraftPoints.childCount; i++) {
                _updraftPoints[i] = updraftPoints.GetChild(i).position;
            }
        }

        public void SetWorldType(TerrainType type) {
            _type = type;
            switch (type) {
                case TerrainType.Steampunk:
                    _bgm.PlayBGM(BGMType.Steampunk);
                    break;
                case TerrainType.Cyber:
                    _bgm.PlayBGM(BGMType.Cyber);
                    break;
                default:
                    _bgm.PlayBGM(BGMType.Forest);
                    break;
            }
            _background.sprite = _currentData.Background;
            foreach (Target target in _targets) {
                target.SetType(_currentData);
            }
            GenerateTargets();
            GenerateUpdrafts();
            if (!DiscoveredWorlds.Contains(type)) {
                DiscoveredWorlds.Add(type);
                if (DiscoveredWorlds.Count == MaxWorldTypes) {
                    OnDiscoveredAllWorlds?.Invoke();
                }
            }
            _player.OnWorldChange();
        }

        public void DestroyTarget(Target target) {
            _targets.Remove(target);
            Destroy(target.gameObject, 0.5f);
            if (_targets.Count == 0) {
                GenerateTargets(false);
            }
        }

        private void GenerateTargets(bool clear = true) {
            int targetCount = UnityEngine.Random.Range(_minTargets, _maxTargets);
            bool spawnedWorld = false;
            if (clear) {
                foreach (Target target in _targets) {
                    Destroy(target.gameObject);
                }
                _targets.Clear();
            }
            List<Vector3> available = _targetPoints.ToList();
            for (int i = 0; i < targetCount; i++) {
                bool spawnWorld = UnityEngine.Random.value <= _worldTargetChance;
                int idx = UnityEngine.Random.Range(0, available.Count);
                Vector3 spawnPoint = available[idx];
                available.RemoveAt(idx);
                if (spawnWorld && !spawnedWorld) {
                    _targets.Add(Instantiate(_worldTargetPrefab, spawnPoint, Quaternion.identity).GetComponent<Target>());
                    _targets.Last().SetType(_currentData);
                    spawnedWorld = true;
                } else {
                    _targets.Add(Instantiate(_targetPrefab, spawnPoint, Quaternion.identity).GetComponent<Target>());
                    _targets.Last().SetType(_currentData);
                }
            }
            available.Clear();
            foreach (Vector3 target in _targetPoints) {
                available.Add(target);
            }
        }

        private void GenerateUpdrafts() {
            foreach (Updraft updraft in _updrafts) {
                Destroy(updraft.gameObject);
            }
            _updrafts.Clear();
            List<Vector3> available = _updraftPoints.ToList();
            for (int i = 0; i < _updraftCount; i++) {
                int idx = UnityEngine.Random.Range(0, available.Count);
                Vector3 spawnPoint = available[idx];
                available.RemoveAt(idx);
                _updrafts.Add(Instantiate(_updraftPrefab, spawnPoint, Quaternion.identity).GetComponent<Updraft>());
            }
        }

        public Color GetAccent(TerrainType type) {
            return _terrainData[_lookup[type]].TargetAccent;
        }

        public void SpawnHitMessage(bool bullseye, int money, Vector3 position) {
            GameObject hit = Instantiate(_hitPrefab, _worldCanvas);
            hit.transform.position = position;
            hit.GetComponentInChildren<TMP_Text>().text = $"{money}{(bullseye ? "\nBullseye!" : "")}";
        }
    }
}