using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AllInOneLauncher.Elements
{
    /// <summary>
    /// Interaction logic for EntryPageChangelogItem.xaml
    /// </summary>
    public partial class EntryPageChangelogItem : UserControl
    {
        const int SECOND = 1;
        const int MINUTE = 60 * SECOND;
        const int HOUR = 60 * MINUTE;
        const int DAY = 24 * HOUR;
        const int MONTH = 30 * DAY;

        public EntryPageChangelogItem()
        {
            InitializeComponent();
        }

        public string Version
        {
            get => version.Text;
            set => version.Text = value;
        }

        private long _creationTime = 0;
        public long CreationTime
        {
            get => _creationTime;
            set
            {
                _creationTime = value;

                TimeSpan ts = DateTime.Now - DateTimeOffset.FromUnixTimeMilliseconds(value).DateTime;
                double seconds = ts.TotalSeconds;

                if (seconds < 1 * MINUTE)
                    date.Text = ts.Seconds == 1 ? "one second ago" : ts.Seconds + " seconds ago";
                else if (seconds < 60 * MINUTE)
                    date.Text = ts.Minutes + " minutes ago";
                else if (seconds < 120 * MINUTE)
                    date.Text = "an hour ago";
                else if (seconds < 24 * HOUR)
                    date.Text = ts.Hours + " hours ago";
                else if (seconds < 48 * HOUR)
                    date.Text = "yesterday";
                else if (seconds < 30 * DAY)
                    date.Text = ts.Days + " days ago";
                else if (seconds < 12 * MONTH)
                {
                    int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                    date.Text = months <= 1 ? "one month ago" : months + " months ago";
                }
                else
                {
                    int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                    date.Text = years <= 1 ? "one year ago" : years + " years ago";
                }
            }
        }

        public string Text
        {
            get => content.Text;
            set => content.Text = value;
        }
    }
}
