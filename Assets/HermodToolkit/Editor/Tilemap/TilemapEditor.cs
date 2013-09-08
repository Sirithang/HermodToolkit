using UnityEngine;
using UnityEditor;
using System.Collections;

public class TilemapEditor :  EditorWindow
{
    public Tilemap editedTilemap;

    Texture2D texture;

    protected Vector2 _clickPos;
    protected bool _inDrag = false;
    protected bool _currentSwitch = false; // for the collision switcher, to avoid flickering

    protected struct TileSelection
    {
        public int startX, startY;
        public int width;
        public int height;
        public int[,] array;
    }

    protected TileSelection _currentSelection;

    //***

    public enum PaintTool
    {
        PENCIL,
        BUCKET,
        ERASER,
        COLLISION_SWITCHER,
        MAX_PAINTTOOL
    }

    protected string[] paintToolNames = {"Pencil", "Bucket", "Eraser", "Collision Switcher"};
    protected PaintTool _currentPaintTool = PaintTool.PENCIL;

    //***

    void OnEnable()
    {
        SceneView.onSceneGUIDelegate += SceneBypass;
        Tools.current = Tool.None;

        texture = new Texture2D(16, 16);
        texture.filterMode = FilterMode.Point;

        for (int i = 0; i < 16; ++i)
        {
            for (int j = 0; j < 16; ++j)
            {
                if (i < 2 || j < 2 || i > 13 || j > 13)
                {
                    texture.SetPixel(i, j, Color.red);
                }
                else
                {
                    texture.SetPixel(i, j, new Color(0, 0, 0, 0));
                }
            }
        }

        texture.Apply();

        wantsMouseMove = true;
    }

    void OnDisable()
    {
        SceneView.onSceneGUIDelegate -= SceneBypass;
        Tools.current = Tool.Move;
        DestroyImmediate(texture);
    }

    void DrawQuad(Rect position, Color color)
    {
        GUI.skin.box.normal.background = texture;
        GUI.Box(position, GUIContent.none);
    }


    void OnGUI()
    {
        int nbByLine = editedTilemap.spriteSheet.width / editedTilemap.tileSize;

        Rect textRect = new Rect(0,0, editedTilemap.spriteSheet.width, editedTilemap.spriteSheet.height);
        GUI.DrawTexture(textRect, editedTilemap.spriteSheet);


        Event e = Event.current;

        if (e.type == EventType.MouseDown)
        {
            if (e.button == 0)
            {
                _clickPos = e.mousePosition;

                if (_clickPos.y <= editedTilemap.spriteSheet.height)
                {
                    _currentSelection = new TileSelection();
                    _inDrag = true;
                }
            }
        }
        else if (Event.current.type == EventType.mouseUp)
        {
            if (e.button == 0)
            {
                _inDrag = false;
            }
        }

        if (_inDrag)
        {
            Vector2 pos = e.mousePosition;
            Vector2 min = Vector2.Min(pos, _clickPos);
            Vector2 max = Vector2.Max(pos, _clickPos);

            int startX = (int)(min.x / editedTilemap.tileSize);
            int startY = (int)((textRect.height - max.y) / editedTilemap.tileSize);
            int endX = (int)(max.x / editedTilemap.tileSize);
            int endY = (int)((textRect.height - min.y) / editedTilemap.tileSize);

            _currentSelection.width = (endX - startX) + 1;
            _currentSelection.height = (endY - startY) + 1;

            _currentSelection.startX = startX;
            _currentSelection.startY = startY;

            _currentSelection.array = new int[_currentSelection.width, _currentSelection.height];

            for (int i = 0; i < _currentSelection.width; ++i)
            {
                for (int j = 0; j < _currentSelection.height; ++j)
                {
                    _currentSelection.array[i, j] = (startX + i) + (startY + j) * nbByLine;
                }
            }
        }

        DrawQuad(new Rect(_currentSelection.startX * editedTilemap.tileSize, (editedTilemap.spriteSheet.height) - (_currentSelection.startY + _currentSelection.height) * editedTilemap.tileSize,
                                   _currentSelection.width * editedTilemap.tileSize, _currentSelection.height * editedTilemap.tileSize), Color.red);
        Repaint();
       

        //---------------------------------

       _currentPaintTool = (PaintTool)GUI.SelectionGrid(new Rect(0, editedTilemap.spriteSheet.height, position.width, 50), (int)_currentPaintTool, paintToolNames, (int)PaintTool.MAX_PAINTTOOL);

       if (_currentSelection.array != null)
       {
           GUI.Label(new Rect(0, editedTilemap.spriteSheet.height + 50, position.width, 20), "Current Tile selected : " + _currentSelection.array[0, 0] +
                              " to " + _currentSelection.array[_currentSelection.width - 1, _currentSelection.height - 1]);
       }
    }

