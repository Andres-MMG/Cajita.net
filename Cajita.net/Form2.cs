using Cajita.net.Models;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Cajita.net
{
    public partial class Form2 : Form
    {
        private const int GripSize = 5; // Tamaño de la región de agarre para redimensionar el formulario

        private Point _mouseOffset;
        private bool _isResizing = false;
        private Point _lastMousePosition;

        // Propiedad que obtiene el RichTextBox actual del tab seleccionado
        private RichTextBox CurrentRichTextBox =>
            TabControl1.SelectedTab?.Controls.OfType<RichTextBox>().FirstOrDefault();

        public Form2()
        {
            InitializeComponent();

            // Configuración inicial del formulario
            ConfigureForm();

            // Asignación de eventos a los elementos de menú
            AssignMenuEvents();

            // Configura el primer RichTextBox si existe
            SetupRichTextBoxEvents();

            // Configuración e inicio del timer
            InitializeTimer();

            // Suscribir al evento de cambio de tab
            TabControl1.SelectedIndexChanged += TabControl1_SelectedIndexChanged;
        }

        private void InitializeRichTextBoxes()
        {
            ContextMenuStrip richTextBoxContextMenu = CreateContextMenu();

            foreach (TabPage tab in TabControl1.TabPages)
            {
                foreach (Control control in tab.Controls)
                {
                    if (control is RichTextBox richTextBox)
                    {
                        // Desuscribir los eventos para evitar duplicados
                        richTextBox.MouseDown -= RichTextBox_MouseDown;
                        richTextBox.MouseMove -= RichTextBox_MouseMove;
                        richTextBox.MouseUp -= RichTextBox_MouseUp;

                        // Asignar el menú contextual al RichTextBox
                        richTextBox.ContextMenuStrip = richTextBoxContextMenu;

                        // Suscribir los eventos
                        richTextBox.MouseDown += RichTextBox_MouseDown;
                        richTextBox.MouseMove += RichTextBox_MouseMove;
                        richTextBox.MouseUp += RichTextBox_MouseUp;
                    }
                }
            }
        }


        private void TabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Cada vez que se cambie de tab, reconfigura los eventos del RichTextBox actual
            SetupRichTextBoxEvents();
        }

        // Configuración inicial del formulario
        private void ConfigureForm()
        {
            this.FormBorderStyle = FormBorderStyle.None; // Eliminar bordes y barra de título
            this.StartPosition = FormStartPosition.CenterScreen; // Centrar en pantalla
            moverToolStripMenuItem.CheckOnClick = true; // Permitir marcar/desmarcar opción de movimiento
        }

        // Asignación de eventos a los elementos del menú
        private void AssignMenuEvents()
        {
            copiarToolStripMenuItem.Click += (sender, e) => CopyText();
            pegarToolStripMenuItem.Click += (sender, e) => PasteText();
            moverToolStripMenuItem.Click += (sender, e) => ToggleMove();
            salirToolStripMenuItem.Click += (sender, e) => this.Close();
        }

        // Inicializar y configurar el timer
        private void InitializeTimer()
        {
            timer1.Interval = 100;
            timer1.Tick += Timer1_Tick;
            timer1.Start();
        }

        // Evento Tick del timer
        private void Timer1_Tick(object sender, EventArgs e)
        {
            this.TopMost = true; // Mantener formulario siempre al frente
        }

        // Copiar texto seleccionado al portapapeles
        private void CopyText()
        {
            if (CurrentRichTextBox != null)
                Clipboard.SetText(CurrentRichTextBox.SelectedText);
        }

        // Pegar texto del portapapeles
        private void PasteText()
        {
            if (CurrentRichTextBox != null && Clipboard.ContainsText())
                CurrentRichTextBox.Paste();
        }

        // Alternar la capacidad de mover el formulario
        private void ToggleMove()
        {
            this.Cursor = moverToolStripMenuItem.Checked ? Cursors.SizeAll : Cursors.Default;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            BeginFormDragOrResize(e);
        }

        // Iniciar arrastre o redimensionamiento del formulario
        private void BeginFormDragOrResize(MouseEventArgs e)
        {
            if (moverToolStripMenuItem.Checked && e.Button == MouseButtons.Left)
            {
                _isResizing = false; // Establecer _isResizing en false para el movimiento
                _mouseOffset = new Point(e.X, e.Y);
                this.Capture = true;
            }
            else if (IsInResizeZone(e.Location))
            {
                _isResizing = true; // Establecer _isResizing en true para el redimensionamiento
                _lastMousePosition = e.Location;
                this.Capture = true;
            }
        }


        // Verificar si el cursor está en la zona de redimensionamiento
        private bool IsInResizeZone(Point location)
        {
            return location.X >= this.ClientSize.Width - GripSize || location.Y >= this.ClientSize.Height - GripSize;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (this.Capture)
            {
                if (moverToolStripMenuItem.Checked && !_isResizing)
                {
                    // Mover el formulario
                    MoveForm(e);
                }
                else if (_isResizing)
                {
                    // Redimensionar el formulario
                    ResizeForm(e);
                }
            }
            else
            {
                // Actualizar el cursor si no estamos redimensionando ni moviendo.
                UpdateCursor(e.Location);
            }
        }
        private void ResizeForm(MouseEventArgs e)
        {
            int width = this.Width;
            int height = this.Height;

            if (e.Location.X > this.ClientSize.Width - GripSize && e.Location.X < this.ClientSize.Width)
            {
                width = this.Width + (e.X - _lastMousePosition.X);
            }

            if (e.Location.Y > this.ClientSize.Height - GripSize && e.Location.Y < this.ClientSize.Height)
            {
                height = this.Height + (e.Y - _lastMousePosition.Y);
            }

            this.Size = new Size(width, height);
            _lastMousePosition = e.Location;
        }

        // Mover el formulario
        private void MoveForm(MouseEventArgs e)
        {
            Point newLocation = this.Location;
            newLocation.X += e.X - _mouseOffset.X;
            newLocation.Y += e.Y - _mouseOffset.Y;
            this.Location = newLocation;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            EndFormDragOrResize();
        }

        // Finalizar arrastre o redimensionamiento
        private void EndFormDragOrResize()
        {
            _isResizing = false;
            this.Capture = false;
        }

        // Actualizar el cursor según la posición
        private void UpdateCursor(Point location)
        {
            this.Cursor = GetCursorForPosition(location);
        }

        // Obtener el cursor adecuado según la posición del ratón
        private Cursor GetCursorForPosition(Point location)
        {
            if (moverToolStripMenuItem.Checked)
                return Cursors.SizeAll;

            bool isOnRightEdge = location.X >= this.ClientSize.Width - GripSize;
            bool isOnBottomEdge = location.Y >= this.ClientSize.Height - GripSize;

            if (isOnRightEdge && isOnBottomEdge)
                return Cursors.SizeNWSE;
            if (isOnRightEdge)
                return Cursors.SizeWE;
            if (isOnBottomEdge)
                return Cursors.SizeNS;

            return Cursors.Default;
        }

        private void SetupRichTextBoxEvents()
        {
            // Asegúrate de que solo el RichTextBox actual esté suscrito a los eventos
            foreach (TabPage tab in TabControl1.TabPages)
            {
                foreach (Control control in tab.Controls)
                {
                    if (control is RichTextBox richTextBox)
                    {
                        // Desuscribir los eventos para evitar duplicados
                        UnsubscribeRichTextBoxEvents(richTextBox);

                        // Asignar el menú contextual al RichTextBox
                        richTextBox.ContextMenuStrip = ContextMenuStrip;

                        // Suscribir a los nuevos eventos
                        SubscribeRichTextBoxEvents(richTextBox);
                    }
                }
            }

            // Configurar eventos del RichTextBox actual
            if (CurrentRichTextBox != null)
            {
                // Asignar el menú contextual al RichTextBox actual
                CurrentRichTextBox.ContextMenuStrip = ContextMenuStrip;

                // Suscribir a los nuevos eventos
                SubscribeRichTextBoxEvents(CurrentRichTextBox);
            }
        }

        private void UnsubscribeRichTextBoxEvents(RichTextBox richTextBox)
        {
            richTextBox.MouseDown -= RichTextBox_MouseDown;
            richTextBox.MouseMove -= RichTextBox_MouseMove;
            richTextBox.MouseUp -= RichTextBox_MouseUp;
            richTextBox.Leave -= RichTextBox_Leave;
        }

        private void SubscribeRichTextBoxEvents(RichTextBox richTextBox)
        {
            richTextBox.MouseDown += RichTextBox_MouseDown;
            richTextBox.MouseMove += RichTextBox_MouseMove;
            richTextBox.MouseUp += RichTextBox_MouseUp;
            richTextBox.Leave += RichTextBox_Leave;
        }

        private void RichTextBox_Leave(object sender, EventArgs e)
        {
            RichTextBox richTextBox = sender as RichTextBox;
            if (richTextBox != null)
            {
                // Aquí tu lógica cuando el RichTextBox pierde el foco
                // Por ejemplo, guardar el contenido del RichTextBox
                SaveContent(richTextBox);
            }
        }

        private void SaveContent(RichTextBox richTextBox)
        {
            // Tu lógica para guardar el contenido del RichTextBox
        }

        private void RichTextBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (moverToolStripMenuItem.Checked)
            {
                var offset = ((Control)sender).PointToScreen(e.Location);
                var formLocation = this.PointToClient(offset);
                OnMouseDown(new MouseEventArgs(e.Button, e.Clicks, formLocation.X, formLocation.Y, e.Delta));
            }
        }

        private void RichTextBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (moverToolStripMenuItem.Checked)
                this.ToggleMove();

            if (moverToolStripMenuItem.Checked && this.Capture)
            {

                var offset = ((Control)sender).PointToScreen(e.Location);
                var formLocation = this.PointToClient(offset);
                OnMouseMove(new MouseEventArgs(e.Button, e.Clicks, formLocation.X, formLocation.Y, e.Delta));
            }
        }

        private void RichTextBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (moverToolStripMenuItem.Checked)
            {
                OnMouseUp(e);
            }
        }


        private ContextMenuStrip CreateContextMenu()
        {
            // Crear el ContextMenuStrip y sus ítems
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            ToolStripMenuItem copiarItem = new ToolStripMenuItem("Copiar");
            copiarItem.Click += (sender, e) => CopyText();
            ToolStripMenuItem pegarItem = new ToolStripMenuItem("Pegar");
            pegarItem.Click += (sender, e) => PasteText();

            // Añadir los ítems al ContextMenuStrip
            contextMenu.Items.AddRange(new ToolStripItem[] { copiarItem, pegarItem });

            return contextMenu;
        }


        private void btnAddTabPage_Click(object sender, EventArgs e)
        {
            string newFilePath = GenerateNewFilePath();
            CreateTabWithFile(newFilePath);
        }



        private string GenerateNewFilePath()
        {
            string directory = Path.Combine(Application.StartupPath, "Notes");
            Directory.CreateDirectory(directory); // Asegúrate de que el directorio existe

            string baseFileName = "notas.Cajita";
            string extension = ".txt";
            int counter = 0;

            // Encuentra un nombre de archivo no usado
            string newFilePath;
            do
            {
                string fileName = (counter == 0) ? $"{baseFileName}{extension}"
                                                 : $"{baseFileName}{counter}{extension}";
                newFilePath = Path.Combine(directory, fileName);
                counter++;
            } while (File.Exists(newFilePath));

            return newFilePath;
        }


        private string GetConfigFilePath()
        {
            return Path.Combine(Application.StartupPath, "config.json");
        }


        private void CreateTabWithFile(string filePath)
        {
            // Crear un nuevo RichTextBox y configurarlo
            RichTextBox newRichTextBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                Location = new Point(3, 3),
                Size = new Size(786, 416),
                ContextMenuStrip = CreateContextMenu(),
            };

            // Si el archivo no existe, crea un archivo nuevo
            if (!File.Exists(filePath))
            {
                using (var sw = File.CreateText(filePath))
                {
                    // Puedes escribir algún contenido inicial en el archivo si es necesario
                    // sw.Write("Contenido inicial...");
                }
            }

            // Cargar el contenido del archivo en el RichTextBox
            newRichTextBox.Text = File.ReadAllText(filePath);

            // Suscribir eventos
            SubscribeRichTextBoxEvents(newRichTextBox);

            // Crear una nueva pestaña para el RichTextBox
            TabPage newTabPage = new TabPage(Path.GetFileName(filePath))
            {
                Location = new Point(4, 24),
                Padding = new Padding(3),
                Size = new Size(792, 422),
                UseVisualStyleBackColor = true
            };
            newTabPage.Controls.Add(newRichTextBox);

            // Crear una nueva instancia de RichTextBoxFileInfo y asociarla con el Tag del TabPage
            RichTextBoxFileInfo richTextBoxFileInfo = new RichTextBoxFileInfo
            {
                TitleTagPage = newTabPage.Text,
                RichTextBox = newRichTextBox,
                FilePath = filePath
            };
            newTabPage.Tag = richTextBoxFileInfo;

            // Añadir la nueva pestaña al TabControl
            TabControl1.TabPages.Add(newTabPage);


            // Asociar el RichTextBox con el archivo
            RichTextBoxFileInfo rtfInfo = new RichTextBoxFileInfo
            {
                TitleTagPage = newTabPage.Text,
                RichTextBox = newRichTextBox,
                FilePath = filePath
            };
            newRichTextBox.Tag = rtfInfo;



        }

       private void Form2_Load(object sender, EventArgs e)
        {
            string configPath = GetConfigFilePath();
            if (File.Exists(configPath))
            {
                string json = File.ReadAllText(configPath);
                List<string> filePaths = JsonConvert.DeserializeObject<List<string>>(json);
                foreach (var filePath in filePaths)
                {
                    CreateTabWithFile(filePath);
                }
            }
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            var filePaths = TabControl1.TabPages
                .Cast<TabPage>()
                .Select(tp => tp.Tag as RichTextBoxFileInfo)
                .Where(rtfi => rtfi != null)
                .Select(rtfi => rtfi.FilePath)
                .ToList();

            string json = JsonConvert.SerializeObject(filePaths, Formatting.Indented);
            File.WriteAllText(GetConfigFilePath(), json);
        }

        private void SaveFileFromRichTextBox(RichTextBox richTextBox, string filePath)
        {
            // Guardar el contenido del RichTextBox en el archivo
            File.WriteAllText(filePath, richTextBox.Text);
        }

    }
}
