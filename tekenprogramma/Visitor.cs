using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Input;
using System.Collections.Generic;

namespace tekenprogramma
{

    public interface IVisitor
    {
        void visitRectangle(Rectangle rectangle);
        void visitEllipse(Ellipse ellipse);
    }

    public class visitResize : IVisitor
    {

      public void visitRectangle(Rectangle rectangle){
            rectangle.addMouseListener(new MouseAdapter(){
                  public void mouseReleased(RoutedEventArgs e){
                        /*
                              Resizes the rectangle when the resizing variable is set to true
                        */
                        if (rectangle.resizing) {
                              Location location = new Location(rectangle.getX(), rectangle.getY(), rectangle.getWidth(), rectangle.getHeight());
                              Order drag = new ResizeShapeCommand(rectangle, location);
                              rectangle.invoker.execute(drag);
                              rectangle.resizing = false;
                        }
                  }
            });

            rectangle.addMouseMotionListener(new MouseAdapter(){
                  public void mouseDragged(RoutedEventArgs e){
                        /*
                              Drags the shape when the cursor is the default cursor
                        */
                        if (rectangle.start != null) {
                              if(rectangle.cursor == Cursor.SE_RESIZE_CURSOR){
                                    rectangle.resizing = true;
                                    rectangle.dragging = false;
      
                                    rectangle.setBounds(rectangle.getX(), rectangle.getY(), rectangle.getWidth() + e.getX() - rectangle.start.x, rectangle.getHeight() + e.getY() - rectangle.start.y);
                                    rectangle.start = e.getPoint();
                                    rectangle.width = rectangle.getWidth();
                                    rectangle.height = rectangle.getHeight();
                                    rectangle.repaint();
                              }
                        }
                  }
            });   
      }

      public void visitEllipse(Ellipse ellipse){
            ellipse.addMouseListener(new MouseAdapter(){
                  public void mouseReleased(RoutedEventArgs e){
                        /*
                              Resizes the shape when resize variable is set to true
                        */
                        if(ellipse.resizing){
                              Location location = new Location(ellipse.getX(), ellipse.getY(), ellipse.getWidth(), ellipse.getHeight());
                              Order drag = new ResizeShapeCommand(ellipse, location);
                              ellipse.invoker.execute(drag);
                              ellipse.resizing = false;
                        }
                  }
            });

            ellipse.addMouseMotionListener(new MouseAdapter(){
                  public void mouseDragged(RoutedEventArgs e){
                        /*
                              Drags the shape when the cursor is the default cursor
                        */
                        if (ellipse.start != null) {
                              if(ellipse.cursor == Cursor.SE_RESIZE_CURSOR){
                                    ellipse.resizing = true;
                                    ellipse.dragging = false;
      
                                    ellipse.setBounds(ellipse.getX(), ellipse.getY(), ellipse.getWidth() + e.getX() - ellipse.start.x, ellipse.getHeight() + e.getY() - ellipse.start.y);
                                    ellipse.start = e.getPoint();
                                    ellipse.width = ellipse.getWidth();
                                    ellipse.height = ellipse.getHeight();
                                    ellipse.repaint();
                              }
                        }
                  }
            });   
      }
    }

    public class visitMove : IVisitor
    {

      public void visitRectangle(Rectangle rectangle){
            rectangle.addMouseListener(new MouseAdapter(){
                  public void mouseReleased(RoutedEventArgs e){
                        /*
                              * Pollution of the undo/redo history is prevented by only adding
                              * DragShapeCommand when the shape has been dragged
                        */
                        if (rectangle.dragging) {
                              Location location = new Location(rectangle.getX(), rectangle.getY(), rectangle.getWidth(), rectangle.getHeight());

                              Order drag = new DragShapeCommand(rectangle, location);
                              rectangle.invoker.execute(drag);
                              rectangle.dragging = false;
                        }
                  }

                  public void mousePressed(RoutedEventArgs e){
                        /*
                              Executes a drag command to note the initial position
                        */
                        if(rectangle.undoStack.size() == 0){
                              Location location = new Location(rectangle.getX(), rectangle.getY(), rectangle.getWidth(), rectangle.getHeight());
                              Order drag = new DragShapeCommand(rectangle, location);
                              rectangle.invoker.execute(drag);
                        }
                  }
            });

            rectangle.addMouseMotionListener(new MouseAdapter(){
                  public void mouseDragged(RoutedEventArgs e){
                        if (rectangle.start != null) {
                              if(rectangle.cursor == Cursor.MOVE_CURSOR){
                                    rectangle.resizing = false;  
                                    rectangle.dragging = true;      

                                    var bounds = rectangle.getBounds();
                                    bounds.translate(e.getX() - rectangle.start.x, e.getY() - rectangle.start.y);
                                    rectangle.setBounds(bounds);
                                    rectangle.width = rectangle.getWidth();
                                    rectangle.height = rectangle.getHeight();
                                    rectangle.repaint();
                              }
                        }
                  }
            });
      }

      public void visitEllipse(Ellipse ellipse){
            ellipse.addMouseListener(new MouseAdapter(){
                  public void mouseReleased(RoutedEventArgs e){
                        /*
                              Pollution of the undo/redo history is prevented by only adding 
                              DragShapeCommand when the shape has been dragged
                        */
                        if(ellipse.dragging){
                              Location location = new Location(ellipse.getX(), ellipse.getY(), ellipse.getWidth(), ellipse.getHeight());
                              Order drag = new DragShapeCommand(ellipse, location);
                              ellipse.invoker.execute(drag);
                              ellipse.dragging = false;
                        }  
                  }

                  public void mousePressed(RoutedEventArgs e){
                        /*
                              Executes a drag command to note the initial position
                        */
                        if(ellipse.undoStack.size() == 0){
                              Location location = new Location(ellipse.getX(), ellipse.getY(), ellipse.getWidth(), ellipse.getHeight());
                              Order drag = new DragShapeCommand(ellipse, location);
                              ellipse.invoker.execute(drag);
                        }
                  }
            });

            ellipse.addMouseMotionListener(new MouseAdapter(){
                  /*
                        Drags the shape when the cursor is the default cursor
                  */
                  public void mouseDragged(RoutedEventArgs e){
                        if (ellipse.start != null) {
                              if(ellipse.cursor == Cursor.MOVE_CURSOR){
                                    ellipse.resizing = false;  
                                    ellipse.dragging = true;      

                                    var bounds = ellipse.getBounds();
                                    bounds.translate(e.getX() - ellipse.start.x, e.getY() - ellipse.start.y);
                                    ellipse.setBounds(bounds);
                                    ellipse.width = ellipse.getWidth();
                                    ellipse.height = ellipse.getHeight();
                                    ellipse.repaint();
                              }
                        }
                  }
            });
      }
}
