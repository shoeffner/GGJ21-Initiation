using System.Collections.Generic;
using UnityEngine;
using Mirror;
using static AbilityManager.Ability;

public static class IListExtensions
{
    /// <summary>
    /// Shuffles the element order of the specified list.
    /// https://forum.unity.com/threads/clever-way-to-shuffle-a-list-t-in-one-line-of-c-code.241052/#post-1596795
    /// </summary>
    public static void Shuffle<T>(this IList<T> ts)
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }
}

namespace Initiation
{

    public class InitiationNetworkManager : NetworkManager
    {
        private List<AbilityManager.Ability> permanentAbilitiesPerPlayer;
        private List<PlayerController> players;

        public override void OnStartServer()
        {
            base.OnStartServer();

            NetworkServer.RegisterHandler<CreatePlayerMessage>(OnCreateCharacter);
            permanentAbilitiesPerPlayer = new List<AbilityManager.Ability> {
                FIREBALL,
                HEALING
            };
            permanentAbilitiesPerPlayer.Shuffle();
            Debug.Log("Shuffled abilities");
            players = new List<PlayerController>();
            Debug.Log("Generated player list");
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);

            // you can send the message here, or wherever else you want
            CreatePlayerMessage characterMessage = new CreatePlayerMessage{};

            conn.Send(characterMessage);
        }

        void OnCreateCharacter(NetworkConnection conn, CreatePlayerMessage message)
        {
            // playerPrefab is the one assigned in the inspector in Network
            // Manager but you can use different prefabs per race for example
            GameObject gameobject = Instantiate(playerPrefab);
            players.Add(gameobject.GetComponent<PlayerController>());

            // Apply data from the message however appropriate for your game
            // Typically Player would be a component you write with syncvars or properties
            Debug.Log(permanentAbilitiesPerPlayer[players.Count % permanentAbilitiesPerPlayer.Count]);
            gameobject.GetComponent<AbilityManager>().permanentAbility = permanentAbilitiesPerPlayer[players.Count % permanentAbilitiesPerPlayer.Count];
            Debug.Log(gameobject.GetComponent<AbilityManager>().permanentAbility);

            // call this to use this gameobject as the primary controller
            NetworkServer.AddPlayerForConnection(conn, gameobject);
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            // TODO: do something with conn.clientOwnedObjects to remove player form lists above etc.
            base.OnClientDisconnect(conn);
        }

    }

} // namespace Initiation