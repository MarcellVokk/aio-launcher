using System;
using System.Linq;
using System.Windows;
using AllInOneLauncher.Core.Utils;
using System.Windows.Media.Imaging;
using AllInOneLauncher.Elements.Generic;
using BfmeFoundationProject.WorkshopKit.Data;
using BfmeFoundationProject.WorkshopKit.Logic;
using BfmeFoundationProject.RegistryKit;
using System.Globalization;
using AllInOneLauncher.Elements;

namespace AllInOneLauncher.Popups;

public partial class WorkshopEntryDetailPopup : PopupBody
{
    public override ColorStyle ColorStyle => ColorStyle.Acrylic;

    public WorkshopEntryDetailPopup()
    {
        InitializeComponent();
    }

    BfmeWorkshopEntryPreview _workshopEntry;
    public BfmeWorkshopEntryPreview WorkshopEntry
    {
        get => _workshopEntry;
        set
        {
            _workshopEntry = value;
            try { icon.Source = new BitmapImage(new Uri(value.ArtworkUrl)); } catch { }
            title.Text = value.Name;
            author.Text = value.Author;
            description.Text = value.Description;
            downloads.Text = NumberUtils.IntToPrettyShortNum(value.Metadata.Downloads);
            game.Text = value.GameName();
            icon_bfme1.Visibility = value.Game == 0 ? Visibility.Visible : Visibility.Collapsed;
            icon_bfme2.Visibility = value.Game == 1 ? Visibility.Visible : Visibility.Collapsed;
            icon_rotwk.Visibility = value.Game == 2 ? Visibility.Visible : Visibility.Collapsed;

            if (value.Type == 0)
                type.Text = Application.Current.FindResource("LibraryTilePatchType").ToString()!;
            else if (value.Type == 1)
                type.Text = Application.Current.FindResource("LibraryTileModType").ToString()!;
            else if (value.Type == 2)
                type.Text = Application.Current.FindResource("LibraryTileEnhancementType").ToString()!;
            else if (value.Type == 3)
                type.Text = Application.Current.FindResource("LibraryTileMapPackType").ToString()!;

            icon_patch.Visibility = value.Type == 0 ? Visibility.Visible : Visibility.Collapsed;
            icon_mod.Visibility = value.Type == 1 ? Visibility.Visible : Visibility.Collapsed;
            icon_enhancement.Visibility = value.Type == 2 ? Visibility.Visible : Visibility.Collapsed;
            icon_mappack.Visibility = value.Type == 3 ? Visibility.Visible : Visibility.Collapsed;

            value.Language = value.Language ?? "EN";
            string curentGameLanguage = BfmeRegistryManager.GameLanguageToLanguageCode(BfmeRegistryManager.GetKeyValue(value.Game, BfmeFoundationProject.RegistryKit.Data.BfmeRegistryKey.Language));
            language_code.Text = value.Language.Contains(curentGameLanguage) ? curentGameLanguage : value.Language.Split(' ')[0];
            language_full.Text = value.Language.Split(' ').Length > 1 ? $"+ {value.Language.Split(' ').Length - 1} More" : BfmeRegistryManager.GameLanguageCodeToLanguage(language_code.Text);

            if (value.Size >= 1_073_741_824)
            {
                size_unit.Text = "GB";
                size_number.Text = Math.Round(value.Size / 1_073_741_824d, 2).ToString(CultureInfo.InvariantCulture).Replace(".", ",");
            }
            else if (value.Size >= 1_048_576)
            {
                size_unit.Text = "MB";
                size_number.Text = Math.Round(value.Size / 1_048_576d, 2).ToString(CultureInfo.InvariantCulture).Replace(".", ",");
            }
            else
            {
                size_unit.Text = "KB";
                size_number.Text = Math.Round(value.Size / 1024d, 2).ToString(CultureInfo.InvariantCulture).Replace(".", ",");
            }

            changelog.Children.Add(new EntryPageChangelogItem() { Version = value.Version, CreationTime = value.CreationTime, Text = value.Changelog });
        }
    }

    private void OnDismiss(object sender, RoutedEventArgs e) => Dismiss();
}