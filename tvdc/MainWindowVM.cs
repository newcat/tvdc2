﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace tvdc
{
    class MainWindowVM : INotifyPropertyChanged, IDisposable
    {

        //Support for INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public int viewerCount
        {
            get { return viewerList.Count; }
        }

        private int _followerCount = 0;
        public int followerCount
        {
            get { return _followerCount; }
            set
            {
                _followerCount = value;
                NotifyPropertyChanged();
            }
        }

        private int _chatSize = 12;
        public int chatSize
        {
            get { return _chatSize; }
            set
            {
                if (value >= 9)
                {
                    _chatSize = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _loadingProgress = 0;
        public int loadingProgress
        {
            get { return _loadingProgress; }
            set
            {
                _loadingProgress = value;
                NotifyPropertyChanged();
            }
        }

        private string _loadingStatus = "";
        public string loadingStatus
        {
            get { return _loadingStatus; }
            set
            {
                _loadingStatus = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<User> viewerList = new ObservableCollection<User>();
        public ObservableCollection<ChatEntry> chatEntryList = new ObservableCollection<ChatEntry>();

        public bool enableSorting { get; set; }

        public static object viewerListLock = new object();
        private object chatListLock = new object();

        public MainWindowVM()
        {
            viewerList.CollectionChanged += ViewerList_CollectionChanged;
            enableSorting = false;
        }

        private void ViewerList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("viewerCount"));

            if (e.NewItems != null)
            {
                foreach (User u in e.NewItems)
                {
                    if (!u.subscribedToBadgeChangedEvent)
                    {
                        u.BadgeChanged += U_BadgeChanged;
                        u.subscribedToBadgeChangedEvent = true;
                    }
                }
            }

        }

        private void U_BadgeChanged(object sender, EventArgs e)
        {
            lock (viewerListLock)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(delegate ()
                {
                    if (enableSorting)
                        viewerList.Sort();
                });
            }
        }

        public void chatEntryList_Add(ChatEntry ce)
        {
            lock (chatListLock)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(delegate ()
                {
                    chatEntryList.Add(ce);
                });
            }
        }

        public void chatEntryList_Remove(ChatEntry ce)
        {
            lock (chatListLock)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(delegate ()
                {
                    chatEntryList.Remove(ce);
                });
            }
        }

        public void chatEntryList_Clear()
        {
            lock (chatListLock)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(delegate ()
                {
                    chatEntryList.Clear();
                });
            }
        }

        public void chatEntryList_GetEntryReference(ChatEntry target, out ChatEntry reference)
        {
            lock (chatListLock)
            {
                reference = chatEntryList[chatEntryList.IndexOf(target)];
            }
        }

        public List<ChatEntry> chatEntryList_Get()
        {
            List<ChatEntry> cl = new List<ChatEntry>();

            lock (chatListLock)
            {
                foreach (ChatEntry ce in chatEntryList)
                {
                    cl.Add(ce);
                }
            }

            return cl;
        }

        public void viewerList_Add(User u)
        {
            lock (viewerListLock)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(delegate ()
                {
                    if (enableSorting)
                    {
                        viewerList.AddSorted(u);
                    } else
                    {
                        viewerList.Add(u);
                    }
                });
            }
        }

        public void viewerList_Remove(User u)
        {
            lock (viewerListLock)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(delegate ()
                {
                    viewerList.Remove(u);
                });
            }
        }

        public void viewerList_Clear()
        {
            lock (viewerListLock)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(delegate ()
                {
                    viewerList.Clear();
                });
            }
        }

        public bool viewerList_ContainsName(string name)
        {
            lock (viewerListLock)
            {
                foreach (User u in viewerList)
                {
                    if (u.name == name || u.displayName == name)
                        return true;
                }
                return false;
            }
        }

        public bool viewerList_TryRemoveName(string name)
        {
            lock (viewerListLock)
            {
                foreach (User u in viewerList)
                {
                    if (u.name == name || u.displayName == name)
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke(delegate ()
                        {
                            viewerList.Remove(u);
                        });
                        return true;
                    }
                }
                return false;
            }
        }

        public User viewerList_GetUserByName(string name)
        {
            foreach (User u in viewerList)
            {
                if (u.name == name || u.displayName == name)
                {
                    return u;
                }
            }
            return null;
        }

        public void viewerList_GetUserInstanceByName(string name, out User user)
        {
            foreach (User u in viewerList)
            {
                if (u.name == name || u.displayName == name)
                {
                    user = u;
                    return;
                }
            }

            user = null;

        }

        public void viewerList_Sort()
        {
            lock (viewerListLock)
            {

                System.Windows.Application.Current.Dispatcher.Invoke(delegate ()
                {
                    viewerList.Sort();
                });
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    
                }

                viewerList = null;
                chatEntryList = null;

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

    }
}