    void SceneBypass(SceneView p_scn)
    {
        Sceneview2D scn = p_scn as Sceneview2D;

        Selection.activeGameObject = editedTilemap.gameObject;

        if (scn == null)
            return;

        minSize = new Vector2(editedTilemap.spriteSheet.width-1, editedTilemap.spriteSheet.height + 69);
        maxSize = new Vector2(editedTilemap.spriteSheet.width, editedTilemap.spriteSheet.height + 70);

        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        Event e = Event.current;

        Vector3 pos = e.mousePosition;
        Vector3 localPos = scn.camera.ScreenToWorldPoint(new Vector3(pos.x, scn.position.height - SceneView.kToolbarHeight - pos.y, 0)) - (editedTilemap.renderer.bounds.min);

        int x = (int)(localPos.x / editedTilemap.tileSize);
        int y = (int)(localPos.y / editedTilemap.tileSize);

        if (e.type == EventType.MouseDown)
        {
            if (_currentPaintTool == PaintTool.COLLISION_SWITCHER)
            {
                _currentSwitch = editedTilemap.IsPassable(x, y);
            }
        }
        else if (e.type == EventType.MouseDrag || e.type == EventType.MouseUp)
        {
            if (e.button == 0)
            {
                e.Use();

                if(_currentPaintTool == PaintTool.PENCIL)
                {
                    for (int i = 0; i < _currentSelection.width; ++i)
                    {
                        for (int j = 0; j < _currentSelection.height; ++j)
                        {
                            editedTilemap.setIndex(x + i, y + j, _currentSelection.array[i,j]);
                        }
                    }
                }
                else if(_currentPaintTool == PaintTool.ERASER)
                {
                    editedTilemap.setIndex(x,y,-1);
                }
                else if (_currentPaintTool == PaintTool.BUCKET)
                {
                    if(_currentSelection.array.Length > 0)
                        editedTilemap.FloodFill(x, y, _currentSelection.array[0, 0]);
                }
                else if (_currentPaintTool == PaintTool.COLLISION_SWITCHER)
                {
                    editedTilemap.SetCollision(x, y, _currentSwitch);
                }

                editedTilemap.MakeTilemap();
            }
        }


        Vector3[] vertex = null;

        if (_currentPaintTool == PaintTool.COLLISION_SWITCHER)
        {

            for (int i = 0; i < editedTilemap.width; ++i)
            {
                for (int j = 0; j < editedTilemap.height; ++j)
                {
                    if (!editedTilemap.IsPassable(i, j))
                    {
                        vertex = new Vector3[]{
                            new Vector3(editedTilemap.renderer.bounds.min.x + i * editedTilemap.tileSize, editedTilemap.renderer.bounds.min.y + j * editedTilemap.tileSize, 0),
                            new Vector3(editedTilemap.renderer.bounds.min.x + i * editedTilemap.tileSize + editedTilemap.tileSize,editedTilemap.renderer.bounds.min.y + j * editedTilemap.tileSize, 0),
                            new Vector3(editedTilemap.renderer.bounds.min.x + i * editedTilemap.tileSize + editedTilemap.tileSize,editedTilemap.renderer.bounds.min.y + j * editedTilemap.tileSize + editedTilemap.tileSize, 0),
                            new Vector3(editedTilemap.renderer.bounds.min.x + i * editedTilemap.tileSize, editedTilemap.renderer.bounds.min.y + j * editedTilemap.tileSize +  editedTilemap.tileSize, 0)};

                        Handles.DrawSolidRectangleWithOutline(vertex, new Color(0.5f, 0, 0, 0.5f), Color.red);
                    }
                }
            }
        }

        if (_currentPaintTool == PaintTool.PENCIL)
        {
            vertex = new Vector3[]{
                    new Vector3(editedTilemap.renderer.bounds.min.x + x * editedTilemap.tileSize, editedTilemap.renderer.bounds.min.y + y * editedTilemap.tileSize, 0),
                    new Vector3(editedTilemap.renderer.bounds.min.x + x * editedTilemap.tileSize + _currentSelection.width * editedTilemap.tileSize,editedTilemap.renderer.bounds.min.y + y * editedTilemap.tileSize, 0),
                    new Vector3(editedTilemap.renderer.bounds.min.x + x * editedTilemap.tileSize + _currentSelection.width * editedTilemap.tileSize,editedTilemap.renderer.bounds.min.y + y * editedTilemap.tileSize + _currentSelection.height * editedTilemap.tileSize, 0),
                    new Vector3(editedTilemap.renderer.bounds.min.x + x * editedTilemap.tileSize, editedTilemap.renderer.bounds.min.y + y * editedTilemap.tileSize + _currentSelection.height * editedTilemap.tileSize, 0)};
        }
        else
        {
            vertex = new Vector3[]{
                    new Vector3(editedTilemap.renderer.bounds.min.x + x * editedTilemap.tileSize, editedTilemap.renderer.bounds.min.y + y * editedTilemap.tileSize, 0),
                    new Vector3(editedTilemap.renderer.bounds.min.x + x * editedTilemap.tileSize + editedTilemap.tileSize,editedTilemap.renderer.bounds.min.y + y * editedTilemap.tileSize, 0),
                    new Vector3(editedTilemap.renderer.bounds.min.x + x * editedTilemap.tileSize + editedTilemap.tileSize,editedTilemap.renderer.bounds.min.y + y * editedTilemap.tileSize + editedTilemap.tileSize, 0),
                    new Vector3(editedTilemap.renderer.bounds.min.x + x * editedTilemap.tileSize, editedTilemap.renderer.bounds.min.y + y * editedTilemap.tileSize +  editedTilemap.tileSize, 0)};
        }

        Handles.color = Color.red;
        Handles.DrawSolidRectangleWithOutline(vertex, new Color(0, 0, 0.4f, 0.1f), Color.red);

       scn.Repaint();
                                        
    }
}