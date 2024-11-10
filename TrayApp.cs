namespace immersion_player
{
    internal class TrayApp : ApplicationContext
    {
        private readonly NotifyIcon _trayIcon;
        internal readonly Player MainPlayer;

        public TrayApp()
        {
            _trayIcon = new NotifyIcon() {
                Icon = new Icon("app.ico"),
                Text = "Immersion Player",
                ContextMenuStrip = CreateContextMenu(),
                Visible = true
            };

            using FolderBrowserDialog folderBrowserDialog = new();
            folderBrowserDialog.Description = "Select Audio Library";
            DialogResult result = folderBrowserDialog.ShowDialog();
            if(result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
            {
                MainPlayer = new Player(folderBrowserDialog.SelectedPath);
            }
            else
            {
                throw new Exception("Invalid Folder selected.");
            }
        }

        private ContextMenuStrip CreateContextMenu()
        {
            ContextMenuStrip menu = new();

            ToolStripMenuItem currentlyPlayingItem = new("Currently Playing");
            ToolStripMenuItem playMenuItem = new("Play");
            ToolStripMenuItem pauseMenuItem = new("Pause");
            ToolStripMenuItem volumeMenuItem = new("Low Volume");
            ToolStripMenuItem muteMenuItem = new("Unmuted");
            ToolStripMenuItem exitMenuItem = new("Exit");

            menu.Items.Add(currentlyPlayingItem);
            menu.Items.Add(playMenuItem);
            menu.Items.Add(pauseMenuItem);
            menu.Items.Add(volumeMenuItem);
            menu.Items.Add(muteMenuItem);
            menu.Items.Add(exitMenuItem);

            currentlyPlayingItem.Click += CurrentlyPlayingItem_Click;
            playMenuItem.Click += PlayMenuItem_Click;
            pauseMenuItem.Click += PauseMenuItem_Click;
            volumeMenuItem.Click += VolumeMenuItem_Click;
            muteMenuItem.Click += MuteMenuItem_Click;
            exitMenuItem.Click += ExitMenuItem_Click;

            return menu;
        }

        private void CurrentlyPlayingItem_Click(object? sender, EventArgs e)
        {
            string fileName = MainPlayer.GetCurrentSong();
            Clipboard.SetText(fileName);
        }

        private void PlayMenuItem_Click(object? sender, EventArgs e)
        {
            MainPlayer.Play();
        }

        private void PauseMenuItem_Click(object? sender, EventArgs e)
        {
            MainPlayer.Pause();
        }

        private void VolumeMenuItem_Click(object? sender, EventArgs e)
        {
            bool lowVolume = MainPlayer.ToggleVolume();
            _trayIcon.ContextMenuStrip!.Items[3].Text = lowVolume ? "Low Volume" : "High Volume";
        }

        private void MuteMenuItem_Click(object? sender, EventArgs e)
        {
            bool muted = MainPlayer.Mute();
            _trayIcon.ContextMenuStrip!.Items[4].Text = muted ? "Muted" : "Unmuted";
        }

        private void ExitMenuItem_Click(object? sender, EventArgs e)
        {
            MainPlayer.Dispose();
            _trayIcon.Dispose();
            Application.Exit();
        }
    }
}
