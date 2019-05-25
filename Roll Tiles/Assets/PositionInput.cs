using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This class handles position inputs such as mouse and touch inputs. It only handles one input at a time. It passes the inputs to a TileInteraction object.


[RequireComponent(typeof(TileInteraction))]
public class PositionInput : MonoBehaviour
{
    [SerializeField, Tooltip("If this is left blank it finds a camera with the MainCamera tag.")]
    private Camera cam;

    private TileInteraction tileInteraction;


    private void Awake()
    {
        tileInteraction = GetComponent<TileInteraction>();
    }


    private void Start()
    {
        if (cam == null) { cam = Camera.main; }
    }


    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            tileInteraction.StartInput(cam.ScreenToWorldPoint(Input.mousePosition));
        }

        if (Input.GetButton("Fire1"))
        {
            tileInteraction.ContinueInput(cam.ScreenToWorldPoint(Input.mousePosition));
        }

        if (Input.GetButtonUp("Fire1"))
        {
            tileInteraction.EndInput();
        }
    }
}
