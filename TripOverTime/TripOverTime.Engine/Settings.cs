﻿using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace TripOverTime.EngineNamespace
{
    public class Settings
    {
        const ushort MAX_LINES = 4;

        RenderWindow _window;
        ushort _selected;
        Text[] _lines;
        SFML.Graphics.Sprite _background;
        uint _charSize = 32;

        internal Settings(RenderWindow window)
            {
             if (window == null) throw new ArgumentException("Window is null");

            _window = window;
            _selected = 0;
            _lines = new Text[MAX_LINES];
            }

        public void StartSettings()
        {
            //Background
            // Set background
            _background = new SFML.Graphics.Sprite(new Texture(@"..\..\..\..\Assets\Backgrounds\colored_desert.png"));
            if (_background == null) throw new Exception("Sprite null!");

            _background.Position = new SFML.System.Vector2f(0, -(float)_window.Size.Y / 2);
            _window.Draw(_background);

            //Lines
            _lines[0] = new Text("Resolution", new Font(@"..\..\..\..\Assets\Fonts\Blanka-Regular.ttf"), _charSize);
            _lines[1] = new Text("FPS", new Font(@"..\..\..\..\Assets\Fonts\Blanka-Regular.ttf"), _charSize);
            _lines[2] = new Text("Key Binding", new Font(@"..\..\..\..\Assets\Fonts\Blanka-Regular.ttf"), _charSize);
            _lines[3] = new Text("Return", new Font(@"..\..\..\..\Assets\Fonts\Blanka-Regular.ttf"), _charSize);

            _lines[0].Position = new SFML.System.Vector2f(_window.Size.X / 2, (_window.Size.Y / 6) * 1);
            _lines[1].Position = new SFML.System.Vector2f(_window.Size.X / 2, (_window.Size.Y / 6) * 2);
            _lines[2].Position = new SFML.System.Vector2f(_window.Size.X / 2, (_window.Size.Y / 6) * 3);
            _lines[3].Position = new SFML.System.Vector2f(_window.Size.X / 2, (_window.Size.Y / 6) * 4);

            _window.Display();
        }

        public void StartSettingsResolution()
        {
            //Background
            // Set background
            _background = new SFML.Graphics.Sprite(new Texture(@"..\..\..\..\Assets\Backgrounds\colored_desert.png"));
            if (_background == null) throw new Exception("Sprite null!");

            _background.Position = new SFML.System.Vector2f(0, -(float)_window.Size.Y / 2);
            _window.Draw(_background);

            //Lines
            _lines[0] = new Text("Resolution1", new Font(@"..\..\..\..\Assets\Fonts\Blanka-Regular.ttf"), _charSize);
            _lines[1] = new Text("Resolution1", new Font(@"..\..\..\..\Assets\Fonts\Blanka-Regular.ttf"), _charSize);
            _lines[2] = new Text("Resolution1", new Font(@"..\..\..\..\Assets\Fonts\Blanka-Regular.ttf"), _charSize);
            _lines[3] = new Text("Return", new Font(@"..\..\..\..\Assets\Fonts\Blanka-Regular.ttf"), _charSize);

            _lines[0].Position = new SFML.System.Vector2f(_window.Size.X / 2, (_window.Size.Y / 6) * 1);
            _lines[1].Position = new SFML.System.Vector2f(_window.Size.X / 2, (_window.Size.Y / 6) * 2);
            _lines[2].Position = new SFML.System.Vector2f(_window.Size.X / 2, (_window.Size.Y / 6) * 3);
            _lines[3].Position = new SFML.System.Vector2f(_window.Size.X / 2, (_window.Size.Y / 6) * 4);

            _window.Display();
        }
        public short RunSettings()
        {
            //Events
            short result = -2;
            short tampon = 0;

            if (Keyboard.IsKeyPressed(Keyboard.Key.Escape))
            {
                _window.Close();
                result = -1;
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.Enter))
            {
                if (_selected == 2) result = -1;
                else result = (short)_selected;
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.Down) && _selected < MAX_LINES - 1)
            {
                tampon = 1;
                _selected++;
            }
            else if (Keyboard.IsKeyPressed(Keyboard.Key.Up) && _selected > 0)
            {
                tampon = 2;
                _selected--;
            }


            //Graphics
            _window.Clear();
            _window.Draw(_background);

            for (int i = 0; i < MAX_LINES; i++)
            {
                if (i == _selected)
                {
                    _lines[i].Color = Color.Red;
                }
                else
                {
                    _lines[i].Color = Color.White;
                }
                _window.Draw(_lines[i]);
            }

            _window.Display();

            if (tampon == 1)
            {
                while (Keyboard.IsKeyPressed(Keyboard.Key.Down)) ; //Tampon
            }
            else if (tampon == 2)
            {
                while (Keyboard.IsKeyPressed(Keyboard.Key.Up)) ; //Tampon
            }

            return result;
        }

    }
}
