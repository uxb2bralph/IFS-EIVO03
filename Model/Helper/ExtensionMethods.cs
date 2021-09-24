﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Model.Models.ViewModel;

namespace Model.Helper
{
    public static class ExtensionMethods
    {
        public static byte[] DecryptKey(this QueryViewModel viewModel)
        {
            return AppResource.Instance.DecryptSalted(Convert.FromBase64String(viewModel.KeyID));
        }

        public static byte[] DecryptKey(this QueryViewModel viewModel,out long ticks)
        {
            return AppResource.Instance.DecryptSalted(Convert.FromBase64String(viewModel.KeyID),out ticks);
        }

        public static void EncryptKey(this QueryViewModel viewModel,byte[] data)
        {
            viewModel.KeyID = Convert.ToBase64String(AppResource.Instance.EncryptSalted(data));
        }

        public static String EncryptKey(this byte[] data)
        {
            return Convert.ToBase64String(AppResource.Instance.EncryptSalted(data));
        }

        public static String EncryptData(this String data)
        {
            return Encoding.Unicode.GetBytes(data).EncryptKey();
        }

        public static String DecryptData(this String data)
        {
            return Encoding.Unicode.GetString(AppResource.Instance.DecryptSalted(Convert.FromBase64String(data)));
        }

        public static String DecryptData(this String data,out long ticks)
        {
            return Encoding.Unicode.GetString(AppResource.Instance.DecryptSalted(Convert.FromBase64String(data), out ticks));
        }

        public static int DecryptKeyValue(this QueryViewModel viewModel)
        {
            return BitConverter.ToInt32(viewModel.DecryptKey(), 0);
        }

        public static int DecryptKeyValue(this QueryViewModel viewModel,out long ticks)
        {
            return BitConverter.ToInt32(viewModel.DecryptKey(out ticks), 0);
        }

        public static int DecryptKeyValue(this String keyValue)
        {
            return BitConverter.ToInt32(AppResource.Instance.DecryptSalted(Convert.FromBase64String(keyValue)), 0);
        }

        public static String EncryptKey(this int keyID)
        {
            return BitConverter.GetBytes(keyID).EncryptKey();
        }

        public static int[] SortIndex(this QueryViewModel viewModel)
        {
            if (viewModel.Sort != null)
            {
                return viewModel.Sort.Where(s => s.HasValue).Select(s => s.Value).ToArray();
            }

            return null;
        }

    }
}