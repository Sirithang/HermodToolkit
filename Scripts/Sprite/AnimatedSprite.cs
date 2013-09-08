using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimatedSprite : Sprite 
{
    protected Dictionary<string, SpriteAnimation> _animations = new Dictionary<string,SpriteAnimation>();

    [SerializeField]
    protected SpriteAnimation _current;
    
    protected int _currentFrame;
    protected float _currentTime;

    [SerializeField]
    protected List<SpriteAnimation> _animationsInternals;

    public Dictionary<string, SpriteAnimation> spriteAnims { get { return _animations; } }
    public List<SpriteAnimation> spriteAnimListInternal { get { return _animationsInternals; } }

    public bool play = false;
    public float playbackSpeed = 1.0f;

    public SpriteAnimation current { get { return _current; } }

    public string currentName
    {
        get { if (_current == null) return ""; return _current.name; }
        set { setAnim(value); }
    }

#if UNITY_EDITOR
    public bool playInEditor = false;
#endif

    protected override void Awake()
    {
        base.Awake();
        RebuildAnimDictionnary();
    }

    protected override void Start()
    {
        base.Start();

        //AddAnimation("idle", new int[] { 9,10,11 }, 0.8f, true);

        //Play("idle");
    }


    protected virtual void Update()
    {
#if UNITY_EDITOR
        if((playInEditor||Application.isPlaying) && play)
#else
        if (play)
#endif
        {
            if(_current == null)
                return;

            changeFrame(_current.SampleAt(_currentTime));
            _currentTime += Time.deltaTime * playbackSpeed;
        }
    }

    protected void setAnim(string name)
    {
        if (_animations.ContainsKey(name))
        {
            _current = _animations[name];
        }
        else
        {
            _current = null;
        }
    }

    //==========================

    protected void changeFrame(int p_Frame)
    {
        int currFrame = p_Frame;

        MeshFilter mf = GetComponent<MeshFilter>();

        Vector2[] uv = mf.sharedMesh.uv;

        int[] offsetsX = { 0, 0, 1, 1 };
        int[] offsetsY = { 1, 0, 0, 1 };

        float x = rect.x;
        float y = rect.y;

        for (int i = 0; i < currFrame; ++i)
        {

            x = (x + rect.width);

            if (x > spriteSheet.width - rect.width)
            {
                x -= spriteSheet.width;
                y += rect.height;
            }
            
        }

        float normalizedX = x / spriteSheet.width;
        float normalizedY = y / spriteSheet.height;
        float normalizedW = rect.width / spriteSheet.width;
        float normalizedH = rect.height / spriteSheet.height;

        for (int i = 0; i < 4; ++i)
        {
            uv[i].Set(normalizedX + offsetsX[i] * normalizedW, normalizedY + offsetsY[i] * normalizedH);
        }

        mf.sharedMesh.uv = uv;

        _currentFrame = currFrame;
    }

    //--------------------------

    public void Play(string p_name)
    {
        SpriteAnimation anim;
        if (_animations.TryGetValue(p_name, out anim))
        {
            _current = anim;
            _currentFrame = _current.startFrame;
            _currentTime = 0;
            changeFrame(_current.SampleAt(_currentTime));
        }
    }


    //--------------------------

    public void AddAnimation(string p_name, int[] p_frames, float time = 1.0f, bool p_looping = true)
    {
        if (_animations.ContainsKey(p_name))
            return;

        SpriteAnimation anim = SpriteAnimation.CreateInstance<SpriteAnimation>();
        anim.wrapMode = WrapMode.Loop;
        anim.name = p_name;
        anim.duration = time;
        anim.startFrame = p_frames[0];
        anim.endFrame = p_frames[p_frames.Length - 1];

        _animations.Add(anim.name, anim);
        _animationsInternals.Add(anim);
    }

    //--------------------------

    public void RebuildAnimDictionnary()
    {
        _animations.Clear();
        foreach (SpriteAnimation a in _animationsInternals)
        {
            _animations.Add(a.name, a);
        }

        if (_current == null && _animationsInternals.Count > 0)
            _current = _animationsInternals[0];
    }

    //------------------------------


}