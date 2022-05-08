namespace Templar.UI
{
    using UnityEngine;

    public class HealView : MonoBehaviour
    {
        [SerializeField] private UnityEngine.UI.Image _image = null;
        [SerializeField] private Sprite _filledSprite = null;
        [SerializeField] private Sprite _emptySprite = null;

        public void Display(bool show)
        {
            gameObject.SetActive(show);
        }

        public void MarkAsFilled(bool filled)
        {
            _image.sprite = filled ? _filledSprite : _emptySprite;
        }
    }
}