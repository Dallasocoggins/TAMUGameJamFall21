using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformController : MonoBehaviour
{
	// How much time it spends between movements
	public float timeBetween;

	// What section starts as
	public int startingSection;

	// How long we have spent since the last point
	private float currentTime;

	// Keeps track of which set of points it is between
	// Acts as an index for points, where 0 represents the first section
	private int section;

	// Keeps track of the locations of each of the points in space
	private Vector3[] points;

	// The moving platform child
	private Transform platform;

	// Start is called before the first frame update
	void Start()
	{
		// A quick recap of the children here:
		// The gameObject at the top does not move
		// 1st child is the platform itself, moving around
		// The remaining children are gameObjects with just locations, so that we know the platform' path
		// We can assume that there are at least 2 of these "point" children
		points = new Vector3[transform.childCount - 1];
		platform = transform.GetChild(0);
		section = startingSection;

		for (int i = 1; i < transform.childCount; i++)
		{
			Transform point = transform.GetChild(i);
			points[i - 1] = point.position;
		}
	}

	// Update is called once per frame
	void Update()
	{
		currentTime += Time.deltaTime;
		if (currentTime < timeBetween)
		{
			// (currentTime / timeBetween) is 0 when at the first point and 1 when at the second point
			platform.position = points[section] * (1 - currentTime / timeBetween) + points[section + 1] * (currentTime / timeBetween);
		}
		else
		{
			section++;
			if (section >= points.Length - 1)
			{
				section = 0;
			}
			platform.position = points[section];
			currentTime = 0;
		}
	}

	// Since we are manually moving the position, and the player needs to know how fast the platform is moving, this is here
	public Vector3 Velocity()
	{
		// Basically copy/paste from above, execpt replaced the first currentTime with currentTime + Time.deltaTime
		return ((points[section] * (1 - (currentTime + Time.deltaTime) / timeBetween) + points[section + 1] * ((currentTime + Time.deltaTime) / timeBetween))
			 - (points[section] * (1 - currentTime / timeBetween) + points[section + 1] * (currentTime / timeBetween))) / Time.deltaTime;
	}
}
