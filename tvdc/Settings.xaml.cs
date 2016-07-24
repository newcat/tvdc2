﻿using System;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
using nUpdate.Updating;
using System.Globalization;
using System.IO;
using System.Text;

namespace tvdc
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {

        private bool clearedCache = false;
        UpdateManager manager;

        public Settings()
        {
            InitializeComponent();

            tbNick.Text = Properties.Settings.Default.nick;
            tbOauth.Text = Properties.Settings.Default.oauth;
            tbChannel.Text = Properties.Settings.Default.channel;
            cbDebug.IsChecked = Properties.Settings.Default.debug;
            cbShowEvents.IsChecked = Properties.Settings.Default.showJoinLeave;
        }

        private void linkGenerate_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start("http://www.twitchapps.com/tmi");
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (tbNick.Text.Equals("") || tbOauth.Text.Equals("") || tbChannel.Text.Equals("") || tbOauth.Text.Substring(0, 6) != "oauth:")
            {
                MessageBox.Show("Invalid input!");
                return;
            }

            DialogResult = true;

            Properties.Settings.Default.nick = tbNick.Text;
            Properties.Settings.Default.oauth = tbOauth.Text;
            Properties.Settings.Default.channel = tbChannel.Text;
            Properties.Settings.Default.debug = (bool)cbDebug.IsChecked;
            Properties.Settings.Default.showJoinLeave = (bool)cbShowEvents.IsChecked;
            Properties.Settings.Default.Save();

            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false || clearedCache;
            Close();
        }

        private void btnClearCache_Click(object sender, RoutedEventArgs e)
        {
            EmoticonManager.clearCache();
            clearedCache = true;
            MessageBox.Show(this, "Cache cleared.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnCheckUpdate_Click(object sender, RoutedEventArgs e)
        {
            UpdateWindow uw = new UpdateWindow();
            uw.ShowDialog();
        }

    }
}
