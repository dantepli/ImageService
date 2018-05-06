using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ImageService.Server;
using ImageService.Controller;
using ImageService.Modal;
using ImageService.Logging;
using ImageService.Logging.Modal;
using System.Configuration;
using ImageService.Infrastructure.Enums;

namespace ImageService
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
        // DLL import for service status function.
        [DllImport("advapi32.dll", SetLastError = true)]
            private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);

        private int eventId = 1;
        private ImageServer m_imageServer;      // The Image Server
		private IImageServiceModel m_model;
		private IImageController m_controller;
        private ILoggingService m_logging;

        public ImageService()
        {
            InitializeComponent();
            eventLog = new EventLog();

            if (!EventLog.SourceExists(ConfigurationManager.AppSettings["SourceName"]))
            {
                EventLog.CreateEventSource(
                    ConfigurationManager.AppSettings["SourceName"],
                    ConfigurationManager.AppSettings["LogName"]);
            }
            eventLog.Source = ConfigurationManager.AppSettings["SourceName"];
            eventLog.Log = ConfigurationManager.AppSettings["LogName"];
        }

		/// <summary>
        /// Starts the service.
        /// </summary>
        /// <param name="args">arguments for the service.</param>
        protected override void OnStart(string[] args)
        {
            // Update the service state to Start Pending.  
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        
            m_logging = new LoggingService();
            m_logging.MessageRecieved += OnLog;

            m_model = new ImageServiceModal(
                ConfigurationManager.AppSettings["OutputDir"],
                Int32.Parse(ConfigurationManager.AppSettings["ThumbnailSize"]));

            m_controller = new ImageController(m_model);
            
            m_imageServer = new ImageServer(m_logging, m_controller);

            CreateHandlers(ConfigurationManager.AppSettings["Handler"]);

            eventLog.WriteEntry("Image Service has Started.", EventLogEntryType.Information, eventId++);

            // Update the service state to Running.
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }
        /// <summary>
        /// Stops the service.
        /// </summary>
        protected override void OnStop()
        {
            // Update the service state to Start Pending.  
            ServiceStatus serviceStatus = new ServiceStatus
            {
                dwCurrentState = ServiceState.SERVICE_STOP_PENDING,
                dwWaitHint = 100000
            };
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            m_imageServer.CloseServer();
            eventLog.WriteEntry("Image Service has Ended.", EventLogEntryType.Information, eventId++);

            // Update the service state to Stopped.
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOPPED;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }


        /// <summary>
        /// Logging Service log event, logs the event using the system event logger.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnLog(object sender, MessageRecievedEventArgs e)
        {
            switch(e.Status)
            {
                case MessageTypeEnum.FAIL:
                        eventLog.WriteEntry(e.Message, EventLogEntryType.Error, eventId++);
                        break;
                case MessageTypeEnum.INFO:
                        eventLog.WriteEntry(e.Message, EventLogEntryType.Information, eventId++);
                        break;
                case MessageTypeEnum.WARNING:
                        eventLog.WriteEntry(e.Message, EventLogEntryType.Warning, eventId++);
                        break;
            }
        }

        /// <summary>
        /// creates a handler for each path given.
        /// </summary>
        /// <param name="handlers_paths">paths for handlers, each path is seperated by ;.</param>
        private void CreateHandlers(string handlers_paths)
        {
            string[] paths = handlers_paths.Split(';');
            foreach (string dir_path in paths)
            {
                m_imageServer.CreateHandler(dir_path);
            }
        }
    }
}
