using System;
using System.Collections.Generic;
using System.Linq;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Unity;
using TwitchListener.Data;
using UnityEngine;

namespace TwitchListener.Core
{
    /// <summary>
    /// Manages the Twitch Chatbot
    /// </summary>
    public sealed class ClientManager : MonoBehaviour
    {
        #region Private Static Fields

        public static ClientManager Instance { get; private set; }

        #endregion

        #region Unity Fields

        [SerializeField]
        private string _channelToConnectTo = Secrets.CHANNEL_TO_CONNECT_TO;

        #endregion

        #region Public Events

        /// <summary>
        /// Fired on command received, transmits the command's name as first parameter and the command's arguments as second parameter
        /// </summary>
        public event Action<string, List<(string, string)>> OnCommandReceived;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the client connected to Twitch
        /// </summary>
        public Client Client { get; private set; }

        #endregion

        #region Unity Callbacks

        /// <summary>
        /// Fired on script awake
        /// </summary>
        private void Awake()
        {
            if (ClientManager.Instance == null)
            {
                GameObject.Destroy(ClientManager.Instance);
                ClientManager.Instance = this;
            }
        }

        /// <summary>
        /// Fired on script destruction
        /// </summary>
        private void OnDestroy()
        {
            if (ClientManager.Instance == this)
                ClientManager.Instance = null;
        }

        /// <summary>
        /// Fired on script launch
        /// </summary>
        private void Start()
        {
            ConnectionCredentials credentials = new ConnectionCredentials(Secrets.USERNAME, Secrets.OAUTH_TOKEN);

            this.Client = new Client();

            this.Client.Initialize(credentials, this._channelToConnectTo);

            this.Client.OnConnected += OnConnected;
            this.Client.OnJoinedChannel += OnJoinedChannel;
            this.Client.OnChatCommandReceived += OnChatCommandReceived;

            this.Client.Connect();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Fired when the client is connected to Twitch
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event args</param>
        private void OnConnected(object sender, OnConnectedArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.AutoJoinChannel))
                Debug.Log($"The bot {e.BotUsername} succesfully connected to Twitch.");
            else
                Debug.Log($"The bot {e.BotUsername} succesfully connected to Twitch and will now attempt to automatically join the channel provided when the Initialize method was called: {e.AutoJoinChannel}");
        }

        /// <summary>
        /// Fired when the client joins a channel
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event args</param>
        private void OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            Debug.Log($"{e.BotUsername} just joined the channel: {e.Channel}");
        }

        /// <summary>
        /// Fired when the client receives a command
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event args</param>
        private void OnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
        {
            this.OnCommandReceived?
                .Invoke(
                    e.Command.CommandText,
                    Enumerable
                        .Range(0, e.Command.ArgumentsAsList.Count / 2)
                        .Select(index => (e.Command.ArgumentsAsList[index * 2], e.Command.ArgumentsAsList[index * 2 + 1]))
                        .ToList()
                );
        }

        #endregion
    }
}