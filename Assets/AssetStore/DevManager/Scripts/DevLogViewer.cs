/// <summary>
/// DevLog
/// For developer debug purpose, On program enable will copy all message form
/// for system default function "Debug.Log()", and Provided the following function.
/// 	1) Filtering by each Debug log's stackTrace
/// 	2) Toggle for more detail of stack trace.
/// 	3) Record time in milliseconds for each message
/// 	4) Color highlight for different warning level.
/// 	5) Log length control (Delete the oldest & out of range data)
/// 	6) Collapse same message
/// 	7) Auto scroll to the last message (when new log append)
/// 	8) Clean log :D
/// 
/// By Canis 20130406
/// </summary>

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevelopManager
{
	public class DevLogViewer : MonoBehaviourDev
	{
		private struct ConsoleMessage
		{
			public readonly string  message;
			public readonly string  stackTrace;
			public readonly string tag;
			public readonly LogType	type;
			public bool displayDetail;
			public DateTime recTime;
			
			public ConsoleMessage (string message, string stackTrace, LogType type)
			{
				this.message    	= message;
				// incoming stackTrace may fail where build project.
				if( stackTrace.Length==0 )
					this.stackTrace = StackTraceUtility.ExtractStackTrace();
				else
					this.stackTrace	= stackTrace;
				this.tag			= FindTag(this.stackTrace);
				this.type       	= type;
				this.displayDetail	= false;
				this.recTime		= DateTime.UtcNow;

			}
		}
		private struct FilterFunc
		{
			public readonly string tag;
			public bool visible;
			public FilterFunc(string _tag)
			{
				this.tag = _tag;
				try
				{
					if( PlayerPrefs.HasKey(_tag) )
					{	// read Prefs to get the last Filter setting. last program running status.
						this.visible = ((PlayerPrefs.GetInt(_tag)==1)?true:false);
					}
					else
					{	// the very first time we have this function.
						this.visible = true;
					}
				}
				catch(Exception)
				{	// In case it's Unknow Threading, we just display it this time.
					this.visible = true;
				}
			}
		}
		private Vector2 mScrollPos = Vector2.zero;
		private Vector2 mFilterPos = Vector2.zero;
		private bool mCollapse = false;
		private List<ConsoleMessage> logHistory = new List<ConsoleMessage>();
		private List<FilterFunc> logFilter = new List<FilterFunc>();
		public string mMaxLogLengthStr = "100";
		private float mTotalScrollHeight = 0;
		private bool mUpdateLog = false;
		private bool mAutoScroll = true;
		private bool mTimePanel = false;
		private bool mFilterList = false;
		private bool mErrorToggle = true;
		private bool mWarnToggle = true;
		private bool mNormalToggle = true;
		private GUIContent mTimeLabel		= new GUIContent("Time", "Time control panel.");
		private GUIContent mClearLabel		= new GUIContent("Clear", "Clear the contents of the console.");
		private GUIContent mCollapseLabel	= new GUIContent("Collapse", "Hide repeated messages.");
		private GUIContent mAutoScrollLabel	= new GUIContent("Auto Scroll", "Auto Scroll to the end.");
		private GUIContent mFilterListLabel	= new GUIContent("Filter","show/hide the process debug log");
		private GUIContent mWarnLabel = new GUIContent("Warn","show/hide the warning level debug log");
		private GUIContent mErrorLabel = new GUIContent("Error","show/hide the warning level debug log");
		private GUIContent mNormalLabel = new GUIContent("Normal","show/hide the normal level debug log");
		private GUIStyle mLineStyle = new GUIStyle();
		private GUIStyle mDetailStyle = new GUIStyle();
		private Texture2D mBlankImg;
		private Texture2D mWarnImg;
		private Texture2D mErrorImg;
		private Texture2D mLightImg;
		private Texture2D mDarkImg;
		private double clickTime;
		private double doubleClickTime=0.3f;
		

		/// <summary>
		/// Raises the enable event.
		/// </summary>
		public override void OnEnable ()
		{
			base.OnEnable();
			mLineStyle.border = new RectOffset(0,0,0,0);
			mLineStyle.padding = new RectOffset(20,20,5,5);
			mLineStyle.margin = new RectOffset(0,0,0,0);
			mLineStyle.alignment = TextAnchor.UpperLeft;
			mBlankImg = FillColor(UnityEngine.Color.clear);
			mWarnImg = FillColor(UnityEngine.Color.yellow);
			mErrorImg = FillColor(UnityEngine.Color.red);
			mLightImg = FillColor(new UnityEngine.Color(0.0f,0.0f,0.0f,0.3f));
			mDarkImg = FillColor(UnityEngine.Color.black);
			mDetailStyle.border = new RectOffset(0,0,0,0);
			mDetailStyle.padding = new RectOffset(50,20,10,10);
			mDetailStyle.margin = new RectOffset(0,0,0,0);
			mDetailStyle.alignment = TextAnchor.UpperLeft;
			mDetailStyle.normal.textColor = UnityEngine.Color.green;
			mDetailStyle.normal.background = mLightImg;
			// Application.RegisterLogCallback(HandleDefaultLog);
			Application.RegisterLogCallbackThreaded(HandleDefaultLog);
			Debug.Log ("DevLog : System online");
		}
		/// <summary>
		/// Raises the disable event.
		/// </summary>
		public override void OnDisable()
		{
			base.OnDisable();
			Debug.Log ("DevLog : Shuting down");
			Application.RegisterLogCallback(null);
		}
		
		/// <summary>
		/// Logged messages are sent through this callback function.
		/// </summary>
		/// <param name="message">The message itself.</param>
		/// <param name="stackTrace">A trace of where the message came from.</param>
		/// <param name="type">The type of message: error/exception, warning, or assert.</param>
		public void HandleDefaultLog (string condition, string stackTrace, LogType type)
		{
			ConsoleMessage _entry = new ConsoleMessage(condition, stackTrace, type);
			// ignore this "GUI Error: You are pushing more GUIClips than you are popping. Make sure they are balanced)"GUI Error: You are pushing more GUIClips than you are popping. Make sure they are balanced)
			if( !condition.StartsWith("GUI Error") )
			{
				mUpdateLog = true;
				logHistory.Add(_entry);
				
				if( !logFilter.Exists(
					delegate(FilterFunc _list){ return _list.tag == _entry.tag; })
					)
				{
					FilterFunc _func = new FilterFunc(_entry.tag);
					logFilter.Add (_func);
				}
				if( logHistory.Count > int.Parse(mMaxLogLengthStr) )
				{
					logHistory.RemoveRange(0, (logHistory.Count-int.Parse(mMaxLogLengthStr)));
				}
			}
		}
		/// <summary>
		/// Handles the GUI by DevManager's OnGUI.
		/// This function will registe by DevManager's Awake().
		/// </summary>
		public override void HandleDebugGUI ()
		{
			GUILayout.BeginHorizontal();
			DrawToolBar();
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal();
			if( mFilterList ) drawFilterList();
			DrawLog();
			if( mTimePanel ) TimeControl();
			GUILayout.EndHorizontal();
			DrawBottomPanel();
			
			if( mAutoScroll && mUpdateLog )
			{
				mScrollPos = new Vector2(0, mTotalScrollHeight);
			}
			mUpdateLog = false;
		}
		private void drawFilterList()
		{
			mFilterPos = GUILayout.BeginScrollView(mFilterPos,
			                                       GUILayout.MaxWidth(150.0f),
			                                       GUILayout.MinWidth(50.0f));
			for (int i = 0; i < logFilter.Count; i++)
			{
				FilterFunc _tmp = new FilterFunc(logFilter[i].tag);
				bool _onChange = logFilter[i].visible;
				_tmp.visible = GUILayout.Toggle(logFilter[i].visible, logFilter[i].tag.ToString(), GUILayout.ExpandWidth(false));
				logFilter[i] = _tmp;
				if( _onChange!=logFilter[i].visible )
				{
					// change detected. save in Prefs
					PlayerPrefs.SetInt(logFilter[i].tag, ((logFilter[i].visible)?1:0) );
					PlayerPrefs.Save();
				}
			}
			GUILayout.EndScrollView();
		}
		private void DrawToolBar()
		{
			// To do filter
			mFilterList = GUILayout.Toggle(mFilterList, mFilterListLabel, GUILayout.ExpandWidth(false));
			
			GUILayout.FlexibleSpace();
			
			// Max log length
			{
				StringBuilder textFieldTemp = new StringBuilder();
				foreach( char c in mMaxLogLengthStr )
				{
					if( char.IsDigit(c) )
						textFieldTemp.Append(c);
				}
				GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
				{
					GUILayout.Label("Max Length");
					GUI.SetNextControlName("DevLog_MaxLength");
					mMaxLogLengthStr = GUILayout.TextField(textFieldTemp.ToString(), 5);
					if ( GUI.GetNameOfFocusedControl() == "DevLog_MaxLength" &&
					    (
						Event.current.keyCode == KeyCode.Escape ||
						Event.current.keyCode == KeyCode.KeypadEnter ||
						Event.current.keyCode == KeyCode.Return ||
						Event.current.keyCode == KeyCode.Tab
						) )
						GUIUtility.keyboardControl = 0;
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.Space(10f);
			// Clear button
			if (GUILayout.Button(mClearLabel,GUILayout.ExpandWidth(false))) {
				logHistory.Clear();
			}
			GUILayout.FlexibleSpace();
			// Normal
			mNormalToggle = GUILayout.Toggle(mNormalToggle, mNormalLabel, GUILayout.ExpandWidth(false));
			// Warning
			mWarnToggle = GUILayout.Toggle(mWarnToggle, mWarnLabel, GUILayout.ExpandWidth(false));
			// Error
			mErrorToggle = GUILayout.Toggle(mErrorToggle, mErrorLabel, GUILayout.ExpandWidth(false));
			
			// Collapse toggle
			mCollapse = GUILayout.Toggle(mCollapse, mCollapseLabel, GUILayout.ExpandWidth(false));
			
			// Auto Scroll
			mAutoScroll = GUILayout.Toggle(mAutoScroll, mAutoScrollLabel, GUILayout.ExpandWidth(false));
			GUILayout.FlexibleSpace();
			

			mTimePanel = GUILayout.Toggle(mTimePanel, mTimeLabel,GUILayout.ExpandWidth(false));
		}
		private void DrawLog()
		{
			mScrollPos = GUILayout.BeginScrollView(mScrollPos);
			{
				mTotalScrollHeight = 0.0f;
				// Go through each logged entry
				for (int i = 0; i < logHistory.Count; i++)
				{
					ConsoleMessage entry = logHistory[i];
					// If this message is the same as the last one and the collapse feature is chosen, skip it
					if (mCollapse && i > 0 && entry.message == logHistory[i - 1].message)
					{
						continue;
					}
					// If Filter have been hidden this type.
					else if( !logFilter.Find(delegate(FilterFunc _list){ return _list.tag == entry.tag; }).visible )
					{
						continue;
					}
					else if( !mNormalToggle && (logHistory[i].type == LogType.Log || logHistory[i].type == LogType.Assert) ) continue;
					else if( !mErrorToggle && (logHistory[i].type == LogType.Exception || logHistory[i].type == LogType.Error) ) continue;
					else if( !mWarnToggle && (logHistory[i].type == LogType.Warning) ) continue;
					
					// find out the total line height & print the message.
					mTotalScrollHeight += AddNewLine(i);
				}
			}
			GUILayout.EndScrollView();
		}

		private void DrawBottomPanel ()
		{
			GUILayout.Label (GUI.tooltip);
		}
		private void TimeControl()
		{
			GUILayout.BeginVertical(GUILayout.Width(80f));
			GUILayout.Space(10f);
			if (GUILayout.Button ("Pause", GUILayout.Width (80)))
				Time.timeScale = 0f;
			GUILayout.Space (5);
			if (GUILayout.Button ("Resume", GUILayout.Width (80)))
				Time.timeScale = 1f;
			GUILayout.Space (20);
			if (GUILayout.Button ("Speed x 0.1", GUILayout.Width (80)))
				Time.timeScale = 0.18f;
			GUILayout.Space (5);
			if (GUILayout.Button ("Speed x 0.5", GUILayout.Width (80)))
				Time.timeScale = 0.5f;
			GUILayout.Space (20);
			if (GUILayout.Button ("Speed x 2", GUILayout.Width (80)))
				Time.timeScale = 2f;
			GUILayout.Space (5);
			if (GUILayout.Button ("Speed x 4", GUILayout.Width (80)))
				Time.timeScale = 4f;
			GUILayout.Space (5);
			GUILayout.EndVertical();
		}

		private float AddNewLine(int _i)
		{
			float _lineHeight = 0.0f;
			mLineStyle.normal.background = mBlankImg;
			mLineStyle.hover.background = mBlankImg;
			mLineStyle.active.background = mBlankImg;
			mLineStyle.focused.background = mBlankImg;
			// Change the text colour according to the log type
			switch (logHistory[_i].type)
			{
				case LogType.Error:
				case LogType.Exception:
				mLineStyle.normal.textColor = UnityEngine.Color.red;
				mLineStyle.hover.textColor = UnityEngine.Color.red;
				mLineStyle.hover.background = mLightImg;	
				mLineStyle.active.textColor = UnityEngine.Color.white;
				mLineStyle.active.background = mErrorImg;
					break;
				case LogType.Warning:
				mLineStyle.normal.textColor = UnityEngine.Color.yellow;
				mLineStyle.hover.textColor = UnityEngine.Color.yellow;	
				mLineStyle.hover.background = mLightImg;
				mLineStyle.active.textColor = UnityEngine.Color.black;
				mLineStyle.active.background = mWarnImg;
					break;
				default:
				mLineStyle.normal.textColor = new UnityEngine.Color(1.0f,1.0f,1.0f,0.8f);
				mLineStyle.hover.textColor = UnityEngine.Color.white;
				mLineStyle.hover.background = mLightImg;
				mLineStyle.active.textColor = UnityEngine.Color.cyan;
				mLineStyle.active.background = mDarkImg;
					break;
			}
			GUIContent _content = new GUIContent(logHistory[_i].message, logHistory[_i].tag +
			                                     " @ Time : "+ logHistory[_i].recTime.ToLocalTime().ToShortTimeString()+
			                                     " "+ logHistory[_i].recTime.Second.ToString()+
			                                     "."+ logHistory[_i].recTime.Millisecond.ToString()+"sec");
			_lineHeight = mLineStyle.CalcHeight(_content,Screen.width) +
				mLineStyle.padding.top +
				mLineStyle.padding.bottom +
				mLineStyle.margin.top +
				mLineStyle.margin.bottom;
			
			// Button of Log & on click action
			if( GUILayout.Button(_content, mLineStyle) && IsDouble() )
			{
				ConsoleMessage _tmp = logHistory[_i];
				_tmp.displayDetail = !_tmp.displayDetail;
				logHistory[_i] = _tmp;
			}
			
			// display detail stackTrace;
			if( logHistory[_i].displayDetail )
			{
				
				GUIContent _stackTrace = new GUIContent(logHistory[_i].stackTrace);
				_lineHeight += mDetailStyle.CalcHeight(_stackTrace,Screen.width) +
					mDetailStyle.padding.top +
					mDetailStyle.padding.bottom +
					mDetailStyle.margin.top +
					mDetailStyle.margin.bottom;
				GUILayout.Box(_stackTrace,mDetailStyle);
			}
			
			return _lineHeight;
		}
		private bool IsDouble()
		{
			bool _rst = Time.time-clickTime<doubleClickTime;
			clickTime=Time.time;
			return _rst;
		}
		private static string FindTag(string _stackTrace)
		{
			string[] _strLine = _stackTrace.Split('\n');
			int _num = _strLine.Length-1;	// delete end of line;
			if( _num < 1 )
			{
				return "Unknow"; // just in case
			}
			
			// Last Function Tag.
			//***
			int _last = _num;
			while( _last>=0 )
			{
				string _tmp = _strLine[_last].ToString();
				if ( _tmp.Length > 0 )
				{
					// Found Last line, 
					// then find the trigger function name as "tag".
					string[] _FirstTag = _strLine[_last].ToString().Split (new char[]{'.',':'});
					return _FirstTag[0].ToString();
				}
				_last--;
			}
			//**/
			
			// First but not Debug.Log
			/***
			int _last = 0;
			for(; _last<_num; _last++)
			{
				string _tmp = _strLine[_last].ToString();
				if ( _tmp.Length > 0 )
				{
					// find the trigger function name as "tag".
					string _line = _strLine[_last].ToString();
					if( _line.StartsWith("UnityEngine.Debug") )
					{	// Just not the Debug.Log itself. next
						continue;
					}
					string[] _FirstTag = _line.Split(new char[]{'.',':'});
					string _tag = _FirstTag[0];
					return _tag.ToString();
				}
			}
			//**/
			return "Unknow";
		}
		public Texture2D FillColor(UnityEngine.Color _bgColor)
		{
			Texture2D _bg = new Texture2D(1,1);
			_bg.SetPixel(1,1, _bgColor);
			_bg.Apply();
			return _bg;
		}
	}//class
}//namespace