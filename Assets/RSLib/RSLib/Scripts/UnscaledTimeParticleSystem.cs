namespace RSLib
{
	using System.Collections;
	using UnityEngine;

	/// <summary>
	/// Attach to a particle system to make it independent from Time.timeScale.
	/// </summary>
	[RequireComponent (typeof (ParticleSystem))]
	public class UnscaledTimeParticleSystem : MonoBehaviour
	{
		[SerializeField] private float _duration = 10f;

		private ParticleSystem _particleSystem;
		private bool _simulating;

		public void Play(bool looping)
		{
			_particleSystem.Stop();
			_particleSystem.Play();	
			StopAllCoroutines();

			if (looping)
				StartCoroutine(PlayLoopingCoroutine());
			else
				StartCoroutine(PlayOneShotCoroutine());
		}

		public void StopLoop()
		{
			StopAllCoroutines();
			Stop();
		}

		private void Stop()
		{
			_particleSystem.Stop();
			_particleSystem.Simulate(0f, true, true);
			_simulating = false;
		}

        private IEnumerator PlayLoopingCoroutine()
		{
			_simulating = true;
			while (true)
				yield return null;
		}

        private IEnumerator PlayOneShotCoroutine()
		{
			_simulating = true;
			yield return new WaitForSecondsRealtime(_duration);
			Stop();
		}

		private void Awake()
		{
			_particleSystem = GetComponent<ParticleSystem>();
		}

        private void Update()
		{
			if (_simulating)
				_particleSystem.Simulate(Time.unscaledDeltaTime, true, false);
		}
	}
}