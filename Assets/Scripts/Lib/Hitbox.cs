using System;
using UnityEngine;

namespace TwitchListener.Lib
{
    /// <summary>
    /// Permits easy subscribing to collision and trigger methods outside of the game object
    /// </summary>
    public class Hitbox : MonoBehaviour
    {
        #region Public Events

        public event Action<Collision, Hitbox> OnHitboxCollisionEnter;
        public event Action<Collision, Hitbox> OnHitboxCollisionExit;
        public event Action<Collision, Hitbox> OnHitboxCollisionStay;
        public event Action<Collider, Hitbox> OnHitboxTriggerEnter;
        public event Action<Collider, Hitbox> OnHitboxTriggerExit;
        public event Action<Collider, Hitbox> OnHitboxTriggerStay;

        #endregion

        #region Unity Callbacks

        private void OnCollisionEnter(Collision collision)
        {
            this.OnHitboxCollisionEnter?.Invoke(collision, this);
        }

        private void OnCollisionExit(Collision collision)
        {
            this.OnHitboxCollisionExit?.Invoke(collision, this);
        }

        private void OnCollisionStay(Collision collision)
        {
            this.OnHitboxCollisionStay?.Invoke(collision, this);
        }

        private void OnTriggerEnter(Collider other)
        {
            this.OnHitboxTriggerEnter?.Invoke(other, this);
        }

        private void OnTriggerExit(Collider other)
        {
            this.OnHitboxTriggerExit?.Invoke(other, this);
        }

        private void OnTriggerStay(Collider other)
        {
            this.OnHitboxTriggerStay?.Invoke(other, this);
        }

        #endregion
    }
}
