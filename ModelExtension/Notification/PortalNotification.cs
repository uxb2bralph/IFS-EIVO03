﻿using Irony.Parsing;
using Model.DataEntity;
using ModelExtension.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace ModelExtension.Notification
{
    public static class PortalNotification
    {

        public static Action<UserProfile> NotifyToResetPassword { get; set; }
        public static Action<UserProfile> NotifyToActivate { get; set; }

        public static string PushToLineNotify(this String message)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {AppSettings.Default.LineToken}");
                    var content = new FormUrlEncodedContent(new[]
                        {
                        new KeyValuePair<string, string>("message", message)
                    }
                    );

                    Task<HttpResponseMessage> task = client.PostAsync(AppSettings.Default.LineNotify, content);
                    task.Wait();
                    HttpResponseMessage response = task.Result;
                    response.EnsureSuccessStatusCode();
                    Task<String> data = response.Content.ReadAsStringAsync();
                    data.Wait();

                    var responseBody = data.Result;
                    return responseBody;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return null;
        }
    }
}
