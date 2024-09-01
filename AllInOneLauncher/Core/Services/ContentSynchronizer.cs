using System;
using System.Threading.Tasks;
using AllInOneLauncher.Data;
using AllInOneLauncher.Elements.Generic;
using AllInOneLauncher.Popups;
using BfmeFoundationProject.RegistryKit;
using BfmeFoundationProject.WorkshopKit.Logic;

namespace AllInOneLauncher.Core.Services;

public class ContentSynchronizer
{
    public static async Task SyncIfNeeded(bool needsResync)
    {
        if (!needsResync)
            return;

        foreach (BfmeGame game in Enum.GetValues(typeof(BfmeGame)))
        {
            if (!BfmeRegistryManager.IsInstalled(game) || (game == BfmeGame.ROTWK && !BfmeRegistryManager.IsInstalled(BfmeGame.BFME2)))
                continue;

            var activeEntry = await BfmeWorkshopStateManager.GetActivePatch((int)game);
            if (activeEntry != null)
            {
                try
                {
                    await BfmeWorkshopSyncManager.Sync(activeEntry.Value);
                }
                catch (Exception ex)
                {
                    PopupVisualizer.ShowPopup(new ErrorPopup(ex));
                }
            }
        }
    }
}