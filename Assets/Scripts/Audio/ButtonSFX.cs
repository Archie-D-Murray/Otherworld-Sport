using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Audio {
    [RequireComponent(typeof(SFXEmitter))]
    public class ButtonSFX : MonoBehaviour, IPointerEnterHandler {

        [SerializeField] private SFXEmitter emitter;

        public void Start() {
            emitter = GetComponent<SFXEmitter>();
            GetComponent<Button>().onClick.AddListener(() => emitter.Play(SoundEffectType.UIClick));
        }

        public void OnPointerEnter(PointerEventData _) {
            emitter.Play(SoundEffectType.UIHover);
        }
    }
}