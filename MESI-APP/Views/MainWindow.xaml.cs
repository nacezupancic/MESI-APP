using MESI_APP.ViewModels;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MESI_APP.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isDragging = false;
        private Point mouseOffset;
        private UIElement draggedElement;


        private MainViewModel _mainViewModel;
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            _mainViewModel = viewModel;
            this.DataContext = _mainViewModel;
        }
        private void Element_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && sender is UIElement element)
            {
                isDragging = true;
                draggedElement = element;

                mouseOffset = e.GetPosition(draggedElement);
                draggedElement.CaptureMouse(); 
            }
        }

        private void Element_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging && draggedElement != null)
            {
                // Calculate the new position for the dragged element
                Point mousePosition = e.GetPosition(MesiCanvas);

                // Get canvas weight/height, used to limit draggable element
                double canvasWidth = MesiCanvas.ActualWidth;
                double canvasHeight = MesiCanvas.ActualHeight;

                double elementWidth = draggedElement.RenderSize.Width;
                double elementHeight = draggedElement.RenderSize.Height;

                double mouseX = mousePosition.X - mouseOffset.X;
                double mouseY = mousePosition.Y - mouseOffset.Y;

                // Prevent dragging object out of canvas
                if (mouseX < 0) mouseX = 0;
                if (mouseY < 0) mouseY = 0;
                if (mouseX + elementWidth > canvasWidth) mouseX = canvasWidth - elementWidth;
                if (mouseY + elementHeight > canvasHeight) mouseY = canvasHeight - elementHeight;

                Canvas.SetLeft(draggedElement, mouseX);
                Canvas.SetTop(draggedElement, mouseY);
            }
        }

        private void Element_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;

            if (draggedElement != null)
            {

                draggedElement.ReleaseMouseCapture(); 
                draggedElement = null;
            }
        }
    }
}