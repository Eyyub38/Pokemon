using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayers : MonoBehaviour{
    [SerializeField] LayerMask solidObjectsLayer;
    [SerializeField] LayerMask interactableLayer;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask grassLayer;
    [SerializeField] LayerMask fovLayer;
    [SerializeField] LayerMask portalLayer;
    [SerializeField] LayerMask triggerLayer;
    [SerializeField] LayerMask ledgeLayer;
    [SerializeField] LayerMask waterLayer;

    public static GameLayers i { get; set;}
    private void Awake(){
        i = this;
    }

    public LayerMask SolidLayer => solidObjectsLayer;
    public LayerMask PlayerLayer => playerLayer;
    public LayerMask InteractableLayer => interactableLayer;
    public LayerMask GrassLayer => grassLayer;
    public LayerMask FovLayer => fovLayer;
    public LayerMask PortalLayer => portalLayer;
    public LayerMask Triggers => triggerLayer; 
    public LayerMask Ledges => ledgeLayer;
    public LayerMask WaterLayer => waterLayer;

    public LayerMask TriggerableLayers => grassLayer | fovLayer | portalLayer | triggerLayer | waterLayer;
}
