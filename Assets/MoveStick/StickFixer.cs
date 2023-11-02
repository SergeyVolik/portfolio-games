using Unity.Scenes;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Prototype.UI
{
    public class StickFixer : MonoBehaviour, IPointerDownHandler, IPointerUpHandler   
    {

        public CanvasGroup[] stickGroup;
        public RectTransform stickBG;

        public Transform renderHandle;
        public Transform renderBG;

        private void Awake()
        {
            foreach (var item in stickGroup)
            {
                item.alpha = 0;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            foreach (var item in stickGroup)
            {
                item.alpha = 1;
            }

            stickBG.localPosition = transform.localPosition;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            foreach (var item in stickGroup)
            {
                item.alpha = 0;
            }

        }

        private void Update()
        {
            var selfPos = transform.position;
            var bgPos = stickBG.position;
            renderHandle.position = selfPos;
            renderBG.position = bgPos;

        }
    }
}

