﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrismAPI.Graphics;

namespace GoOS.GUI
{
    public abstract class Window
    {
        public Canvas Contents;
        public ushort X, Y;
        public string Title;
        public bool Visible;

        public abstract void Update();
    }
}
