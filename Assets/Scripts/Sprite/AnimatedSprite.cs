using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class AnimatedSprite : Sprite 
{
    public struct SpriteAnimation
    {
        public string name;
        public int[] frames;
        public float time;
        public bool loop;

        public float _frameTime;
    }

    protected Dictionary<string, SpriteAnimation> _animations = new Dictionary<string,SpriteAnimation>();
    protected SpriteAnimation _current;
    
    protected int _currentFrame;
    protected float _lastTime;

    public bool play = false;

    protected override void Start()
    {
        base.Start();

        AddAnimation("idle", new int[] { 9,10,11 }, 0.8f, true);

        Play("idle");
    }


    protected void Update()
    {
        if (play)
        {
            if (_lastTime > _current._frameTime)
            {
                changeFrame(1);
                _lastTime = 0;
            }
            else
            {
                _lastTime += Time.deltaTime;
            }
        }
    }

    //==========================

    protected void changeFrame(int p_change)
    {
        int currFrame = _currentFrame + p_change;
        if (currFrame > _current.frames[_current.frames.Length - 1])
            currFrame = _current.frames[0];


        MeshFilter mf = GetComponent<MeshFilter>();

        Vector2[] uv = mf.sharedMesh.uv;

        int[] offsetsX = { 0, 0, 1, 1 };
        int[] offsetsY = { 0, 1, 1, 0 };

        float x = rect.x;
        float y = rect.y;

        for (int i = 0; i <= currFrame; ++i)
        {

            x = (x + rect.width);

            if (x >= spriteSheet.width)
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
            //color[vertex] = new Color32(255, 255, 255, 255);
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
            _currentFrame = _current.frames[0];
            _lastTime = 0;
        }
    }


    //--------------------------

    public void AddAnimation(string p_name, int[] p_frames, float time = 1.0f, bool p_looping = true)
    {
        SpriteAnimation anim = new SpriteAnimation();
        anim.frames = p_frames;
        anim.loop = p_looping;
        anim.name = p_name;
        anim.time = time;

        anim._frameTime = anim.time / anim.frames.Length;

        _animations.Add(anim.name, anim);
    }
}