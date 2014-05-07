using UnityEngine;
using System;
using System.Collections;

using DevelopManager;
/// <summary>MonoBehaviour for DevManager</summary>
public class MonoBehaviourDev : MonoBehaviour
{
	[System.Serializable]
	public class DevWinConfig
	{
		public string DevFunctionLabel = string.Empty;
		public Rect WindowPos = new Rect();
	}
	public DevWinConfig winConfig;

	private object mHandle = new object();	// used for GUI draw lock.

	private bool DevWindowMoveable = true;
	private bool DevManagerExist = false;
	private bool DevDisplayGUI = false;
	private DevWindowTask mTask;
	
	public virtual void OnEnable()
	{
		if( winConfig.DevFunctionLabel.Equals(string.Empty) )
		{	// If you don't define the label, I make one for you.
			winConfig.DevFunctionLabel = this.name.ToString();
		}
		if( PlayerPrefs.HasKey("DevManager") )
		{
			DevManagerExist = true;
			if( winConfig.WindowPos.Equals(new Rect()) )
			{	// full screen
				mTask = DevelopManager.DevManager.Instance.Register(this.GetInstanceID(), winConfig.DevFunctionLabel, HandleDebugGUI, false);
			}
			else
			{
				mTask = DevelopManager.DevManager.Instance.Register(this.GetInstanceID(), winConfig.DevFunctionLabel, winConfig.WindowPos, HandleDebugGUI, DevWindowMoveable);
			}
			Debug.Log("\""+PlayerPrefs.GetString("DevManager")+"\" detected - "+winConfig.DevFunctionLabel+" registed.");
			mTask.onFocusCallback += OnFocus;
			mTask.onBlurCallback += OnBlur;
		}
	}
	public virtual void OnDisable()
	{
		if( DevManagerExist )
		{
			Debug.Log (winConfig.DevFunctionLabel + " UnRegister.");
			DevelopManager.DevManager.Instance.UnRegister(this.GetInstanceID());
			mTask.onFocusCallback -= OnFocus;
			mTask.onBlurCallback -= OnBlur;
		}
	}
	public virtual void OnGUI()
	{
		if( !DevManagerExist && DevDisplayGUI )
		{
			lock( mHandle )
			{
				try{ HandleDebugGUI(); }
				catch(ArgumentException){}
			}
		}
	}
	/// <summary>override this function and do your own GUI over here.</summary>
	/// DevManager will handle the rest.
	public virtual void HandleDebugGUI(){}

	/// <summary>Raises the focus event of GUI, handle by DevWindowTask.</summary>
	public virtual void OnFocus(){}
	/// <summary>Raises the blur event of GUI, handle by DevWindowTask.</summary>
	public virtual void OnBlur(){}
}