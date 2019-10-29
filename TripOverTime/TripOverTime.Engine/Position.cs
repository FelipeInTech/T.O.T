﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TripOverTime.EngineNamespace
{
    public class Position
    {
        float _x;
        float _y;

        /// <summary>
        /// Init position
        /// </summary>
        /// <param name="x">Default 0</param>
        /// <param name="y">Default 0</param>
        public Position(float x = 0, float y = 0)
        {
            _x = x;
            _y = y;
        }

        public float X
        {
            get => _x;
            internal set => _x = value;
        }
        public float Y
        {
            get => _y;
            internal set => _y = value;
        }
    }
}