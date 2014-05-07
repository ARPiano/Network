/// <summary>
/// DevManager
/// For developer debug purpose, which can display the custorm Debug GUI panel.
/// 
/// 
/// Usage :
/// 	Call "DevManager.reg(xxxxx) to easily regist custorm GUI panel under DevManager control
/// Version 1.2
/// Ref :
/// 
/// Version 1.1
/// Ref :
/// 	by default will register DevLog As default Unity3D debug console. 
/// 
/// By Canis 20130406
using UnityEngine;
using System.Collections.Generic;

namespace DevelopManager
{
	/// <summary>Develop Manager.</summary>
	/// <remarks>Developed by Canis Wong (www.clonefactor.com)</remarks>
	public class DevManager : DevSingleTon<DevManager>
	{
		#region -- variable
		const int PANEL_HEIGHT = 75;
		const float ICON_SIZE = 32f;

		public Texture2D logoIcon;
		public bool visible = false;
		private string version = "DevManager GUI ver 1.1";

		[System.Serializable]
		public class DevSetting
		{
			public bool AllowTriggerByKey = true;
			public KeyCode TriggerMenuKey = KeyCode.ScrollLock;
			public int WindowStartAddress = 1000;
		}
		public DevSetting devSetting;  
		
		public GUISkin DockSkin;
		public GUISkin WinSkin;

		private Rect mDockPos;
		private Vector2 mDockScroll = Vector2.zero;
		private List<DevWindowTask> mWinTask = new List<DevWindowTask>();
		public int AvaibleWindowID{
			get{ return (mWinTask.Count+devSetting.WindowStartAddress); }
		}
		#endregion
		#region -- public function
		/// <summary>Display DevManager Interface</summary>
		public void Show(){visible = true;}
		/// <summary>Hide DevManager Interface</summary>
		public void Hide(){visible = false;}
		/// <summary>Toggle DevManager Interface</summary>
		public void Toggle(){visible=!visible;}
		
