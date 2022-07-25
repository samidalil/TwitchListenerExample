using TwitchListener.Game.Character;
using UnityEngine;

namespace TwitchListener.Game.Input
{
    public class InputManager : MonoBehaviour
    {
        #region Unity Fields

        [SerializeField]
        private PlayerLook _playerLook;

        [SerializeField]
        private PlayerMovement _playerMovement;

        [SerializeField]
        private PlayerShoot _playerShoot;

        #endregion

        #region Private Fields

        private PlayerActions _inputActions;

        private Vector2 _lookInput;
        private Vector2 _movementInput;

        #endregion

        private void Awake()
        {
            if (this._playerMovement == null)
            {
                Debug.LogError($"Missing PlayerMovement in {this.name}");
                return;
            }
            if (this._playerLook == null)
            {
                Debug.LogError($"Missing PlayerLook in {this.name}");
                return;
            }

            this._inputActions = new PlayerActions();

            this._inputActions.GroundMovement.Move.performed += ctx => this._movementInput = ctx.ReadValue<Vector2>();
            this._inputActions.GroundMovement.Jump.performed += _ => this._playerMovement.Jump();

            this._inputActions.Look.MouseX.performed += ctx => this._lookInput.x = ctx.ReadValue<float>();
            this._inputActions.Look.MouseY.performed += ctx => this._lookInput.y = ctx.ReadValue<float>();

            this._inputActions.Shoot.Primary.performed += _ => this._playerShoot.ShootPrimary();
        }

        private void OnDisable()
        {
            this._inputActions.Disable();
        }

        private void OnEnable()
        {
            this._inputActions.Enable();
        }

        private void FixedUpdate()
        {
            this._playerMovement.Move(this._movementInput);
        }

        private void Update()
        {
            this._playerLook.Look(this._lookInput);
        }
    }
}
