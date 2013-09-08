using UnityEngine;
using System.Collections;

public class Cursor : Base 
{
	// Update is called once per frame
	void Update () 
	{
        Vector3 pos = Camera.main.ViewportToWorldPoint(new Vector3(Input.mousePosition.x / Screen.width, (Input.mousePosition.y / Screen.height), Camera.main.nearClipPlane));
        //Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _coord = _map.FindCaseAt(pos);

        pos = _map.PositionOfCase((int)_coord.x, (int)_coord.y);
        pos.z = -1.5f;
        transform.position = pos;
	}

    static public Cursor CreateCursor(Tilemap world)
    {
        GameObject obj = new GameObject("Cursor");
        Cursor c = obj.AddComponent<Cursor>();

        Sprite s = (Instantiate(Resources.Load("cursor")) as GameObject).GetComponent<Sprite>();
        c.InitBase(0, 0, world, s);

        return c;
    }
}