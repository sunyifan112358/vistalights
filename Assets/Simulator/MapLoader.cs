﻿using UnityEngine;
using System.Collections;

public class MapLoader : MonoBehaviour {

	// Use this for initialization
	void Start () {
		// Load map
		
		MapController mapController = GameObject.Find("Map").GetComponent<MapController>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
