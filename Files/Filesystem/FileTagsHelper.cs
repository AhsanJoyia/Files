﻿using Common;
using Files.Helpers;
using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Files.Filesystem
{
    public class FileTagsHelper
    {
        public static string FileTagsDbPath => System.IO.Path.Combine(ApplicationData.Current.LocalFolder.Path, "filetags.db");

        private static FileTagsDb _DbInstance;
        public static FileTagsDb DbInstance
        {
            get
            {
                if (_DbInstance == null)
                {
                    _DbInstance = new FileTagsDb(FileTagsDbPath, true);
                }
                return _DbInstance;
            }
        }

        public static string ReadFileTag(string filePath)
        {
            IntPtr hStream = NativeFileOperationsHelper.CreateFileFromApp($"{filePath}:files",
                NativeFileOperationsHelper.GENERIC_READ, 0, IntPtr.Zero, NativeFileOperationsHelper.OPEN_EXISTING, (uint)NativeFileOperationsHelper.File_Attributes.BackupSemantics, IntPtr.Zero);
            if (hStream.ToInt64() == -1) return null;
            byte[] buff = new byte[4096];
            int dwBytesRead;
            string str = null;
            unsafe
            {
                fixed (byte* pBuff = buff)
                {
                    NativeFileOperationsHelper.ReadFile(hStream, pBuff, 4096 - 1, &dwBytesRead, IntPtr.Zero);
                    str = Encoding.UTF8.GetString(pBuff, dwBytesRead);
                }
            }
            NativeFileOperationsHelper.CloseHandle(hStream);
            return str;
        }

        public static void WriteFileTag(string filePath, string tag)
        {
            if (tag == null)
            {
                NativeFileOperationsHelper.DeleteFileFromApp($"{filePath}:files");
            }
            else
            {
                IntPtr hStream = NativeFileOperationsHelper.CreateFileFromApp($"{filePath}:files",
                    NativeFileOperationsHelper.GENERIC_WRITE, 0, IntPtr.Zero, NativeFileOperationsHelper.CREATE_ALWAYS, (uint)NativeFileOperationsHelper.File_Attributes.BackupSemantics, IntPtr.Zero);
                if (hStream.ToInt64() == -1) return;
                byte[] buff = Encoding.UTF8.GetBytes(tag);
                int dwBytesWritten;
                unsafe
                {
                    fixed (byte* pBuff = buff)
                    {
                        NativeFileOperationsHelper.WriteFile(hStream, pBuff, buff.Length, &dwBytesWritten, IntPtr.Zero);
                    }
                }
                NativeFileOperationsHelper.CloseHandle(hStream);
            }
        }

        private FileTagsHelper()
        {

        }
    }

    public class FileTag
    {
        public string Tag { get; set; }
        public SolidColorBrush Color { get; set; }

        public FileTag(string tag = null, SolidColorBrush color = null)
        {
            Tag = tag;
            Color = color ?? new SolidColorBrush(Colors.Transparent);
        }

        public FileTag(string tag = null, Color? color = null)
        {
            Tag = tag;
            Color = new SolidColorBrush(color ?? Colors.Transparent);
        }

        public FileTag(string tag = null, string color = null)
        {
            Tag = tag;
            Color = new SolidColorBrush(color?.ToColor() ?? Colors.Transparent);
        }
    }
}