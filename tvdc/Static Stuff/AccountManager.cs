﻿using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace tvdc
{
    static class AccountManager
    {

        private static string _username = "";
        public static string Username
        {
            get
            {
                if (_username != "")
                    return _username;
                else
                {
                    MessageBox.Show("Illegal access on _username before logging in.");
                    return "";
                }
            }
        }

        private static string _oauth = "";
        public static string Oauth
        {
            get
            {
                if (_oauth != "")
                    return _oauth;
                else
                {
                    MessageBox.Show("Illegal access on _oauth before logging in.");
                    return "";
                }
            }
        }

        public static string OauthWithoutPrefix
        {
            get
            {
                if (_oauth != "")
                    return _oauth.Substring(6);
                else
                {
                    MessageBox.Show("Illegal access on OauthWithoutPrefix before logging in.");
                    return "";
                }
            }
        }

        public static async Task<bool> Login()
        {

            //Reload settings and check if there is already an oAuth saved
            Properties.Settings.Default.Reload();

            if (Properties.Settings.Default.oauth != "")
            {
                _oauth = Properties.Settings.Default.oauth;
                return await downloadUserData();
            }
            else
            {
                //request the user to log in to his twitch account
                LoginWindow lw = new LoginWindow();
                if (lw.ShowDialog(true) == true)
                {
                    _oauth = lw.Oauth;
                    Properties.Settings.Default.oauth = _oauth;
                    Properties.Settings.Default.Save();
                    return await downloadUserData();
                }
                else
                {
                    return false;
                }
            }

        }

        private static async Task<bool> downloadUserData()
        {

            if (Oauth == null || Oauth == "")
                return false;

            LoginWindow lw = new LoginWindow();
            lw.Show(false);

            WebClient wc = new WebClient();
            wc.Headers.Add("Client-ID", Properties.Resources.client_id);

            try
            {
                string userdataJson = await wc.DownloadStringTaskAsync("https://api.twitch.tv/kraken/user?oauth_token=" + OauthWithoutPrefix);

                if (userdataJson == "")
                    return false;

                JObject root = JObject.Parse(userdataJson);

                if (!root.HasValues || root["name"] == null)
                    return false;

                _username = root["name"].ToString();

            } catch (Exception)
            {
                return false;
            } finally
            {
                wc.Dispose();
                lw.Close();
            }

            return true;

        }

    }
}