		/// <summary>
		/// Reg the specified debug GUI for any other function.
		/// simply using "Debug.Log" for debug.
		/// </summary>
		/// <param name="_instanceID">GetInstanceID.</param>
		/// <param name="_label">label of window you want to display.</param>
		/// <param name="_defaultPos">_default position & size of the window</param>
		/// <param name="_callback">_callback OnGUI() function for your OWN debug panel</param>
		/// <param name="_allowDrag">_allow drag of the window.</param>
		public DevWindowTask Register(int _instanceID, string _label, Rect _defaultPos, DrawGUI _callback, bool _allowDrag)
		{
			DevWindowTask _win = new DevWindowTask(AvaibleWindowID,_instanceID,_label,_defaultPos,_callback);
			_win.allowDrag = _allowDrag;
			_win.visible = CheckLastPrefsVisible(_label);
			mWinTask.Add (_win);
			return _win;
		}
		/// <summary>
		/// Reg the specified debug GUI for any other function.
		/// simply using "Debug.Log" for debug.
		/// </summary>
		/// <param name="_instanceID">GetInstanceID.</param>
		/// <param name="_label">label of window you want to display.</param>
		/// <param name="_callback">_callback OnGUI() function for your OWN debug panel</param>
		/// <param name="_allowDrag">_allow drag of the window.</param>
		public DevWindowTask Register(int _instanceID, string _label, DrawGUI _callback, bool _allowDrag)
		{
			return this.Register(_instanceID,_label,new Rect(0,0,Screen.width,Screen.height),_callback,_allowDrag);
		}
		/// <summary>
		/// Reg the specified debug GUI for any other function.
		/// simply using "Debug.Log" for debug.
		/// </summary>
		/// <param name="_instanceID">GetInstanceID.</param>
		/// <param name="_label">label of window you want to display.</param>
		/// <param name="_callback">_callback OnGUI() function for your OWN debug panel</param>
		public DevWindowTask Register(int _instanceID, string _label, DrawGUI _callback)
		{
			return this.Register(_instanceID,_label,_callback,false);
		}
		/// <summary>
		/// Un-Registe window task.
		/// </summary>
		/// <param name="_instanceID">GetInstanceID.</param>
		public void UnRegister(int _instanceID)
		{
			mWinTask.RemoveAll(delegate(DevWindowTask _obj){return _obj.instanceID==_instanceID;});
		}
		private bool CheckLastPrefsVisible(string _winLabel)
		{
			return PlayerPrefs.HasKey("DevGUI_"+_winLabel);
		}
		#endregion
		#region -- main thread
		public override void Awake ()
		{	// Link up debug console event
			base.Awake ();
			PlayerPrefs.SetString("DevManager",version);
		}
		private void OnEnable(){}
		private void OnDisable()
		{
			mWinTask.Clear();
		}
		private void OnDestory(){}
		private void Start()
		{
			mDockPos = new Rect(0,0,Screen.width,PANEL_HEIGHT);
		}
		private void Update ()
		{
			this.HandleKeyTrigger();
		}
		private void OnGUI()
		{
			if( !visible ) return;
			DrawDevManager();
		}
		#endregion
		#region -- tools
		private void DrawDevManager()
		{
			GUI.skin = DockSkin;
			mDockPos.width = Screen.width;
			mDockPos = GUILayout.Window( AvaibleWindowID, mDockPos, DrawDock, "");
			DrawRegistedWindowTask();
		}
		private void HandleKeyTrigger()
		{	// Triggers DevManager GUI by config key.
			if( devSetting.AllowTriggerByKey &&
			   Input.GetKeyUp(devSetting.TriggerMenuKey) )
			{
				// Only do following by TriggerMenuKey press
				this.Toggle();
				Debug.Log ("DevManager "+ ((visible)?"Enable":"Disable") );
			}
		}
		private void DrawDock(int id)
		{
			// update Screen size.
			GUILayout.BeginVertical();
			{
				// Logo
				GUILayout.Label(version);

				mDockScroll = GUILayout.BeginScrollView(mDockScroll,false,false);
				{
					GUILayout.BeginHorizontal();
					{
						if( logoIcon != null )
						{
							GUILayout.Label(logoIcon, GUILayout.MaxWidth(ICON_SIZE), GUILayout.MaxHeight(ICON_SIZE) );
							GUILayout.Space(10f);
						}
						// all registed window's toggle button
						for(int i=0; i<mWinTask.Count; i++)
						{
							bool _tmp = mWinTask[i].visible;
							// Toggle button for each regist window
							mWinTask[i].visible = GUILayout.Toggle(mWinTask[i].visible, new GUIContent(mWinTask[i].label), GUILayout.ExpandWidth(false) );
							if( _tmp != mWinTask[i].visible )
							{
								// developer chanage debugGUI show/hide
								string _winKey = "DevGUI_"+mWinTask[i].label;
								if( mWinTask[i].visible && !PlayerPrefs.HasKey(_winKey) )
								{
									PlayerPrefs.SetInt(_winKey, 1 );
								}
								else if( !mWinTask[i].visible && PlayerPrefs.HasKey(_winKey) )
								{
									PlayerPrefs.DeleteKey(_winKey);
								}
								PlayerPrefs.Save();
							}
							GUILayout.Space(10.0f);
						}
					}
					GUILayout.EndHorizontal();
				}
				GUILayout.EndScrollView();
			}
			GUILayout.EndVertical();
		}
		private void DrawRegistedWindowTask()
		{
			GUI.skin = WinSkin;
			for(int i=0; i<mWinTask.Count; i++)
			{
				DevWindowTask _task = mWinTask[i];
				// when it's going to visible.
				if( _task.visible )
				{
					// Auto Full Screen
					if( _task.autoFullScreen )
					{	// when Full Screen
						_task.defaultPos.width = Screen.width;
						_task.defaultPos.height = Screen.height-PANEL_HEIGHT;
					}//if
					// When Developer design. not allow to Overlap DevManager panel.
					if( _task.defaultPos.y < PANEL_HEIGHT ) _task.defaultPos.y = PANEL_HEIGHT;
					if( _task.defaultPos.height+PANEL_HEIGHT > Screen.height )
					{
						_task.defaultPos.height = Screen.height-PANEL_HEIGHT;
					}
					// Allow Drag window or not
					if( _task.allowDrag )
					{
						_task.defaultPos = ClampToScreen(GUI.Window( _task.windowID, _task.defaultPos, _task.UpdateGUI, ""));
					}
					else
					{
						GUILayout.Window( _task.windowID, _task.defaultPos, _task.UpdateGUI,"");
					}//if
				}
				mWinTask[i] = _task;
			}
		}
		private Rect ClampToScreen(Rect _area)
		{
			_area.x = Mathf.Clamp(_area.x,0,Screen.width-_area.width);
			_area.y = Mathf.Clamp(_area.y,0,Screen.height-_area.height);
			return _area;
		}
		#endregion

	}//class
}// namespace