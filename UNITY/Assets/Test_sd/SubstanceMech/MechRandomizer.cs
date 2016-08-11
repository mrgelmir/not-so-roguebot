using UnityEngine;
using System.Collections;

public class MechRandomizer : MonoBehaviour {

	public float rotateSpeed = 1f;
	private ProceduralMaterial mat;
	
	protected void Start()
	{
		mat = GetComponent<Renderer>().material as ProceduralMaterial;

		StartCoroutine(RandomizeRoutine());
	}

	private IEnumerator RandomizeRoutine()
	{
		for(;;)
		{
			Randomize();
			yield return new WaitForSeconds(Random.Range(3f, 5f));
		}
	}

	public void Randomize()
	{
		mat.SetProceduralFloat("Dirt", Random.Range(0f, 1f));
		mat.SetProceduralFloat("Hue", Random.Range(0f, 1f));
		mat.SetProceduralFloat("Saturation", Random.Range(0f, 1f));

		mat.SetProceduralColor("LensColor", new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));

		mat.RebuildTextures();
	}

	void Update ()
	{
		// Temp rotate model
		transform.Rotate(Vector3.forward, Time.deltaTime * rotateSpeed);
	}
}
