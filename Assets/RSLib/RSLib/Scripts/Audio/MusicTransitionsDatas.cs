namespace RSLib.Audio
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Music Transition", menuName = "RSLib/Audio/Music Transition")]
    public class MusicTransitionsDatas : ScriptableObject
    {
        [SerializeField, Min(0f)] private float _duration = 1f;
        [SerializeField] private Maths.Curve _curveOut = Maths.Curve.InOutSine;
        [SerializeField] private Maths.Curve _curveIn = Maths.Curve.InOutSine;
        [SerializeField] private bool _crossFade = true;

        public static MusicTransitionsDatas Default
        {
            get
            {
                MusicTransitionsDatas transitionDatas = CreateInstance<MusicTransitionsDatas>();
                transitionDatas._duration = 1f;
                transitionDatas._curveOut = Maths.Curve.InOutSine;
                transitionDatas._curveIn = Maths.Curve.InOutSine;
                transitionDatas._crossFade = true;

                return transitionDatas;
            }
        }

        public static MusicTransitionsDatas Instantaneous
        {
            get
            {
                MusicTransitionsDatas transitionDatas = CreateInstance<MusicTransitionsDatas>();
                transitionDatas._duration = 0f;
                transitionDatas._curveOut = Maths.Curve.Linear;
                transitionDatas._curveIn = Maths.Curve.Linear;
                transitionDatas._crossFade = true;

                return transitionDatas;
            }
        }

        public float Duration => _duration;
        public Maths.Curve CurveOut => _curveOut;
        public Maths.Curve CurveIn => _curveIn;
        public bool CrossFade => _crossFade;
    }
}