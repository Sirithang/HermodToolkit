using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Worldmap : MonoBehaviour 
{
    public Tilemap WorldTilemap;
    public Tilemap UpperTilemap;
    public Tilemap OverlayTilemap;

    public TextAsset LDText;

    //***************

    public struct WorldCase
    {
        public enum CaseType
        {
            GRASS,
            TREE,
            ROCK,
            CABANE
        };

        public CaseType _type;
        public bool _passable;
        public GameObject _obj;
    };

    //******************

    public WorldCase[,] _world;

    public Vector2 _startCase;

	// Use this for initialization
	[ContextMenu("ForceStart")]
    void Awake() 
    {
        StreamReader sr = new StreamReader(Application.streamingAssetsPath+"/LD.txt");
        string text = sr.ReadToEnd();

        string[] lines = text.Split('\n');
        string[] cases = lines[0].Split('\t');

        _world = new WorldCase[cases.Length, lines.Length];

        for (int j = 0; j < lines.Length; ++j)
        {
            cases = lines[j].Split('\t');
            for (int i = 0; i < cases.Length; ++i)
            {
                _world[i, lines.Length - 1 - j]._type = WorldCase.CaseType.GRASS;
                _world[i, lines.Length - 1 - j]._passable = true;
                _world[i, lines.Length - 1 - j]._obj = null;
                WorldTilemap.SetCollision(i, lines.Length - 1 - j, false);

                switch (cases[i])
                {
                    case "C":
                        _world[i, lines.Length - 1 - j]._type = WorldCase.CaseType.CABANE;
                        _world[i, lines.Length - 1 - j]._passable = false;
                        WorldTilemap.SetCollision(i, lines.Length - 1 - j, true);
                        break;
                    case "ROC":
                        _world[i, lines.Length - 1 - j]._type = WorldCase.CaseType.ROCK;
                        _world[i, lines.Length - 1 - j]._passable = false;
                        WorldTilemap.SetCollision(i, lines.Length - 1 - j, true);
                        break;
                    case "TREE":
                        _world[i, lines.Length - 1 - j]._type = WorldCase.CaseType.TREE;
                        _world[i, lines.Length - 1 - j]._passable = false;
                        WorldTilemap.SetCollision(i, lines.Length - 1 - j, true);
                        break;
                    case "P":
                        _startCase.x = i;
                        _startCase.y = lines.Length - 1 - j;
                        break;
                    case "OR":
                        _world[i, lines.Length - 1 - j]._obj = Grabbable.CreateOfType(Grabbable.GrabbableType.NORMALBASE);
                        break;
                    case "FLA":
                        _world[i, lines.Length - 1 - j]._obj = Grabbable.CreateOfType(Grabbable.GrabbableType.FLAMAND);
                        break;
                    case "BRA":
                        _world[i, lines.Length - 1 - j]._obj = Grabbable.CreateOfType(Grabbable.GrabbableType.SOUTIF);
                        break;
                    case "VEN":
                        _world[i, lines.Length - 1 - j]._obj = Grabbable.CreateOfType(Grabbable.GrabbableType.VENTILO);
                        break;
                    default:
                        _world[i, lines.Length - 1 - j]._type = WorldCase.CaseType.GRASS;
                        _world[i, lines.Length - 1 - j]._passable = true;
                        break;
                };

                if (_world[i, lines.Length - 1 - j]._obj != null)
                {
                    Vector3 p = WorldTilemap.PositionOfCase(i, lines.Length - 1 - j);
                    p.z = -0.5f;
                    _world[i, lines.Length - 1 - j]._obj.transform.position = p;
                }
            }
        }

        WorldTilemap.width = cases.Length;
        WorldTilemap.height = lines.Length;

        UpperTilemap.width = cases.Length;
        UpperTilemap.height = lines.Length;

        OverlayTilemap.width = cases.Length;
        OverlayTilemap.height = lines.Length;

        int currentCabaneSprite = 0;

        for (int j = 0; j < WorldTilemap.height; ++j)
        {
            for (int i = 0; i < WorldTilemap.width; ++i)
            {
                WorldTilemap.setIndex(i, j, 7);
                UpperTilemap.setIndex(i, j, -1);
                OverlayTilemap.setIndex(i, j, -1);

                switch (_world[i, j]._type)
                {
                    case WorldCase.CaseType.GRASS:
                        WorldTilemap.setIndex(i,j, Random.Range(6, 11));
                        break;
                    case WorldCase.CaseType.CABANE:
                        WorldTilemap.setIndex(i, j, currentCabaneSprite);
                        currentCabaneSprite += 1;
                        break;
                    case WorldCase.CaseType.ROCK:
                        UpperTilemap.setIndex(i, j, 3);
                        break;
                    case WorldCase.CaseType.TREE:
                        UpperTilemap.setIndex(i, j, 2);
                        break;
                    default:
                        break;
                };
            }
        }

        WorldTilemap.MakeTilemap();
        UpperTilemap.MakeTilemap();
        OverlayTilemap.MakeTilemap();

        WorldTilemap.transform.position = Vector3.zero;
        UpperTilemap.transform.position = Vector3.zero - Vector3.forward * 0.5f;
        OverlayTilemap.transform.position = Vector3.zero - Vector3.forward * 0.7f;
	}

    public void HighlightPassable(int x, int y, ref List<Vec2i> valids)
    {
        foreach (Vec2i  v in valids)
        {
            OverlayTilemap.setIndex(v.x, v.y, -1);
        }

        valids.Clear();

        if (Player.instance.normaBase > 0)
        {
            Vector2[] sens = { new Vector2(0, 1), new Vector2(0, -1), new Vector2(1, 0), new Vector2(-1, 0) };
            for (int i = 0; i < 4; ++i)
            {
                Vector2 pos = new Vector2(x, y) + sens[i];

                bool ok = true;

                int treeBaseRest = Player.instance.treeBase;
                int rockBaseRest = Player.instance.rockBase;

                while (pos.x < UpperTilemap.width && pos.y < UpperTilemap.height && pos.x >= 0 && pos.y >= 0 && ok)
                {
                    int posx = (int)pos.x;
                    int posy = (int)pos.y;

                    ok = true;
                    bool toAdd = true;

                    switch (_world[posx, posy]._type)
                    {
                        case WorldCase.CaseType.TREE:
                            ok = false;
                            if (treeBaseRest > 0)
                            {
                                toAdd = true;
                                treeBaseRest -= 1;
                            }
                            else
                            {
                                toAdd = false;
                            }
                            break;
                        case WorldCase.CaseType.ROCK:
                            ok = false;
                            if (rockBaseRest > 0)
                            {
                                toAdd = true;
                                rockBaseRest -= 1;
                            }
                            else
                            {
                                toAdd = false;
                            }
                            break;
                        default:
                            ok &= _world[posx, posy]._passable;
                            toAdd = ok;
                            break;
                    }

                    if (toAdd)
                    {
                        valids.Add(new Vec2i((int)pos.x, (int)pos.y));
                        OverlayTilemap.setIndex((int)pos.x, (int)pos.y, 107);
                    }

                    pos += sens[i];
                }
            }
        }

        OverlayTilemap.MakeTilemap();
    }
}

//---------------------------

public struct Vec2i
{
    public int x, y;

    public Vec2i(int x = 0, int y = 0)
    {
        this.x = x;
        this.y = y;
    }
}
