using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class HornTutorial : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	[Header("The Sound Of The Horn")]
	[SerializeField]
	private AudioSource hornSound;

	[Header("The Minimum Duration Of The Sound")]
	[SerializeField]
	private float minHornDuration = 0.05f;


	#region Events
	//Checks if a coroutine is already running, and if it is, don't start another one
	private bool coroutineRunning;


	//Detects when the player clicks/touches the horn button
	public void OnPointerDown(PointerEventData eventData)
	{
		if (!hornSound.isPlaying)
        {
			StartHorn();

			StartCoroutine(CheckIfSoundFinished());
		}
	}

	//Detects when the player stops clicking the horn button
	public void OnPointerUp(PointerEventData eventData)
	{
		if (hornSound.isPlaying && !coroutineRunning)
			StartCoroutine(StopHornDelayed());
	}
	#endregion

	#region Start Horn
	private void StartHorn()
	{
		hornSound.Play();

		PlayHornAnimation();
	}
	#endregion

	#region Stop Horn
	private void StopHorn()
	{
		if (hornSound.isPlaying)
			hornSound.Stop();

		StopHornAnimation();
	}

	private IEnumerator StopHornDelayed()
	{
		coroutineRunning = true;

		yield return new WaitForSeconds(minHornDuration);
		StopHorn();

		coroutineRunning = false;
	}
	#endregion

	#region Animations
	[Header("The Scale Of The Horn When Being Animated")]
	[SerializeField]
	private Vector2 hornAnimationScale = new Vector2(0.9f, 0.9f);

	[Header("The speed of the horn animation")]
	[SerializeField]
	private float animationSpeed = 10f;

	//The transform of the horn button, which will be animated
	private Transform horn;

	//The initial horn scale. The horn scale will be reset to the initial value when the player stops clicking the horn
	//button or when the horn sound finishes playing(if it is not set to loop)
	private Vector3 initialHornScale;
	private Coroutine animateHornCoroutine;

	private void Start()
	{
		//Get the horn transform
		horn = transform;

		//Get the initial scale
		initialHornScale = horn.localScale;
	}

	private void PlayHornAnimation()
	{
		if (animateHornCoroutine != null)
			StopCoroutine(animateHornCoroutine);

		animateHornCoroutine = StartCoroutine(AnimateHorn(hornAnimationScale));
	}

	private IEnumerator AnimateHorn(Vector3 newScale)
    {
		float percentage = 0f;
		Vector3 oldScale = horn.localScale;
		newScale.z = 1;

		while(horn.localScale != newScale)
        {
			percentage += Time.deltaTime * animationSpeed;

			horn.localScale = Vector3.Lerp(oldScale, newScale, percentage);

			yield return null;
        }
    }
	
	private void StopHornAnimation()
	{
		if (animateHornCoroutine != null)
			StopCoroutine(animateHornCoroutine);

		animateHornCoroutine = StartCoroutine(AnimateHorn(initialHornScale));
	}
	#endregion

	#region Check If Sound Finished
	//If the horn sound is not set to loop and it finishes playing, the horn animation still plays, this checks if the sound finished and stops the animation
	private IEnumerator CheckIfSoundFinished()
	{
		yield return new WaitForSeconds(0.1f);

		if (horn.localScale != initialHornScale)
        {
			if (!hornSound.isPlaying)
				StopHornAnimation();

			else
				StartCoroutine(CheckIfSoundFinished());
		}
	}
	#endregion
}
