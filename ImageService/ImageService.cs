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
using ImageService.Infrastructure;

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
        private ImageServer m_imageServer;          // The Image Server
		private IImageServiceModal modal;
		private IImageController controller;
		private ILoggingService logging;

		// Here You will Use the App Config!
        protected override void OnStart(string[] args)
        {
            // Update the service state to Start Pending.  
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            eventLog.WriteEntry("Image Service has Started.", EventLogEntryType.Information);
            logging = new LoggingService();
            logging.MessageRecieved += LoggingService_log;

            // Update the service state to Running.
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }

        protected override void OnStop()
        {
            // Update the service state to Start Pending.  
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOP_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            eventLog.WriteEntry("Image Service has Ended", EventLogEntryType.Information);

            // Update the service state to Running.  
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOPPED;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }

        private void LoggingService_log(object sender, MessageRecievedEventArgs e)
        {
            switch(e.Status)
            {
                case MessageTypeEnum.FAIL:
                        eventLog.WriteEntry(e.Message, EventLogEntryType.Error);
                        break;
                case MessageTypeEnum.INFO:
                        eventLog.WriteEntry(e.Message, EventLogEntryType.Information);
                        break;
                case MessageTypeEnum.WARNING:
                        eventLog.WriteEntry(e.Message, EventLogEntryType.Warning);
                        break;
            }
        }
    }
}
