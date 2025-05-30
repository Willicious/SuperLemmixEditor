﻿using System;
using System.Drawing;
using System.IO;

namespace NLEditor
{
    /// <summary>
    /// This class reads the SuperLemmix player's settings
    /// and determines how much of the level gets displayed
    /// </summary>
    public class ScreenSize
    {
        public ScreenSize()
        {
            SetDefault();
        }

        Size windowSize;
        Size windowSizeWithPanel;
        int zoomFactor;
        bool isFullScreen;
        bool isFillBorder;
        bool isCompactSkillPanel;

        private int skillPanelWidth => isCompactSkillPanel ? 320 : 416;
        private int skillPanelHeight => 40;

        public void InizializeSettings()
        {
            ReadPlayerSettings();
            if (isFullScreen)
            {
                windowSizeWithPanel = GetMonitorResolution();
            }
            PreprocessSettings();
        }

        /// <summary>
        /// Sets the default values for the various screen settings.
        /// </summary>
        private void SetDefault()
        {
            windowSizeWithPanel = new Size(320, 200);
            windowSize = new Size(320, 160);
            zoomFactor = 1;
            isFullScreen = false;
            isFillBorder = false;
            isCompactSkillPanel = true;
        }

        /// <summary>
        /// Reads the SuperLemmix player settings.
        /// </summary>
        private void ReadPlayerSettings()
        {
            string filePath = GetSettingsFilePath();
            if (filePath == null)
            {
                SetDefault();
                return;
            }

            try
            {
                int windowWidth = 320;
                int windowHeight = 200;

                var fileReader = new StreamReader(filePath);

                string line;
                while ((line = fileReader.ReadLine()) != null)
                {
                    line = line.Trim();
                    int seperatorIndex = line.IndexOf('=');
                    int value = (seperatorIndex > 0) ? int.Parse(line.Substring(seperatorIndex + 1)) : 0;
                    if (line.StartsWith("ZoomLevel"))
                    {
                        zoomFactor = value;
                    }
                    else if (line.StartsWith("FullScreen"))
                    {
                        isFullScreen = (value == 1);
                    }
                    else if (line.StartsWith("WindowWidth"))
                    {
                        windowWidth = value;
                    }
                    else if (line.StartsWith("WindowHeight"))
                    {
                        windowHeight = value;
                    }
                    else if (line.StartsWith("CompactSkillPanel"))
                    {
                        isCompactSkillPanel = (value == 1);
                    }
                    else if (line.StartsWith("IncreaseZoom"))
                    {
                        isFillBorder = (value == 1);
                    }
                }

                windowSizeWithPanel = new Size(windowWidth, windowHeight);

                fileReader.Dispose();
            }
            catch
            {
                SetDefault();
            }
        }

        /// <summary>
        /// Returns the location where the settings file is stored, or null if none exists.
        /// </summary>
        private string GetSettingsFilePath()
        {
            if (File.Exists(C.AppPathPlayerSettings))
                return C.AppPathPlayerSettings;
            else if (File.Exists(C.AppPathPlayerSettingsOld))
                return C.AppPathPlayerSettingsOld;
            else
                return null;
        }

        /// <summary>
        /// Gets the screen resolution of the monitor.
        /// </summary>
        private Size GetMonitorResolution()
        {
            var monitor = System.Windows.Forms.Screen.PrimaryScreen;
            return monitor.WorkingArea.Size;
        }

        /// <summary>
        /// Computes the window size used for the level
        /// </summary>
        private void PreprocessSettings()
        {
            int skillPanelZoom = Math.Min(windowSize.Width / skillPanelWidth, zoomFactor);
            int skillPanelScreenHeight = skillPanelHeight * skillPanelZoom;
            windowSize = new Size(windowSizeWithPanel.Width, windowSizeWithPanel.Height - skillPanelScreenHeight);
        }

        /// <summary>
        /// Returns the size of the level area that is covered by the window screen.
        /// </summary>
        /// <param name="levelSize"></param>
        public Size ScreenArea(int levelWidth, int levelHeight)
        {
            int zoom = zoomFactor;

            if (isFillBorder)
            {
                // Round up on the separate zoom factors.
                int zoomHoriz = (windowSize.Width + levelWidth - 1) / levelWidth;
                int zoomVert = (windowSize.Height + levelHeight - 1) / levelHeight;
                zoom = Math.Max(Math.Max(zoomHoriz, zoomVert), zoomFactor);
            }

            // Round up on the screen width and height.
            int screenWidth = (windowSize.Width + zoom - 1) / zoom;
            int screenHeight = (windowSize.Height + zoom - 1) / zoom;

            if (!isFillBorder)
            {
                screenWidth = Math.Min(screenWidth, levelWidth);
                screenHeight = Math.Min(screenHeight, levelHeight);
            }

            return new Size(screenWidth, screenHeight);
        }

    }
}
