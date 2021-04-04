using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Input;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
//using System.Windows.Controls;
using System.IO;
using System.Text.RegularExpressions;

namespace tekenprogramma
{
    public sealed partial class MainPage : Page
    {
        string type = "Rectangle";
        bool selecting = false;
        bool grouping = false;
        Rectangle backuprectangle;
        Ellipse backupellipse;

        public Invoker invoker = new Invoker();
        public List<Shape> selectedShapesList = new List<Shape>();
        public FrameworkElement selectedElement;
        public List<FrameworkElement> selectElementsList = new List<FrameworkElement>();

        public MainPage()
        {
            InitializeComponent();
        }

        private void Drawing_pressed(object sender, PointerRoutedEventArgs e)
        {
            //if grouping on
            if (grouping == true)
            {
                grouped(sender, e);
            }
            //common shapes
            else
            {
                non_grouped(sender, e);
            }

        }

        //grouped
        private void non_grouped(object sender, PointerRoutedEventArgs e)
        {
            //selecting false
            if (selecting == false)
            {
                //select shapes
                FrameworkElement backupprep = e.OriginalSource as FrameworkElement;
                if (backupprep.Name == "Rectangle")
                {
                    Rectangle tmp = backupprep as Rectangle;
                    backuprectangle = tmp;
                    double top = (double)tmp.GetValue(Canvas.TopProperty);
                    double left = (double)tmp.GetValue(Canvas.LeftProperty);
                    double width = tmp.Width;
                    double height = tmp.Height;
                    Shape shape = new Shape(left, top, width, height);
                    ICommand select = new Select(shape, e);
                    this.invoker.Execute(select);
                    //selecting = true;
                    selectedShapesList.Add(shape);
                    selectedElement = tmp;
                    selectElementsList.Add(selectedElement);
                }
                else if (backupprep.Name == "Ellipse")
                {
                    Ellipse tmp = backupprep as Ellipse;
                    backupellipse = tmp;
                    double top = (double)tmp.GetValue(Canvas.TopProperty);
                    double left = (double)tmp.GetValue(Canvas.LeftProperty);
                    double width = tmp.Width;
                    double height = tmp.Height;
                    Shape shape = new Shape(left, top, width, height);
                    ICommand select = new Select(shape, e);
                    this.invoker.Execute(select);
                    //selecting = true;
                    selectedShapesList.Add(shape);
                    selectedElement = tmp;
                    selectElementsList.Add(selectedElement);
                }
                else
                {
                    //make shapes
                    if (type == "Rectangle")
                    {
                        MakeRectangle(sender, e);
                        selectElementsList.Clear();
                    }
                    else if (type == "Elipse")
                    {
                        MakeEllipse(sender, e);
                        selectElementsList.Clear();
                    }
                }
            }
            //selecting true
            else
            {

                //move
                if (type == "Move")
                {
                    movingShape(sender, e);
                    selectElementsList.Clear();
                }
                //resize
                else if (type == "Resize")
                {
                    resizingShape(sender, e);
                    selectElementsList.Clear();
                }



            }
        }

