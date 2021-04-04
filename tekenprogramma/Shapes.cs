using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using Windows.Storage;
using Windows.ApplicationModel.Activation;

namespace tekenprogramma
{

    public class Shape
    {
        private double x;
        private double y;
        private double width;
        private double height;
        private bool selected;
        private bool drawed;

        private List<ICommand> actionsList = new List<ICommand>();
        private List<ICommand> redoList = new List<ICommand>();

        private List<FrameworkElement> undoList = new List<FrameworkElement>();
        private List<FrameworkElement> reList = new List<FrameworkElement>();

        public Invoker invoker;
        public Canvas paintSurface;
        public PointerRoutedEventArgs pet;

        //Rectangle backuprectangle; //rectangle shape
        //Ellipse backupellipse; //ellipse shape
        string type = "Rectangle"; //default shape type
        //bool moved = false; //moving
        private FrameworkElement backelement; //backup element

        //file IO
        public string FileText { get; set; }

        //shape
        public Shape(double x, double y, double width, double height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        // Selects the shape
        public void select(PointerRoutedEventArgs e)
        {
            this.selected = true;
        }

        // Deselects the shape
        public void deselect(PointerRoutedEventArgs e)
        {
            this.selected = false;
        }

        //give smallest
        public double returnSmallest(double first, double last)
        {
            if (first < last)
            {
                return last - first;
            }
            else
            {
                return first - last;
            }
        }

        //make rectangle
        public void makeRectangle(Invoker invoker, Canvas paintSurface)
        {
            this.drawed = false;
            this.type = "Rectangle";
            Rectangle newRectangle = new Rectangle(); //instance of new rectangle shape
            newRectangle.Width = width; //set width
            newRectangle.Height = height; //set height     
            SolidColorBrush brush = new SolidColorBrush(); //brush
            brush.Color = Windows.UI.Colors.Blue; //standard brush color is blue
            newRectangle.Fill = brush; //fill color
            newRectangle.Name = "Rectangle"; //attach name
            Canvas.SetLeft(newRectangle, x); //set left position
            Canvas.SetTop(newRectangle, y); //set top position 
            paintSurface.Children.Add(newRectangle);
        }

        //make ellipse
        public void makeEllipse(Invoker invoker, Canvas paintSurface)
        {
            this.type = "Ellipse";
            Ellipse newEllipse = new Ellipse(); //instance of new ellipse shape
            newEllipse.Width = width;
            newEllipse.Height = height;
            SolidColorBrush brush = new SolidColorBrush();//brush
            brush.Color = Windows.UI.Colors.Blue;//standard brush color is blue
            newEllipse.Fill = brush;//fill color
            newEllipse.Name = "Ellipse";//attach name
            Canvas.SetLeft(newEllipse, x);//set left position
            Canvas.SetTop(newEllipse, y);//set top position
            paintSurface.Children.Add(newEllipse);
        }

        //undo create
        public void remove(Invoker invoker, Canvas paintSurface)
        {
            paintSurface.Children.Clear();
            this.drawed = false;
        }

        //moving shape
        public void moving(PointerRoutedEventArgs e, FrameworkElement element, Canvas paintSurface)
        {
            x = e.GetCurrentPoint(paintSurface).Position.X;
            y = e.GetCurrentPoint(paintSurface).Position.Y;
            Canvas.SetLeft(element, x);
            Canvas.SetTop(element, y);
            this.undoList.Add(element);
        }

        public void undoMoving(Canvas paintSurface)
        {
            FrameworkElement element = this.undoList.Last();
            x = element.ActualOffset.X;
            y = element.ActualOffset.Y;
            Canvas.SetLeft(element, x);
            Canvas.SetTop(element, y);
            this.reList.Add(element);
        }

        public void redoMoving(Canvas paintSurface)
        {
            FrameworkElement element = this.reList.Last();
            x = element.ActualOffset.X;
            y = element.ActualOffset.Y;
            Canvas.SetLeft(element, x);
            Canvas.SetTop(element, y);
            this.undoList.Add(element);
        }

        //resize shape
        public void resize(PointerRoutedEventArgs e, FrameworkElement element, Canvas paintSurface)
        {
            double ex = e.GetCurrentPoint(paintSurface).Position.X;
            double ey = e.GetCurrentPoint(paintSurface).Position.Y;
            double lw = Convert.ToDouble(element.ActualOffset.X); //set width
            double lh = Convert.ToDouble(element.ActualOffset.Y); //set height
            double w = returnSmallest(ex, lw);
            double h = returnSmallest(ey, lh);
            element.Width = w;
            element.Height = h;
            this.undoList.Add(element);
        }

        public void undoResize(Canvas paintSurface)
        {
            FrameworkElement prevelement = this.undoList.Last();
            backelement.Width = prevelement.Width;
            backelement.Height = prevelement.Height;
            x = prevelement.ActualOffset.X;
            y = prevelement.ActualOffset.Y;
            Canvas.SetLeft(prevelement, x);
            Canvas.SetTop(prevelement, y);
            this.reList.Add(backelement);
        }

        public void redoResize(Canvas paintSurface)
        {
            FrameworkElement prevelement = this.reList.Last();
            backelement.Width = prevelement.Width;
            backelement.Height = prevelement.Height;
            x = prevelement.ActualOffset.X;
            y = prevelement.ActualOffset.Y;
            Canvas.SetLeft(prevelement, x);
            Canvas.SetTop(prevelement, y);
            this.undoList.Add(backelement);
        }

        //saving
        public async void saving(Canvas paintSurface)
        {

            try
            {
                string lines = "";

                foreach (FrameworkElement child in paintSurface.Children)
                {
                    if (child is Rectangle)
                    {
                        double top = (double)child.GetValue(Canvas.TopProperty);
                        double left = (double)child.GetValue(Canvas.LeftProperty);
                        string str = "rectangle " + left + " " + top + " " + child.Width + " " + child.Height + "\n";
                        lines += str;
                    }
                    else
                    {
                        double top = (double)child.GetValue(Canvas.TopProperty);
                        double left = (double)child.GetValue(Canvas.LeftProperty);
                        string str = "ellipse " + left + " " + top + " " + child.Width + " " + child.Height + "\n";
                        lines += str;
                    }
                }
                //create and write to file
                Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                Windows.Storage.StorageFile sampleFile = await storageFolder.CreateFileAsync("dp2data.txt", Windows.Storage.CreationCollisionOption.ReplaceExisting);
                await Windows.Storage.FileIO.WriteTextAsync(sampleFile, lines);
            }
            catch (System.IO.FileNotFoundException)
            {
                FileText = "File not found.";
            }
            catch (System.IO.FileLoadException)
            {
                FileText = "File Failed to load";
            }
            catch (System.IO.IOException e)
            {
                FileText = "File IO error " + e;
            }
            catch (Exception err)
            {
                FileText = err.Message;
            }

        }

        //loading
        public async void loading(Canvas paintSurface)
        {
            //clear previous canvas
            paintSurface.Children.Clear();
            //read file
            Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile saveFile = await storageFolder.GetFileAsync("dp2data.txt");
            string text = await Windows.Storage.FileIO.ReadTextAsync(saveFile);
            //load shapes
            string[] readText = Regex.Split(text, "\\n+");
            foreach (string s in readText)
            {
                if (s.Length > 2)
                {
                    string[] line = Regex.Split(s, "\\s+");
                    if (line[0] == "Ellipse")
                    {
                        this.getEllipse(s, paintSurface);
                    }
                    else
                    {
                        this.getRectangle(s, paintSurface);
                    }
                }
            }
        }

        //load ellipse
        public void getEllipse(String lines, Canvas paintSurface)
        {
            string[] line = Regex.Split(lines, "\\s+");

            double x = Convert.ToDouble(line[1]);
            double y = Convert.ToDouble(line[2]);
            double width = Convert.ToDouble(line[3]);
            double height = Convert.ToDouble(line[4]);

            Ellipse newEllipse = new Ellipse(); //instance of new ellipse shape
            newEllipse.Width = width;
            newEllipse.Height = height;
            SolidColorBrush brush = new SolidColorBrush();//brush
            brush.Color = Windows.UI.Colors.Green;//standard brush color is blue
            newEllipse.Fill = brush;//fill color
            newEllipse.Name = "Ellipse";//attach name
            Canvas.SetLeft(newEllipse, x);//set left position
            Canvas.SetTop(newEllipse, y);//set top position
            paintSurface.Children.Add(newEllipse);
        }

        //load rectangle
        public void getRectangle(String lines, Canvas paintSurface)
        {
            string[] line = Regex.Split(lines, "\\s+");

            double x = Convert.ToDouble(line[1]);
            double y = Convert.ToDouble(line[2]);
            double width = Convert.ToDouble(line[3]);
            double height = Convert.ToDouble(line[4]);

            Rectangle newRectangle = new Rectangle(); //instance of new rectangle shape
            newRectangle.Width = width; //set width
            newRectangle.Height = height; //set height     
            SolidColorBrush brush = new SolidColorBrush(); //brush
            brush.Color = Windows.UI.Colors.Green; //standard brush color is blue
            newRectangle.Fill = brush; //fill color
            newRectangle.Name = "Rectangle"; //attach name
            Canvas.SetLeft(newRectangle, x); //set left position
            Canvas.SetTop(newRectangle, y); //set top position 
            paintSurface.Children.Add(newRectangle);
        }

    }

}
