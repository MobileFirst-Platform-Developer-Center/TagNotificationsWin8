/**
* Copyright 2015 IBM Corp.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/
 
﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using IBM.Worklight;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using Windows.UI.Xaml.Media.Imaging;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Popups;
using Microsoft.VisualBasic;
using Windows.UI;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TagNotificationsWin8
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        WLClient wlClient = null;
        internal static MainPage _this;

        public MainPage()
        {
            this.InitializeComponent();
            _this = this;

            try
            {
                wlClient = WLClient.getInstance();
                WLPush wlPush = wlClient.getPush();
                OnReadyToSubscribeListener onReadyToSubscribeListener = new OnReadyToSubscribeListener();
                wlPush.onReadyToSubscribeListener = onReadyToSubscribeListener;

                MyNotificationListener myNotificationListener = new MyNotificationListener();
                wlPush.notificationListener = myNotificationListener;

                MyResponseListener myRespListener = new MyResponseListener(null);
                wlClient.connect(myRespListener);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
            }

        }

        private void isSubscribed_Click(object sender, RoutedEventArgs e)
        {
            Boolean isTag1Subscribed = wlClient.getPush().isTagSubscribed("sample-tag1");
            Boolean isTag2Subscribed = wlClient.getPush().isTagSubscribed("sample-tag2");

            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                async () =>
                {
                    MainPage._this.Console.Text = "\n\nsample-tag1 ::" + isTag1Subscribed + "\n" + "sample-tag2 ::" + isTag2Subscribed;
                });

        }

        private void subscribeSampletag1_Click(object sender, RoutedEventArgs e)
        {
            MySubscribeListener mySubscribeListener = new MySubscribeListener();
            wlClient.getPush().subscribeTag("sample-tag1", null, mySubscribeListener);
        }

        private void subscribeSampletag2_Click(object sender, RoutedEventArgs e)
        {
            MySubscribeListener mySubscribeListener = new MySubscribeListener();
            wlClient.getPush().subscribeTag("sample-tag2", null, mySubscribeListener);
        }

        private void unsubscribeSampletag1_Click(object sender, RoutedEventArgs e)
        {
            MyUnSubscribeListener myUnsubscribeListener = new MyUnSubscribeListener();
            wlClient.getPush().unsubscribeTag("sample-tag1", myUnsubscribeListener);
        }

        private void unsubscribeSampletag2_Click(object sender, RoutedEventArgs e)
        {
            MyUnSubscribeListener myUnsubscribeListener = new MyUnSubscribeListener();
            wlClient.getPush().unsubscribeTag("sample-tag2", myUnsubscribeListener);
        }

        private void ClearConsole(object sender, DoubleTappedRoutedEventArgs e)
        {
            Console.Text = "";
        }

        class MyResponseListener : WLResponseListener
        {

            MainPage page;
            public MyResponseListener(MainPage mainPage)
            {
                page = mainPage;
            }

            public void onSuccess(WLResponse resp)
            {
                Debug.WriteLine("Successfully connected to server " + resp.getResponseText());

                CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                async () =>
                {
                    MainPage._this.Console.Text = "\n\nSuccessfully connected to server";
                });

            }

            public void onFailure(WLFailResponse resp)
            {
                Debug.WriteLine("Failure connecting to server" + resp.getErrorMsg());
                CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                async () =>
                {
                    MainPage._this.Console.Text = resp.getErrorMsg();
                });
            }
        }

        class MyNotificationListener : WLNotificationListener
        {

            public void onMessage(string props, string payload)
            {
                Debug.WriteLine("Props: " + props);
                Debug.WriteLine("Payload: " + payload);
                CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                async () =>
                {
                    MainPage._this.Console.Text = "Received Message\nprops:" + props + "\npayload:" + payload;
                });
            }
        }

        class OnReadyToSubscribeListener : WLOnReadyToSubscribeListener
        {

            public void onReadyToSubscribe()
            {
                Debug.WriteLine("On ready to subscribe");

                CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                async () =>
                {

                    MainPage._this.IsSubscribed.IsEnabled = true;
                    MainPage._this.SubscribeTag1.IsEnabled = true;
                    MainPage._this.SubscribeTag2.IsEnabled = true;
                    MainPage._this.UnsubscribeTag1.IsEnabled = true;
                    MainPage._this.UnsubscribeTag2.IsEnabled = true;

                    MainPage._this.Console.Text = "On ready to subscribe";

                });
            }
        }

        class MySubscribeListener : WLResponseListener
        {
            public void onSuccess(WLResponse resp)
            {
                Debug.WriteLine("Push subscription success " + resp.getResponseText());
                CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                async () =>
                {
                    MainPage._this.Console.Text = "\n\nPush subscription success " + resp.getResponseText();
                });
            }

            public void onFailure(WLFailResponse resp)
            {
                Debug.WriteLine("Push subscription failure" + resp.getResponseText());
                CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                async () =>
                {
                    MainPage._this.Console.Text = "\n\nPush subscription failure " + resp.getResponseText();
                });
            }
        }

        class MyUnSubscribeListener : WLResponseListener
        {
            public void onSuccess(WLResponse resp)
            {
                Debug.WriteLine("Push unsubscribe success " + resp.getResponseText());
                CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                async () =>
                {
                    MainPage._this.Console.Text = "\n\nPush unsubscription success";
                });
            }

            public void onFailure(WLFailResponse resp)
            {
                Debug.WriteLine("Push unsubscribe failure" + resp.getResponseText());
                CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                async () =>
                {
                    MainPage._this.Console.Text = "\n\nFailed to un-subscribe from tag " + resp.getResponseText();
                });
            }
        }

        private void ShowConsole(object sender, TappedRoutedEventArgs e)
        {
            MainPage._this.ConsolePanel.Visibility = Visibility.Visible;
            MainPage._this.InfoPanel.Visibility = Visibility.Collapsed;
            MainPage._this.ConsoleTab.Foreground = new SolidColorBrush(Colors.DodgerBlue);
            MainPage._this.InfoTab.Foreground = new SolidColorBrush(Colors.Gray);
        }

        private void ShowInfo(object sender, TappedRoutedEventArgs e)
        {
            MainPage._this.ConsolePanel.Visibility = Visibility.Collapsed;
            MainPage._this.InfoPanel.Visibility = Visibility.Visible;
            MainPage._this.InfoTab.Foreground = new SolidColorBrush(Colors.DodgerBlue);
            MainPage._this.ConsoleTab.Foreground = new SolidColorBrush(Colors.Gray);
        }

    }
}