        //non grouped
        private void grouped(object sender, PointerRoutedEventArgs e)
        {
            //selecting
            if (selecting == false)
            {
                //select shapes
                FrameworkElement backupprep = e.OriginalSource as FrameworkElement;
                if (backupprep.Name == "Rectangle")
                {
                    Rectangle tmp = backupprep as Rectangle;
                    backuprectangle = tmp;
                    double top = (double)tmp.GetValue(Canvas.TopProperty);
                    double left = (double)tmp.GetValue(Canvas.LeftProperty);
                    double width = tmp.Width;
                    double height = tmp.Height;
                    selecting = true;
                    //selectedShapesList.Add(shape);
                    selectedElement = tmp;
                    selectElementsList.Add(selectedElement);
                    Group group = new Group(left, top, width, height, "rectangle", 0, 0, paintSurface, invoker, selectedElement);
                    ICommand place = new MakeGroup(group, paintSurface, invoker);
                    this.invoker.Execute(place);
                }
                else if (backupprep.Name == "Ellipse")
                {
                    Ellipse tmp = backupprep as Ellipse;
                    backupellipse = tmp;
                    double top = (double)tmp.GetValue(Canvas.TopProperty);
                    double left = (double)tmp.GetValue(Canvas.LeftProperty);
                    double width = tmp.Width;
                    double height = tmp.Height;

                    selecting = true;
                    //selectedShapesList.Add(shape);
                    selectedElement = tmp;
                    selectElementsList.Add(selectedElement);
                    Group group = new Group(left, top, width, height, "ellipse", 0, 0, paintSurface, invoker, selectedElement);
                    ICommand place = new MakeGroup(group, paintSurface, invoker);
                    this.invoker.Execute(place);
                }

            }
            else
            {
                //move
                if (type == "Move")
                {
                    movingGroup(sender, e);
                    selectElementsList.Clear();
                }
                //resize
                else if (type == "Resize")
                {
                    resizingGroup(sender, e);
                    selectElementsList.Clear();
                }

            }
        }

        //make rectangle shape
        private void MakeRectangle(object sender, PointerRoutedEventArgs e)
        {
            Shape shape = new Shape(e.GetCurrentPoint(paintSurface).Position.X, e.GetCurrentPoint(paintSurface).Position.Y, 50, 50);
            ICommand place = new MakeRectangles(shape, this.invoker, paintSurface);
            this.invoker.Execute(place);
        }

        //make ellipse shape
        private void MakeEllipse(object sender, PointerRoutedEventArgs e)
        {
            Shape shape = new Shape(e.GetCurrentPoint(paintSurface).Position.X, e.GetCurrentPoint(paintSurface).Position.Y, 50, 50);
            ICommand place = new MakeEllipses(shape, this.invoker, paintSurface);
            this.invoker.Execute(place);
        }

        //moving shape
        private void movingShape(object sender, PointerRoutedEventArgs e)
        {
            Shape shape = selectedShapesList.First();
            ICommand place = new Moving(shape, e, paintSurface, invoker, selectedElement);
            this.invoker.Execute(place);
            type = "deselecting";
            selecting = false;
            selectedShapesList.RemoveAt(0);
            selectedElement = null;
        }

        //resizing shape
        private void resizingShape(object sender, PointerRoutedEventArgs e)
        {
            Shape shape = selectedShapesList.First();
            ICommand place = new Resize(shape, e, paintSurface, invoker, selectedElement);
            this.invoker.Execute(place);
            type = "deselecting";
            selecting = false;
            selectedShapesList.RemoveAt(0);
            selectedElement = null;
        }

        //moving group
        private void movingGroup(object sender, PointerRoutedEventArgs e)
        {
            Shape shape = selectedShapesList.First();
            ICommand place = new Moving(shape, e, paintSurface, invoker, selectedElement);
            this.invoker.Execute(place);
            type = "deselecting";
            selecting = false;
            selectedShapesList.RemoveAt(0);
            selectedElement = null;
        }

        //resizing group
        private void resizingGroup(object sender, PointerRoutedEventArgs e)
        {
            Shape shape = selectedShapesList.First();
            ICommand place = new Resize(shape, e, paintSurface, invoker, selectedElement);
            this.invoker.Execute(place);
            type = "deselecting";
            selecting = false;
            selectedShapesList.RemoveAt(0);
            selectedElement = null;
        }

        //move click
        private void Move_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement button = e.OriginalSource as FrameworkElement;
            type = button.Name;
            grouping = false;
            selecting = true;
        }

