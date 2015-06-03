using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Tpv.Ui.Model;

namespace Tpv.Ui.Service
{
    public class FactoryButton
    {
        private const string URI_RESOURCE = "pack://application:,,,/Images/";



        public static Button CreateModifierGroupButton(ItemGroupModifier groupModifier, int i, Action<object, RoutedEventArgs> onClickGroupModifier)
        {
            var button = new Button
            {
                Name = SharedConstants.BUTTON_GROUP_MODIFIER + groupModifier.Id,
                BorderBrush = null,
                Foreground = Brushes.White,
                Height = 50,
                Width = 170,
                Background = new ImageBrush(new BitmapImage(new Uri(URI_RESOURCE + "ButtonGrpMod.jpg"))),
                Content = new TextBlock
                {
                    FontWeight = FontWeights.Bold,
                    FontFamily = new FontFamily("Franklin Gothic Medium"),
                    FontSize = 18,
                    Text = groupModifier.Name
                },
                IsDefault = i == 0,
                Tag = groupModifier,                
            };

            if (onClickGroupModifier != null) button.Click += new RoutedEventHandler(onClickGroupModifier);

            return button;
        }

        public static Button CreateModifierButton(ItemModifier modifier)
        {
            var grid = new Grid();

            grid.Children.Add(new TextBlock
            {
                TextWrapping = TextWrapping.Wrap,
                TextAlignment = TextAlignment.Center,
                FontWeight = FontWeights.Bold,
                FontFamily = new FontFamily("Franklin Gothic Medium"),
                FontSize = 18,
                Foreground = new SolidColorBrush(Color.FromRgb(12, 58, 214)),
                Margin = new Thickness(4, 4, 4, -4),
                Text = modifier.Name
            });

            grid.Children.Add(new TextBlock
            {
                TextWrapping = TextWrapping.Wrap,
                TextAlignment = TextAlignment.Center,
                Foreground = new SolidColorBrush(Color.FromRgb(12, 58, 214)),
                Margin = new Thickness(127, 32, 4, -2),
                Text = modifier.PriceTxt
            });


            var button = new Button
            {
                Name = SharedConstants.BUTTON_MODIFIER + modifier.Id,
                BorderBrush = null,
                Foreground = new SolidColorBrush(Color.FromRgb(12, 58, 214)),
                Height = 68,
                Width = 170,
                Background = new ImageBrush(new BitmapImage(new Uri(URI_RESOURCE + "Button.jpg"))),
                Content = grid,
                Tag = modifier
            };

            return button;
        }
    }
}

