using NAudio.Wave;

namespace immersion_player
{
    internal class Player
    {
        private readonly string _libraryPath;
        private readonly List<string> _library;
        private readonly Random _rng;
        private int _currentlyPlaying;
        private bool _paused;
        private float _fullVolume;
        private AudioFileReader? _audioFileReader;
        private readonly WaveOutEvent _outputDevice;

        private readonly string[] _fileFormats = [
            "mp3", "aac", "m4a", "wma", "ogg", "flac", "alac", "wav", "aiff", "dsd", "dff", "dsf"
        ];

        internal Player(string filePath)
        {
            _libraryPath = filePath;

            _rng = new Random();
            _currentlyPlaying = -1;
            _paused = false;
            _fullVolume = 0.01f;

            _library = [];
            string[] allFiles = Directory.GetFiles(_libraryPath);
            foreach(string file in allFiles)
            {
                string[] file_extensions = file.Split('.');
                if(file_extensions.Length > 1)
                {
                    if(_fileFormats.Any(file_extensions[^1].Contains))
                    {
                        _library.Add(file);
                    }
                }
            }

            _outputDevice = new() {
                Volume = _fullVolume
            };
            _outputDevice.PlaybackStopped += OutputDevice_PlaybackStopped;
            Play();
        }

        internal void Play()
        {
            if(_audioFileReader == null)
            {
                int nextPlaying;
                do
                {
                    nextPlaying = _rng.Next(0, _library.Count);
                }
                while(nextPlaying == _currentlyPlaying);
                _currentlyPlaying = nextPlaying;

                _audioFileReader = new AudioFileReader(_library[_currentlyPlaying]);

                try
                {
                    _outputDevice.Init(_audioFileReader);
                }
                catch(Exception ex)
                {
                    throw new Exception(ex.Message + "\nFile: " + _library[_currentlyPlaying].ToString() + "(" + _currentlyPlaying + ")");
                }
            }
            _outputDevice.Play();
        }

        private void OutputDevice_PlaybackStopped(object? sender, StoppedEventArgs e)
        {
            if(!_paused)
            {
                _audioFileReader?.Dispose();
                _audioFileReader = null;
                Play();
            }
        }

        internal void Pause()
        {
            _paused = true;
            _outputDevice.Pause();
        }

        internal bool Mute()
        {
            if(_outputDevice.Volume > 0)
            {
                _outputDevice.Volume = 0;
                return true;
            }
            _outputDevice.Volume = _fullVolume;
            return false;
        }

        internal bool ToggleVolume()
        {
            if(_fullVolume == 1f)
            {
                _fullVolume = 0.01f;
                Mute();
                Mute();
                return true;
            }
            else
            {
                _fullVolume = 1f;
                Mute();
                Mute();
                return false;
            }
        }

        internal string GetCurrentSong()
        {
            if(_audioFileReader != null)
            {
                return _library[_currentlyPlaying];
            }
            return "null";
        }

        internal void Dispose()
        {
            _audioFileReader?.Dispose();
            _outputDevice.Dispose();
        }
    }
}
