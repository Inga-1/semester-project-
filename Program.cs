using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms; 
using System.Drawing.Imaging;
using System.IO;

using static System.Console;
using static System.Math;

enum Tool {
    Color, Line, Rectangle, Circle, FilledRect, FilledCircle, FilledColor,
};

class MyForm : Form {
    public Bitmap image;
    Tool tool = Tool.Line;
    bool mouseDown;
    Point start, end;
    public Pen pen1 = Pens.Black;
    public Color backgroundColor = Color.White;
    bool restarted = false; 
    string filename = null; 
    SaveFileDialog save = new SaveFileDialog();
    public SolidBrush brush = new SolidBrush(Color.Black); 
    ToolStripButton colorButton, lineButton, rectButton, circleButton, filledRectButton, filledCircleButton, filledColorButton; 
    
    public MyForm() {
        ClientSize = new Size(500, 500);
        StartPosition = FormStartPosition.CenterScreen;
        image = new Bitmap(Size.Width, Size.Height);
         
        // menu
        
        ToolStripMenuItem[] fileItems = {
            new ToolStripMenuItem("Open", null, onOpen),
            new ToolStripMenuItem("Save", null, onSave),
            new ToolStripMenuItem("Quit", null, onQuit),
            new ToolStripMenuItem("Restart", null, onRestart)
        };

        ToolStripMenuItem[] colorItems = {
            new ToolStripMenuItem("Pen Color", null, onPenColor),
            new ToolStripMenuItem("Background Color", null, onBackColor), 
            new ToolStripMenuItem("Fill color", null, onFilled), 
        };


        ToolStripMenuItem[] topItems = {
            new ToolStripMenuItem("File", null, fileItems),
            new ToolStripMenuItem("Colors", null, colorItems), 
        };
        
        MenuStrip menuStrip = new MenuStrip();
        foreach (var item in topItems)
            menuStrip.Items.Add(item);
        
        // toolbar
        
        Bitmap colorImage = new Bitmap(16, 16);

        Bitmap lineImage = new Bitmap(16, 16);
        using (Graphics g = Graphics.FromImage(lineImage))
            {
                g.DrawLine(pen1, 2, 14, 14, 2);
            }
            
        Bitmap rectImage = new Bitmap(16, 16);
        using (Graphics g = Graphics.FromImage(rectImage)){
            g.DrawRectangle(pen1, 2, 2, 12, 12);
        }
            
        Bitmap circleImage = new Bitmap (16, 16);
        using (Graphics g = Graphics.FromImage(circleImage))
        {
            g.DrawEllipse(pen1, 2, 2, 12, 12); 
        }

        Bitmap filledRectImage = new Bitmap(16, 16);
        using (Graphics g = Graphics.FromImage(filledRectImage))
        {
            g.FillRectangle(brush, 2, 2, 12, 12);
        }
        
        Bitmap filledCircleImage = new Bitmap(16, 16);
        using (Graphics g = Graphics.FromImage(filledCircleImage))
        {
            g.FillEllipse(brush, 2, 2, 12, 12);
        }

        Bitmap filledColorImage = new Bitmap(16, 16);
            
        ToolStripButton makeButton(Image image, EventHandler handler) {
            ToolStripButton b = new ToolStripButton(null, image, handler);
            b.CheckOnClick = true;
            return b;
        }
        
        colorButton = makeButton(colorImage, onColor); 
        lineButton = makeButton(lineImage, onLine);
        rectButton = makeButton(rectImage, onRectangle);
        circleButton = makeButton(circleImage, onCircle);
        filledRectButton = makeButton(filledRectImage, onFilledRect); 
        filledCircleButton = makeButton(filledCircleImage, onFilledCircle);
        filledColorButton = makeButton(filledColorImage, onFilledColor);
        
        ToolStripButton[] buttons = { colorButton, lineButton, rectButton, circleButton, filledRectButton, filledCircleButton, filledColorButton};

        //toolStrips

        colorButton.AutoToolTip = false; 
        colorButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
        colorButton.ToolTipText = "Pen Color";

        filledColorButton.AutoToolTip = false;  
        filledColorButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
        filledColorButton.ToolTipText = "Fill Color";

        colorButton.BackColor = Color.Black;
        filledColorButton.BackColor = Color.Black;

        lineButton.Checked = true; 

        ToolStrip toolStrip = new ToolStrip();
        foreach (ToolStripButton button in buttons)
            toolStrip.Items.Add(button);
        
        Controls.AddRange(new Control[]{toolStrip});
        Controls.Add(menuStrip);
    }

    
    void onOpen (object sender, EventArgs e){
        _onOpen();
    }

    void onSave(object sender, EventArgs e){
        _onSave();    
    }
    
    void onQuit(object sender, EventArgs e) {
        _onQuit();
        
    }

    void onColor(object sender, EventArgs e){
        colorButton.Checked = false;
        _onPenColor();
    }

    void onLine(object sender, EventArgs e) {
        tool = Tool.Line;  
        rectButton.Checked = circleButton.Checked = filledRectButton.Checked = filledCircleButton.Checked =  false;
    }
    
    void onRectangle(object sender, EventArgs e) {
        tool = Tool.Rectangle;
        lineButton.Checked = circleButton.Checked = filledRectButton.Checked = filledCircleButton.Checked = false;
    }
    
    void onCircle(object sender, EventArgs e) {
        tool = Tool.Circle;
        rectButton.Checked = lineButton.Checked = filledRectButton.Checked = filledCircleButton.Checked = false;
    }
     void onFilledRect(object sender, EventArgs e){
        tool = Tool.FilledRect; 
        rectButton.Checked = lineButton.Checked = circleButton.Checked = filledCircleButton.Checked = false;
    }

