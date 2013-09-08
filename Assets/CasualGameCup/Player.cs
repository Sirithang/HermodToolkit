using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour 
{
    static public Player instance;

    public AnimatedSprite sprite;
    public Worldmap world;
    public string[] spriteAnims;

    public Cursor cursor;

    public float speed = 200;

    public int x, y;

    public enum Direction { UP, LEFT, DOWN, RIGHT }

    protected Direction _currentDir;

    //****

    public enum State { IDLE, WALKING, FROZEN, BACKTOBASE }

    protected State _currentState;
    protected Base _nextBase;

    protected List<Base> _baseList;
    protected int _baseIdx;

    protected List<Vec2i> _validsPos;


    public TextMesh NormalBaseText;
    public TextMesh TreeBaseText;
    public TextMesh RockBaseText;

    protected int maxNormalBase;
    protected int currentNormalBase;

    protected int maxTreeBase;
    protected int currentTreeBase;

    protected int maxRockBase;
    protected int currentRockBase;


    public int normaBase { get { return currentNormalBase; } }
    public int treeBase { get { return currentTreeBase; } }
    public int rockBase { get { return currentRockBase; } }

    //****

    protected float _sinceFreeze = 0.0f;
    protected float _frozeTime = 0.0f;
    protected State _unfrozeState;
    protected float _frozenPlaybackSpeed;

    protected GameObject grabbedObject = null;

    protected Vector3 CamTarget;
    protected CameraScript cam;


    protected bool SoutifFound = false;
    protected bool FlamandFound = false;
    protected bool VentiloFound = false;
    public GameObject SoutifGO;
    public GameObject FlamandGO;
    public GameObject VentiloGO;

    public void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        if (sprite.transform.parent != transform)
        {
            sprite.transform.parent = transform;
        }

        _validsPos = new List<Vec2i>();

        sprite.Play(spriteAnims[0]);
        sprite.playbackSpeed = 0.0f;
        _currentDir = Direction.UP;

        cursor = Cursor.CreateCursor(world.WorldTilemap);

        _baseList = new List<Base>();
        _nextBase = cursor;
        _baseIdx = 0;

        maxNormalBase = 3;
        maxTreeBase = 1;
        maxRockBase = 0;

        ReturnToStart();


        world.HighlightPassable(x, y, ref _validsPos);

        cam = Camera.main.GetComponent<CameraScript>();
    }

    public void ReturnToStart()
    {
        x = (int)world._startCase.x;
        y = (int)world._startCase.y;

        _currentState = State.IDLE;

        Vector3 pos = world.WorldTilemap.PositionOfCase(x, y);
        pos.z = -1;
        transform.position = pos;

        sprite.playbackSpeed = 0.0f;
        _currentDir = Direction.UP;


        currentNormalBase = maxNormalBase;
        currentTreeBase = maxTreeBase;
        currentRockBase = maxRockBase;
    }

    //==================================

    public void Update()
    {
        switch (_currentState)
        {
            case State.IDLE:
                IdleUpdate();
                break;
            case State.WALKING:
                WalkingUpdate();
                break;
            case State.FROZEN:
                UpdateFrozen();
                break;
            case State.BACKTOBASE:
                 _baseIdx = 0;

                foreach (Base b in _baseList)
                    Destroy(b.gameObject);

                _baseList.Clear();

                ReturnToStart();

                world.HighlightPassable(x, y, ref _validsPos);

                currentNormalBase = maxNormalBase;
                currentTreeBase = maxTreeBase;
                currentRockBase = maxTreeBase;
                break;
            default:
                break;
        }

        UpdateUI();
    }

    public void LateUpdate()
    {
        FixCamera();
    }

    protected void UpdateCoord()
    {
        Vector2 coord = world.WorldTilemap.FindCaseAt(transform.position);
        x = (int)coord.x;
        y = (int)coord.y;
    }


    protected void IdleUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            cam.TargetPixelSize = cam.TargetPixelSize == 1 ? 2 : 1;

            CamTarget = cursor.transform.position;
        }

        if (Mathf.Abs(cam.pixelSize - cam.TargetPixelSize) > 0.01f)
        {
            cam.Target = CamTarget;
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vec2i pos = new Vec2i((int)cursor.coord.x, (int)cursor.coord.y);

            if (BaseExist(pos.x, pos.y))
            {
                Base b = _baseList[_baseList.Count - 1];
                
                if ((int)b.coord.x == pos.x && (int)b.coord.y == pos.y)
                {
                    Destroy(b.gameObject);
                    _baseList.RemoveAt(_baseList.Count-1);

                    switch (world._world[pos.x, pos.y]._type)
                    {
                        case Worldmap.WorldCase.CaseType.GRASS:
                            currentNormalBase += 1;
                            break;
                        case Worldmap.WorldCase.CaseType.TREE:
                            currentTreeBase += 1;
                            break;
                        case Worldmap.WorldCase.CaseType.ROCK:
                            currentRockBase += 1;
                            break;

                        default: break;
                    }

                    if (_baseList.Count == 0)
                    {
                        _nextBase = cursor;
                        world.HighlightPassable(x, y, ref _validsPos);
                    }
                    else
                        world.HighlightPassable((int)_baseList[_baseList.Count - 1].coord.x, (int)_baseList[_baseList.Count - 1].coord.y, ref _validsPos);
                }
            }
            else if (_validsPos.Contains(pos))
            {

                Base b = Base.SpawnBaseAt((int)cursor.coord.x, (int)cursor.coord.y, world.WorldTilemap, (Instantiate(Resources.Load("base")) as GameObject).GetComponent<Sprite>());
                _baseList.Add(b);
                if (_baseList.Count == 1)
                    _nextBase = b;

                switch (world._world[pos.x, pos.y]._type)
                {
                    case Worldmap.WorldCase.CaseType.GRASS:
                        currentNormalBase -= 1;
                        break;
                    case Worldmap.WorldCase.CaseType.TREE:
                        currentTreeBase -= 1;
                        break;
                    case Worldmap.WorldCase.CaseType.ROCK:
                        currentRockBase -= 1;
                        break;

                    default: break;
                }
                 
                world.HighlightPassable((int)_baseList[_baseList.Count - 1].coord.x, (int)_baseList[_baseList.Count - 1].coord.y, ref _validsPos);
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            if (_baseList.Count > 0)
            {
                _currentState = State.WALKING;
                _currentDir = OrientTowardCurrentBase();
                sprite.Play(spriteAnims[(int)_currentDir]);
                sprite.playbackSpeed = 1.0f;
            }
        }

        const int band = 30;
        const float speed = 400.0f;
        if (Input.mousePosition.x > Screen.width - band)
        {
            cam._onTarget = false;
            cam.transform.position += Vector3.right * speed * Time.deltaTime;
        }

        if (Input.mousePosition.x < band)
        {
            cam._onTarget = false;
            cam.transform.position -= Vector3.right * speed * Time.deltaTime;
        }

        if (Input.mousePosition.y > Screen.height - band)
        {
            cam._onTarget = false;
            cam.transform.position += Vector3.up * speed * Time.deltaTime;
        }

        if (Input.mousePosition.y < band)
        {
            cam._onTarget = false;
            cam.transform.position -= Vector3.up * speed * Time.deltaTime;
        }

        Direction d = OrientTowardCurrentBase();

        if (d != _currentDir)
        {
            _currentDir = d;
            sprite.Play(spriteAnims[(int)_currentDir]);
        }
    }

    protected void WalkingUpdate()
    {
        UpdateCoord();

        if (_nextBase.isOnBase(transform.position))
        {
            if (_baseList.Count-1 <= _baseIdx)
            {
                FreezeForTime(4.0f, State.BACKTOBASE);
                return;
            }

            _baseIdx++;
            _nextBase = _baseList[_baseIdx];

            _currentDir = OrientTowardCurrentBase();
            sprite.Play(spriteAnims[(int)_currentDir]);
        }

        Vec2i currCase = GoToCase((int)_nextBase.coord.x, (int)_nextBase.coord.y);

        if (world._world[currCase.x, currCase.y]._obj != null)
        {
            OnObject(world._world[currCase.x, currCase.y]._obj);
            world._world[currCase.x, currCase.y]._obj = null;
        }

        cam.Target = transform.position;
    }

    //===================================

    protected void UpdateFrozen()
    {
        _sinceFreeze += Time.deltaTime;

        if (_sinceFreeze > _frozeTime || Input.GetMouseButtonDown(0))
        {
            _currentState = _unfrozeState;
            sprite.playbackSpeed = _frozenPlaybackSpeed;

            if (grabbedObject != null)
            {
                Destroy(grabbedObject);
                grabbedObject = null;
            }
        }
    }

    //===================================

    public Vec2i GoToCase(int x, int y)
    {
        Vector3 position = world.WorldTilemap.PositionOfCase(x, y);
        position.z = transform.position.z;

        transform.position = Vector3.MoveTowards(transform.position, position, Time.deltaTime * speed);

        Vector2 c = world.WorldTilemap.FindCaseAt(transform.position);

        return new Vec2i((int)c.x, (int)c.y);
    }

    //==================================

    protected Direction OrientTowardCurrentBase()
    {
        Vector2 nextBase = _nextBase.coord;
        
        Vector2 delta = nextBase - new Vector2(x,y);

        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
        {
            if (delta.x > 0)
                return Direction.LEFT;

            return Direction.RIGHT;
        }
        else
        {
            if (delta.y > 0)
                return Direction.UP;

            return Direction.DOWN;
        }
    }

    //=========================================

    protected void UpdateUI()
    {
        NormalBaseText.text = currentNormalBase.ToString() + " / " + maxNormalBase.ToString();
        TreeBaseText.text = currentTreeBase.ToString() + " / " + maxTreeBase.ToString();
        RockBaseText.text = currentRockBase.ToString() + " / " + maxRockBase.ToString();
    }

    //=========================================

    protected bool BaseExist(int x, int y)
    {
        for (int i = 0; i < _baseList.Count; ++i)
        {
            if (_baseList[i].coord.x == x && _baseList[i].coord.y == y)
            {
                return true;
            }
        }

        return false;
    }

    //=======================================

    public void FixCamera()
    {
        cam.transform.position = CameraPosFor(cam.transform.position);
    }

    public Vector3 CameraPosFor(Vector3 position)
    {
        Rect cameraRect = new Rect(position.x - cam.Window.x * cam.pixelSize / 2.0f,
                                   position.y - cam.Window.y * cam.pixelSize / 2.0f,
                                   cam.Window.x * cam.pixelSize,
                                   cam.Window.y * cam.pixelSize);

        Vector3 minCase = world.WorldTilemap.PositionOfCase(0, 0);
        Vector3 maxCase = world.WorldTilemap.PositionOfCase(world.WorldTilemap.width - 1, world.WorldTilemap.height - 1)
                        + new Vector3(world.WorldTilemap.tileSize, world.WorldTilemap.tileSize, 0);

        Vector3 pos = position;

        if (cameraRect.x < minCase.x)
        {
            pos.x = minCase.x + cameraRect.width / 2.0f;
        }
        else if (cameraRect.xMax > maxCase.x)
        {
            pos.x = maxCase.x - cameraRect.width / 2.0f;
        }

        if (cameraRect.y < minCase.y)
        {
            pos.y = minCase.y + cameraRect.height / 2.0f;
        }
        else if (cameraRect.yMax > maxCase.y)
        {
            pos.y = maxCase.y - cameraRect.height / 2.0f;
        }

        return pos;
    }

    //========================================

    public void FreezeForTime(float time, State UnfrozeState)
    {
        _frozeTime = time;
        _unfrozeState = UnfrozeState;
        _currentState = State.FROZEN;
        _sinceFreeze = 0.0f;

        _frozenPlaybackSpeed = sprite.playbackSpeed;
        sprite.playbackSpeed = 0.0f;
    }

    protected void OnObject(GameObject obj)
    {
        Grabbable grab = obj.GetComponent<Grabbable>();

        if(grab == null)
            return;

        grab.Grabbed();

        grabbedObject = obj;

        FreezeForTime(3.0f, _currentState);

        switch (grab.GrabedObject)
        {
            case Grabbable.GrabbableType.NORMALBASE:
                maxNormalBase += 1;
                break;
            case Grabbable.GrabbableType.SOUTIF:
                SoutifFound = true;
                SoutifGO.SetActive(true);
                break;
            case Grabbable.GrabbableType.FLAMAND:
                FlamandFound = true;
                FlamandGO.SetActive(true);
                break;
            case Grabbable.GrabbableType.VENTILO:
                VentiloFound = true;
                VentiloGO.SetActive(true);
                break;
            default:
                break;
        }
    }
}