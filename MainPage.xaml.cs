
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace LightsOutUWP1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private LightsOutGame game;
        public MainPage()
        {

            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            game = new LightsOutGame();
            CreateGrid();
        }
        private void CreateGrid()
        {
            // Remove all previously-existing rectangles
            boardCanvas.Children.Clear();

            int rectSize = (int)boardCanvas.Width / game.GridSize;

            // Turn entire grid on and create rectangles to represent it
            for (int r = 0; r < game.GridSize; r++)
            {
                for (int c = 0; c < game.GridSize; c++)
                {
                    Rectangle rect = new Rectangle();
                    rect.Fill = new SolidColorBrush(Windows.UI.Colors.White);
                    rect.Width = rectSize + 1;
                    rect.Height = rect.Width + 1;
                    rect.Stroke = new SolidColorBrush(Windows.UI.Colors.Black);

                    // Store each row and col as a Point
                    rect.Tag = new Point(r, c);
                    rect.PointerPressed += Rect_PointerPressedAsync;

                    int x = c * rectSize;
                    int y = r * rectSize;

                    Canvas.SetTop(rect, y);
                    Canvas.SetLeft(rect, x);

                    // Add the new rectangle to the canvas' children
                    boardCanvas.Children.Add(rect);
                }
            }
        }
        private async void Rect_PointerPressedAsync(object sender, PointerRoutedEventArgs e)
        {
            // Get row and column from Rectangle's Tag
            Rectangle rect = sender as Rectangle;
            var rowCol = (Point)rect.Tag;
            int row = (int)rowCol.X;
            int col = (int)rowCol.Y;
            game.Move(row, col);
            DrawGrid();
            // Event was handled
            e.Handled = true;

            if (game.IsGameOver())
            {
                MessageDialog msgDialog = new MessageDialog("Congratulations! You've won!", "LightsOut!");

                msgDialog.Commands.Add(new UICommand("OK"));

                await msgDialog.ShowAsync();
            }
        }

        private void DrawGrid()
        {
            int index = 0;

            // Set the colors of the rectangles
            for (int r = 0; r < game.GridSize; r++)
            {
                for (int c = 0; c < game.GridSize; c++)
                {
                    Rectangle rect = boardCanvas.Children[index] as Rectangle;
                    index++;
                    if (game.GetGridValue(r, c))
                    {
                        // On
                        rect.Fill = new SolidColorBrush(Windows.UI.Colors.White);
                        rect.Stroke = new SolidColorBrush(Windows.UI.Colors.Black);
                    }
                    else
                    {
                        // Off
                        rect.Fill = new SolidColorBrush(Windows.UI.Colors.Black);
                        rect.Stroke = new SolidColorBrush(Windows.UI.Colors.White);
                    }
                }
            }
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(aboutPage));
        }

        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            game.NewGame();
            DrawGrid();
        }
    }
}