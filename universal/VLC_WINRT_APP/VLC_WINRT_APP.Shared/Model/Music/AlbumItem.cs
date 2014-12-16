﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using SQLite;
using VLC_WINRT_APP.Commands.Music;
using VLC_WINRT_APP.Commands.MusicPlayer;
using VLC_WINRT_APP.Common;
using VLC_WINRT_APP.Helpers.MusicLibrary;
using VLC_WINRT_APP.ViewModels.MusicVM;

namespace VLC_WINRT_APP.Model.Music
{

    public class AlbumItem : BindableBase
    {
        private string _name;
        private string _artist;
        private string _picture = "";
        private uint _year;
        private bool _favorite;
        private bool _isPictureLoaded = false;
        private bool _isTracksLoaded = false;
        private ObservableCollection<TrackItem> _trackItems;
        private PlayAlbumCommand _playAlbumCommand = new PlayAlbumCommand();
        private FavoriteAlbumCommand _favoriteAlbumCommand = new FavoriteAlbumCommand();
        private AlbumTrackClickedCommand _trackClickedCommand = new AlbumTrackClickedCommand();
        private ArtistClickedCommand _viewArtist = new ArtistClickedCommand();
        private PinAlbumCommand _pinAlbumCommand = new PinAlbumCommand();
#if WINDOWS_PHONE_APP
        private SeeArtistShowsCommand seeArtistShowsCommand;
#endif
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }

        public int ArtistId { get; set; }

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public string Artist
        {
            get { return _artist; }
            set
            {
                SetProperty(ref _artist, value);
            }
        }

        public bool Favorite
        {
            get { return _favorite; }
            set
            {
                SetProperty(ref _favorite, value);
            }
        }

        [Ignore]
        public ObservableCollection<TrackItem> Tracks
        {
            get
            {
                if (!_isTracksLoaded)
                {
                    _isTracksLoaded = true;
                    Task.Run(async () => await this.GetTracks());
                }
                return _trackItems ?? (_trackItems = new ObservableCollection<TrackItem>());
            }
            set { SetProperty(ref _trackItems, value); }
        }

        [Ignore]
        public string Picture
        {
            get
            {
                if (!_isPictureLoaded)
                {
                    _isPictureLoaded = true;
                    _picture = "ms-appx:///Assets/NoCover.jpg";
                    Task.Run(() => LoadPicture());
                }
                return _picture;
            }
            set
            {
                SetProperty(ref _picture, value);
                OnPropertyChanged();
            }
        }

        public bool IsPictureLoaded
        {
            get { return _isPictureLoaded; }
            set
            {
                SetProperty(ref _isPictureLoaded, value);
                if (value)
                {
                    _picture = "ms-appdata:///local/albumPic/" + Id + ".jpg";
                    OnPropertyChanged("Picture");
                }
            }
        }

        public uint Year
        {
            get { return _year; }
            set { SetProperty(ref _year, value); }
        }

        public async Task LoadPicture()
        {
            try
            {
                await ArtistInformationsHelper.GetAlbumPicture(this);
            }
            catch (Exception)
            {
                // TODO: Tell user we could not get their album art.
                Debug.WriteLine("Error getting album art...");
            }
        }

        [Ignore]
        public ArtistClickedCommand ViewArtist
        {
            get { return _viewArtist; }
            set { SetProperty(ref _viewArtist, value); }
        }

        [Ignore]
        public PlayAlbumCommand PlayAlbum
        {
            get { return _playAlbumCommand; }
            set { SetProperty(ref _playAlbumCommand, value); }
        }

        [Ignore]
        public FavoriteAlbumCommand FavoriteAlbum
        {
            get { return _favoriteAlbumCommand; }
            set { SetProperty(ref _favoriteAlbumCommand, value); }
        }

        [Ignore]
        public AlbumTrackClickedCommand TrackClicked
        {
            get { return _trackClickedCommand; }
            set { SetProperty(ref _trackClickedCommand, value); }
        }

        [Ignore]
        public PinAlbumCommand PinAlbumCommand
        {
            get { return _pinAlbumCommand; }
            set { SetProperty(ref _pinAlbumCommand, value); }
        }

#if WINDOWS_PHONE_APP
        [Ignore]
        public SeeArtistShowsCommand SeeArtistShowsCommand
        {
            get
            {
                return seeArtistShowsCommand ?? (seeArtistShowsCommand = new SeeArtistShowsCommand());
            }
        }
#endif
    }
}
