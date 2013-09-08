using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class allow to load a material per picture only once
/// It avoid to have to create material by hand in the editor
/// </summary>
public class MaterialDatabase 
{
    public class CountedMaterial
    {
        public Material material = null;
        public int count = 0;
    }

    static protected Dictionary<int, CountedMaterial> _database = new Dictionary<int,CountedMaterial>();

    //-------------------------------------------------------

    static public Material Get(Texture2D p_texture)
    {
        CountedMaterial cm;
        int id = p_texture.GetInstanceID();

        if (!_database.TryGetValue(id, out cm))
        {
            cm = new CountedMaterial();

            cm.material = new Material(Resources.Load("Jormungandr/SimplestCutout") as Shader);
            cm.material.mainTexture = p_texture;
        }

        cm.count += 1;

        return cm.material;
    }

    //--------------------------------------------------------

    static public void Unload(Texture2D p_texture)
    {
        CountedMaterial cm;
        int id = p_texture.GetInstanceID();

        if (_database.TryGetValue(id, out cm))
        {
            cm.count -= 1;

            if (cm.count <= 0)
            {
                _database.Remove(id);
            }
        }
    }

}