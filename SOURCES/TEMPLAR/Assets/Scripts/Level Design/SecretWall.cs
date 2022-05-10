namespace Templar
{
    using RSLib.Extensions;
    using UnityEngine;

    public class SecretWall : MonoBehaviour, Attack.IHittable, Flags.IIdentifiable
    {
        [Header("IDENTIFIER")]
        [SerializeField] private Flags.SecretWallIdentifier _secretWallIdentifier = null;
        
        [Header("REFS")]
        [SerializeField] private UnityEngine.Tilemaps.Tilemap[] _tilemapsToCarve = null;
        [SerializeField] private UnityEngine.Tilemaps.Tilemap _shapeTilemap = null;
        [SerializeField] private Collider2D _collider2D = null;

        [Header("EVENT")]
        [SerializeField] private UnityEngine.Events.UnityEvent _secretWallOpen = null;

        public Flags.IIdentifier Identifier => _secretWallIdentifier;
        
        public bool SpawnVFXOnHit => true;
        public Attack.HitLayer HitLayer => Attack.HitLayer.SECRET_WALL;
        
        public bool CanBeHit(Templar.Attack.HitInfos hitInfos)
        {
            return _collider2D.enabled;
        }

        public void OnHit(Templar.Attack.HitInfos hitInfos)
        {
            OpenSecretWall(false);
        }

        private void OpenSecretWall(bool onLoad)
        {
            for (int i = _tilemapsToCarve.Length - 1; i >= 0; --i)
                _tilemapsToCarve[i].CarveTilemap(_shapeTilemap);
            
            _collider2D.enabled = false;
            
            _secretWallOpen?.Invoke();
            
            if (!onLoad)
                Manager.FlagsManager.Register(this);
        }

        private void Start()
        {
            _shapeTilemap.color = _shapeTilemap.color.WithA(0f);

            if (Manager.FlagsManager.Check(this))
                OpenSecretWall(true);
        }
    }
}
