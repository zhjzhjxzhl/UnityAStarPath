using UnityEngine;
using System.Collections;

public class DragObj : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnMouseDrag()
    {
        Debug.Log(Input.mousePosition);
        Vector3 worldP = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        gameObject.transform.position = new Vector3(worldP.x, worldP.y, gameObject.transform.position.z);
    }
}
