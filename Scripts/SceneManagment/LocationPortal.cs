using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LocationPortal : MonoBehaviour,IPlayerTriggerable
{
    [SerializeField] Transform spawnPoint;
    [SerializeField] DestinationIdentifier destinationPortal;
    PlayerController player;
    Fader fader;

    public void OnPlayerTriggered(PlayerController player)
    {
        player.Character.Animator.IsMoving = false;
        this.player = player;
        StartCoroutine(Teleport());
    }

    [System.Obsolete]
    private void Start(){
        fader = FindObjectOfType<Fader>();

    }

    [System.Obsolete]
    IEnumerator Teleport(){
        GameController.Instance.PauseGame(true);
        yield return fader.FadeIn(0.5f);  
        var destPortal = FindObjectsOfType<LocationPortal>().First(x => x != this && x.destinationPortal == this.destinationPortal);
        player.Character.SetPositionAndSnapToTile(destPortal.SpawnPoint.position);
        yield return fader.FadeOut(0.5f);

        GameController.Instance.PauseGame(false);
    }
    public Transform SpawnPoint => spawnPoint;

    public bool TriggerRepeatdly => false;
}
