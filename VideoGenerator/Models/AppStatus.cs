﻿using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace VideoGenerator.Models
{
    public interface IAppStatus
    {
        string Status { get; }
        bool Notify { get; }
        Brush TextColor { get; }
        bool Update (object data);
        void Hide ();
    }

    public class DefaultAppStatus : ObservableObject, IAppStatus
    {
        public string Status => "";

        public bool Notify => false;

        public Brush TextColor => Brushes.Black;

        public void Hide ()
        {
            
        }

        public bool Update (object data)
        {
            return false;
        }
    }

    public class LoadingAppStatus : ObservableObject, IAppStatus, IDisposable
    {
        private string _itemLabel = "Item";
        private string _itemLabelPlural = "Items";

        public LoadingAppStatus (int startCount, int totalCount, string itemLabel, string itemLabelPlural)
        {
            _startCount = startCount;
            _totalCount = totalCount;
            _itemLabel = itemLabel;
            _itemLabelPlural = itemLabelPlural;
            ProgressPercent = 0;
            Notify = true;
        }

        public void Dispose ()
        {
            throw new NotImplementedException();
        }


        #region Properties

        private int _startCount;
        public int StartCount
        {
            get => _startCount;
            set => SetProperty(ref _startCount, value);
        }

        private int _totalCount;
        public int TotalCount
        {
            get => _totalCount;
            set => SetProperty(ref _totalCount, value);
        }

        private string? _status;
        public string Status
        {
            get => _status ??= "";
            set => SetProperty(ref _status, value);
        }

        private bool _notify;
        public bool Notify
        {
            get => _notify;
            set => SetProperty(ref _notify, value);
        }

        private float _progressPercent;
        public float ProgressPercent
        {
            get => _progressPercent;
            set => SetProperty(ref _progressPercent, value);
        }

        private Brush _textColor = Brushes.Black;
        public Brush TextColor
        {
            get => _textColor;
            set => SetProperty(ref _textColor, value);
        }

        #endregion Properties

        #region Commands

        //Commands

        #endregion Commands

        #region Public Methods

        public bool Update (object data)
        {
            int loadingCount = _totalCount - _startCount;
            if (data is not int value || loadingCount <= 0)
            {
                Status = "Invalid Status";
                TextColor = Brushes.Red;
                return false;
            }

            TextColor = Brushes.Black;
            ProgressPercent = (value - _startCount) / (float)loadingCount;
            Status = $"{ProgressPercent:P1} Complete : Loaded {value - _startCount}/{loadingCount} {(value > 1 ? _itemLabelPlural : _itemLabel)}";
            return true;
        }

        public void Hide ()
        {
            Notify = false;
        }

        #endregion Public Methods

        #region Private Methods

        //Private Methods

        #endregion Private Methods
    }
}