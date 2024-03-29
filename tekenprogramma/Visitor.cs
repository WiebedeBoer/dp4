﻿using System;
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

    //moving visitor client for group
    public class MoveClient
    {
        //The client code can run visitor operations over any set of elements without figuring out their concrete classes. 
        //The accept operation directs a call to the appropriate operation in the visitor object.
        public void Client(List<IComponent> components, List<FrameworkElement> drawnElements, Group selectedgroup, IVisitor visitor, Invoker invoker, PointerRoutedEventArgs e, Canvas paintSurface, FrameworkElement selectedelement)
        {
            //invoker.removedGroups.Add(selectedgroup);
            //calculate difference in location
            double leftOffset = Convert.ToDouble(selectedelement.ActualOffset.X) - e.GetCurrentPoint(paintSurface).Position.X;
            double topOffset = Convert.ToDouble(selectedelement.ActualOffset.Y) - e.GetCurrentPoint(paintSurface).Position.Y;
            if (selectedgroup.drawnElements.Count() > 0)
            {
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
                    FrameworkElement madeElement = component.Accept(visitor, invoker, e, paintSurface, movedElement, location, true);
                    //selectedgroup.movedElements.Add(madeElement);
                }
            }

            if (selectedgroup.addedGroups.Count() > 0)
            {
                foreach (Group subgroup in selectedgroup.addedGroups)
                {
                    MoveClient submover = new MoveClient();
                    IVisitor subvisitor = new ConcreteVisitorMove();
                    submover.Client(subgroup.drawnComponents, subgroup.drawnElements, subgroup, subvisitor, invoker, e, paintSurface, selectedelement);
                }
            }
            /*
            //add to moved or resized
            invoker.movedGroups.Add(selectedgroup);
            //remove selected group
            invoker.unselectedGroups.Add(selectedgroup);
            invoker.selectedGroups.RemoveAt(invoker.selectedGroups.Count() - 1);

            invoker.unmovedGroups.Add(selectedgroup);
            */
            selectedgroup.Repaint(invoker, paintSurface); //repaint
        }
    }

    //resizing client for group
    public class ResizeClient
    {
        public void Client(List<IComponent> components, List<FrameworkElement> drawnElements, Group selectedgroup, IVisitor visitor, Invoker invoker, PointerRoutedEventArgs e, Canvas paintSurface, FrameworkElement selectedelement)
        {
            //invoker.removedGroups.Add(selectedgroup);
            //calculate difference in size
            double newWidth = selectedgroup.ReturnSmallest(e.GetCurrentPoint(paintSurface).Position.X, Convert.ToDouble(selectedelement.ActualOffset.X));
            double newHeight = selectedgroup.ReturnSmallest(e.GetCurrentPoint(paintSurface).Position.Y, Convert.ToDouble(selectedelement.ActualOffset.Y));
            double widthOffset = selectedelement.Width - newWidth;
            double heightOffset = selectedelement.Height - newHeight;

            if (selectedgroup.drawnElements.Count() > 0)
            {
                int i = 0;
                foreach (var component in components)
                {
                    FrameworkElement movedElement = drawnElements[i];
                    Location location = new Location();
                    location.x = Convert.ToDouble(movedElement.ActualOffset.X);
                    location.y = Convert.ToDouble(movedElement.ActualOffset.Y);
                    location.width = Convert.ToDouble(movedElement.Width) - widthOffset;
                    location.height = Convert.ToDouble(movedElement.Height) - heightOffset;
                    invoker.executer++; //acceskey add
                    i++;
                    FrameworkElement madeElement = component.Accept(visitor, invoker, e, paintSurface, movedElement, location, true);
                    //selectedgroup.movedElements.Add(madeElement);
                }
            }

            if (selectedgroup.addedGroups.Count() > 0)
            {
                foreach (Group subgroup in selectedgroup.addedGroups)
                {
                    ResizeClient subresizer = new ResizeClient();
                    IVisitor subvisitor = new ConcreteVisitorResize();
                    subresizer.Client(subgroup.drawnComponents, subgroup.drawnElements, subgroup, subvisitor, invoker, e, paintSurface, selectedelement);
                }
            }
            /*
            //add to moved or resized
            invoker.movedGroups.Add(selectedgroup);
            //remove selected group
            invoker.unselectedGroups.Add(selectedgroup);
            invoker.selectedGroups.RemoveAt(invoker.selectedGroups.Count() - 1);

            invoker.unmovedGroups.Add(selectedgroup);
            */

            selectedgroup.Repaint(invoker, paintSurface);//repaint
        }
    }

    //
    //components
    //

    public interface IComponent
    {
        FrameworkElement Accept(IVisitor visitor, Invoker invoker, PointerRoutedEventArgs e, Canvas paintSurface, FrameworkElement selectedelement, Location location, Boolean moved);
        string Write(IWriter visitor, FrameworkElement element, Canvas paintSurface);
    }

    //rectangle component
    public class ConcreteComponentRectangle : IComponent
    {
        public double x;
        public double y;
        public double width;
        public double height;

        public ConcreteComponentRectangle(double x, double y, double width, double height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        //Note that calling ConcreteComponent which matches the current class name. 
        //This way we let the visitor know the class of the component it works with.
        public FrameworkElement Accept(IVisitor visitor, Invoker invoker, PointerRoutedEventArgs e, Canvas paintSurface, FrameworkElement selectedelement, Location location, Boolean moved)
        {
            FrameworkElement madeElement = visitor.VisitConcreteComponentRectangle(this, invoker, selectedelement, paintSurface, location, e, moved);
            return madeElement;
        }

        public string Write(IWriter visitor, FrameworkElement element, Canvas paintSurface)
        {
            string str = visitor.WriteRectangle(this, element, paintSurface);
            return str;
        }

        /*
        // Concrete Components may have special methods that don't exist in their base class or interface. 
        //The Visitor is still able to use these methods since it's aware of the component's concrete class.
        public void ExclusiveMethodOfConcreteComponentRectangle()
        {
            return "Rectangle";
            Rectangle newRectangle = new Rectangle(); //instance of new rectangle shape
            newRectangle.AccessKey = invoker.executer.ToString();
            newRectangle.Width = width; //set width
            newRectangle.Height = height; //set height     
            SolidColorBrush brush = new SolidColorBrush(); //brush
            brush.Color = Windows.UI.Colors.Blue; //standard brush color is blue
            newRectangle.Fill = brush; //fill color
            newRectangle.Name = "Rectangle"; //attach name
            Canvas.SetLeft(newRectangle, x); //set left position
            Canvas.SetTop(newRectangle, y); //set top position 
        }
        */
    }

    //ellipse component
    public class ConcreteComponentEllipse : IComponent
    {
        public double x;
        public double y;
        public double width;
        public double height;

        public ConcreteComponentEllipse(double x, double y, double width, double height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        // Same here: ConcreteComponent => ConcreteComponent
        public FrameworkElement Accept(IVisitor visitor, Invoker invoker, PointerRoutedEventArgs e, Canvas paintSurface, FrameworkElement selectedelement, Location location, Boolean moved)
        {
            FrameworkElement madeElement = visitor.VisitConcreteComponentEllipse(this, invoker, selectedelement, paintSurface, location, e, moved);
            return madeElement;
        }

        public string Write(IWriter visitor, FrameworkElement element, Canvas paintSurface)
        {
            string str = visitor.WriteEllipse(this, element, paintSurface);
            return str;
        }

        /*
        public void ExclusiveMethodOfConcreteComponentEllipse()
        {
            return "Ellipse";
            Ellipse newEllipse = new Ellipse(); //instance of new ellipse shape
            newEllipse.AccessKey = invoker.executer.ToString();
            newEllipse.Width = width;
            newEllipse.Height = height;
            SolidColorBrush brush = new SolidColorBrush();//brush
            brush.Color = Windows.UI.Colors.Blue;//standard brush color is blue
            newEllipse.Fill = brush;//fill color
            newEllipse.Name = "Ellipse";//attach name
            Canvas.SetLeft(newEllipse, x);//set left position
            Canvas.SetTop(newEllipse, y);//set top position   
        }
        */
    }

    //visitor cinterface
    public interface IVisitor
    {
        FrameworkElement VisitConcreteComponentRectangle(ConcreteComponentRectangle component, Invoker invoker, FrameworkElement element, Canvas paintSurface, Location location, PointerRoutedEventArgs e, Boolean moved);

        FrameworkElement VisitConcreteComponentEllipse(ConcreteComponentEllipse component, Invoker invoker, FrameworkElement element, Canvas paintSurface, Location location, PointerRoutedEventArgs e, Boolean moved);
    }

    //move visitor
    class ConcreteVisitorMove : IVisitor
    {

        public List<FrameworkElement> newDrawn = new List<FrameworkElement>(); //drawn elements
        public List<FrameworkElement> newSelected = new List<FrameworkElement>(); //selected elements

        private void PrepareUndo(Invoker invoker, FrameworkElement element)
        {
            //prepare undo
            //fetch
            List<FrameworkElement> fetchDrawn = invoker.undoElementsList.Last();
            //key
            string key = element.AccessKey;
            foreach (FrameworkElement addElement in fetchDrawn)
            {
                //if not selected
                if (addElement.AccessKey != key)
                {
                    newDrawn.Add(addElement); //add
                }
            }
            //shuffle selected
            newSelected = invoker.unselectElementsList.Last();
            invoker.reselectElementsList.Add(newSelected); //2b+
            invoker.unselectElementsList.RemoveAt(invoker.unselectElementsList.Count() - 1); //2a-
        }

        private void NewDrawn(Invoker invoker, FrameworkElement element)
        {
            newDrawn.Add(element);
            invoker.undoElementsList.Add(newDrawn);
        }

        public FrameworkElement VisitConcreteComponentRectangle(ConcreteComponentRectangle component, Invoker invoker, FrameworkElement element, Canvas paintSurface, Location location, PointerRoutedEventArgs e, Boolean moved)
        {
            PrepareUndo(invoker, element);
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
            returnelement = newRectangle;
            NewDrawn(invoker, returnelement);
            return returnelement;
        }

        public FrameworkElement VisitConcreteComponentEllipse(ConcreteComponentEllipse component, Invoker invoker, FrameworkElement element, Canvas paintSurface, Location location, PointerRoutedEventArgs e, Boolean moved)
        {
            PrepareUndo(invoker, element);
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
            returnelement = newEllipse;
            NewDrawn(invoker, returnelement);
            return returnelement;
        }



    }

    //resize visitor
    class ConcreteVisitorResize : IVisitor
    {
        public List<FrameworkElement> newDrawn = new List<FrameworkElement>(); //drawn elements
        public List<FrameworkElement> newSelected = new List<FrameworkElement>(); //selected elements
        private void PrepareUndo(Invoker invoker, FrameworkElement element)
        {
            //prepare undo
            //fetch
            List<FrameworkElement> fetchDrawn = invoker.undoElementsList.Last();
            //key
            string key = element.AccessKey;
            foreach (FrameworkElement addElement in fetchDrawn)
            {
                //if not selected
                if (addElement.AccessKey != key)
                {
                    newDrawn.Add(addElement); //add
                }
            }
            //shuffle selected
            newSelected = invoker.unselectElementsList.Last();
            invoker.reselectElementsList.Add(newSelected); //2b+
            invoker.unselectElementsList.RemoveAt(invoker.unselectElementsList.Count() - 1); //2a-
        }

        private void NewDrawn(Invoker invoker, FrameworkElement element)
        {
            newDrawn.Add(element);
            invoker.undoElementsList.Add(newDrawn);
        }
        public FrameworkElement VisitConcreteComponentRectangle(ConcreteComponentRectangle component, Invoker invoker, FrameworkElement element, Canvas paintSurface, Location location, PointerRoutedEventArgs e, Boolean moved)
        {

            //calculate size
            //double ex = e.GetCurrentPoint(paintSurface).Position.X;
            //double ey = e.GetCurrentPoint(paintSurface).Position.Y;
            //double lw = Convert.ToDouble(element.ActualOffset.X); //set width
            //double lh = Convert.ToDouble(element.ActualOffset.Y); //set height
            //double w = ReturnSmallest(ex, lw);
            //double h = ReturnSmallest(ey, lh);

            //Location nlocation = new Location();
            //nlocation.x = Convert.ToDouble(element.ActualOffset.X);
            //nlocation.y = Convert.ToDouble(element.ActualOffset.Y);
            //nlocation.width = w;
            //nlocation.height = h;

            PrepareUndo(invoker, element);
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
            returnelement = newRectangle;
            NewDrawn(invoker, returnelement);
            return returnelement;
        }

        public FrameworkElement VisitConcreteComponentEllipse(ConcreteComponentEllipse component, Invoker invoker, FrameworkElement element, Canvas paintSurface, Location location, PointerRoutedEventArgs e, Boolean moved)
        {

            //calculate size
            //double ex = e.GetCurrentPoint(paintSurface).Position.X;
            //double ey = e.GetCurrentPoint(paintSurface).Position.Y;
            //double lw = Convert.ToDouble(element.ActualOffset.X); //set width
            //double lh = Convert.ToDouble(element.ActualOffset.Y); //set height
            //double w = ReturnSmallest(ex, lw);
            //double h = ReturnSmallest(ey, lh);

            //Location nlocation = new Location();
            //nlocation.x = Convert.ToDouble(element.ActualOffset.X);
            //nlocation.y = Convert.ToDouble(element.ActualOffset.Y);
            //nlocation.width = w;
            //nlocation.height = h;

            PrepareUndo(invoker, element);

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
            returnelement = newEllipse;
            NewDrawn(invoker, returnelement);
            return returnelement;
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



    public class WriteClient
    {
        public async void Client(Canvas paintSurface, Invoker invoker, IWriter visitor)
        {
            string fileText = "";
            
            try
            {
                string lines = "";
                int i = 0;
                //ungrouped and drawn
                foreach (FrameworkElement child in paintSurface.Children)
                {
                    int elmcheck = CheckInGroup(invoker, child); //see if already in group
                    if (elmcheck == 0)
                    {
                        IComponent component = invoker.drawnComponents[i];
                        //IWriter visitor = new ConcreteVisitorWrite();
                        string str = component.Write(visitor, child, paintSurface);
                        lines += str;

                    //if (child is Rectangle)
                    //{
                    //    double top = (double)child.GetValue(Canvas.TopProperty);
                    //    double left = (double)child.GetValue(Canvas.LeftProperty);
                    //    string str = "rectangle " + left + " " + top + " " + child.Width + " " + child.Height + "\n";
                    //    lines += str;
                    //}
                    //else if (child is Ellipse)
                    //{
                    //    double top = (double)child.GetValue(Canvas.TopProperty);
                    //    double left = (double)child.GetValue(Canvas.LeftProperty);
                    //    string str = "ellipse " + left + " " + top + " " + child.Width + " " + child.Height + "\n";
                    //    lines += str;
                    //}
                    }
                    i++;
                }
            //grouped and drawn
            if (invoker.undoGroupsList.Count() > 0)
            {
                //grouped and drawn
                foreach (Group group in invoker.undoGroupsList.Last())
                {
                    string gstr = group.Display(0, group.depth, group);
                    lines += gstr;
                }
            }
            //create and write to file
            Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                Windows.Storage.StorageFile sampleFile = await storageFolder.CreateFileAsync("dp4data.txt", Windows.Storage.CreationCollisionOption.ReplaceExisting);
                await Windows.Storage.FileIO.WriteTextAsync(sampleFile, lines);
            }
            //file errors
            catch (System.IO.FileNotFoundException)
            {
                fileText = "File not found.";
            }
            catch (System.IO.FileLoadException)
            {
                fileText = "File Failed to load.";
            }
            catch (System.IO.IOException e)
            {
               fileText = "File IO error " + e;
            }
            catch (Exception err)
            {
                fileText = err.Message;
            }

        }

        //check if element is already in group
        public int CheckInGroup(Invoker invoker, FrameworkElement element)
        {
            int counter = 0;
            List<Group> checkList = invoker.undoGroupsList.Last();
            if (checkList.Count() > 0)
            {
                foreach (Group group in checkList)
                {
                    if (group.undoElementsList.Count() > 0)
                    {
                        foreach (FrameworkElement groupelement in group.undoElementsList.Last())
                        {
                            if (groupelement.AccessKey == element.AccessKey)
                            {
                                counter++;
                                return counter;
                            }
                        }
                    }
                    if (group.addedGroups.Count() > 0 && counter == 0)
                    {
                        foreach (Group subgroup in group.addedGroups)
                        {
                            counter = CheckInSubGroup(subgroup, invoker, element);
                            if (counter > 0)
                            {
                                return counter;
                            }
                        }
                    }
                }
            }
            return counter;
        }

        //check if element in sub group
        public int CheckInSubGroup(Group group, Invoker invoker, FrameworkElement element)
        {
            int counter = 0;
            if (group.undoElementsList.Count() > 0)
            {
                foreach (FrameworkElement groupelement in group.undoElementsList.Last())
                {
                    if (groupelement.AccessKey == element.AccessKey)
                    {
                        counter++;
                        return counter;
                    }
                }
            }
            if (group.addedGroups.Count() > 0 && counter == 0)
            {
                foreach (Group subgroup in group.addedGroups)
                {
                    counter = CheckInSubGroup(subgroup, invoker, element);
                    if (counter > 0)
                    {
                        return counter;
                    }
                }
            }
            return counter;
        }

        //display lines for saving
        public string Display(int depth, Group group, IWriter visitor, Canvas paintSurface)
        {
            //Display group.
            string str = "";
            //Add group.
            int i = 0;
            while (i < depth)
            {
                str += "\t";
                i++;
            }
            int groupcount = group.drawnElements.Count() + group.addedGroups.Count();
            str = str + "group " + groupcount + "\n";

            //Recursively display child nodes.
            depth = depth + 1; //add depth tab
            if (group.drawnElements.Count() > 0)
            {
                int j = 0;
                //foreach (FrameworkElement child in group.drawnElements)
                foreach(IComponent component in group.drawnComponents)
                {
                    FrameworkElement child = group.drawnElements[j];
                    j++;

                    int k = 0;
                    while (k < depth)
                    {
                        str += "\t";
                        k++;
                    }

                    str = str + component.Write(visitor, child, paintSurface);
                    //lines += str;
                    //if (child is Rectangle)
                    //{
                    //    int j = 0;
                    //    while (j < depth)
                    //    {
                    //        str += "\t";
                    //        j++;
                    //    }
                    //    str = str + "rectangle " + child.ActualOffset.X + " " + child.ActualOffset.Y + " " + child.Width + " " + child.Height + "\n";
                    //}
                    ////else if (child is Ellipse)
                    //else
                    //{
                    //    int j = 0;
                    //    while (j < depth)
                    //    {
                    //        str += "\t";
                    //        j++;
                    //    }
                    //    str = str + "ellipse " + child.ActualOffset.X + " " + child.ActualOffset.Y + " " + child.Width + " " + child.Height + "\n";
                    //}
                }
            }
            if (group.addedGroups.Count() > 0)
            {
                foreach (Group subgroup in group.addedGroups)
                {
                    Display(depth + 1, subgroup, visitor, paintSurface);
                }
            }
            return str;
        }



    }    

    //interface for writing to file
    public interface IWriter
    {
        string WriteRectangle(ConcreteComponentRectangle component, FrameworkElement element, Canvas paintSurface);
        string WriteEllipse(ConcreteComponentEllipse component, FrameworkElement element, Canvas paintSurface);
    }


    public class ConcreteVisitorWrite : IWriter
    {

        public string WriteRectangle(ConcreteComponentRectangle component, FrameworkElement element, Canvas paintSurface)
        {
            double top = (double)element.GetValue(Canvas.TopProperty);
            double left = (double)element.GetValue(Canvas.LeftProperty);
            string str = "rectangle " + left + " " + top + " " + element.Width + " " + element.Height + "\n";
            //lines += str;
            return str;
        }

        public string WriteEllipse(ConcreteComponentEllipse component, FrameworkElement element, Canvas paintSurface)
        {
            double top = (double)element.GetValue(Canvas.TopProperty);
            double left = (double)element.GetValue(Canvas.LeftProperty);
            string str = "ellipse " + left + " " + top + " " + element.Width + " " + element.Height + "\n";
            //lines += str;
            return str;
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









    /*



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

    */




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
