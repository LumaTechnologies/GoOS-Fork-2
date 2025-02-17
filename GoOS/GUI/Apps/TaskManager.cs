﻿using System;
using Cosmos.System;
using PrismAPI.Graphics;

namespace GoOS.GUI.Apps
{
    public class TaskManager : Window
    {
        Button EndButton;
        Button AboutButton;
        List Windows;

        public TaskManager()
        {
            Contents = new Canvas(270, 310);
            X = 200;
            Y = 100;
            Title = "Task Manager";
            Visible = true;
            Closable = true;
            Unkillable = true;

            EndButton = new Button(this, Convert.ToUInt16(Contents.Width - 90), Convert.ToUInt16(Contents.Height - 30), 80, 20, " End task ") { Clicked = EndButton_Click };
            AboutButton = new Button(this, Convert.ToUInt16(Contents.Width - 124), Convert.ToUInt16(Contents.Height - 30), 24, 20, "?") { Clicked = ShowAboutDialog };
            Windows = new List(this, 10, 10, Convert.ToUInt16(Contents.Width - 20), Convert.ToUInt16(Contents.Height - 60), "Processes", Array.Empty<string>());

            WindowManager.TaskmanHook = Update;

            // Render the buttons.
            Contents.Clear(Color.White);
            RenderSystemStyleBorder();
            Contents.DrawFilledRectangle(2, Convert.ToUInt16(Contents.Height - 40), Convert.ToUInt16(Contents.Width - 4), 38, 0, new Color(234, 234, 234));
            AboutButton.Render();
            EndButton.Render();
        }

        public override void HandleKey(KeyEvent key)
        {
            switch (key.Key)
            {
                case ConsoleKeyEx.F1:
                    ShowAboutDialog();
                    break;

                case ConsoleKeyEx.F5:
                    Update();
                    break;
            }
        }

        private void Update()
        {
            Windows.Items = new string[WindowManager.windows.Count]; // Reallocate array size.
            for (int i = 0; i < Windows.Items.Length; i++)
                Windows.Items[i] = WindowManager.windows[i].Title; // Copy the title from the windows array to the items array.

            Windows.Render(); // Render the window list.
        }

        private void EndButton_Click()
        {
            if (WindowManager.windows[Windows.Selected].Unkillable)
            {
                Dialogue.Show(
                    "Error",
                    "System processes are not\nendable.",
                    null,
                    WindowManager.errorIcon);
            }
            else
            {
                WindowManager.windows[Windows.Selected].Closing = true; // Close the window.
            }
        }
    }
}
