using System;
using UnityEngine;
using System.Collections.Generic;

namespace GridRPG
{
	public class MapControl : MonoBehaviour
	{
        private const float RESIZE_RATE = 2f;

		public float dragSpeed = 1;
	    private Vector3 dragOrigin;
		private Vector3 mapOrigin;
		//bool mouseDown = false;
	 
	 
	    void Update()
	    {
            float scroll = Input.mouseScrollDelta.y;
            if (scroll != 0)
            {
                //Debug.Log("Scroll = " + (float)Math.Pow(RESIZE_RATE, scroll));
                transform.localScale = new Vector3(transform.localScale.x * (float)Math.Pow(RESIZE_RATE, scroll), transform.localScale.y * (float)Math.Pow(RESIZE_RATE, scroll), transform.localScale.z * (float)Math.Pow(RESIZE_RATE, scroll));

            }
            if (Input.GetMouseButtonDown(1))
	        {
	            dragOrigin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				mapOrigin = transform.position;
				//Debug.Log((double)Screen.width/Screen.currentResolution.width+","+(double)Screen.height/Screen.currentResolution.height);
				//Debug.Log("Map Origin: ("+mapOrigin.x+","+mapOrigin.y+")");
				//Debug.Log("Drag Origin: ("+dragOrigin.x+","+dragOrigin.y+")");
	            return;
	        }
	 
	        if (!Input.GetMouseButton(1)) return;
	 		
			//Position map
	        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - dragOrigin;
	        Vector3 newPos = new Vector3((pos.x) +mapOrigin.x, pos.y +mapOrigin.y,GridRPG.Map.layer);
			transform.position = newPos;

            
            //transform.localScale = new Vector3(transform.localScale.x * (float)Math.Pow(2,scroll), transform.localScale.y * (float)Math.Pow(2, scroll), transform.localScale.z * (float)Math.Pow(2, scroll));

        }
	}
}