        //resize click
        private void Resize_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement button = e.OriginalSource as FrameworkElement;
            type = button.Name;
            grouping = false;
            selecting = true;
        }

        //elipse click
        private void Elipse_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement button = e.OriginalSource as FrameworkElement;
            type = button.Name;
            grouping = false;
            selecting = false;
        }

        //rectangle click
        private void Rectangle_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement button = e.OriginalSource as FrameworkElement;
            type = button.Name;
            grouping = false;
            selecting = false;
        }

        //ornament click
        private void Ornament_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement button = e.OriginalSource as FrameworkElement;
            type = button.Name;
            grouping = false;
        }

        //group click
        private void Group_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement button = e.OriginalSource as FrameworkElement;
            type = button.Name;

            Group group = new Group(0, 0, 0, 0, "group", 0, 0, paintSurface, invoker, selectedElement);
            ICommand place = new MakeGroup(group, paintSurface, invoker);
            this.invoker.Execute(place);

            Canvas grid = new Canvas();
            //grid.Background = Opacity(1);
            SolidColorBrush groupbrush = new SolidColorBrush(); //brush
            groupbrush.Color = Windows.UI.Colors.Yellow; //standard brush color is blue
            groupbrush.Opacity = 0.5; //half opacity
            grid.Background = groupbrush;
            grid.Height = paintSurface.Height;
            grid.Width = paintSurface.Width;
            Canvas.SetTop(grid, 0);
            Canvas.SetLeft(grid, 0);


            foreach (FrameworkElement elm in selectElementsList)
            {
                double top = (double)elm.GetValue(Canvas.TopProperty);
                double left = (double)elm.GetValue(Canvas.LeftProperty);
                double width = elm.Width;
                double height = elm.Height;
                Group selectedgroup = new Group(left, top, width, height, elm.Name, 0, 0, paintSurface, invoker, elm);
                //ICommand addgroup = new MakeGroup(selectedgroup, paintSurface, invoker);
                //this.invoker.Execute(addgroup);
                group.add(selectedgroup);
                //grid.Children.Add(elm);

                if (elm.Name == "Ellipse")
                {
                    Ellipse newEllipse = new Ellipse(); //instance of new ellipse shape
                    newEllipse.Width = width;
                    newEllipse.Height = height;
                    SolidColorBrush elmbrush = new SolidColorBrush();//brush
                    elmbrush.Color = Windows.UI.Colors.Yellow;//standard brush color is blue
                    newEllipse.Fill = elmbrush;//fill color
                    newEllipse.Name = "Ellipse";//attach name
                    Canvas.SetLeft(newEllipse, left);//set left position
                    Canvas.SetTop(newEllipse, top);//set top position
                    grid.Children.Add(newEllipse);
                }
                else if (elm.Name == "Rectangle")
                {
                    Rectangle newRectangle = new Rectangle(); //instance of new rectangle shape
                    newRectangle.Width = width; //set width
                    newRectangle.Height = height; //set height     
                    SolidColorBrush elmbrush = new SolidColorBrush(); //brush
                    elmbrush.Color = Windows.UI.Colors.Yellow; //standard brush color is blue
                    newRectangle.Fill = elmbrush; //fill color
                    newRectangle.Name = "Rectangle"; //attach name
                    Canvas.SetLeft(newRectangle, left); //set left position
                    Canvas.SetTop(newRectangle, top); //set top position 
                    grid.Children.Add(newRectangle);
                }


            }
            this.paintSurface.Children.Add(grid);
            grouping = true;
        }

        //undo click
        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement button = e.OriginalSource as FrameworkElement;
            type = button.Name;
            invoker.Undo();
            grouping = false;
        }

        //redo click
        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement button = e.OriginalSource as FrameworkElement;
            type = button.Name;
            invoker.Redo();
            grouping = false;
        }

        //save click
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement button = e.OriginalSource as FrameworkElement;
            type = button.Name;
            Shape command = new Shape(0, 0, 0, 0);
            ICommand place = new Saved(command, paintSurface);
            invoker.Execute(place);
            grouping = false;
        }

        //load click
        private void Load_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement button = e.OriginalSource as FrameworkElement;
            type = button.Name;
            Shape command = new Shape(0, 0, 0, 0);
            ICommand place = new Loaded(command, paintSurface);
            invoker.Execute(place);
            grouping = false;
        }

        private void Front_canvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {

        }

        private void Width_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Height_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

    }
}