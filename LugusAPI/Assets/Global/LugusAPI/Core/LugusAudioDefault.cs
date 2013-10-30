

using UnityEngine;
using System.Collections;

public class LugusAudio : LugusSingletonRuntime<LugusAudioDefault>
{
}

public class LugusAudioDefault : MonoBehaviour
{
	public AudioSource oneShotSource = null;
	public AudioSource backgroundSource = null;
	
	public void PlayOneShot(AudioClip clip)
	{
		//Logus.LogWarning ("LuGusAudio:PlayOneShot " + clip.name);
		
		if( audioEnabled && clip != null )
			oneShotSource.PlayOneShot( clip ); 
	}
	
	public void PlayOneShot(AudioClip clip, Vector3 position)
	{
		if( audioEnabled && clip != null )
			AudioSource.PlayClipAtPoint(clip, position, oneShotSource.volume);
	}
	
	public void PlayBackground(AudioClip clip)
	{
		//Logus.LogWarning ("LuGusAudio:PlayBackground " + clip.name);
		
		if( clip != null )
			backgroundSource.clip = clip;
		
		if( audioEnabled && clip != null )
		{
			backgroundSource.Play();
		}
	}
	
	public void PlayBackgroundDelayed(AudioClip clip, float delay)
	{
		StartCoroutine( PlayBackgroundRoutine(clip, delay) );
	}
	
	protected IEnumerator PlayBackgroundRoutine(AudioClip clip, float delay)
	{
		if( delay != 0.0f )
			yield return new WaitForSeconds(delay);
		
		PlayBackground( clip );
	}
	
	public void FadeBackgroundTo(AudioClip newBackground, float fadeDuration)
	{
		if( newBackground == null )
		{
			//Debug.LogError("Error 1");
			return;
		}
		
		if( !this.BackgroundEnabled )
		{
			//Debug.LogError("Error 2");
			backgroundSource.clip = newBackground;
			return;
		}
		else
		{
			if( backgroundSource.clip != newBackground ) // if same clip, no fading to be done...
			{
				StartCoroutine( FadeBackgroundRoutine(newBackground, fadeDuration) );
			}
//			else
//			{
//				Debug.LogError("Error 3");
//			}
		}
	}
	
	protected IEnumerator FadeBackgroundRoutine( AudioClip newBackground, float fadeDuration )
	{
		Debug.Log("FadeBackgroundROutine : started. Fading to " + newBackground.name + " / " + fadeDuration);
		
		float timeCount = 0.0f;
		
		float startingVolume = backgroundSource.volume;
		
		while( timeCount < (fadeDuration / 2.0f) )
		{
			backgroundSource.volume = Mathf.Lerp( startingVolume, 0.0f, timeCount / (fadeDuration /2.0f) );
			
			timeCount += Time.deltaTime;
			
			yield return null;
		}
		
		backgroundSource.clip = newBackground; 
		backgroundSource.Play();
		
		timeCount = 0.0f; 
		while( timeCount < (fadeDuration / 2.0f) )
		{
			backgroundSource.volume = Mathf.Lerp( 0.0f, startingVolume, timeCount / (fadeDuration /2.0f) );
			
			timeCount += Time.deltaTime;
			
			yield return null;
		}
	}
	
	
	public void PlayOneShotDelayed(AudioClip clip, float delay)
	{
		StartCoroutine( PlayOneShotRoutine(clip, delay) );
	}
	
	protected IEnumerator PlayOneShotRoutine(AudioClip clip, float delay)
	{
		if( delay != 0.0f )
			yield return new WaitForSeconds(delay);
		
		PlayOneShot( clip ); 
	}
	
	void Awake()
	{
		oneShotSource = LugusCamera.game.gameObject.AddComponent<AudioSource>();
		oneShotSource.volume = 1.0f;
		
		backgroundSource = LugusCamera.game.gameObject.AddComponent<AudioSource>();
		backgroundSource.loop = true;
		backgroundSource.volume = 0.25f; 
		
		// get and set perform other stuff, so trigger that on Awake
		this.BackgroundEnabled = true;//this.BackgroundEnabled;
		this.EffectsEnabled = true;//this.EffectsEnabled;
	}
	
	void Update()
	{
		
	}
	
	public void Init()
	{
		
	}
	
	public bool BackgroundEnabled  
	{
		get 
		{
			int v = LugusConfig.use.Get<int>( "audio.music", 1 );
			return ( v == 1);
		}
		
		set
		{
			//Debug.LogError("LuGusAudio : set bg enabled " + value);
			
			LugusConfig.use.Set( "audio.music", ((value) ? 1 : 0) );
			if( value )
			{
				backgroundSource.volume = 0.25f;
			}
			else
			{
				this.StopAllCoroutines();
				backgroundSource.volume = 0.0f;
			}
		}
	}
	
	public bool EffectsEnabled
	{  
		get
		{
			int v = LugusConfig.use.Get<int>( "audio.effects", 1 );
			return ( v == 1);
		}
		
		set
		{
			LugusConfig.use.Set( "audio.effects", ((value) ? 1 : 0) );
			if( value )
				oneShotSource.volume = 1.0f;
			else
				oneShotSource.volume = 0.0f;
		}
	}
	
	public bool IsOneShotPlaying()
	{
		if (oneShotSource != null)
			return oneShotSource.isPlaying;
		
		return false;
	}
	
	public void StopOneShot()
	{
		oneShotSource.Stop();
	}
	
	public void StopBackground()
	{
		backgroundSource.Stop();
	}
	
	public void StopAll()
	{
		oneShotSource.Stop();
		backgroundSource.Stop();
	}
	
	public void PauzeAll()
	{
		oneShotSource.Stop();
		backgroundSource.Pause();
	}
	
	public void PlayAll()
	{
		backgroundSource.Play();
	}
	
	public bool ToggleAll()
	{
		oneShotSource.Stop();
		
		audioEnabled = !audioEnabled;
		
		if( audioEnabled )
		{
			backgroundSource.Play();
		}
		else
		{
			backgroundSource.Pause();
		}
		
		return audioEnabled;
	}
	
	public bool audioEnabled = true;
	
}