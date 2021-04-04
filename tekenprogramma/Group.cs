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
        //public double height;
        //public double width;
        //public double x;
        //public double y;
        public string type;
        public int depth;
        public int id;
        //public List<Baseshape> groupitems;
        public Canvas paintSurface;

        public List<Baseshape> children = new List<Baseshape>();

        public Invoker invoker;
        public FrameworkElement element;

        public Group(double height, double width, double x, double y, string type, int depth, int id, Canvas paintSurface, Invoker invoker, FrameworkElement element) : base(height, width, x, y)
        {
            this.height = height;
            this.width = width;
            this.x = x;
            this.y = y;
            this.type = type;
            this.depth = depth;
            this.id = id;
            this.paintSurface = paintSurface;
            this.invoker = invoker;
            this.element = element;
        }

        public void add(Baseshape newgroup)
        {
            children.Add(newgroup);
        }

        public void remove(Baseshape newgroup)
        {
            children.Remove(newgroup);
        }

        public List<Baseshape> getGroup()
        {
            return children;
        }

        public override string display(int depth)
        {
            //Display group.
            string str = "";
            //Add group.
            int i = 0;
            while (i < depth)
            {
                str += "\t";
            }
            str = str + "group" + id + "\n";

            //Recursively display child nodes.
            depth = depth + 1;
            foreach (Baseshape child in children)
            {
                //child.Display(depth + 1);

                if (child is Rectangle)
                {
                    int j = 0;
                    while (j < depth)
                    {
                        str += "\t";
                    }
                    str = str + "rectangle " + child.x + " " + child.y + " " + child.width + " " + child.height + "\n";
                }
                else if (child is Ellipse)
                {

                    int j = 0;
                    while (j < depth)
                    {
                        str += "\t";
                    }
                    str = str + "ellipse " + child.x + " " + child.y + " " + child.width + " " + child.height + "\n";
                }
                else
                {
                    child.display(depth + 1);
                }

            }
            return str;

        }

        //moving
        public override void moving(Location location)
        {
            bool s = false;
            foreach (Baseshape shape in this.children)
            {
                if (shape.selected)
                {
                    Location childLocation = new Location(location.x, location.y, shape.width, shape.height);
                    shape.moving(childLocation);
                    s = true;
                }
            }

            if (s == false)
            {
                foreach (Baseshape shape in this.children)
                {
                    double dx = location.x - this.start.x + shape.start.x;
                    double dy = location.y - this.start.y + shape.start.y;

                    Location childLocation = new Location();
                    childLocation.x = dx;
                    childLocation.y = dy;
                    childLocation.width = shape.width;
                    childLocation.height = shape.height;

                    shape.moving(childLocation);

                }

            }
        }

        //undo moving
        public override void undoMoving()
        {
            Location location = this.undoList.Last();
            this.redoList.Add(location);
            this.x = location.x;
            this.y = location.y;
            this.width = location.width;
            this.height = location.height;
        }

        //redo moving
        public override void redoMoving()
        {
            if (this.redoList.Count() > 0)
            {
                Location location = this.redoList.Last();
                this.undoList.Add(location);
                this.x = location.x;
                this.y = location.y;
                this.width = location.width;
                this.height = location.height;
            }
        }

        //resize
        public override void resize(Location location)
        {
            bool s = false;
            foreach (Baseshape shape in this.children)
            {
                if (shape.selected)
                {
                    s = true;
                    Location childLocation = new Location(shape.x, shape.y, location.width - (shape.start.x - location.x), location.height - (shape.start.y - location.y));
                    shape.resize(childLocation);
                }
            }

            if (s == false)
            {
                float percentageWidth = (float)location.width / (float)this.start.width;
                float percentageHeight = (float)location.height / (float)this.start.height;

                foreach (Baseshape shape in this.children)
                {
                    float diffX = ((float)shape.start.x - (float)this.x) * percentageWidth;
                    float diffY = ((float)shape.start.y - (float)this.y) * percentageHeight;

                    Location childLocation = new Location();
                    childLocation.x = this.start.x + Math.Round(diffX);
                    childLocation.y = this.start.y + Math.Round(diffY);
                    childLocation.width = Math.Round((float)shape.start.width * percentageWidth);
                    childLocation.height = Math.Round((float)shape.start.height * percentageHeight);
                    shape.resize(childLocation);
                }
            }
        }

        //undo resize
        public override void undoResize()
        {
            Location location = this.undoList.Last();
            this.redoList.Add(location);
            this.x = location.x;
            this.y = location.y;
            this.width = location.width;
            this.height = location.height;
        }

        //redo resize
        public override void redoResize()
        {
            if (this.redoList.Count() > 0)
            {
                Location location = this.redoList.Last();
                this.undoList.Add(location);
                this.x = location.x;
                this.y = location.y;
                this.width = location.width;
                this.height = location.height;
            }
        }


        //select group
        public override void select(PointerRoutedEventArgs e, Canvas paintSurface)
        {
            foreach (Baseshape baseshape in this.children)
            {
                Shape shape = new Shape(baseshape.x, baseshape.y, baseshape.width, baseshape.height);

                if (this.selected)
                {
                    if (baseshape.getIfSelected(e.GetCurrentPoint(paintSurface).Position.X, e.GetCurrentPoint(paintSurface).Position.Y))
                    {
                        ICommand select = new Select(shape, e);
                        this.invoker.Execute(select);
                    }
                    else
                    {
                        ICommand deselect = new Deselect(shape, e);
                        this.invoker.Execute(deselect);
                    }

                    if (baseshape.getHandleIfSelected(e.GetCurrentPoint(paintSurface).Position.X, e.GetCurrentPoint(paintSurface).Position.Y))
                    {
                        ICommand resize = new Resize(shape, e, paintSurface, this.invoker, this.element);
                        this.invoker.Execute(resize);
                    }
                }
            }
            this.selected = true;
        }

        //deselect group
        public override void deselect(PointerRoutedEventArgs e)
        {
            foreach (Baseshape baseshape in this.children)
            {
                Shape shape = new Shape(baseshape.x, baseshape.y, baseshape.width, baseshape.height);
                if (baseshape.selected)
                {
                    ICommand deselect = new Deselect(shape, e);
                    this.invoker.Execute(deselect);
                }
            }
            this.selected = false;
        }




        //if selected
        public override bool getIfSelected(double x, double y)
        {
            foreach (Baseshape shape in this.children)
            {
                if (shape.getIfSelected(x, y))
                {
                    return true;
                }
            }
            return false;
        }

        public override bool getHandleIfSelected(double x, double y)
        {
            bool s = false;
            foreach (Baseshape shape in this.children)
            {
                if (shape.getHandleIfSelected(x, y))
                {
                    s = true;
                    return true;
                }
            }

            if (s == false)
            {
                for (double i = this.x + this.width - 6; i < this.x + this.width + 6; i++)
                {
                    for (double j = this.y + this.height - 6; j < this.y + this.height + 6; j++)
                    {
                        if (i == x)
                        {
                            if (j == y)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
