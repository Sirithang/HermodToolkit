using UnityEngine;
using System.Collections;

public class Base : MonoBehaviour
{
    public Vector2 coord { get { return _coord; } }

    protected Vector2 _coord;
    protected Tilemap _map;
    protected Sprite _sprite;

    static public Base SpawnBaseAt(int x, int y, Tilemap map, Sprite spriteBase)
    {
        GameObject obj = new GameObject("base");
        Base b = obj.AddComponent<Base>();

        b.InitBase(x, y, map, spriteBase);

        return b;
    }

    protected void InitBase(int x, int y, Tilemap map, Sprite spriteBase)
    {
        _sprite = spriteBase;
        _sprite.transform.parent = transform;
        _sprite.transform.localPosition = new Vector3(0, 0, 0);

        _map = map;
        _coord = new Vector2(x, y);

        Vector3 pos = map.PositionOfCase(x, y);
        pos.z = -1;
        transform.position = pos;
    }

    public bool isOnBase(Vector3 position)
    {
        return Mathf.Approximately(position.x, transform.position.x) && Mathf.Approximately(position.y, transform.position.y);
    }
}