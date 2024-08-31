using AllInOneLauncher.Elements;
using System.Linq;
using System.Windows;

namespace AllInOneLauncher.Popups
{
    /// <summary>
    /// Interaction logic for MessagePopup.xaml
    /// </summary>
    public partial class MessagePopup : PopupBody
    {
        public MessagePopup(string title, string message)
        {
            InitializeComponent();
            this.title.Text = string.Join("", title.Split("{").Select(x => !x.Contains("}") ? x : ((Application.Current.FindResource(x.Split("}")[0]).ToString() ?? "") + x.Split("}")[1])));
            this.message.Text = string.Join("", message.Split("{").Select(x => !x.Contains("}") ? x : ((Application.Current.FindResource(x.Split("}")[0]).ToString() ?? "") + x.Split("}")[1])));
        }

        private void ButtonCancelClicked(object sender, RoutedEventArgs e) => Dismiss();
    }
}