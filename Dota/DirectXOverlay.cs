using AssaultCubeESP.game;
using AssaultCubeESP.util;
using Overlay.NET;
using Process.NET;
using Process.NET.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssaultCubeESP
{
    class DirectXOverlay
    {
        private OverlayPlugin _directXoverlayPlugin;
        private ProcessSharp _processSharp;
        private Logger _logger;
        private System.Diagnostics.Process _process;

        public DirectXOverlay()
        {
            _logger = new Logger();
        }

        public void StartDirectX()
        {
            _logger.logInfo("Process name {0} using for application.", AppConstant.ProcessName);
            _process = System.Diagnostics.Process.GetProcessesByName(AppConstant.ProcessName).FirstOrDefault();
            if (_process == null)
            {
                _logger.logError("No process found with name {0}",AppConstant.ProcessName);
                Console.ReadLine();
                return;
            }
            _directXoverlayPlugin = new AppDirectXOverlay(_process);
            _processSharp = new ProcessSharp(_process, MemoryType.Remote);
            _logger.logInfo("Application using frame rate : {0}", AppConstant.Fps);
            var d3DOverlay = (AppDirectXOverlay)_directXoverlayPlugin;
            d3DOverlay.Settings.Current.UpdateRate = 1000 / AppConstant.Fps;
            _directXoverlayPlugin.Initialize(_processSharp.WindowFactory.MainWindow);
            _directXoverlayPlugin.Enable();

            // Log some info about the overlay.
            _logger.logInfo("Starting update loop (open the process you specified and drag around)");
            _logger.logInfo("Update rate: {0}" ,d3DOverlay.Settings.Current.UpdateRate);

            var info = d3DOverlay.Settings.Current;

            _logger.logInfo($"Author: {info.Author}");
            _logger.logInfo($"Description: {info.Description}");
            _logger.logInfo($"Name: {info.Name}");
            _logger.logInfo($"Identifier: {info.Identifier}");
            _logger.logInfo($"Version: {info.Version}");

            _logger.logInfo("Note: Settings are saved to a settings folder in your main app folder.");

            _logger.logInfo("Give your window focus to enable the overlay (and unfocus to disable..)");

         

            while (true)
            {
                _directXoverlayPlugin.Update();
            }

        }
    }
}
