using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.InfiniteRunnerEngine
{	
	/// <summary>
	/// Controller for the CaveRacer playable character
	/// </summary>
	public class CaveRacer : Rocket 
	{
		/// the rocket container
		public GameObject RocketContainer;
		/// the rocket gameobject (separate for finer control over its axis)
		public GameObject ActualRocket;
		/// the left reactor particle system
		public ParticleSystem ReactorLeft;
		/// the right reactor particle system
		public ParticleSystem ReactorRight;
		/// the explosion that happens when the plane hits the ground
	    public GameObject Explosion;
	    /// the shake vector we'll apply to the camera on death
	    public Vector3 DeathShakeVector = new Vector3(2f,2f,2f);
		/// the sound fx we'll play on death
		public AudioClip DeathSoundFx;
		/// the forms that the player can switch to --- Scissors, Paper, Rock
		public GameObject avatar1, avatar2, avatar3;
		// variable contains which avatar is on and active
		public int whichAvatarIsOn = 1;

		protected float _horizontalRotationSpeed=5f;
		protected float _verticalRotationSpeed=2f;
		protected Vector3 _newAngle = Vector3.zero;
		protected float _horizontalRotationTarget;
		protected bool _barrelRolling=false;

		/// <summary>
		/// Init
		/// </summary>
		protected override void Start()
		{
			if (ActualRocket!=null)
			{
				_newAngle=ActualRocket.transform.localEulerAngles;
			}
			_animator = ActualRocket.GetComponent<Animator>();
		}

		/// <summary>
		/// On update we handle flight, orientation and reactors
		/// </summary>
		protected override void Update()
		{
			base.Update();
	        HandleOrientation();
            HandleReactors();
			TransformPlayer();
		}

		/// <summary>
		/// When pressing the main action button for the first time we start boosting
		/// </summary>
		public override void MainActionStart()
		{
			if (GameManager.Instance.Status==GameManager.GameStatus.GameInProgress)
			{
				base.MainActionStart();
			}
			if (GameManager.Instance.Status==GameManager.GameStatus.GameOver)
			{

			}
			if (GameManager.Instance.Status==GameManager.GameStatus.BeforeGameStart) 
			{
				LevelManager.Instance.LevelStart();
				base.MainActionStart();

			}

		}

		/// <summary>
		/// Handles the orientation of the plane based on speed and altitude.
		/// </summary>
		protected virtual void HandleOrientation()
		{
			// we make the nose of the plane go up faster if it's climbing
			float climbingAdjuster = 0.8f;
			if (_rigidbodyInterface.Velocity.y > 0)
			{
				climbingAdjuster = 0.8f;
			}
			// we determine the new target to aim the plane's nose at
			Vector3 target = new Vector3(transform.position.x + 5f,transform.position.y + (_rigidbodyInterface.Velocity.y/3f)*climbingAdjuster, transform.position.z);
			Debug.DrawLine(transform.position,target);
			// rotates the rocket towards its direction
			Vector3 vectorToTarget = target - transform.position;
			float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
			Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
			RocketContainer.transform.rotation = Quaternion.Slerp(RocketContainer.transform.rotation, q, Time.deltaTime * _verticalRotationSpeed);

			// we make the plane wiggle
			//_horizontalRotationTarget = Mathf.Sin(15f * Time.time) * 10f;
			_horizontalRotationTarget = 0f;

			// we make the plane roll according to its vertical speed
			_horizontalRotationTarget += _rigidbodyInterface.Velocity.y * 3f;

			// if we're barrelRolling, we apply a modifier to the rotation target
			if (_barrelRolling)
			{
				_horizontalRotationTarget += 360f;
			}

			// we set the x axis of our plane
			_newAngle.x=Mathf.Lerp(_newAngle.x,_horizontalRotationTarget,Time.deltaTime * _horizontalRotationSpeed );
			ActualRocket.transform.localEulerAngles=_newAngle;


		}

		/// <summary>
		/// If we're boosting, we'll increase the size of the reactor's effects
		/// </summary>
		protected virtual void HandleReactors()
		{
			if (_boosting)
			{
				ParticleSystem.MainModule mainModule = ReactorLeft.main;
				mainModule.startSize = 2f;
				mainModule = ReactorRight.main;
				mainModule.startSize = 2f;

			}
			else
			{
				ParticleSystem.MainModule mainModule = ReactorLeft.main;
				mainModule.startSize = 1f;
				mainModule = ReactorRight.main;
				mainModule.startSize = 1f;		
			}
		}

		/// <summary>
		/// Performs a barrel roll
		/// </summary>
		/// <returns>The roll.</returns>
		/// <param name="delay">Delay.</param>
		protected virtual IEnumerator BarrelRoll(float delay)
		{
			float time=0f;
			float barrelRollDuration=2.3f;

			yield return new WaitForSeconds(delay);

			while (time < barrelRollDuration)
			{
				_barrelRolling=true;
				time += Time.deltaTime;
				yield return null;
			}
			_barrelRolling=false;
		}

		/// <summary>
		/// When the CaveRacer gets destroyed, we add an explosion
		/// </summary>
		public override void Die()
		{
			GameObject explosion = (GameObject)Instantiate(Explosion);
	        explosion.transform.position = transform.position;

			// if we have a sound manager and if we've specified a song to play when this object is picked
			if (SoundManager.Instance!=null && DeathSoundFx!=null)
			{
				// we play that sound once
				SoundManager.Instance.PlaySound(DeathSoundFx,transform.position);	
			}

	        if (Camera.main.GetComponent<CameraBehavior>() != null)
	        {
				Camera.main.GetComponent<CameraBehavior>().Shake(DeathShakeVector);
	        }

			#if UNITY_ANDROID || UNITY_IPHONE
				Handheld.Vibrate();
			#endif
	        base.Die();
		}

		/// <summary>
		/// Triggered when entering a trigger (in this case probably a coin or a power up)
		/// </summary>
		/// <param name="collidingObject">Colliding object.</param>
		protected override void TriggerEnter(GameObject collidingObject)
		{
			// if we're not already doing a barrel roll, we ask for one in a second (to celebrate)
			if (!_barrelRolling)
			{
				StartCoroutine(BarrelRoll(1f));
			}
		}
		
		/// <summary>
		/// Changes made to script to adapt it to Scissors, Paper, Rock
		/// </summary>
		public void TransformPlayer()
		{


			if (Input.GetKeyDown("q"))
			{
				whichAvatarIsOn = 1;
				
				SwitchAvatar();
			}
			else if (Input.GetKeyDown("w"))
			{
				whichAvatarIsOn = 2;
				
				SwitchAvatar();
			}
			else if (Input.GetKeyDown("e"))
			{
				whichAvatarIsOn = 3;
				SwitchAvatar();
			}
		}

		// public method to switch avatars by pressing UI button
		public void SwitchAvatar()
		{

			// processing whichAvatarIsOn variable
			switch (whichAvatarIsOn)
			{

				// if the first avatar is on
				case 1:
					//Debug.Log("Transforming into Scissors and whichAvatarIsOn is now:" + whichAvatarIsOn);

					// then the second avatar is on now
					whichAvatarIsOn = 1;

					// disable the first one and anable the second one
					avatar1.gameObject.SetActive(true);
					avatar2.gameObject.SetActive(false);
					avatar3.gameObject.SetActive(false);

					// set tag to scissors for gate check
					this.gameObject.tag = "Scissors";
					//Debug.Log("Tag is now set to: " + this.gameObject.tag);
					break;

				// if the second avatar is on
				case 2:
					//Debug.Log("Transforming into Paper and whichAvatarIsOn is now:" + whichAvatarIsOn);

					// then the first avatar is on now
					whichAvatarIsOn = 2;

					// disable the second one and anable the first one
					avatar1.gameObject.SetActive(false);
					avatar2.gameObject.SetActive(true);
					avatar3.gameObject.SetActive(false);

					// set tag to scissors for gate check
					this.gameObject.tag = "Paper";
					//Debug.Log("Tag is now set to: " + this.gameObject.tag);
					break;

				case 3:
					//Debug.Log("Transforming into Rock and whichAvatarIsOn is now:" + whichAvatarIsOn);

					// then the first avatar is on now
					whichAvatarIsOn = 3;

					// disable the second one and anable the first one
					avatar1.gameObject.SetActive(false);
					avatar2.gameObject.SetActive(false);
					avatar3.gameObject.SetActive(true);

					// set tag to scissors for gate check
					this.gameObject.tag = "Rock";
					//Debug.Log("Tag is now set to: " + this.gameObject.tag);
					break;
			}

		}

	}
}
