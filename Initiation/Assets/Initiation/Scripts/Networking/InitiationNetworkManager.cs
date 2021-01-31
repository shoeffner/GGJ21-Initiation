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
        private List<NetworkStartPosition> spawnPoints;
        public List<RuneManager> runes;
        public RitualTrigger ritualTrigger;

        public override void OnStartServer()
        {
            base.OnStartServer();

            NetworkServer.RegisterHandler<CreatePlayerMessage>(OnCreateCharacter);
            permanentAbilitiesPerPlayer = new List<AbilityManager.Ability> {
                FIREBALL,
                HEALING,
                SHIELD,
                GHOST,
            };
            permanentAbilitiesPerPlayer.Shuffle();
            Debug.Log("Shuffled abilities");
            players = new List<PlayerController>();
            Debug.Log("Generated player list");
            spawnPoints = new List<NetworkStartPosition>(FindObjectsOfType<NetworkStartPosition>());
            if (ritualTrigger == null)
            {
                Debug.LogWarning("No ritual trigger set yet!");
            }
            else
            {
                foreach (RuneManager rm in runes)
                {
                    rm.OnRuneActivation += ritualTrigger.OnRuneActivation;
                }
                ritualTrigger.requiredRunes = runes.Count;
            }
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

            // Apply data from the message however appropriate for your game
            // Typically Player would be a component you write with syncvars or properties
            
            // Apply spawn position (always round robin though)
            gameobject.transform.position = spawnPoints[players.Count % spawnPoints.Count].transform.position;
            gameobject.transform.rotation = spawnPoints[players.Count % spawnPoints.Count].transform.rotation;

            // TODO(@shoeffner): If a player joins AFTER a rune is activated, they will not lose abilites at the moment...
            AbilityManager am = gameobject.GetComponent<AbilityManager>();
            am.permanentAbility = permanentAbilitiesPerPlayer[players.Count % permanentAbilitiesPerPlayer.Count];
            foreach (RuneManager rm in runes)
            {
                rm.OnRuneActivation += am.RpcOnRuneActivation;
            }

            players.Add(gameobject.GetComponent<PlayerController>());
            if (ritualTrigger != null)
            {
                ritualTrigger.playersRequiredForRitual = players.Count;
            }

            // call this to use this gameobject as the primary controller
            NetworkServer.AddPlayerForConnection(conn, gameobject);
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            PlayerController pc = null;
            foreach (NetworkIdentity ni in conn.clientOwnedObjects)
            {
                pc = ni.gameObject.GetComponent<PlayerController>();
                if (pc != null && players.Contains(pc))
                {
                    players.Remove(pc);
                    break;
                }
            }
            if (pc == null)
            {
                Debug.LogError("Could not unregister player properly.");
                base.OnClientDisconnect(conn);
                return;
            }

            AbilityManager am = pc.gameObject.GetComponent<AbilityManager>();
            foreach (RuneManager rm in runes)
            {
                rm.OnRuneActivation -= am.RpcOnRuneActivation;
            }

            base.OnClientDisconnect(conn);
        }

    }

} // namespace Initiation