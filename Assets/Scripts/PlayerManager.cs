using System.Collections;

using Terrain;

using TMPro;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using Utilities;

public class PlayerManager : MonoBehaviour {
    [SerializeField] private PlayerController _player;
    [SerializeField] private Vector3 _initialPosition;
    [SerializeField] private Collider2D _floor;
    [SerializeField] private Button _start;
    [SerializeField] private TMP_Text _message;
    [SerializeField] private CanvasGroup _ui;
    [SerializeField] private Image _stamina;

    private void Start() {
        _player = FindFirstObjectByType<PlayerController>();
        _player.RunEnded += EndRun;
        _initialPosition = _player.transform.position;
        _ui = GetComponent<CanvasGroup>();
        _start = GetComponentInChildren<Button>();
        _start.onClick.AddListener(ChangeStart);
        _start.onClick.AddListener(StartRun);
    }

    private void FixedUpdate() {
        if (_floor.enabled) {
            _stamina.fillAmount = 1.0f;
            _stamina.color = Color.blue;
        } else {
            _stamina.fillAmount = _player.StaminaPercent;
            _stamina.color = Color.Lerp(Color.red, Color.green, _player.StaminaPercent);
        }
    }

    private void ChangeStart() {
        _start.GetComponentInChildren<TMP_Text>().text = "Reset!";
        _start.onClick.RemoveListener(ChangeStart);
    }

    private void StartRun() {
        TerrainManager.Instance.SetWorldType(TerrainType.Forest);
        _start.interactable = false;
        _ui.FadeCanvas(1.0f, true, this);
        EventSystem.current.SetSelectedGameObject(null);
        _floor.enabled = false;
        _player.Drop();
    }

    private void EndRun(EndCause cause) {
        _message.text = GetCause(cause);
        _ui.FadeCanvas(1.0f, false, this);
        _start.interactable = true;
        _floor.enabled = true;
        _player.Reset(_initialPosition);
    }

    private string GetCause(EndCause cause) {
        return cause switch {
            EndCause.Floor => "You touched the ground!",
            _ => "Ran out of stamina!"
        };
    }
}