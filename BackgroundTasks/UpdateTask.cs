﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.UI.StartScreen;

namespace BackgroundTasks
{
    public sealed class UpdateTask : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var deferral = taskInstance.GetDeferral();

            // Refresh jump list to update string resources
            try { await RefreshJumpList(); } catch { }

            // Delete previous version log files
            try { DeleteLogFiles(); } catch { }

            deferral.Complete();
        }

        private void DeleteLogFiles()
        {
            File.Delete(Path.Combine(ApplicationData.Current.LocalFolder.Path, "debug.log"));
            File.Delete(Path.Combine(ApplicationData.Current.LocalFolder.Path, "debug_fulltrust.log"));
        }

        private async Task RefreshJumpList()
        {
            if (JumpList.IsSupported())
            {
                var instance = await JumpList.LoadCurrentAsync();
                // Disable automatic jumplist. It doesn't work with Files UWP.
                instance.SystemGroupKind = JumpListSystemGroupKind.None;

                var jumpListItems = instance.Items.ToList();

                // Clear all items to avoid localization issues
                instance.Items.Clear();

                foreach (var temp in jumpListItems)
                {
                    var jumplistItem = JumpListItem.CreateWithArguments(temp.Arguments, temp.DisplayName);
                    jumplistItem.Description = jumplistItem.Arguments;
                    jumplistItem.GroupName = "ms-resource:///Resources/JumpListRecentGroupHeader";
                    jumplistItem.Logo = new Uri("ms-appx:///Assets/FolderIcon.png");
                    instance.Items.Add(jumplistItem);
                }

                await instance.SaveAsync();
            }
        }
    }
}
