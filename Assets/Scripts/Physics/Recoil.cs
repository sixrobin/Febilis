namespace Templar.Physics
{
    using UnityEngine;

    public class Recoil
    {
        private Datas.Attack.RecoilDatas _recoilDatas;
        private bool? _overrideCheckEdge;

        public Recoil(float dir, Datas.Attack.RecoilDatas recoilDatas, bool? overrideCheckEdge = null)
        {
            _recoilDatas = recoilDatas;
            _overrideCheckEdge = overrideCheckEdge;

            Dur = _recoilDatas.Dur;
            Dir = dir;
        }

        public float Force => _recoilDatas.Force;
        public float AirborneMult => _recoilDatas.AirborneMult;
        public bool CheckEdge => _overrideCheckEdge ?? _recoilDatas.CheckEdge;

        public float Dur { get; private set; }

        public float Dir { get; private set; }
        public bool IsComplete => Dur <= 0f;
        
        public void Update()
        {
            Dur -= Time.deltaTime;
        }
    }
}