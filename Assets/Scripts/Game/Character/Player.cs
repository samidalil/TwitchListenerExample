using TwitchListener.Lib;
using UnityEngine;

namespace TwitchListener.Game.Character
{
    public class Player : MonoBehaviour
    {
        #region Unity Fields

        [SerializeField]
        private Hitbox _hitbox;

        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            if (this._hitbox == null)
            {
                Debug.LogError($"Missing Hitbox in {this.name}");
                return;
            }

            this._hitbox.OnHitboxTriggerEnter += this.OnHitboxTriggerEnter;
        }

        #endregion

        #region Private Methods

        private void OnHitboxTriggerEnter(Collider other, Hitbox _)
        {
        }

        #endregion
    }
}
