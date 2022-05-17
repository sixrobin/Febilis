namespace Templar
{
    using UnityEngine;

    public class StencilManager : RSLib.Framework.ConsoleProSingleton<StencilManager>
    {
        [SerializeField] private GameObject _unitStencilPrefab = null;
        [SerializeField] private GameObject _cache = null;

        private System.Collections.Generic.Queue<GameObject> _stencils = new System.Collections.Generic.Queue<GameObject>();
        private Manager.PaletteManager.RampsGroup _rampsGroup;
        
        public static event System.Action StencilShown;
        public static event System.Action StencilHidden;

        public static void ShowPlayerStencil(float delay)
        {
            System.Collections.Generic.List<Unit.UnitView> unitViews = new System.Collections.Generic.List<Templar.Unit.UnitView>(1)
            {
                Manager.GameManager.PlayerCtrl.PlayerView
            };

            ShowStencils(unitViews, delay);
        }
        
        public static void ShowStencils(System.Collections.Generic.List<Unit.UnitView> unitViews, float delay)
        {
            StencilShown?.Invoke();

            Instance._rampsGroup = Manager.PaletteManager.GetGroupFromRamp(Manager.GameManager.CameraCtrl.GrayscaleRamp.TextureRamp);
            
            if (delay > 0f)
                Instance.StartCoroutine(ShowStencilsCoroutine(unitViews, delay));
            else
                ShowStencils(unitViews);
        }

        public static void HideStencils()
        {
            StencilHidden?.Invoke();
            
            while (Instance._stencils.Count > 0)
                Instance._stencils.Dequeue().SetActive(false);
            
            Instance._cache.SetActive(false);
            Manager.GameManager.CameraCtrl.GrayscaleRamp.OverrideRamp(Instance._rampsGroup.Base);
        }

        private static void ShowStencils(System.Collections.Generic.List<Unit.UnitView> unitViews)
        {
            for (int i = unitViews.Count - 1; i >= 0; --i)
                GenerateStencil(unitViews[i]);
            
            Instance._cache.SetActive(true);
            Manager.GameManager.CameraCtrl.GrayscaleRamp.OverrideRamp(Instance._rampsGroup.NoWhite);
        }
        
        private static void GenerateStencil(Unit.UnitView source)
        {
            GameObject stencil = RSLib.Framework.Pooling.Pool.Get(Instance._unitStencilPrefab);
            stencil.transform.position = source.transform.position;

            SpriteRenderer stencilSpriteRenderer = stencil.GetComponent<SpriteRenderer>();
            stencilSpriteRenderer.sprite = source.Renderer.sprite;
            stencilSpriteRenderer.flipX = source.Renderer.flipX;
            stencilSpriteRenderer.flipY = source.Renderer.flipY;
            
            Instance._stencils.Enqueue(stencil);
        }
        
        private static System.Collections.IEnumerator ShowStencilsCoroutine(System.Collections.Generic.List<Unit.UnitView> unitViews, float delay)
        {
            yield return RSLib.Yield.SharedYields.WaitForSeconds(delay);            
            ShowStencils(unitViews);
        }
    }
}
