using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace tekenprogramma
{
    //class invoker
    public class Invoker
    {
        //actions
        public List<ICommand> actionsList = new List<ICommand>();
        public List<ICommand> redoList = new List<ICommand>();
        //elements
        //state 0, remove draw
        public List<List<FrameworkElement>> undoElementsList = new List<List<FrameworkElement>>(); //4a keep track of undone
        public List<List<FrameworkElement>> redoElementsList = new List<List<FrameworkElement>>(); //4b keep track of redone
        //state 2, unselect, select
        public List<List<FrameworkElement>> unselectElementsList = new List<List<FrameworkElement>>(); //4a keep track of undone
        public List<List<FrameworkElement>> reselectElementsList = new List<List<FrameworkElement>>(); //4b keep track of redone
        //groups
        //state 0  
        public List<List<Group>> undoGroupsList = new List<List<Group>>(); //4a keep track of undone
        public List<List<Group>> redoGroupsList = new List<List<Group>>(); //4b keep track of redone
        //state 2
        public List<List<Group>> unselectGroupsList = new List<List<Group>>(); //4a keep track of undone
        public List<List<Group>> reselectGroupsList = new List<List<Group>>(); //4b keep track of redone
        //counting numbers for execution and elements
        public int counter = 0;
        public int executer = 0;

        //components
        public List<IComponent> drawnComponents = new List<IComponent>();
        public List<IComponent> removedComponents = new List<IComponent>();
        public List<IComponent> movedComponents = new List<IComponent>();
        public List<IComponent> unmovedComponents = new List<IComponent>();
        public List<IComponent> selectComponentsList = new List<IComponent>();
        public List<IComponent> unselectComponentsList = new List<IComponent>();
        public List<IComponent> undoComponents = new List<IComponent>();
        public List<IComponent> redoComponents = new List<IComponent>();

        public Invoker()
        {
            this.actionsList = new List<ICommand>();
            this.redoList = new List<ICommand>();
        }

        //execute
        public void Execute(ICommand cmd)
        {
            actionsList.Add(cmd);
            redoList.Clear();
            cmd.Execute();
            counter++;
            executer++;
        }

        //undo
        public void Undo()
        {
            if (actionsList.Count >= 1)
            {
                ICommand cmd = actionsList.Last();
                actionsList.RemoveAt(actionsList.Count - 1);
                redoList.Add(cmd);
                cmd.Undo();
                counter--;
            }
        }

        //redo
        public void Redo()
        {
            if (redoList.Count >= 1)
            {
                ICommand cmd = redoList.Last();
                actionsList.Add(cmd);
                redoList.RemoveAt(redoList.Count - 1);
                cmd.Redo();
                counter++;
            }
        }
    }
}
