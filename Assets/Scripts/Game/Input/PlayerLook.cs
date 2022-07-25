using UnityEngine;

namespace TwitchListener.Game.Input
{
    public sealed class PlayerLook : MonoBehaviour
    {
        #region Unity Fields

        [SerializeField]
        private Transform _cameraTarget;

        [SerializeField]
        private Transform _playerTarget;

        [SerializeField]
        private Vector2 _sensibility;

        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
        }

        #endregion

        #region Private Fields

        private float _xRotation = 0f;
        
        #endregion

        #region Public Methods

        public void Look(Vector2 delta)
        {
            delta *= this._sensibility * Time.deltaTime;
            this._playerTarget.Rotate(Vector3.up, delta.x);
            this._xRotation = Mathf.Clamp(this._xRotation - delta.y, -50f, 50f);

            Vector3 cameraRotation = this._playerTarget.eulerAngles;
            cameraRotation.x = this._xRotation;
            
            this._cameraTarget.eulerAngles = cameraRotation;
        }

        #endregion
    }
}
