using System;
using System.Collections.Generic;
using TwitchListener.Lib;
using UnityEngine;

namespace TwitchListener.Core
{
    /// <summary>
    /// Interface for the helper command structure
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Executes a command
        /// </summary>
        void Execute();
    }

    /// <summary>
    /// Represents a command manager linked to the Twitch client
    /// </summary>
    public sealed class CommandManager : MonoBehaviour
    {
        #region Private Static Fields

        public static CommandManager Instance { get; private set; }

        #endregion

        #region Private Fields

        private readonly Dictionary<string, Type> _typeByCommandName = new Dictionary<string, Type>();

        private readonly Dictionary<string, List<Action<object>>> _listeners = new Dictionary<string, List<Action<object>>>();

        #endregion

        #region Unity Callbacks

        /// <summary>
        /// Fired on script awake
        /// </summary>
        private void Awake()
        {
            if (CommandManager.Instance == null)
            {
                GameObject.Destroy(CommandManager.Instance);
                CommandManager.Instance = this;
            }
        }

        /// <summary>
        /// Fired on script destruction
        /// </summary>
        private void OnDestroy()
        {
            if (CommandManager.Instance == this)
                CommandManager.Instance = null;
        }

        /// <summary>
        /// Fired on script launch
        /// </summary>
        private void Start()
        {
            ClientManager.Instance.OnCommandReceived += OnClientCommandReceived;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Listens on a command and executes listener by giving it the parsed arguments corresponding to given type
        /// </summary>
        /// <typeparam name="TCommandArgs">Arguments type</typeparam>
        /// <param name="command">Listened command</param>
        /// <param name="listener">Action to execute</param>
        /// <returns>This instance</returns>
        /// <exception cref="TypeAccessException">Throws if the command is already listened with another uncompatible type</exception>
        public CommandManager On<TCommandArgs>(string command, Action<TCommandArgs> listener)
        {
            if (!this._listeners.ContainsKey(command))
            {
                this._listeners.Add(command, new List<Action<object>>());
                this._typeByCommandName.Add(command, typeof(TCommandArgs));
            }
            else if (!typeof(TCommandArgs).IsAssignableFrom(this._typeByCommandName[command]))
                throw new TypeAccessException();

            this._listeners[command].Add((obj) => listener((TCommandArgs)obj));
            return this;
        }

        /// <summary>
        /// Listens on a command and executes given command
        /// </summary>
        /// <typeparam name="TCommand">The command type</typeparam>
        /// <param name="command">Listened to command</param>
        /// <returns>This instance</returns>
        public CommandManager On<TCommand>(string command) where TCommand : ICommand
        {
            return this.On<TCommand>(command, (obj) => obj.Execute());
        }

#if UNITY_EDITOR
        /// <summary>
        /// For debug purposes only, calls all the command's listeners
        /// </summary>
        /// <param name="command">Command to execute</param>
        /// <param name="arguments">Arguments of the command</param>
        public void DEBUG_OnClientCommandReceived(string command, List<(string, string)> arguments)
        {
            this.OnClientCommandReceived(command, arguments);
        }
#endif

        #endregion

        #region Private Methods

        /// <summary>
        /// Fired when the Twitch client receives a command
        /// </summary>
        /// <param name="command">Command to execute</param>
        /// <param name="arguments">Arguments of the command</param>
        private void OnClientCommandReceived(string command, List<(string, string)> arguments)
        {
            if (this._listeners.ContainsKey(command) && this._listeners[command].Count > 0)
            {
                try
                {
                    object args = CLIParser.Parse(arguments, this._typeByCommandName[command]);

                    foreach (Action<dynamic> listener in this._listeners[command])
                        listener(args);
                }
                catch
                { }
            }
        }

        #endregion
    }
}
