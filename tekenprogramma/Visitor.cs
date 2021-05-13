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

    public abstract class Strategy
    {
        protected ICommand cmd;
        protected Invoker invoker;
        protected Canvas paintSurface;
        public Shape shape;
        //public abstract void place();
    }

    public class Client
    {
        //The client code can run visitor operations over any set of elements without figuring out their concrete classes. 
        //The accept operation directs a call to the appropriate operation in the visitor object.
        public static void ClientCode(List<IComponent> components, List<FrameworkElement> drawnElements, IVisitor visitor, Invoker invoker, PointerRoutedEventArgs e, Canvas paintSurface, FrameworkElement selectedelement)
        {
            Group selectedgroup = invoker.selectedGroups.Last();
            invoker.removedGroups.Add(selectedgroup);
            //calculate difference in location
            double leftOffset = Convert.ToDouble(selectedelement.ActualOffset.X) - e.GetCurrentPoint(paintSurface).Position.X;
            double topOffset = Convert.ToDouble(selectedelement.ActualOffset.Y) - e.GetCurrentPoint(paintSurface).Position.Y;
            int i = 0;
            foreach (var component in components)
            {
                FrameworkElement movedElement = drawnElements[i];
                Location location = new Location();
                location.x = Convert.ToDouble(movedElement.ActualOffset.X) - leftOffset;
                location.y = Convert.ToDouble(movedElement.ActualOffset.Y) - topOffset;
                location.width = movedElement.Width;
                location.height = movedElement.Height;
                invoker.executer++;//acceskey add
                i++;
                FrameworkElement madeElement = component.Accept(visitor, invoker, e, paintSurface, selectedelement, location);
            }
        }
    }

    public interface IComponent
    {
        FrameworkElement Accept(IVisitor visitor, Invoker invoker, PointerRoutedEventArgs e, Canvas paintSurface, FrameworkElement selectedelement, Location location);
    }

    public class ConcreteComponentRectangle : IComponent
    {
        //Note that calling ConcreteComponent which matches the current class name. 
        //This way we let the visitor know the class of the component it works with.
        public FrameworkElement Accept(IVisitor visitor, Invoker invoker, PointerRoutedEventArgs e, Canvas paintSurface, FrameworkElement selectedelement, Location location)
        {
            FrameworkElement madeElement = visitor.VisitConcreteComponentRectangle(this, invoker, e, selectedelement, paintSurface, location);
            return madeElement;
        }

        /*
        // Concrete Components may have special methods that don't exist in their base class or interface. 
        //The Visitor is still able to use these methods since it's aware of the component's concrete class.
        public string ExclusiveMethodOfConcreteComponentRectangle()
        {
            return "Rectangle";
        }
        */
    }

    public class ConcreteComponentEllipse : IComponent
    {
        // Same here: ConcreteComponent => ConcreteComponent
        public FrameworkElement Accept(IVisitor visitor, Invoker invoker, PointerRoutedEventArgs e, Canvas paintSurface, FrameworkElement selectedelement, Location location)
        {
            FrameworkElement madeElement = visitor.VisitConcreteComponentEllipse(this, invoker, e, selectedelement, paintSurface, location);
            return madeElement;
        }

        /*
        public string SpecialMethodOfConcreteComponentEllipse()
        {
            return "Ellipse";
        }
        */
    }

    public interface IVisitor
    {
        FrameworkElement VisitConcreteComponentRectangle(ConcreteComponentRectangle component, Invoker invoker, PointerRoutedEventArgs e, FrameworkElement element, Canvas paintSurface, Location location);

        FrameworkElement VisitConcreteComponentEllipse(ConcreteComponentEllipse component, Invoker invoker, PointerRoutedEventArgs e, FrameworkElement element, Canvas paintSurface, Location location);
    }

    class ConcreteVisitorMove : IVisitor
    {
        public FrameworkElement VisitConcreteComponentRectangle(ConcreteComponentRectangle component, Invoker invoker, PointerRoutedEventArgs e, FrameworkElement element, Canvas paintSurface, Location location)
        {
            FrameworkElement returnelement = null;
            Rectangle newRectangle = new Rectangle(); //instance of new rectangle shape
            newRectangle.AccessKey = invoker.executer.ToString();
            newRectangle.Width = location.width; //set width
            newRectangle.Height = location.height; //set height     
            SolidColorBrush brush = new SolidColorBrush(); //brush
            brush.Color = Windows.UI.Colors.Yellow; //standard brush color is blue
            newRectangle.Fill = brush; //fill color
            newRectangle.Name = "Rectangle"; //attach name
            Canvas.SetLeft(newRectangle, location.x);//set left position
            Canvas.SetTop(newRectangle, location.y); //set top position          
            invoker.drawnElements.Add(newRectangle);
            returnelement = newRectangle;
            return returnelement;
        }

        public FrameworkElement VisitConcreteComponentEllipse(ConcreteComponentEllipse component, Invoker invoker, PointerRoutedEventArgs e, FrameworkElement element, Canvas paintSurface, Location location)
        {
            FrameworkElement returnelement = null;
            Ellipse newEllipse = new Ellipse(); //instance of new ellipse shape
            newEllipse.AccessKey = invoker.executer.ToString();
            newEllipse.Width = location.width;
            newEllipse.Height = location.height;
            SolidColorBrush brush = new SolidColorBrush();//brush
            brush.Color = Windows.UI.Colors.Yellow;//standard brush color is blue
            newEllipse.Fill = brush;//fill color
            newEllipse.Name = "Ellipse";//attach name
            Canvas.SetLeft(newEllipse, location.x);//set left position
            Canvas.SetTop(newEllipse, location.y);//set top position
            invoker.drawnElements.Add(newEllipse);
            returnelement = newEllipse;
            return returnelement;
        }
    }

    class ConcreteVisitorResize : IVisitor
    {
        public FrameworkElement VisitConcreteComponentRectangle(ConcreteComponentRectangle component, Invoker invoker, PointerRoutedEventArgs e, FrameworkElement element, Canvas paintSurface, Location location)
        {
            FrameworkElement returnelement = null;
            Rectangle newRectangle = new Rectangle(); //instance of new rectangle shape
            newRectangle.AccessKey = invoker.executer.ToString();
            newRectangle.Width = location.width; //set width
            newRectangle.Height = location.height; //set height     
            SolidColorBrush brush = new SolidColorBrush(); //brush
            brush.Color = Windows.UI.Colors.Yellow; //standard brush color is blue
            newRectangle.Fill = brush; //fill color
            newRectangle.Name = "Rectangle"; //attach name
            Canvas.SetLeft(newRectangle, location.x);
            Canvas.SetTop(newRectangle, location.y);
            invoker.drawnElements.Add(newRectangle);
            returnelement = newRectangle;
            return returnelement;
        }

        public FrameworkElement VisitConcreteComponentEllipse(ConcreteComponentEllipse component, Invoker invoker, PointerRoutedEventArgs e, FrameworkElement element, Canvas paintSurface, Location location)
        {
            FrameworkElement returnelement = null;
            Ellipse newEllipse = new Ellipse(); //instance of new ellipse shape
            newEllipse.AccessKey = invoker.executer.ToString();
            newEllipse.Width = location.width; //set width
            newEllipse.Height = location.height; //set height 
            SolidColorBrush brush = new SolidColorBrush();//brush
            brush.Color = Windows.UI.Colors.Yellow;//standard brush color is blue
            newEllipse.Fill = brush;//fill color
            newEllipse.Name = "Ellipse";//attach name
            Canvas.SetLeft(newEllipse, location.x);//set left position
            Canvas.SetTop(newEllipse, location.y);//set top position
            invoker.drawnElements.Add(newEllipse);
            returnelement = newEllipse;
            return returnelement;
        }
    }





    /*
    //interface for creation, moving,resizing
    public interface IVisitor
    {
        void VisitRectangle(Invoker invoker, PointerRoutedEventArgs e, FrameworkElement element, Canvas paintSurface, Location location);
        void VisitEllipse(Invoker invoker, PointerRoutedEventArgs e, FrameworkElement element, Canvas paintSurface, Location location);
    }
    */

    //interface for writing to file
    public interface IWriter
    {
        string WriteRectangle(FrameworkElement element, Canvas paintSurface);
        string WriteEllipse(FrameworkElement element, Canvas paintSurface);
    }

    //interface for the visitor
    //public interface IShapes
    //{
    //    //void Prepare(int x, int y, int width, int height);
    //    //void Place();
    //}

    ////abstract for the visitor
    //public abstract class Visitor : IShapes
    //{
    //    private Invoker invoker;
    //    private Canvas paintSurface;
    //    public Shape shape;

    //    //public abstract void Prepare(int x, int y, int width, int height);
    //    //public abstract void Place();
    //}



    public interface IShapeVisitor
    {
        void VisitGroup(Grouping group);
        void VisitRectangle(RectangleShape rectangle);
        void VisitEllipse(EllipseShape ellipse);

    }

    public class ShapeDisplayVisitor : IShapeVisitor
    {

       public void VisitGroup(Grouping grouping)
       {
            //Displaying group.

        }

       public void VisitRectangle(RectangleShape rectangle)
       {
            //Displaying rectangle.

        }

       public void VisitEllipse(EllipseShape ellipse)
       {
            //Displaying ellipse.

        }

    }


    public class ShapeResizeVisitor : IShapeVisitor
    {

        public void VisitGroup(Grouping grouping)
        {
            //Resizing group.

        }

        public void VisitRectangle(RectangleShape rectangle)
        {
            //Resizing rectangle.
            void VisitRectangle(Invoker invoker, PointerRoutedEventArgs e, FrameworkElement element, Canvas paintSurface, Location location)
            {
                FrameworkElement returnelement = null;
                Rectangle newRectangle = new Rectangle(); //instance of new rectangle shape
                newRectangle.AccessKey = invoker.executer.ToString();
                newRectangle.Width = location.width; //set width
                newRectangle.Height = location.height; //set height     
                SolidColorBrush brush = new SolidColorBrush(); //brush
                brush.Color = Windows.UI.Colors.Yellow; //standard brush color is blue
                newRectangle.Fill = brush; //fill color
                newRectangle.Name = "Rectangle"; //attach name
                Canvas.SetLeft(newRectangle, location.x);
                Canvas.SetTop(newRectangle, location.y);
                invoker.drawnElements.Add(newRectangle);
                returnelement = newRectangle;
            }
        }

        public void VisitEllipse(EllipseShape ellipse)
        {
            //Resizing ellipse.
            void VisitEllipse(Invoker invoker, PointerRoutedEventArgs e, FrameworkElement element, Canvas paintSurface, Location location)
            {
                FrameworkElement returnelement = null;
                Ellipse newEllipse = new Ellipse(); //instance of new ellipse shape
                newEllipse.AccessKey = invoker.executer.ToString();
                newEllipse.Width = location.width; //set width
                newEllipse.Height = location.height; //set height 
                SolidColorBrush brush = new SolidColorBrush();//brush
                brush.Color = Windows.UI.Colors.Yellow;//standard brush color is blue
                newEllipse.Fill = brush;//fill color
                newEllipse.Name = "Ellipse";//attach name
                Canvas.SetLeft(newEllipse, location.x);//set left position
                Canvas.SetTop(newEllipse, location.y);//set top position
                invoker.drawnElements.Add(newEllipse);
                returnelement = newEllipse;
            }
        }

    }

    public class ShapeMoveVisitor : IShapeVisitor
    {

        public void VisitGroup(Grouping grouping)
        {
            //Moving group.

        }

        public void VisitRectangle(RectangleShape rectangle)
        {
            //Moving rectangle.
            void VisitRectangle(Invoker invoker, PointerRoutedEventArgs e, FrameworkElement element, Canvas paintSurface, Location location)
            {
                FrameworkElement returnelement = null;
                Rectangle newRectangle = new Rectangle(); //instance of new rectangle shape
                newRectangle.AccessKey = invoker.executer.ToString();
                newRectangle.Width = location.width; //set width
                newRectangle.Height = location.height; //set height     
                SolidColorBrush brush = new SolidColorBrush(); //brush
                brush.Color = Windows.UI.Colors.Yellow; //standard brush color is blue
                newRectangle.Fill = brush; //fill color
                newRectangle.Name = "Rectangle"; //attach name
                Canvas.SetLeft(newRectangle, location.x);//set left position
                Canvas.SetTop(newRectangle, location.y); //set top position          
                invoker.drawnElements.Add(newRectangle);
                returnelement = newRectangle;
            }
        }

        public void VisitEllipse(EllipseShape ellipse)
        {
            //Moving ellipse.
            void VisitEllipse(Invoker invoker, PointerRoutedEventArgs e, FrameworkElement element, Canvas paintSurface, Location location)
            {
                FrameworkElement returnelement = null;
                Ellipse newEllipse = new Ellipse(); //instance of new ellipse shape
                newEllipse.AccessKey = invoker.executer.ToString();
                newEllipse.Width = location.width;
                newEllipse.Height = location.height;
                SolidColorBrush brush = new SolidColorBrush();//brush
                brush.Color = Windows.UI.Colors.Yellow;//standard brush color is blue
                newEllipse.Fill = brush;//fill color
                newEllipse.Name = "Ellipse";//attach name
                Canvas.SetLeft(newEllipse, location.x);//set left position
                Canvas.SetTop(newEllipse, location.y);//set top position
                invoker.drawnElements.Add(newEllipse);
                returnelement = newEllipse;
            }
        }

    }


    public interface IShapes
    {
        void accept(IShapeVisitor shapeVisitor);
    }

    public class RectangleShape : IShapes
    {
       public void accept(IShapeVisitor shapeVisitor)
        {
            shapeVisitor.VisitRectangle(this);
        }
    }

    public class EllipseShape : IShapes
    {
        public void accept(IShapeVisitor shapeVisitor)
        {
            shapeVisitor.VisitEllipse(this);
        }
    }

    public class Grouping : IShapes
    {

        IShapes[] parts;

        public Grouping()
        {
            parts = new IShapes[] { new RectangleShape(), new EllipseShape() };
        }

        public void accept(IShapeVisitor shapeVisitor)
        {
            for (int i = 0; i < parts.Length; i++)
            {
                parts[i].accept(shapeVisitor);
            }
            shapeVisitor.VisitGroup(this);
        }
    }






    /*
    //resizing
    public class VisitResize : IVisitor
    {

        public void VisitRectangle(Invoker invoker, PointerRoutedEventArgs e, FrameworkElement element, Canvas paintSurface, Location location)
        {
            FrameworkElement returnelement = null;
            Rectangle newRectangle = new Rectangle(); //instance of new rectangle shape
            newRectangle.AccessKey = invoker.executer.ToString();
            newRectangle.Width = location.width; //set width
            newRectangle.Height = location.height; //set height     
            SolidColorBrush brush = new SolidColorBrush(); //brush
            brush.Color = Windows.UI.Colors.Yellow; //standard brush color is blue
            newRectangle.Fill = brush; //fill color
            newRectangle.Name = "Rectangle"; //attach name
            Canvas.SetLeft(newRectangle, location.x);
            Canvas.SetTop(newRectangle, location.y);
            invoker.drawnElements.Add(newRectangle);
            returnelement = newRectangle;
        }



        public void VisitEllipse(Invoker invoker, PointerRoutedEventArgs e, FrameworkElement element, Canvas paintSurface, Location location)
        {
            FrameworkElement returnelement = null;
            Ellipse newEllipse = new Ellipse(); //instance of new ellipse shape
            newEllipse.AccessKey = invoker.executer.ToString();
            newEllipse.Width = location.width; //set width
            newEllipse.Height = location.height; //set height 
            SolidColorBrush brush = new SolidColorBrush();//brush
            brush.Color = Windows.UI.Colors.Yellow;//standard brush color is blue
            newEllipse.Fill = brush;//fill color
            newEllipse.Name = "Ellipse";//attach name
            Canvas.SetLeft(newEllipse, location.x);//set left position
            Canvas.SetTop(newEllipse, location.y);//set top position
            invoker.drawnElements.Add(newEllipse);
            returnelement = newEllipse;
        }

        public void KeyNumber(FrameworkElement element, Invoker invoker)
        {
            string key = element.AccessKey;
            int inc = 0;
            int number = 0;
            foreach (FrameworkElement drawn in invoker.drawnElements)
            {
                if (drawn.AccessKey == key)
                {
                    number = inc;
                }
                inc++;
            }
            invoker.drawnElements.RemoveAt(number);
            invoker.removedElements.Add(element);
            invoker.movedElements.Add(element);
        }

        public void Repaint(Invoker invoker, Canvas paintSurface)
        {
            paintSurface.Children.Clear();
            foreach (FrameworkElement drawelement in invoker.drawnElements)
            {
                paintSurface.Children.Add(drawelement); //add
            }
        }

        //give smallest
        public double ReturnSmallest(double first, double last)
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

    }
    */


    /*

    //moving visitor
    public class VisitMove : IVisitor
    {

        public void VisitRectangle(Invoker invoker, PointerRoutedEventArgs e, FrameworkElement element, Canvas paintSurface, Location location)
        {
            FrameworkElement returnelement = null;
            Rectangle newRectangle = new Rectangle(); //instance of new rectangle shape
            newRectangle.AccessKey = invoker.executer.ToString();
            newRectangle.Width = location.width; //set width
            newRectangle.Height = location.height; //set height     
            SolidColorBrush brush = new SolidColorBrush(); //brush
            brush.Color = Windows.UI.Colors.Yellow; //standard brush color is blue
            newRectangle.Fill = brush; //fill color
            newRectangle.Name = "Rectangle"; //attach name
            Canvas.SetLeft(newRectangle, location.x);//set left position
            Canvas.SetTop(newRectangle, location.y); //set top position          
            invoker.drawnElements.Add(newRectangle);
            returnelement = newRectangle;
        }

        public void VisitEllipse(Invoker invoker, PointerRoutedEventArgs e, FrameworkElement element, Canvas paintSurface, Location location)
        {
            FrameworkElement returnelement = null;
            Ellipse newEllipse = new Ellipse(); //instance of new ellipse shape
            newEllipse.AccessKey = invoker.executer.ToString();
            newEllipse.Width = location.width;
            newEllipse.Height = location.height;
            SolidColorBrush brush = new SolidColorBrush();//brush
            brush.Color = Windows.UI.Colors.Yellow;//standard brush color is blue
            newEllipse.Fill = brush;//fill color
            newEllipse.Name = "Ellipse";//attach name
            Canvas.SetLeft(newEllipse, location.x);//set left position
            Canvas.SetTop(newEllipse, location.y);//set top position
            invoker.drawnElements.Add(newEllipse);
            returnelement = newEllipse;
        }

        public void KeyNumber(FrameworkElement element, Invoker invoker)
        {
            string key = element.AccessKey;
            int inc = 0;
            int number = 0;
            foreach (FrameworkElement drawn in invoker.drawnElements)
            {
                if (drawn.AccessKey == key)
                {
                    number = inc;
                }
                inc++;
            }
            invoker.drawnElements.RemoveAt(number);
            invoker.removedElements.Add(element);
            invoker.movedElements.Add(element);
        }

        public void Repaint(Invoker invoker, Canvas paintSurface)
        {
            paintSurface.Children.Clear();
            foreach (FrameworkElement drawelement in invoker.drawnElements)
            {
                paintSurface.Children.Add(drawelement); //add
            }
        }


    }
    */

    public class VisitWrite : IWriter
    {


        public string WriteRectangle(FrameworkElement element, Canvas paintSurface)
        {
            double top = (double)element.GetValue(Canvas.TopProperty);
            double left = (double)element.GetValue(Canvas.LeftProperty);
            string str = "rectangle " + left + " " + top + " " + element.Width + " " + element.Height + "\n";
            //lines += str;
            return str;
        }

        public string WriteEllipse(FrameworkElement element, Canvas paintSurface)
        {
            double top = (double)element.GetValue(Canvas.TopProperty);
            double left = (double)element.GetValue(Canvas.LeftProperty);
            string str = "ellipse " + left + " " + top + " " + element.Width + " " + element.Height + "\n";
            //lines += str;
            return str;
        }

        /*
        public async void WriteToFile(Canvas paintSurface)
        {
            string lines = "";

            foreach (FrameworkElement child in paintSurface.Children)
            {
                //rectangle
                if (child is Rectangle)
                {
                    double top = (double)child.GetValue(Canvas.TopProperty);
                    double left = (double)child.GetValue(Canvas.LeftProperty);
                    string str = "rectangle " + left + " " + top + " " + child.Width + " " + child.Height + "\n";
                    lines += str;
                }
                //ellipse
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
        */
    }
}
