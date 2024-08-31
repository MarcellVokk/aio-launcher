﻿using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace AllInOneLauncher.Elements
{
    /// <summary>
    /// Interaction logic for NanoSpinner.xaml
    /// </summary>
    public partial class NanoSpinner : UserControl
    {
        public NanoSpinner()
        {
            InitializeComponent();
            mainStoryboard = (Storyboard)image_loading.Resources["rotateAnim"];
            mainStoryboard.Begin();
        }

        private readonly Storyboard mainStoryboard;

        private bool _isLoading = false;
        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }

            set
            {
                _isLoading = value;

                if (value)
                {
                    Visibility = Visibility.Visible;
                    mainStoryboard.Resume();
                }
                else
                {
                    Visibility = Visibility.Hidden;
                    mainStoryboard.Pause();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (IsLoading)
            {
                Visibility = Visibility.Visible;
                mainStoryboard.Resume();
            }
            else
            {
                Visibility = Visibility.Hidden;
                mainStoryboard.Pause();
            }
        }
    }
}
