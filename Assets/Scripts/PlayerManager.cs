using System.Collections;

using Terrain;

using TMPro;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

using Utilities;

public class PlayerManager : MonoBehaviour {
    [SerializeField] private PlayerController _player;
    [SerializeField] private Vector3 _initialPosition;
    [SerializeField] private Collider2D _floor;
    [SerializeField] private Button _restart;
    [SerializeField] private TMP_Text _message;
    [SerializeField] private CanvasGroup _ui;
    [SerializeField] private CanvasGroup _fade;

    private void Start() {
        _fade.FadeCanvas(0.5f, true, this);
        _player = FindFirstObjectByType<PlayerController>();
        _player.RunEnded += EndRun;
        _initialPosition = _player.transform.position;
        _ui = GetComponent<CanvasGroup>();
        _restart = GetComponentInChildren<Button>(true);
        _restart.onClick.AddListener(ChangeStart);
        _restart.onClick.AddListener(StartRun);
    }

    private IEnumerator ResetEnable() {
        yield return Yielders.WaitForSeconds(1.0f);
        UpgradeManager.Instance.EnableButtons();
        _restart.GetComponentInChildren<TMP_Text>().text = "Retry!";
    }

    public void MainMenu() {
        _fade.FadeCanvas(1.0f, false, this);
        StartCoroutine(GotoMainMenu());
    }

    private IEnumerator GotoMainMenu() {
        yield return Yielders.WaitForSeconds(1.0f);
        SceneManager.LoadScene(0);
    }

    private void ChangeStart() {
        StartCoroutine(ResetEnable());
        _restart.onClick.RemoveListener(ChangeStart);
    }

    private void StartRun() {
        TerrainManager.Instance.SetWorldType(TerrainType.Forest);
        _restart.interactable = false;
        _ui.FadeCanvas(1.0f, true, this);
        EventSystem.current.SetSelectedGameObject(null);
        _floor.enabled = false;
        _player.Drop();
    }

    private void EndRun(EndCause cause) {
        _message.text = GetCause(cause);
        _ui.FadeCanvas(1.0f, false, this);
        _restart.interactable = true;
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