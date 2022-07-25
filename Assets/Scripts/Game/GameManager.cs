using System;
using System.Collections.Generic;
using TwitchListener.Core;
using TwitchListener.Game.AI;
using TwitchListener.Game.Character;
using TwitchListener.Game.Commands;
using UnityEngine;

namespace TwitchListener.Game
{
    public sealed class GameManager : MonoBehaviour
    {
        #region Private Static Fields

        public static GameManager Instance { get; private set; }

        #endregion

        #region Unity Fields

        [SerializeField]
        private Player _player;

        [SerializeField]
        private Transform _prefabParent;

        [SerializeField]
        private List<GameObject> _prefabs = new List<GameObject>();

        #endregion

        private float _spawnTime = 0f;

        #region Unity Callbacks

        /// <summary>
        /// Fired on script awake
        /// </summary>
        private void Awake()
        {
            if (GameManager.Instance == null)
            {
                GameObject.Destroy(GameManager.Instance);
                GameManager.Instance = this;
            }
        }

        /// <summary>
        /// Fired on script launch
        /// </summary>
        private void Start()
        {
            CommandManager.Instance.On<SpawnPrefabCommandArgs>("Spawn", this.Spawn);
        }

        /// <summary>
        /// Fired on script destruction
        /// </summary>
        private void OnDestroy()
        {
            if (GameManager.Instance == this)
                GameManager.Instance = null;
        }

        #endregion

        #region Public Methods

        public void Spawn(SpawnPrefabCommandArgs args)
        {
            try
            {
                Debug.Log($"Spawn Command : {this._spawnTime}");
                if (Time.time > this._spawnTime + 10f)
                {
                    this._spawnTime = Time.time;

                    GameObject prefab = this._prefabs.Find(prefab => prefab.name.Equals(args.Name));
                    int count = Math.Min(args.Count, 10);

                    if (prefab != null)
                    {
                        for (int i = 0; i < count; ++i)
                        {
                            GameObject
                                .Instantiate(prefab, this._prefabParent)
                                .GetComponent<FollowAI>()
                                .PlayerToFollow = this._player;
                        }
                    }

                    Debug.Log($"Spawn {args.Name} {count} time(s)");
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        #endregion
    }
}
