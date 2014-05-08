using UnityEngine;
using System.Collections;

public class Keys : MonoBehaviour,IVirtualButtonEventHandler  {

	ClientMain cm;
	public GameObject network;

	void Start () {
		VirtualButtonBehaviour [] btns = transform.GetComponentsInChildren<VirtualButtonBehaviour> ();
		foreach (VirtualButtonBehaviour btn in btns)
		{
			btn.RegisterEventHandler(this);
		}
		if(network!=null){
			cm = network.GetComponent<ClientMain>();
		}
	}

	public void OnButtonPressed(VirtualButtonAbstractBehaviour vb)
	{
		vb.audio.Play();
		if(cm!=null){
			cm.SendInfoToServer("press:"+vb.VirtualButtonName);
		}
	}
	public void OnButtonReleased(VirtualButtonAbstractBehaviour vb)
	{
		vb.audio.Stop();
		if(cm!=null){
			cm.SendInfoToServer("release:"+vb.VirtualButtonName);
		}
	}
}
