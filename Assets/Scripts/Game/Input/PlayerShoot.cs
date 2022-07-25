using System.Collections;
using TwitchListener.Lib;
using UnityEngine;

namespace TwitchListener.Game.Input
{
    public sealed class PlayerShoot : MonoBehaviour
    {
        #region Unity Fields

        [SerializeField]
        private GameObject _bulletPrefab;

        [SerializeField]
        private Transform _bulletOrigin;

        [SerializeField]
        private float _bulletSpeed;

        #endregion

        #region Public Methods

        public void ShootPrimary()
        {
            GameObject bullet = GameObject.Instantiate(this._bulletPrefab, this._bulletOrigin.position, this._bulletOrigin.rotation);

            bullet
                .GetComponent<Rigidbody>()
                .AddRelativeForce(Vector3.forward * this._bulletSpeed);

            bullet
                .GetComponent<Hitbox>()
                .OnHitboxTriggerEnter += (_, _) => this.OnBulletCollide(bullet);

            this.StartCoroutine(this.DestroyInSeconds(bullet, 10));
        }

        #endregion

        #region Private Methods

        private IEnumerator DestroyInSeconds(GameObject bullet, float seconds)
        {
            yield return new WaitForSeconds(seconds);

            if (bullet != null)
                GameObject.Destroy(bullet);
        }

        private void OnBulletCollide(GameObject bullet)
        {
            GameObject.Destroy(bullet);
        }

        #endregion
    }
}
