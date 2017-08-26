using System;
using UnityEngine;
using System.Collections.Generic;

namespace GridRPG
{
	public class MapControl : MonoBehaviour
	{
		public float dragSpeed = 1;
	    private Vector3 dragOrigin;
		private Vector3 mapOrigin;
		//bool mouseDown = false;
	 
	 
	    void Update()
	    {
	        if (Input.GetMouseButtonDown(0))
	        {
	            dragOrigin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				mapOrigin = transform.position;
				//Debug.Log((double)Screen.width/Screen.currentResolution.width+","+(double)Screen.height/Screen.currentResolution.height);
				//Debug.Log("Map Origin: ("+mapOrigin.x+","+mapOrigin.y+")");
				//Debug.Log("Drag Origin: ("+dragOrigin.x+","+dragOrigin.y+")");
	            return;
	        }
	 
	        if (!Input.GetMouseButton(0)) return;
	 		
			//Debug.Log("mouse: ("+Input.mousePosition.x+","+Input.mousePosition.y+")");
	        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - dragOrigin;
			//Debug.Log("pos: ("+pos.x+","+pos.y+")");
	        Vector3 newPos = new Vector3((pos.x) +mapOrigin.x, pos.y +mapOrigin.y,GridRPG.Map.layer);
			
			//Debug.Log("newpos: (" + move.x + ","+move.y+")");
	 		
			transform.position = newPos; 
	        //transform.Translate(move, UnityEngine.Space.World);  
			//dragOrigin = Input.mousePosition;
	    }
	}
}

