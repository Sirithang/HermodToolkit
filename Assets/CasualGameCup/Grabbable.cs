using UnityEngine;
using System.Collections;

public class Grabbable : MonoBehaviour 
{
    public enum GrabbableType { NORMALBASE, TREEBASE, ROCKBASE, FLAMAND, VENTILO, SOUTIF };

    public GrabbableType GrabedObject;

    public TextMesh FoundText;

	// Use this for initialization
	void Start () 
    {
        FoundText.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    public void Grabbed()
    {
        CameraScript cam = GameObject.FindGameObjectWithTag("GUICam").GetComponent<CameraScript>();
        Sprite spr = GetComponent<Sprite>();

        FoundText.gameObject.SetActive(true);

        gameObject.layer = LayerMask.NameToLayer("Water");
        foreach (Transform t in transform)
        {
            t.gameObject.layer = LayerMask.NameToLayer("Water");
        }

        transform.localScale = Vector3.one * (2.0f);

        transform.parent = cam.transform;
        transform.localPosition = Vector3.zero + Vector3.forward - new Vector3(spr.rect.width, spr.rect.height, 0);
    }

    public static GameObject CreateOfType(GrabbableType type)
    {
        GameObject grabbable = null;

        Debug.Log("created of type : " + type.ToString());

        switch (type)
        {
            case GrabbableType.NORMALBASE:
                grabbable = Instantiate(Resources.Load("NormalBaseGrabble")) as GameObject;
                break;
            case GrabbableType.FLAMAND:
                grabbable = Instantiate(Resources.Load("FlamandGrabbable")) as GameObject;
                break;
            case GrabbableType.SOUTIF:
                grabbable = Instantiate(Resources.Load("SoutifGrabbable")) as GameObject;
                break;
            case GrabbableType.VENTILO:
                grabbable = Instantiate(Resources.Load("VentiloGrabbable")) as GameObject;
                break;
            default: break;
        }

        return grabbable;
    }
}
