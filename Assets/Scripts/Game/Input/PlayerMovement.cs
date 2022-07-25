using UnityEngine;

namespace TwitchListener.Game.Input
{
    public sealed class PlayerMovement : MonoBehaviour
    {
        #region Unity Fields

        [SerializeField]
        private float _jumpForce;

        [SerializeField]
        private Rigidbody _rigidbody;

        [SerializeField]
        private float _maxMoveMagnitude;

        [SerializeField]
        private float _moveSpeed;

        #endregion

        #region Private Fields

        private bool _canJump = true;

        #endregion

        #region Public Methods

        public void Jump()
        {
            if (this._canJump)
                this._rigidbody.AddForce(Vector3.up * this._jumpForce, ForceMode.Impulse);
        }

        public void Move(Vector2 movement)
        {
            Vector3 direction = new Vector3(movement.x, 0, movement.y).normalized * this._moveSpeed;

            this._rigidbody.velocity = Vector3.ClampMagnitude(this._rigidbody.velocity + transform.TransformDirection(direction) * Time.fixedDeltaTime, this._maxMoveMagnitude);
        }

        #endregion
    }
}
