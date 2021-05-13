using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Input;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace tekenprogramma
{

    //location class
    public class Location
    {
        public double x;
        public double y;
        public double width;
        public double height;

        public Location(double x, double y, double width, double height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public Location()
        {

        }
    }

    //class group
    public class Group : Baseshape
    {
        public double height;
        public double width;
        public double x;
        public double y;
        public string type;
        public int depth;
        public int id;
        //public List<Baseshape> groupitems;
        public Canvas selectedCanvas;

        //public List<Baseshape> children = new List<Baseshape>();

        //public List<FrameworkElement> selectedElements = new List<FrameworkElement>();
        //public List<FrameworkElement> unselectedElements = new List<FrameworkElement>();

        public List<FrameworkElement> drawnElements = new List<FrameworkElement>();
        public List<FrameworkElement> removedElements = new List<FrameworkElement>();
        public List<FrameworkElement> movedElements = new List<FrameworkElement>();

        public List<IComponent> components = new List<IComponent>();

        //public List<Canvas> groupedCanvases = new List<Canvas>();
        //public List<Canvas> ungroupedCanvases = new List<Canvas>();

        public List<Group> addedGroups = new List<Group>();
        public List<Group> movedGroups = new List<Group>();

        public Invoker invoker;
        public FrameworkElement element;
        public Canvas lastCanvas;

        public Group(double height, double width, double x, double y, string type, int depth, int id, Canvas selectedCanvas, Invoker invoker, FrameworkElement element) : base(height, width, x, y)
        {
            this.height = height;
            this.width = width;
            this.x = x;
            this.y = y;
            this.type = type;
            this.depth = depth;
            this.id = id;
            this.selectedCanvas = selectedCanvas;
            this.invoker = invoker;
            this.element = element;
        }

        //make new group
        public void MakeGroup(Group group, Canvas selectedCanvas, Invoker invoker)
        {
            if (invoker.selectElementsList.Count() > 0)
            {
                //get selected elements
                foreach (FrameworkElement elm in invoker.selectElementsList)
                //for (int index = 0; index < invoker.selectElementsList.Count(); index++)
                {
                    //if (invoker.selectElementsList.Count() > 0)
                    //{
                    //FrameworkElement elm = invoker.selectElementsList[index];
                    elm.Opacity = 0.9;
                    //check if selected is not already grouped element
                    int elmcheck = CheckInGroup(invoker, elm);
                    if (elmcheck == 0)
                    {
                        this.drawnElements.Add(elm);
                    }
                    //remove selected
                    invoker.unselectElementsList.Add(elm);
                    //invoker.selectElementsList.RemoveAt(invoker.selectElementsList.Count() - 1);
                    //}


                }
                invoker.selectElementsList.Clear();
            }
            if (invoker.selectedGroups.Count() > 0)
            {
                //get selected groups
                foreach (Group selectedgroup in invoker.selectedGroups)
                {
                    this.addedGroups.Add(selectedgroup);
                    //remove selected
                    invoker.unselectedGroups.Add(selectedgroup);
                    //invoker.selectedGroups.RemoveAt(invoker.selectedGroups.Count() - 1);
                    SelectedGroup(selectedgroup, invoker); //remove from drawn groups
                }
                invoker.selectedGroups.Clear();
            }
            invoker.drawnGroups.Add(this);
            this.id = invoker.executer; //id

        }

        //removed selected group from drawn elements
        public void SelectedGroup(Group group, Invoker invoker)
        {
            int key = group.id;
            int inc = 0;
            int number = 0;
            foreach (Group drawn in invoker.drawnGroups)
            {
                if (drawn.id == key)
                {
                    number = inc;
                }
                inc++;
            }
            invoker.drawnGroups.RemoveAt(number);
        }

        //un group
        public void UnGroup(Canvas selectedCanvas, Invoker invoker)
        {
            Group lastgroup = invoker.drawnGroups.Last();
            invoker.drawnGroups.RemoveAt(invoker.drawnGroups.Count() - 1);
            if (lastgroup.drawnElements.Count() > 0)
            {
                //get elements
                foreach (FrameworkElement elm in lastgroup.drawnElements)
                {
                    //add selected
                    invoker.selectElementsList.Add(elm);
                    if (invoker.unselectElementsList.Count() > 0)
                    {
                        invoker.unselectElementsList.RemoveAt(invoker.unselectElementsList.Count() - 1);
                    }
                    //elm.Opacity = 0.5;
                }
            }
            if (lastgroup.addedGroups.Count() > 0)
            {
                //get groups
                foreach (Group selectedgroup in lastgroup.addedGroups)
                {
                    //add selected
                    invoker.selectedGroups.Add(selectedgroup);
                    if (invoker.unselectedGroups.Count() > 0)
                    {
                        invoker.unselectedGroups.RemoveAt(invoker.unselectedGroups.Count() - 1);
                    }
                    invoker.drawnGroups.Add(selectedgroup); //re add to drawn
                }
            }
            invoker.removedGroups.Add(lastgroup);
        }

        //re group
        public void ReGroup(Canvas selectedCanvas, Invoker invoker)
        {
            Group lastgroup = invoker.removedGroups.Last();

            if (lastgroup.drawnElements.Count() > 0)
            {
                //get elements
                foreach (FrameworkElement elm in lastgroup.drawnElements)
                {
                    //remove selected
                    invoker.unselectElementsList.Add(elm);
                    invoker.selectElementsList.RemoveAt(invoker.selectElementsList.Count() - 1);
                }
            }
            if (lastgroup.addedGroups.Count() > 0)
            {
                //get groups
                foreach (Group selectedgroup in lastgroup.addedGroups)
                {
                    //remove selected
                    invoker.unselectedGroups.Add(selectedgroup);
                    invoker.selectedGroups.RemoveAt(invoker.selectedGroups.Count() - 1);
                    invoker.drawnGroups.RemoveAt(invoker.drawnGroups.Count() - 1);
                }
            }
            invoker.drawnGroups.Add(lastgroup);
            invoker.removedGroups.RemoveAt(invoker.removedGroups.Count() - 1);
        }

        //
        //moving
        //

        //moving
        public void Moving(Invoker invoker, PointerRoutedEventArgs e, Canvas paintSurface, FrameworkElement selectedelement)
        {
            //FrameworkElement selectedelement = invoker.selectElementsList.Last();
            Group selectedgroup = invoker.selectedGroups.Last();
            invoker.removedGroups.Add(selectedgroup);
            //calculate difference in location
            double leftOffset = Convert.ToDouble(selectedelement.ActualOffset.X) - e.GetCurrentPoint(paintSurface).Position.X;
            double topOffset = Convert.ToDouble(selectedelement.ActualOffset.Y) - e.GetCurrentPoint(paintSurface).Position.Y;
            //elements in group
            if (selectedgroup.drawnElements.Count() > 0)
            {
                foreach (FrameworkElement movedElement in selectedgroup.drawnElements)
                {
                    Location location = new Location();
                    location.x = Convert.ToDouble(movedElement.ActualOffset.X) - leftOffset;
                    location.y = Convert.ToDouble(movedElement.ActualOffset.Y) - topOffset;
                    location.width = movedElement.Width;
                    location.height = movedElement.Height;
                    invoker.executer++;//acceskey add
                    FrameworkElement madeElement = MovingElement(movedElement, invoker, paintSurface, location);
                    selectedgroup.movedElements.Add(madeElement);
                    //selectedgroup.drawnElements.Add(madeElement);
                    //invoker.movedElements.Add(movedElement);
                }
                //selectedgroup.drawnElements.Clear();
            }
            //if (selectedgroup.movedElements.Count() >0)
            //{
            //    foreach (FrameworkElement drewElement in selectedgroup.movedElements)
            //    {
            //        selectedgroup.drawnElements.Add(drewElement);
            //    }
            //}

            //remove selected element
            //invoker.unselectElementsList.Add(selectedelement);
            //invoker.selectElementsList.RemoveAt(invoker.selectElementsList.Count() - 1);

            if (selectedgroup.addedGroups.Count() > 0)
            {
                foreach (Group subgroup in selectedgroup.addedGroups)
                {
                    selectedgroup.SubMoving(invoker, subgroup, leftOffset, topOffset, paintSurface); //subgroups
                }
            }
            //add to moved or resized
            invoker.movedGroups.Add(selectedgroup);
            //remove selected group
            invoker.unselectedGroups.Add(selectedgroup);
            invoker.selectedGroups.RemoveAt(invoker.selectedGroups.Count() - 1);

            Repaint(invoker, paintSurface); //repaint
        }

        //recursively moving subgroups
        public void SubMoving(Invoker invoker, Group selectedgroup, double leftOffset, double topOffset, Canvas paintSurface)
        {
            //elements in group
            if (selectedgroup.drawnElements.Count() > 0)
            {
                foreach (FrameworkElement movedElement in selectedgroup.drawnElements)
                {
                    Location location = new Location();
                    location.x = Convert.ToDouble(movedElement.ActualOffset.X) - leftOffset;
                    location.y = Convert.ToDouble(movedElement.ActualOffset.Y) - topOffset;
                    location.width = movedElement.Width;
                    location.height = movedElement.Height;
                    invoker.executer++;//acceskey add
                    FrameworkElement madeElement = MovingElement(movedElement, invoker, paintSurface, location);
                    selectedgroup.movedElements.Add(madeElement);
                    //selectedgroup.drawnElements.Add(madeElement);

                }
                selectedgroup.movedGroups.Add(selectedgroup);
            }
            if (selectedgroup.addedGroups.Count() > 0)
            {
                selectedgroup.SubMoving(invoker, selectedgroup, leftOffset, topOffset, paintSurface);
            }
        }

        //
        //undo redo move resize
        //

        //undo moving or resizing
        public void Undo(Invoker invoker, Canvas paintSurface)
        {
            Group selectedgroup = invoker.removedGroups.Last();
            invoker.movedGroups.RemoveAt(invoker.movedGroups.Count() - 1);
            invoker.removedGroups.Add(selectedgroup);
            invoker.selectedGroups.Add(selectedgroup); //re add to selected

            if (selectedgroup.drawnElements.Count() > 0)
            {
                foreach (FrameworkElement movedElement in selectedgroup.drawnElements)
                {
                    invoker.drawnElements.RemoveAt(invoker.drawnElements.Count() - 1);
                    //invoker.removedElements.Add(movedElement);
                }
            }
            if (selectedgroup.movedGroups.Count() > 0)
            {
                foreach (Group subgroup in selectedgroup.movedGroups)
                {
                    selectedgroup.SubUndo(subgroup, invoker);
                }
            }
            if (selectedgroup.drawnElements.Count() > 0)
            {
                foreach (FrameworkElement removedElement in selectedgroup.drawnElements)
                {
                    //invoker.movedElements.RemoveAt(invoker.movedElements.Count() - 1);
                    invoker.drawnElements.Add(removedElement);
                }
            }
            Repaint(invoker, paintSurface); //repaint   
        }

        public void SubUndo(Group selectedgroup, Invoker invoker)
        {
            if (selectedgroup.drawnElements.Count() > 0)
            {
                foreach (FrameworkElement movedElement in selectedgroup.drawnElements)
                {
                    invoker.drawnElements.RemoveAt(invoker.drawnElements.Count() - 1);
                    //invoker.removedElements.Add(movedElement);
                }
            }
            if (selectedgroup.movedGroups.Count() > 0)
            {
                foreach (Group subgroup in selectedgroup.movedGroups)
                {
                    selectedgroup.SubUndo(subgroup, invoker);
                }
            }
            if (selectedgroup.drawnElements.Count() > 0)
            {
                foreach (FrameworkElement removedElement in selectedgroup.drawnElements)
                {
                    //invoker.movedElements.RemoveAt(invoker.movedElements.Count() - 1);
                    invoker.drawnElements.Add(removedElement);
                }
            }
        }

        //redo moving or resizing
        public void Redo(Invoker invoker, Canvas paintSurface)
        {
            Group selectedgroup = invoker.removedGroups.Last();
            invoker.removedGroups.RemoveAt(invoker.removedGroups.Count() - 1);
            invoker.movedGroups.Add(selectedgroup);
            invoker.selectedGroups.RemoveAt(invoker.selectedGroups.Count() - 1); //remove selected

            if (selectedgroup.movedElements.Count() > 0)
            {
                foreach (FrameworkElement movedElement in selectedgroup.movedElements)
                {
                    invoker.drawnElements.RemoveAt(invoker.drawnElements.Count() - 1);
                    //invoker.removedElements.Add(movedElement);
                    //invoker.movedElements.Add(movedElement);              
                }
            }
            if (selectedgroup.addedGroups.Count() > 0)
            {
                foreach (Group subgroup in selectedgroup.addedGroups)
                {
                    selectedgroup.SubRedo(subgroup, invoker);
                }
            }
            if (selectedgroup.movedElements.Count() > 0)
            {
                foreach (FrameworkElement removedElement in selectedgroup.movedElements)
                //foreach (FrameworkElement removedElement in selectedgroup.removedElements)
                {
                    //invoker.removedElements.RemoveAt(invoker.removedElements.Count() - 1);
                    invoker.drawnElements.Add(removedElement);
                    //invoker.movedElements.Add(removedElement);
                }
            }
            Repaint(invoker, paintSurface); //repaint   
        }

        public void SubRedo(Group selectedgroup, Invoker invoker)
        {
            if (selectedgroup.movedElements.Count() > 0)
            {
                foreach (FrameworkElement movedElement in selectedgroup.movedElements)
                {
                    invoker.drawnElements.RemoveAt(invoker.drawnElements.Count() - 1);
                    //invoker.removedElements.Add(movedElement);
                    //invoker.movedElements.Add(movedElement);
                }
            }
            if (selectedgroup.addedGroups.Count() > 0)
            {
                foreach (Group subgroup in selectedgroup.addedGroups)
                {
                    selectedgroup.SubRedo(subgroup, invoker);
                }
            }
            if (selectedgroup.movedElements.Count() > 0)
            {
                foreach (FrameworkElement removedElement in selectedgroup.movedElements)
                //foreach (FrameworkElement removedElement in selectedgroup.removedElements)
                {
                    //invoker.removedElements.RemoveAt(invoker.removedElements.Count() - 1);
                    invoker.drawnElements.Add(removedElement);
                    //invoker.movedElements.Add(removedElement);
                }
            }
        }

        //
        //resizing
        //

        //resize
        public void Resize(Invoker invoker, PointerRoutedEventArgs e, Canvas paintSurface, FrameworkElement selectedelement)
        {
            //FrameworkElement selectedelement = invoker.selectElementsList.Last();
            Group selectedgroup = invoker.selectedGroups.Last();
            invoker.removedGroups.Add(selectedgroup);
            //calculate difference in size
            //double newWidth = ReturnSmallest(e.GetCurrentPoint(paintSurface).Position.X, Convert.ToDouble(element.ActualOffset.X));
            //double newHeight = ReturnSmallest(e.GetCurrentPoint(paintSurface).Position.Y, Convert.ToDouble(element.ActualOffset.Y));
            double newWidth = ReturnSmallest(e.GetCurrentPoint(paintSurface).Position.X, Convert.ToDouble(selectedelement.ActualOffset.X));
            double newHeight = ReturnSmallest(e.GetCurrentPoint(paintSurface).Position.Y, Convert.ToDouble(selectedelement.ActualOffset.Y));
            double widthOffset = selectedelement.Width - newWidth;
            double heightOffset = selectedelement.Height - newHeight;
            //elements in group
            if (selectedgroup.drawnElements.Count() > 0)
            {
                foreach (FrameworkElement movedElement in selectedgroup.drawnElements)
                {
                    Location location = new Location();
                    location.x = Convert.ToDouble(movedElement.ActualOffset.X);
                    location.y = Convert.ToDouble(movedElement.ActualOffset.Y);
                    location.width = Convert.ToDouble(movedElement.Width) - widthOffset;
                    location.height = Convert.ToDouble(movedElement.Height) - heightOffset;
                    invoker.executer++; //acceskey add
                    FrameworkElement madeElement = ResizingElement(movedElement, invoker, paintSurface, location);
                    selectedgroup.movedElements.Add(madeElement);
                    //this.drawnElements.RemoveAt(this.drawnElements.Count() -1);
                    //selectedgroup.drawnElements.Add(madeElement);
                }
                //selectedgroup.drawnElements.Clear();
            }
            //if (selectedgroup.movedElements.Count() > 0)
            //{
            //    foreach (FrameworkElement drewElement in selectedgroup.movedElements)
            //    {
            //        selectedgroup.drawnElements.Add(drewElement);
            //    }
            //}

            //remove selected element
            //invoker.unselectElementsList.Add(selectedelement);
            //invoker.selectElementsList.RemoveAt(invoker.selectElementsList.Count() - 1);

            if (selectedgroup.addedGroups.Count() > 0)
            {
                foreach (Group subgroup in selectedgroup.addedGroups)
                {
                    subgroup.SubResize(invoker, selectedgroup, widthOffset, heightOffset, paintSurface); //subgroups
                }
            }
            //add to moved or resized
            invoker.movedGroups.Add(selectedgroup);
            //remove selected group
            invoker.unselectedGroups.Add(selectedgroup);
            invoker.selectedGroups.RemoveAt(invoker.selectedGroups.Count() - 1);
            Repaint(invoker, paintSurface);//repaint
        }

        //recursively resize subgroups
        public void SubResize(Invoker invoker, Group selectedgroup, double widthOffset, double heightOffset, Canvas paintSurface)
        {
            if (selectedgroup.drawnElements.Count() > 0)
            {
                foreach (FrameworkElement movedElement in selectedgroup.drawnElements)
                {
                    Location location = new Location();
                    location.x = Convert.ToDouble(movedElement.ActualOffset.X);
                    location.y = Convert.ToDouble(movedElement.ActualOffset.Y);
                    location.width = Convert.ToDouble(movedElement.Width) - widthOffset;
                    location.height = Convert.ToDouble(movedElement.Height) - heightOffset;
                    invoker.executer++; //acceskey add
                    FrameworkElement madeElement = ResizingElement(movedElement, invoker, paintSurface, location);
                    selectedgroup.movedElements.Add(madeElement);
                    //this.drawnElements.RemoveAt(this.drawnElements.Count() -1);
                    //selectedgroup.drawnElements.Add(madeElement);
                }
                selectedgroup.movedGroups.Add(selectedgroup);
            }
            if (selectedgroup.addedGroups.Count() > 0)
            {
                selectedgroup.SubResize(invoker, selectedgroup, widthOffset, heightOffset, paintSurface);
            }
        }



        //repaint
        public void Repaint(Invoker invoker, Canvas paintSurface)
        {
            paintSurface.Children.Clear();
            foreach (FrameworkElement drawelement in invoker.drawnElements)
            {
                paintSurface.Children.Add(drawelement); //add
            }
        }

        //check if element is already in group
        public int CheckInGroup(Invoker invoker, FrameworkElement element)
        {
            int counter = 0;
            foreach (Group group in invoker.drawnGroups)
            {
                if (group.drawnElements.Count() > 0)
                {
                    foreach (FrameworkElement groupelement in group.drawnElements)
                    {
                        if (groupelement.AccessKey == element.AccessKey)
                        {
                            counter++;
                        }
                    }
                }
                CheckInSubgroup(group, element.AccessKey);
            }
            return counter;
        }

        public int CheckInSubgroup(Group group, string key)
        {
            int counter = 0;
            if (group.drawnElements.Count() > 0)
            {
                foreach (FrameworkElement groupelement in group.drawnElements)
                {
                    if (groupelement.AccessKey == element.AccessKey)
                    {
                        counter++;
                    }
                }
            }
            if (group.addedGroups.Count() > 0)
            {
                foreach (Group subgroup in group.addedGroups)
                {
                    counter = subgroup.CheckInSubgroup(subgroup, key);
                }
            }
            return counter;
        }

        //see if element is in group and select the group
        public void SelectInGroup(FrameworkElement selectedElement, Invoker invoker)
        {
            string key = selectedElement.AccessKey;
            if (invoker.drawnGroups.Count() > 0)
            {
                foreach (Group group in invoker.drawnGroups)
                {
                    if (group.drawnElements.Count() > 0)
                    {
                        foreach (FrameworkElement drawn in group.drawnElements)
                        {
                            if (drawn.AccessKey == key)
                            {
                                invoker.selectedGroups.Add(group);
                                ////remove selected element from list if in group
                                //if (invoker.selectElementsList.Count() >0)
                                //{
                                //    invoker.selectElementsList.RemoveAt(invoker.selectElementsList.Count() - 1);
                                //}                         
                            }
                        }
                    }
                    SelectInGroupHandle(invoker, key, group); //subgroup recursive
                }
            }
        }

        //recursively see if element is in subgroup and select the group
        public void SelectInGroupHandle(Invoker invoker, string key, Group group)
        {
            if (group.addedGroups.Count() > 0)
            {
                foreach (Group subgroup in group.addedGroups)
                {
                    subgroup.SelectInGroupHandle(invoker, key, group);
                    foreach (FrameworkElement drawn in subgroup.drawnElements)
                    {
                        if (drawn.AccessKey == key)
                        {
                            invoker.selectedGroups.Add(group);
                        }
                    }
                }
            }
        }

        //set group unselected
        public void UnselectGroup(FrameworkElement selectedElement, Invoker invoker)
        {
            string key = selectedElement.AccessKey;
            if (invoker.drawnGroups.Count() > 0)
            {
                foreach (Group group in invoker.drawnGroups)
                {
                    if (group.drawnElements.Count() > 0)
                    {
                        foreach (FrameworkElement drawn in group.drawnElements)
                        {
                            if (drawn.AccessKey == key)
                            {
                                invoker.selectedGroups.RemoveAt(invoker.selectedGroups.Count() - 1);
                            }
                        }
                    }
                    UnselectGroupHandle(invoker, key, group); //subgroup recursive
                }
            }
        }

        //set subgroup unselected
        public void UnselectGroupHandle(Invoker invoker, string key, Group group)
        {
            if (group.addedGroups.Count() > 0)
            {
                foreach (Group subgroup in group.addedGroups)
                {
                    subgroup.SelectInGroupHandle(invoker, key, group);
                    foreach (FrameworkElement drawn in subgroup.drawnElements)
                    {
                        if (drawn.AccessKey == key)
                        {
                            invoker.selectedGroups.RemoveAt(invoker.selectedGroups.Count() - 1);
                        }
                    }
                }
            }
        }

        //remove selected element by access key
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

        //give smallest number
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

        //moving element in group
        public FrameworkElement MovingElement(FrameworkElement element, Invoker invoker, Canvas paintSurface, Location location)
        {
            FrameworkElement returnelement = null;
            KeyNumber(element, invoker); //move selected at removed
            //create at new location
            if (element.Name == "Rectangle")
            {
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
            else if (element.Name == "Ellipse")
            {
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
            return returnelement;
        }

        //resizing element in group
        public FrameworkElement ResizingElement(FrameworkElement element, Invoker invoker, Canvas paintSurface, Location location)
        {
            FrameworkElement returnelement = null;
            KeyNumber(element, invoker); //move selected at removed
            //create at new size
            if (element.Name == "Rectangle")
            {
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
            else if (element.Name == "Ellipse")
            {
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
            return returnelement;
        }

        //display lines for saving
        public override string Display(int depth, Group group)
        {
            //Display group.
            string str = "";
            //Add group.
            int i = 0;
            while (i < depth)
            {
                str += "\t";
            }
            int groupcount = group.drawnElements.Count() + group.addedGroups.Count();
            str = str + "group " + groupcount + "\n";

            //Recursively display child nodes.
            depth = depth + 1; //add depth tab
            if (group.drawnElements.Count() > 0)
            {
                foreach (FrameworkElement child in group.drawnElements)
                {
                    if (child is Rectangle)
                    {
                        int j = 0;
                        while (j < depth)
                        {
                            str += "\t";
                            j++;
                        }
                        str = str + "rectangle " + child.ActualOffset.X + " " + child.ActualOffset.Y + " " + child.Width + " " + child.Height + "\n";
                    }
                    //else if (child is Ellipse)
                    else
                    {
                        int j = 0;
                        while (j < depth)
                        {
                            str += "\t";
                            j++;
                        }
                        str = str + "ellipse " + child.ActualOffset.X + " " + child.ActualOffset.Y + " " + child.Width + " " + child.Height + "\n";
                    }
                }
            }
            if (group.addedGroups.Count() > 0)
            {
                foreach (Group subgroup in group.addedGroups)
                {
                    subgroup.Display(depth + 1, subgroup);
                }
            }
            return str;
        }


        public void LoadGroup(Group grouping, Canvas paintSurface, Invoker invoker, int linenumber, int start, int stop, string text)
        {
            string[] readText = Regex.Split(text, "\\n+");
            FrameworkElement elm = null;
            int i = start;
            while (i < stop)
            {
                string s = readText[i];
                string[] line = Regex.Split(s, "\\s+");
                if (line[0] == "ellipse")
                {
                    i++;
                    GetEllipse(s, paintSurface, invoker);
                    elm = invoker.drawnElements.Last();
                    grouping.drawnElements.Add(elm);
                }
                else if (line[0] == "rectangle")
                {
                    i++;
                    GetRectangle(s, paintSurface, invoker);
                    elm = invoker.drawnElements.Last();
                    grouping.drawnElements.Add(elm);
                }
                else if (line[0] == "group")
                {
                    i++;
                    invoker.executer++;
                    Group subgrouping = new Group(0, 0, 0, 0, "group", 0, invoker.executer, paintSurface, invoker, elm);
                    LoadGroup(subgrouping, paintSurface, invoker, Convert.ToInt32(line[1]), i, i + Convert.ToInt32(line[1]), text);
                    grouping.addedGroups.Add(subgrouping);
                }
            }
            invoker.drawnGroups.Add(grouping);
        }

        //load ellipse
        public void GetEllipse(String lines, Canvas paintSurface, Invoker invoker)
        {
            string[] line = Regex.Split(lines, "\\s+");

            double x = Convert.ToDouble(line[1]);
            double y = Convert.ToDouble(line[2]);
            double width = Convert.ToDouble(line[3]);
            double height = Convert.ToDouble(line[4]);

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
            paintSurface.Children.Add(newEllipse);
            invoker.drawnElements.Add(newEllipse);
        }

        //load rectangle
        public void GetRectangle(String lines, Canvas paintSurface, Invoker invoker)
        {
            string[] line = Regex.Split(lines, "\\s+");

            double x = Convert.ToDouble(line[1]);
            double y = Convert.ToDouble(line[2]);
            double width = Convert.ToDouble(line[3]);
            double height = Convert.ToDouble(line[4]);

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
            paintSurface.Children.Add(newRectangle);
            invoker.drawnElements.Add(newRectangle);
        }



    }
}
