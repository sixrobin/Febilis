namespace Templar.Boss
{
    using UnityEngine;

    public class BossFightWonCutscene : RSLib.Framework.ConsoleProSingleton<BossFightWonCutscene>
    {
        [SerializeField] private GameObject _unitStencilPrefab = null;
        [SerializeField] private GameObject _cache = null;

        private System.Collections.Generic.Queue<GameObject> _stencils = new System.Collections.Generic.Queue<GameObject>();
        private Manager.PaletteManager.RampsGroup _rampsGroup;
        
        public static event System.Action CutsceneStarted;
        public static event System.Action CutsceneOver;
        
        public static void ShowStencils(Unit.UnitView bossUnitView, float delay)
        {
            CutsceneStarted?.Invoke();

            Instance._rampsGroup = Manager.PaletteManager.GetGroupFromRamp(Manager.GameManager.CameraCtrl.GrayscaleRamp.TextureRamp);
            
            if (delay > 0f)
                Instance.StartCoroutine(ShowStencilsCoroutine(bossUnitView, delay));
            else
                ShowStencils(bossUnitView);
        }

        public static void HideStencils()
        {
            CutsceneOver?.Invoke();
            
            while (Instance._stencils.Count > 0)
                Instance._stencils.Dequeue().SetActive(false);
            
            Instance._cache.SetActive(false);
            Manager.GameManager.CameraCtrl.GrayscaleRamp.OverrideRamp(Instance._rampsGroup.Base);
        }

        private static void ShowStencils(Unit.UnitView bossUnitView)
        {
            GenerateStencil(bossUnitView);
            GenerateStencil(Manager.GameManager.PlayerCtrl.PlayerView);
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
        
        private static System.Collections.IEnumerator ShowStencilsCoroutine(Unit.UnitView bossUnitView, float delay)
        {
            yield return RSLib.Yield.SharedYields.WaitForSeconds(delay);            
            ShowStencils(bossUnitView);
        }
    }
}
