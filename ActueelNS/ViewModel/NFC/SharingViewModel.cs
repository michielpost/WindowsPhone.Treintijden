/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
    To see all Code Samples for Windows Store apps, visit http://code.msdn.microsoft.com/windowsapps.
  
*/
using System;
using System.Diagnostics;
using System.Collections.Generic;
using ActueelNS.Services.Interfaces;
using GalaSoft.MvvmLight.Ioc;
using Treintijden.PCL.Api.Interfaces;
using Treintijden.Shared.Services.Interfaces;
using Treintijden.PCL.Api.Models;
using Newtonsoft.Json;

namespace ActueelNS.ViewModel
{

    public class ShareSearch
    {

        public PlannerSearch PlannerSearch { get; set; }
        public List<ReisMogelijkheid> ReisMogelijkheden { get; set; }
        public int? Index { get; set; }

        // Convert an object to a byte array
        public byte[] ObjectToByteArray()
        {
            string str = JsonConvert.SerializeObject(this);

            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;

        }

        public static ShareSearch Desserialize(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            var str = new string(chars);

            return JsonConvert.DeserializeObject<ShareSearch>(str);

        }

    }

    /// <summary>
    /// Singleton class that contains the logic to control the connection 
    /// between peer apps and to send pictures between them.
    /// </summary>
    public class SharingViewModel : CustomViewModelBase
    {
        public event EventHandler<ReisadviesReceivedEventArgs> ReisadviesReceived;
        public event EventHandler<ConnectionStatusChangedEventArgs> ConnectionStatusChanged;
        //public Action<Action> UIDispatcher { get; set; }
        public static SharingViewModel Instance { get; private set; }

        private byte[] _queueData;

        // Alternate identities so that peer apps can connect from Windows Phone 8 to Windows 8.
        #if NETFX_CORE
            readonly Dictionary<string, string> _alternateIdentities = new Dictionary<string, string>() { { Strings.WP8_AlternateIdentifier, Strings.WP8_AlternateIdentity } };
        #else
            //readonly Dictionary<string, string> _alternateIdentities = new Dictionary<string, string>() { { Strings.W8_AlternateIdentifier, Strings.W8_AlternateIdentity } };
        #endif
            PeerConnector _peerConnector; // PeerConnector takes care of connecting to, and communicating with, a peer app.

        static SharingViewModel()
        {
            Instance = new SharingViewModel();
        }

        SharingViewModel()
        {
            IsConnected = false;
            _peerConnector = new PeerConnector();
            _peerConnector.ConnectionStatusChanged += OnConnectionStatusChanged;
            _peerConnector.ReisadviesReceived += OnReisadviesReceived;
        }

        /// <summary>
        /// Attempt to start a sharing session
        /// </summary>
        /// <remarks>This does not mean we are connected to a peer. It just means we have successfully started the request to share.
        /// The SessionConnectionCompleted event will tell us whether a connection was established.</remarks>
        public void StartSharingSession(byte[] imageBytes)
        {
            _queueData = imageBytes;

            if (!IsConnected)
            {
                // Create a PeerConnector instance if necessary
                if (_peerConnector == null)
                {
                    _peerConnector = new PeerConnector();
                }

                // Start the connection
                _peerConnector.StartConnect();
            }
            else
                SendReisadviesToPeer(_queueData);
        }

        /// <summary>
        /// Uses PeerConnector to stop a sharing session
        /// </summary>
        public void StopSharingSession()
        {
          if (_peerConnector != null)
          {
              if (IsConnected)
                  _peerConnector.StopConnect();
              else
                  _peerConnector.CancelConnect();
             
          }

          IsConnected = false;
        }

        public bool IsConnected { get; private set; }


        /// <summary>
        /// Uses PeerConnector to send an image to a peer.
        /// </summary>
        /// <param name="imageBytes">The encoded image to send, represented as an array of bytes.</param>
        public void SendReisadviesToPeer(byte[] imageBytes)
        {
            _queueData = null;

            if (!this.IsConnected)
            {
                StartSharingSession(imageBytes);
            }
            else
            {
                if (_peerConnector != null)
                {
                    _peerConnector.SendReisadviesAsync(imageBytes);
                }
            }
        }

        ///// <summary>
        ///// Gets and sets the current picture byte array.
        ///// </summary>
        //public byte[] CurrentSharedPicture
        //{
        //    get
        //    {
        //        return _currentSharedPicture;
        //    }

        //    protected set
        //    {
        //        _currentSharedPicture = value;
        //        RaisePropertyChanged(() => CurrentSharedPicture);
        //    }
        //}

        async void OnReisadviesReceived(object sender, ReisadviesReceivedEventArgs args)
        {

            var plannerService = SimpleIoc.Default.GetInstance<IPlannerService>();
            ShareSearch s = ShareSearch.Desserialize(args.Bytes);

            await plannerService.AddSearchAsync(s.PlannerSearch);
            await plannerService.PermStoreSearchResultAsync(s.PlannerSearch.Id, s.ReisMogelijkheden);

            //SetCurrentPicture(args.Bytes);

            if (ReisadviesReceived != null)
                ReisadviesReceived(this, new ReisadviesReceivedEventArgs() { Id = s.PlannerSearch.Id, Index = s.Index });

            Debug.WriteLine("Received {0} bytes", args.Bytes.Length);

            //this.StopSharingSession();
        }

        //byte[] _currentSharedPicture;

        //void SetCurrentPicture(byte[] imageBytes)
        //{
        //    UIDispatcher(() =>
        //    {
        //        // Update the CurrentPicture with the bytes we received.
        //        this.CurrentSharedPicture = imageBytes;
        //    });
        //}

        void OnConnectionStatusChanged(object sender, ConnectionStatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case ConnectionStatus.Completed:
                    IsConnected = true;
                    if (_queueData != null)
                        SendReisadviesToPeer(_queueData);
                    break;
                case ConnectionStatus.Canceled:
                    // If I am already connected, the canel just means we accidentally tapped again and I want to stay connected.
                    // If I am not already connected, IsConnected is already false, so no need to do anything.
                    break;
                case ConnectionStatus.Disconnected:
                case ConnectionStatus.Failed:
                case ConnectionStatus.TapNotSupported:
                    IsConnected = false;
                    break;
            }

            if (ConnectionStatusChanged != null)
                ConnectionStatusChanged(this, new ConnectionStatusChangedEventArgs() { Status = e.Status });
        }
    }
}
