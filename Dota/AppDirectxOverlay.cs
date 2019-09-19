﻿using AssaultCubeESP.game;
using AssaultCubeESP.structs;
using AssaultCubeESP.util;
using Binarysharp.MemoryManagement;
using Overlay.NET.Common;
using Overlay.NET.Directx;
using Process.NET.Windows;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssaultCubeESP
{
    [RegisterPlugin("DirectXverlayDemo-1", "Jacob Kemple", "DirectXOverlayDemo", "0.0",
       "A basic demo of the DirectXoverlay.")]
    class AppDirectXOverlay : DirectXOverlayPlugin
    {
        private readonly TickEngine _tickEngine = new TickEngine();
        public readonly ISettings<OverlaySettings> Settings = new SerializableSettings<OverlaySettings>();

        private int _displayFps;
        private int _font;
        private int _hugeFont;
        private int _i;
        private int _interiorBrush;
        private int _redBrush;
        private int _redOpacityBrush;
        private float _rotation;
        private Stopwatch _watch;
        private System.Diagnostics.Process _process;
        private Memory _memory;

        private Player _mePlayer;
        private List<Player> _players = new List<Player>();
        private int _numPlayers;

        private Matrix _viewMatrix;

       

        public AppDirectXOverlay(System.Diagnostics.Process process)
        {
            this._process = process;
        }

        public override void Initialize(IWindow targetWindow)
        {
            
            // Set target window by calling the base method
            base.Initialize(targetWindow);
            _memory = new Memory(_process);

            // For demo, show how to use settings
            var current = Settings.Current;
            
            var type = GetType();

            if (current.UpdateRate == 0)
                current.UpdateRate = 1000 / 60;

            current.Author = GetAuthor(type);
            current.Description = GetDescription(type);
            current.Identifier = GetIdentifier(type);
            current.Name = GetName(type);
            current.Version = GetVersion(type);

            // File is made from above info
            Settings.Save();
            Settings.Load();
            Console.Title = @"Application";

            OverlayWindow = new DirectXOverlayWindow(targetWindow.Handle, false);
            _watch = Stopwatch.StartNew();

            _redBrush = OverlayWindow.Graphics.CreateBrush(0x7FFF0000);
            _redOpacityBrush = OverlayWindow.Graphics.CreateBrush(Color.FromArgb(80, 255, 0, 0));
            _interiorBrush = OverlayWindow.Graphics.CreateBrush(0x7FFFFF00);

            _font = OverlayWindow.Graphics.CreateFont("Arial", 12);
            _hugeFont = OverlayWindow.Graphics.CreateFont("Arial", 15, true);

            _rotation = 0.0f;
            _displayFps = 0;
            _i = 0;
            // Set up update interval and register events for the tick engine.

            _tickEngine.PreTick += OnPreTick;
            _tickEngine.Tick += OnTick;
        }

        private void OnTick(object sender, EventArgs e)
        {
            if (!OverlayWindow.IsVisible)
            {
                return;
            }

            OverlayWindow.Update();
            InternalRender();
        }

        private void OnPreTick(object sender, EventArgs e)
        {
            var targetWindowIsActivated = TargetWindow.IsActivated;
            if (!targetWindowIsActivated && OverlayWindow.IsVisible)
            {
                _watch.Stop();
                ClearScreen();
                OverlayWindow.Hide();
            }
            else if (targetWindowIsActivated && !OverlayWindow.IsVisible)
            {
                OverlayWindow.Show();
            }
        }

        // ReSharper disable once RedundantOverriddenMember
        public override void Enable()
        {
            _tickEngine.Interval = Settings.Current.UpdateRate.Milliseconds();
            _tickEngine.IsTicking = true;
            base.Enable();
        }

        // ReSharper disable once RedundantOverriddenMember
        public override void Disable()
        {
            _tickEngine.IsTicking = false;
            base.Disable();
        }

        public override void Update() => _tickEngine.Pulse();

        protected void InternalRender()
        {
            if (!_watch.IsRunning)
            {
                _watch.Start();
            }

     
           // ReadGameMemory();
           // Draw();
         
            _rotation += 0.03f; //related to speed

            if (_rotation > 50.0f) //size of the swastika
            {
                _rotation = -50.0f;
            }

            if (_watch.ElapsedMilliseconds > 1000)
            {
                _i = _displayFps;
                _displayFps = 0;
                _watch.Restart();
            }

            else
            {
                _displayFps++;
            }

            OverlayWindow.Graphics.DrawText("fps: " + _i, _font, _redBrush, 10, 30, false);

            OverlayWindow.Graphics.EndScene();
        }

     

        public override void Dispose()
        {
            OverlayWindow.Dispose();
            base.Dispose();
        }

        private void ClearScreen()
        {
            OverlayWindow.Graphics.BeginScene();
            OverlayWindow.Graphics.ClearScene();
            OverlayWindow.Graphics.EndScene();
        }
        private void ReadGameMemory()
        {

            IntPtr ptrPlayerSelf = _memory[AppConstant.AddBaseGame+AppConstant.OffPlayerEntity,false].Read<IntPtr>();
            _mePlayer = new Player(ptrPlayerSelf, _memory);
            _players.Clear();
            _numPlayers = _memory[AppConstant.AddBaseGame + AppConstant.OffNumPlayers,false].Read<int>();
            IntPtr ptrPlayerArray = _memory[AppConstant.AddBaseGame + AppConstant.OffPlayerArray, false].Read<IntPtr>();
            for (int i = 0; i < _numPlayers; i++)
            {
                //each pointer is 4 bytes apart in the array
                //pointer to player = pointer to array + index of player * byte size
                int offset = (i * 0x04);
                IntPtr ptrPlayer = _memory[ptrPlayerArray + offset, false].Read<IntPtr>();
                if(ptrPlayer != IntPtr.Zero)
                    _players.Add(new Player(ptrPlayer,_memory));
            }

            _viewMatrix = _memory.ReadMatrix(AppConstant.AddViewMatrix);
        }
        private void Draw()
        {
            OverlayWindow.Graphics.BeginScene();
            OverlayWindow.Graphics.ClearScene();
            OverlayWindow.Graphics.DrawText($"Players:{_numPlayers}", _font, _redBrush, 90, 30);
            foreach (Player p in _players)
            {
                if (p.Health <= 0) continue;
                var color = p.Team == _mePlayer.Team ? _interiorBrush : _redBrush;
                SharpDX.Vector2 headPos, footPos;
                int offset = 20;
                if (_viewMatrix.WorldToScreen(p.PositionHead, TargetWindow.Width, TargetWindow.Height, out headPos) &&
                   _viewMatrix.WorldToScreen(p.PositionFoot, TargetWindow.Width, TargetWindow.Height, out footPos))
                {
                    float height = Math.Abs(headPos.Y - footPos.Y);
                    float width = height / 2;
                    //OverlayWindow.Graphics.DrawText(p.Name, _font, color, (int)p.PositionHead.X, (int)p.PositionHead.Y);
                    OverlayWindow.Graphics.DrawRectangle((int)(headPos.X - width / 2),(int) headPos.Y - offset, (int)width,(int) height + offset,2,color);
                }
            }
                //first row
                /*  OverlayWindow.Graphics.DrawText("DrawBarH", _font, _redBrush, 50, 40);
                  OverlayWindow.Graphics.DrawBarH(50, 70, 20, 100, 80, 2, _redBrush, _interiorBrush);

                  OverlayWindow.Graphics.DrawText("DrawBarV", _font, _redBrush, 200, 40);
                  OverlayWindow.Graphics.DrawBarV(200, 120, 100, 20, 80, 2, _redBrush, _interiorBrush);

                  OverlayWindow.Graphics.DrawText("DrawBox2D", _font, _redBrush, 350, 40);
                  OverlayWindow.Graphics.DrawBox2D(350, 70, 50, 100, 2, _redBrush, _redOpacityBrush);

                  OverlayWindow.Graphics.DrawText("DrawBox3D", _font, _redBrush, 500, 40);
                  OverlayWindow.Graphics.DrawBox3D(500, 80, 50, 100, 10, 2, _redBrush, _redOpacityBrush);

                  OverlayWindow.Graphics.DrawText("DrawCircle3D", _font, _redBrush, 650, 40);
                  OverlayWindow.Graphics.DrawCircle(700, 120, 35, 2, _redBrush);

                  OverlayWindow.Graphics.DrawText("DrawEdge", _font, _redBrush, 800, 40);
                  OverlayWindow.Graphics.DrawEdge(800, 70, 50, 100, 10, 2, _redBrush);

                  OverlayWindow.Graphics.DrawText("DrawLine", _font, _redBrush, 950, 40);
                  OverlayWindow.Graphics.DrawLine(950, 70, 1000, 200, 2, _redBrush);
      */
                //second row
                /*   OverlayWindow.Graphics.DrawText("DrawPlus", _font, _redBrush, 50, 250);
                   OverlayWindow.Graphics.DrawPlus(70, 300, 15, 2, _redBrush);

                   OverlayWindow.Graphics.DrawText("DrawRectangle", _font, _redBrush, 200, 250);
                   OverlayWindow.Graphics.DrawRectangle(200, 300, 50, 100, 2, _redBrush);

                   OverlayWindow.Graphics.DrawText("DrawRectangle3D", _font, _redBrush, 350, 250);
                   OverlayWindow.Graphics.DrawRectangle3D(350, 320, 50, 100, 10, 2, _redBrush);

                   OverlayWindow.Graphics.DrawText("FillCircle", _font, _redBrush, 800, 250);
                   OverlayWindow.Graphics.FillCircle(850, 350, 50, _redBrush);

                   OverlayWindow.Graphics.DrawText("FillRectangle", _font, _redBrush, 950, 250);
                   OverlayWindow.Graphics.FillRectangle(950, 300, 50, 100, _redBrush);
       */
            }
    }
}
