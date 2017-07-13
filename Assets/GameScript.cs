using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameScript : MonoBehaviour {
	
	private UnityEngine.Object[] textures;
	
	//private GameObject testSpace;
	private GridRPG.Map map;
	//GameObject displaySpace;
	
	//private GridRPG.Core core;
	//public static List<GridRPG.Terrain> terrainList;
	
	// Use this for initialization
	void Start () {
		
		//Load Textures
		//textures = new List<UnityEngine.Object>();
		/*textures = Resources.LoadAll("Sprites",typeof(Texture2D));
		foreach (var t in textures)
        {
            Debug.Log(t.name);
        }*/	
		
		//testSpace = new GameObject("test");
		
		//GridRPG.Space testSpaceComponent = new GridRPG.Space(terrainList[0]);
		/*testSpace.AddComponent<SpriteRenderer>();
		testSpace.AddComponent<GridRPG.Space>();
		testSpace.GetComponent<GridRPG.Space>().setTerrain("void");
		testSpace.GetComponent<GridRPG.Space>().setTerrain("empty");
		testSpace.GetComponent<GridRPG.Space>().setTerrain("SomeMessedUpShit");
		testSpace.GetComponent<GridRPG.Space>().setTerrain("grass", Sprite.Create((Texture2D)Resources.Load("Sprites/Terrain/Grass",typeof(Texture2D)),new Rect(0.0f,0.0f,32.0f,32.0f),new Vector2(0.5f,0.5f)));
		*/
		
		map = new GridRPG.Map("Assets/Resources/Maps/MapA.xml");
		map.centerMapOnCamera(Camera.main);
		GridRPG.CampaignUnit testUnit= new GridRPG.CampaignUnit();
		map.addUnitToSpace(testUnit,4,4);
		//map.mapParent.transform.localScale=new Vector3(2,2,1);
		
		Sprite sprite = new Sprite();
		//Debug.Log(sprite.pixelsPerUnit);
		//testSpace.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		//map.mapParent.transform.localScale.Scale(new Vector3(2,2,1));
	}
	
	void LateUpdate(){
		//map.mapParent.transform.localScale.Scale(new Vector3(2,2,1));
	}
}
