using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace ExPlayer.Models
{
    public class CursorBehavior : Behavior<ListView>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewKeyDown += ListView_PreviewKeyDown;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewKeyDown -= ListView_PreviewKeyDown;
        }

        private void ListView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is not ListView lv)
            {
                return;
            }

            if (lv.Items.Count <= 0)
            {
                return;
            }

            if (e.Key == Key.J && lv.SelectedIndex < lv.Items.Count)
            {
                lv.SelectedIndex++;
            }

            if (e.Key == Key.K && lv.SelectedIndex > 0)
            {
                lv.SelectedIndex--;
            }

            lv.ScrollIntoView(lv.SelectedItem);
        }
    }
}