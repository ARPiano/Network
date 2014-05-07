using UnityEngine;
using System.Collections;

namespace DevelopManager
{
	public delegate void DrawGUI();
	public delegate void OnForce();
	public delegate void OnBlur();

	/// <summary>
	/// Sub-class of DevManager used for save the window info.
	/// </summary>
	public class DevWindowTask
	{
		#region -- construct
		public int windowID;
		public int instanceID;
		public string label;
		public Rect defaultPos;
		public DrawGUI callback;
		public OnForce onFocusCallback;
		public OnBlur onBlurCallback;
		#endregion
		#region --optional




		public bool allowDrag		=false;
		public bool autoFullScreen	=false;
		public bool visible			=false;
		#endregion
		private Vector2 mScrollPos	=Vector2.zero;
		/// <summary>
		/// Initializes a new instance of the <see cref="DevelopManager.DevWindowTask"/> class.
		/// </summary>
		/// <param name="_windowID">_window ID.</param>
		/// <param name="_instanceID">_instance ID.</param>
		/// <param name="_label">the label of the instance.</param>
		/// <param name="_defaultPos">default position.</param>
		/// <param name="_callback">GUI callback.</param>
		public DevWindowTask(int _windowID, int _instanceID, string _label, Rect _defaultPos, DrawGUI _callback)
		{
			windowID = _windowID;
			instanceID = _instanceID;
			label = _label;
			defaultPos = _defaultPos;
			callback = _callback;
			visible = false;
			mScrollPos = Vector2.zero;
		}
		// Call a passed-in delegate function to draw GUI Window.
		public void UpdateGUI(int _id)
		{
			FocusChecker(_id,Event.current);
			if( _id != windowID ) return;
			mScrollPos = GUILayout.BeginScrollView(mScrollPos);
			callback();
			GUILayout.EndScrollView();
			if( allowDrag ) GUI.DragWindow(new Rect(0,0,defaultPos.width,defaultPos.height));
		}

		#region --Window Focus Checker
		static private int focusedWindowID=-1;	// Global value.
		private bool isfocus		=false;
		private void FocusChecker(int _id, Event _e)
		{
			if( focusedWindowID!=_id &&
			   _e.button==0 &&
			   _e.type==EventType.MouseUp )
			{
				focusedWindowID=_id;
				isfocus=true;
				if( onFocusCallback!=null ) onFocusCallback();
			}
			else if( isfocus && focusedWindowID!=_id )
			{
				isfocus=false;
				if( onBlurCallback!=null ) onBlurCallback();
			}
		}
		#endregion
	}
}
