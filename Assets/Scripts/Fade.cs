using UnityEngine;

using TMPro;

public class Fade : MonoBehaviour {
    [SerializeField] private TMP_Text _text;

    private void FixedUpdate() {
        transform.position += Vector3.up * Time.fixedDeltaTime;
        _text.alpha -= Time.fixedDeltaTime;
        if (_text.alpha < 0) {
            Destroy(gameObject);
        }
    }
}