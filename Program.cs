using System;
using System.Drawing;
using System.Windows.Forms;

using static System.Console;
using static System.Math;

enum Tool {
    Line, Rectangle, Circle 
};

class MyForm : Form {
    Bitmap image;
    Tool tool = Tool.Line;
    bool mouseDown;
    Point start, end;
    
    public MyForm() {
        ClientSize = new Size(500, 500);
        StartPosition = FormStartPosition.CenterScreen;
        image = new Bitmap(Size.Width, Size.Height);
        
        // menu
        
        ToolStripMenuItem[] fileItems = {
            new ToolStripMenuItem("Quit", null, onQuit)
        };
        
        ToolStripMenuItem[] topItems = {
            new ToolStripMenuItem("File", null, fileItems)
        };
        
        MenuStrip menuStrip = new MenuStrip();
        foreach (var item in topItems)
            menuStrip.Items.Add(item);
        
        // toolbar
        
        Bitmap lineImage = new Bitmap(16, 16);
        using (Graphics g = Graphics.FromImage(lineImage))
            g.DrawLine(Pens.Black, 2, 14, 14, 2);
        
        Bitmap rectImage = new Bitmap(16, 16);
        using (Graphics g = Graphics.FromImage(rectImage))
            g.DrawRectangle(Pens.Black, 2, 2, 12, 12);
        
        Bitmap circleImage = new Bitmap (16, 16);
        using (Graphics g=Graphics.FromImage(circleImage))
            g.DrawEllipse(Pens.Black, 2, 14, 14, 2); 

        ToolStripButton[] buttons = {
            new ToolStripButton(null, lineImage, onLine),
            new ToolStripButton(null, rectImage, onRectangle),
            new ToolStripButton(null, circleImage, onCircle)
        };
        
        ToolStrip toolStrip = new ToolStrip();
        foreach (ToolStripButton button in buttons)
            toolStrip.Items.Add(button);
        
        Controls.Add(toolStrip);
        Controls.Add(menuStrip);
    }
    
    void onQuit(object sender, EventArgs e) {
        Application.Exit();
    }
    
    void onLine(object sender, EventArgs e) {
        tool = Tool.Line;
    }
    
    void onRectangle(object sender, EventArgs e) {
        tool = Tool.Rectangle;
    }
    
    void onCircle(object sender, EventArgs e) {
        tool = Tool.Circle;
    }
    
    void draw(Graphics g) {
        switch (tool) {
            case Tool.Line:
                g.DrawLine(Pens.Black, start, end);
                break;
            case Tool.Rectangle:
                int x = Min(start.X, end.X), y = Min(start.Y, end.Y);
                g.DrawRectangle(Pens.Black, x, y, Abs(end.X - start.X), Abs(end.Y - start.Y));
                break;
        }
    }
    
    protected override void OnMouseDown(MouseEventArgs args) {
        mouseDown = true;
        start = end = args.Location;
    }
    
    protected override void OnMouseMove(MouseEventArgs args) {
        if (mouseDown) {
            end = args.Location;
            Invalidate();
        }
    }
    
    protected override void OnMouseUp(MouseEventArgs args) {
        mouseDown = false;
        using (Graphics g = Graphics.FromImage(image))
            draw(g);
    }
    
    protected override void OnPaint(PaintEventArgs args) {
        Graphics g = args.Graphics;
        g.DrawImage(image, 0, 0);
        if (mouseDown)
            draw(g);
    }
}

class Hello {
    [STAThread]
    static void Main() {
        Form form = new MyForm();
        
        Application.Run(form);
    }
}
