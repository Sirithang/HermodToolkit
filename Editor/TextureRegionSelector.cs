using UnityEngine;
using UnityEditor;
using System.Collections;

public class TextureRegionSelector :  EditorWindow
{
    public Texture2D _target;

    public Rect _currentSelection;

    public Editor _owner;

    public bool isOpen { get { return _isopen; } }

    protected Vector2 _origin;
    protected bool _clicked = false;
    protected bool _isopen;

    void OnEnable()
    {
        _currentSelection = new Rect();
        _isopen = true;
    }

    void OnDestroy()
    {
        _isopen = false;
    }

    void OnGUI()
    {
        if (!_target || !_owner)
            return;


        minSize = new Vector2(_target.width, _target.height);
        maxSize = new Vector2(_target.width + 1, _target.height + 1);

        Event e = Event.current;

        if (e.type == EventType.MouseDown)
        {
            _clicked = true;
            _origin = e.mousePosition;
        }
        else if (e.type == EventType.MouseUp)
        {
            _clicked = false;
        }

        if (_clicked)
        {
            Vector2 pos = e.mousePosition;

            Vector2 min = Vector3.Min(pos, _origin);
            Vector2 max = Vector3.Max(pos, _origin);

            _currentSelection.x = min.x;
            _currentSelection.y = min.y;
            _currentSelection.width = max.x - min.x;
            _currentSelection.height = max.y - min.y;
        }

        GUI.DrawTexture(new Rect(0, 0, _target.width, _target.height), _target);
        Toolset.DrawGUIQuad(_currentSelection, Color.red);
        Repaint();
        _owner.Repaint();
    }
}