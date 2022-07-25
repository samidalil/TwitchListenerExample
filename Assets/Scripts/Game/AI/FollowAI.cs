using TwitchListener.Game.Character;
using UnityEngine;

namespace TwitchListener.Game.AI
{
    public class FollowAI : MonoBehaviour
    {
        #region Unity Fields

        [SerializeField]
        private Rigidbody _rigidbody;

        [SerializeField]
        private float _speed;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the player this AI should follow
        /// </summary>
        public Player PlayerToFollow
        {
            get;
            set;
        }

        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            if (this._rigidbody == null)
            {
                Debug.LogError($"Missing Rigidbody in {this.name}");
                return;
            }
        }

        private void FixedUpdate()
        {
            if (this.PlayerToFollow != null)
            {
                Vector3 direction = (this.PlayerToFollow.transform.position - this.transform.position).normalized;

                this._rigidbody.AddForce(direction * this._speed);
            }
        }

        #endregion
    }
}