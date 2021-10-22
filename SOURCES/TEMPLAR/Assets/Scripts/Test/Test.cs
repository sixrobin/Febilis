namespace Templar
{
    using RSLib.Extensions;
    using UnityEngine;

    public class Test : MonoBehaviour
    {
        public SpriteRenderer spriteRenderer;

        private void Start()
        {
            Texture2D spr = spriteRenderer.sprite.texture;
            spr = spr.FlipX();
            spriteRenderer.sprite = Sprite.Create(spr, spriteRenderer.sprite.rect, Vector2.zero, 18);
        }
    }
}