    void onFilledCircle(object sender, EventArgs e){
        tool = Tool.FilledCircle;
        rectButton.Checked = lineButton.Checked = filledRectButton.Checked = circleButton.Checked = false;
    } 

    void onFilledColor(object sender, EventArgs e){
        filledColorButton.Checked = false; 
        _onfilled();        
    }

    void onPenColor(object sender, EventArgs e){
        _onPenColor(); 
    }

    void onBackColor(object sender, EventArgs e){
        ColorDialog backgroundDialog = new ColorDialog();
        Graphics g = Graphics.FromImage(image);
        _color(backgroundDialog);
        if (backgroundDialog.ShowDialog() == DialogResult.OK){ 
            BackColor = backgroundDialog.Color;
        }
        backgroundColor = backgroundDialog.Color;
        if (restarted){
            g.Clear(backgroundColor);
        }
    }

    void onFilled (object sender, EventArgs e){
        _onfilled();
    }

    void onRestart(object sender, EventArgs e){
        _onRestart();
    }

    public void ClearImage(){
        Graphics g = Graphics.FromImage(image);
        Color back = Color.White;
        pen1 = Pens.Black;
        BackColor = Color.White;
        g.Clear(back);
        Invalidate();

    }

    //helper functions 

    public void _onSave(){
        SaveFileDialog save = new SaveFileDialog();
        if (filename == null){
        save.DefaultExt = "png";
        if (save.ShowDialog() == DialogResult.Cancel){
            return; 
        }else if (save.ShowDialog() == DialogResult.OK){
            image.Save(save.FileName, ImageFormat.Png);
            filename = save.FileName; 
        }
        }else{
            image.Save(filename, ImageFormat.Png);
        }
        Text = string.Format(filename);
    }

    public void _onOpen(){
        OpenFileDialog openFile = new OpenFileDialog();
        if (openFile.ShowDialog() == DialogResult.OK){
            string name = openFile.FileName;
            image = new Bitmap(name); 
            filename = name; 
            Text = string.Format(openFile.FileName);
        }
        Invalidate();
    }
    public void _color(ColorDialog e){ 
        e.ShowHelp = true; //shows help button
        e.FullOpen = true; //allows the user to choose custom colors
    }

    public void _onPenColor(){
        ColorDialog penDialog= new ColorDialog();
        _color(penDialog);
        if (penDialog.ShowDialog() == DialogResult.OK){
            Pen pen = new Pen(penDialog.Color, 2);
            pen1 = pen; 
            colorButton.BackColor = pen1.Color; 
        }
    }

    public void _onfilled (){
        ColorDialog filling = new ColorDialog();
        _color(filling);
        if (filling.ShowDialog() == DialogResult.OK)
        {
            SolidBrush sol = new SolidBrush(filling.Color); 
            brush = sol; 
            filledColorButton.BackColor = brush.Color;
        }
    }

    public void _onRestart(){
        restarted = true; 
        DialogResult result = MessageBox.Show("Are you sure you want to delete the current Image?", "Restart", MessageBoxButtons.YesNoCancel);
        if (result == DialogResult.Yes){
            ClearImage();
        }
        if (result == DialogResult.No || result == DialogResult.Cancel){
            return;
        }
        colorButton.BackColor = Color.Black; 
        filledColorButton.BackColor = Color.Black;
        rectButton.Checked = circleButton.Checked = filledRectButton.Checked = filledCircleButton.Checked = lineButton.Checked = false;
        tool = Tool.Line;
        lineButton.Checked = true; 
        brush.Color = Color.Black; 
        filename = null;  
        Text = null; 
    }

    public void _onQuit(){
        DialogResult result1 = MessageBox.Show("Are you sure you want to quit?", "Quit", MessageBoxButtons.YesNo);
        if (result1 == DialogResult.Yes){
            Application.Exit();
        }
        else{
            return;
        }
    }

    //actual drawing process

    void draw(Graphics g) {
        g.SmoothingMode = SmoothingMode.AntiAlias; 
        switch (tool) {
            case Tool.Rectangle:
                int x = Min(start.X, end.X), y = Min(start.Y, end.Y);
                g.DrawRectangle(pen1, x, y, Abs(end.X - start.X), Abs(end.Y - start.Y));
                break; 
            case Tool.Circle:
                int x1=Min (start.X, end.X), y1= Min(start.Y, end.Y);
                Rectangle rect= new Rectangle(x1, y1, Abs(end.X - start.X), Abs(end.Y - start.Y));
                g.DrawEllipse(pen1, rect);
                break;
            case Tool.FilledRect:
                int x2 = Min(start.X, end.X), y2 = Min(start.Y, end.Y);
                g.FillRectangle(brush, x2, y2, Abs(end.X - start.X), Abs(end.Y - start.Y));
                break;
            case Tool.FilledCircle:
                int x3 = Min(start.X, end.X), y3 = Min(start.Y, end.Y);
                g.FillEllipse(brush, x3, y3, Abs(end.X - start.X), Abs(end.Y - start.Y));
                break;
            default:
                g.DrawLine(pen1, start, end);
                break;
        }
    }
    //mouse/key stuff 
    
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

    protected override void OnKeyDown(KeyEventArgs e){
        if (Keys.Delete == e.KeyCode){
            _onRestart();
        }
        if (Keys.Escape == e.KeyCode){
            _onQuit();
        }
        if (Keys.S == e.KeyCode){
            _onSave();
        }
        if (Keys.O == e.KeyCode){
            _onOpen();
        }
    }
}

class Hello {
    [STAThread]
    static void Main() {
        Form form = new MyForm();
        Application.Run(form);
    }
}
