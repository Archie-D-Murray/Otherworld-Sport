using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Utilities;

public class SpriteFader : MonoBehaviour {
    [SerializeField] private float fadeTime;
    private SpriteRenderer spriteRenderer;
    private Color initialColour;
    CountDownTimer timer;
    void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        initialColour = spriteRenderer.color;
        timer = new CountDownTimer(fadeTime);
        timer.Start();
    }

    // Update is called once per frame
    void FixedUpdate() {
        timer.Update(Time.fixedDeltaTime);
        spriteRenderer.color = Color.Lerp(initialColour, Color.clear, timer.Progress());
    }
}