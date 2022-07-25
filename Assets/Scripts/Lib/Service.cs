using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitchListener.Lib
{
    /// <summary>
    /// Enumerates the status of a service
    /// </summary>
    public enum ServiceStatus
    {
        NONE,
        STARTING,
        STARTED,
        STOPPING,
        STOPPED,
    }

    public abstract class Service
    {
        #region Public Events

        /// <summary>
        /// Fires on service start
        /// </summary>
        public event Action<Service> OnStarted;

        /// <summary>
        /// Fires on service stop
        /// </summary>
        public event Action<Service> OnStopped;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the dependencies of this service
        /// </summary>
        public IEnumerable<Service> Dependencies
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the current status of the service
        /// </summary>
        public ServiceStatus Status
        {
            get;
            protected set;
        } = ServiceStatus.NONE;

        #endregion

        #region Protected Fields

        protected Task _startingTask = null;
        protected Task _stoppingTask = null;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="dependencies">The services this service depends on</param>
        public Service(IEnumerable<Service> dependencies)
        {
            this.Dependencies = dependencies;

            foreach (Service service in this.Dependencies)
                service.OnStopped += this.OnDependencyStopped;
        }

        /// <summary>
        /// Class destructor
        /// </summary>
        ~Service()
        {
            foreach (Service service in this.Dependencies)
                service.OnStopped -= this.OnDependencyStopped;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Starts the service
        /// </summary>
        /// <returns>A task waiting for the service to start, holding the service</returns>
        public async Task<Service> Start()
        {
            await (this.Status switch
            {
                ServiceStatus.STARTING or ServiceStatus.STARTED => this._startingTask,
                ServiceStatus.STOPPING or ServiceStatus.STOPPED => this._startingTask = this._stoppingTask.ContinueWith(this.InternalStart),
                _ => this._startingTask = this.InternalStart(this._startingTask),
            });

            return this;
        }

        /// <summary>
        /// Stops the service
        /// </summary>
        /// <returns>A task waiting for the service to stop, holding the service</returns>
        public async Task<Service> Stop()
        {
            if (this.Status == ServiceStatus.NONE)
                return this;

            await (this.Status switch
            {
                ServiceStatus.STOPPING or ServiceStatus.STOPPED => this._stoppingTask,
                ServiceStatus.STARTING => this._stoppingTask = this._startingTask.ContinueWith(this.InternalStop),
                _ => this._stoppingTask = this.InternalStop(this._stoppingTask),
            });

            return this;
        }

        #endregion

        #region Protected Abstract Methods

        /// <summary>
        /// Contains logic to eject the service
        /// </summary>
        /// <returns>A task waiting for the service ejection</returns>
        protected abstract Task Eject();

        /// <summary>
        /// Contains logic to launch the service
        /// </summary>
        /// <returns>A task waiting for the service launching</returns>
        protected abstract Task Launch();

        #endregion

        #region Protected Overridable Methods

        /// <summary>
        /// Fires when a dependency is stopped
        /// </summary>
        /// <param name="sender">The stopped service</param>
        protected virtual void OnDependencyStopped(Service sender)
        {
            _ = this.Stop();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Handles common logic for the service start
        /// </summary>
        /// <param name="_">Task parameter for ContinueWith compatibility</param>
        /// <returns>A task waiting for the service to launch</returns>
        private async Task InternalStart(Task _)
        {
            this.Status = ServiceStatus.STARTING;
            await Task.WhenAll(this.Dependencies.Select(service => service.Start()));
            await this.Launch();
            this.Status = ServiceStatus.STARTED;

            this.OnStarted?.Invoke(this);
        }

        /// <summary>
        /// Handles common logic for the service stop
        /// </summary>
        /// <param name="_">Task parameter for ContinueWith compatibility</param>
        /// <returns>A task waiting for the service to stop</returns>
        private async Task InternalStop(Task _)
        {
            this.Status = ServiceStatus.STOPPING;
            await this.Eject();
            this.Status = ServiceStatus.STOPPED;

            this.OnStopped?.Invoke(this);
        }

        #endregion
    }
}
