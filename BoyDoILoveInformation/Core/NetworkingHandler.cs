using System;
using System.Collections.Generic;
using System.Linq;
using BoyDoILoveInformation.Tools;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace BoyDoILoveInformation.Core;

public class NetworkingHandler : MonoBehaviour
{
    private const byte NetworkingByte = 135;
    
    public static Action<bool> OnMenuStateUpdate;

    private GameObject emptyMenuPrefab;

    private Dictionary<VRRig, GameObject> playerMenus = new();

    private void Start()
    {
        emptyMenuPrefab                    =  Plugin.BDILIBundle.LoadAsset<GameObject>("EmptyMenu");
        BDILIUtils.OnPlayerCosmeticsLoaded += rig =>
                                              {
                                                  
                                              };
        BDILIUtils.OnPlayerRigCached       += rig =>
                                              {
                                                  Destroy(playerMenus[rig]);
                                                  playerMenus.Remove(rig);
                                              };

        OnMenuStateUpdate += open => PhotonNetwork.RaiseEvent(NetworkingByte, open,
                                     new RaiseEventOptions() { Receivers = ReceiverGroup.Others, }, SendOptions.SendReliable);

        PhotonNetwork.NetworkingClient.EventReceived += eventData =>
                                                        {
                                                            if (eventData.Code != NetworkingByte)
                                                                return;
                                                            
                                                            VRRig sender = GorillaParent.instance.vrrigs
                                                                   .FirstOrDefault(rig => rig.OwningNetPlayer.ActorNumber ==
                                                                        eventData.Sender);

                                                            if (!playerMenus.ContainsKey(sender))
                                                                return;
                                                            
                                                            if (!eventData.Parameters.TryGetValue(ParameterCode.Data, out object data))
                                                                return;
                                                            
                                                            playerMenus[sender].SetActive((bool)data);
                                                        };
    }
}