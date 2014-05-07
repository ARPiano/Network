using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DevDetectLeak : MonoBehaviourDev
{
	private bool sorting = false;
	public override void HandleDebugGUI()
	{
		Object[] _objects = FindObjectsOfType(typeof (UnityEngine.Object));
		Dictionary<string, int> _dictionary = new Dictionary<string, int>();
		
		for (int i = 0; i < _objects.Length; i++)
		{
			string key = _objects[i].GetType().ToString();
			if (_dictionary.ContainsKey (key))
				_dictionary [key]++;
			else
				_dictionary [key] = 1;
		}


		List<KeyValuePair<string, int>> _List = new List<KeyValuePair<string, int>>(_dictionary);

		GUILayout.Box("Object Watcher :");
		sorting = GUILayout.Toggle(sorting, "Sorting");
		if( sorting ) _List.Sort(delegate(KeyValuePair<string, int> firstPair,KeyValuePair<string, int> nextPair){
			return nextPair.Value.CompareTo((firstPair.Value));
		});
		
		for (int i = 0; i < _List.Count; i++)
		{
			GUILayout.Label (_List[i].Key + ": " + _List[i].Value);
		}
	}
}