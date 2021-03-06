﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using ImageService.Logging;
using ImageService.Logging.Modal;
using ImageService.Modal;
using ImageService.Controller;
using ImageService.Server;
using System.Configuration;
using ImageService.Communication.Commands;
using ImageService.Infrastructure.Enums;
using ImageService.Communication;
using ImageService.ImageService.Configuration;
using ImageService.Infrastructure;

namespace ImageService.ImageService
{
    public enum ServiceState
    {
        SERVICE_STOPPED = 0x00000001,
        SERVICE_START_PENDING = 0x00000002,
        SERVICE_STOP_PENDING = 0x00000003,
        SERVICE_RUNNING = 0x00000004,
        SERVICE_CONTINUE_PENDING = 0x00000005,
        SERVICE_PAUSE_PENDING = 0x00000006,
        SERVICE_PAUSED = 0x00000007,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ServiceStatus
    {
        public int dwServiceType;
        public ServiceState dwCurrentState;
        public int dwControlsAccepted;
        public int dwWin32ExitCode;
        public int dwServiceSpecificExitCode;
        public int dwCheckPoint;
        public int dwWaitHint;
    };

    public partial class ImageService : ServiceBase
    {
        private System.ComponentModel.IContainer components;
        private System.Diagnostics.EventLog eventLog1;
        private ImageServer m_server;
        private ConfigManager manager;
        private ILoggingService logger;
        private TcpServer tcpServer;
        private TcpServer tcpApp;
        private IClientManager handler;
        private StatusUpdater s_updater;

        public ImageService()
        {
            InitializeComponent();

            this.manager = new ConfigManager();

            eventLog1 = new System.Diagnostics.EventLog();

            //getting the Source name and the Log name
            if (!System.Diagnostics.EventLog.SourceExists(manager.SourceName))
            {
                System.Diagnostics.EventLog.CreateEventSource(manager.SourceName, manager.LogName);
            }
            eventLog1.Source = manager.SourceName;
            eventLog1.Log = manager.LogName;
        }

        protected override void OnStart(string[] args)
        {
            initialize();
                        
            // Update the service state to Start Pending.  
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            // Update the service state to Running.  
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);            
        }

        private void initialize()
        {
            #region creating the LoggingService
            this.logger = new LoggingService();
            logger.MessageReceived += OnMessage;
            #endregion

            #region creating the LogArchive
            LogArchive archive = new LogArchive();
            logger.MessageReceived += archive.OnLog;
            #endregion

            logger.Log("Service started", MessageTypeEnum.INFO);                     

            #region creating the ImageModal
            IImageServiceModal modal = new ImageServiceModal(manager.OutputDirectory, manager.ThumbnailSize);
            #endregion

            #region creating the ImageController
            IImageController controller = new ImageController(modal);
            #endregion

            #region creating the ImageServer
            m_server = new ImageServer(controller, logger, manager.Handlers);
            // manager will get notified that a handler was removed
            m_server.RemoveHandler += manager.OnHandlerRemove;
            #endregion

            #region creating the class who notifies clients about a change in the status
            s_updater = new StatusUpdater();
            #endregion

            #region creating dictionary of commands and giving them to ClientHandler
            Dictionary<int, IHandlerCommand> commands = new Dictionary<int, IHandlerCommand>();
            commands[(int)CommandEnum.LogCommand] = new DataCommand(archive);
            commands[(int)CommandEnum.ConfigCommand] = new DataCommand(manager);
            commands[(int)CommandEnum.StatusCommand] = new DataCommand(s_updater);
            commands[(int)CommandEnum.CloseCommand] = new RemoveHandlerCommand(m_server);
            this.handler = new ClientManager(commands);
            #endregion
            
            #region creating two classes who are notified of new logs and change in settings
            LogUpdater l_updater = new LogUpdater();
            logger.MessageReceived += l_updater.OnLog;
            l_updater.logUpdate += handler.Broadcast;
            SettingsUpdater c_updater = new SettingsUpdater();
            m_server.RemoveHandler += c_updater.OnHandlerRemoved;
            c_updater.settingsUpdate += handler.Broadcast;
            #endregion

            #region creating the TCP server
            this.tcpServer = new TcpServer(handler, this.manager, ServerEnum.UIServer);
            // starting to wait for client connections
            tcpServer.Start();
            #endregion

            s_updater.statusUpdate += handler.Broadcast;
            
            s_updater.OnStatus("Service started");
            
            #region creating tcp server for app communication
            AppHandler app_handler = new AppHandler(manager.Handlers[0], logger);
            this.tcpApp = new TcpServer(app_handler, this.manager, ServerEnum.AppServer);
            this.tcpApp.Start();
            #endregion
        }

        protected override void OnPause()
        {
            logger.Log("Service paused", MessageTypeEnum.INFO);
            s_updater.OnStatus("Service paused");

            // Update the service state to Pause Pending.  
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_PAUSE_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            // Update the service state to Paused.  
            serviceStatus.dwCurrentState = ServiceState.SERVICE_PAUSED;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }

        protected override void OnContinue()
        {
            logger.Log("Service continued", MessageTypeEnum.INFO);
            s_updater.OnStatus("Service continued");

            // Update the service state to Continue Pending.  
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_CONTINUE_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }

        protected override void OnStop()
        {
            m_server.ServerClose();
            logger.Log("Service stopped", MessageTypeEnum.INFO);
            s_updater.OnStatus("Service stopped");
            this.handler.CloseClients();
            this.tcpServer.Stop();
            this.tcpApp.Stop();

            // Update the service state to Stop Pending.  
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOP_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            // Update the service state to Stopped.  
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOPPED;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }

        private void OnMessage(object sender, MessageRecievedEventArgs e)
        {
            eventLog1.WriteEntry(e.Message);
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);
    }
}
