﻿using Cajita.net.Models;
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
        private const int GripSize = 11; // Tamaño de la región de agarre para redimensionar el formulario

        private Point _mouseOffset;
        private bool _isResizing = false;
        private Point _lastMousePosition;
        private TabPage tabUnderCursor;

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
            cerrarToolStripMenuItem.Click += (sender, e) => CerrarTabItem_Click();
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
                SaveContent(richTextBox);
            }
        }

        private void SaveContent(RichTextBox richTextBox)
        {
            // Verificar si el RichTextBox tiene una asociación con un archivo
            if (richTextBox.Tag is RichTextBoxFileInfo richTextBoxFileInfo && !string.IsNullOrEmpty(richTextBoxFileInfo.FilePath))
            {
                // Guardar el contenido del RichTextBox en el archivo
                File.WriteAllText(richTextBoxFileInfo.FilePath, richTextBox.Text);
            }
            else
            {
                // Manejar casos donde no hay un archivo asociado o donde no se debe guardar
                // Por ejemplo, mostrar un mensaje al usuario o abrir un diálogo de "Guardar como..."
            }
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



        private void ContextMenu_Opening(object sender, CancelEventArgs e)
        {
            ContextMenuStrip menu = sender as ContextMenuStrip;
            if (menu != null)
            {
                Point p = menu.PointToClient(Control.MousePosition);
                // Ajustar p para que el punto esté relativo al TabControl
                p = TabControl1.PointToScreen(p);

                // Determinar sobre qué pestaña se hizo clic derecho
                for (int i = 0; i < TabControl1.TabCount; i++)
                {
                    Rectangle r = TabControl1.GetTabRect(i);
                    if (r.Contains(p))
                    {
                        // Establecer la pestaña sobre la que se hizo clic derecho
                        TabControl1.SelectedTab = TabControl1.TabPages[i];
                        break;
                    }
                }
            }
        }

        private void AddTabPage_dblClick(object sender, EventArgs e)
        {
            AddTabPage();
        }

        private void AddTabPage()
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
                Size = new Size(786, 416)
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
            else
            {
                //no existe ninguno se crea uno nuevo por defecto
                AddTabPage();
            }
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            var filePaths = new List<string>();
            foreach (TabPage tabPage in TabControl1.TabPages)
            {
                if (tabPage.Tag is RichTextBoxFileInfo rtfi)
                {
                    // Guardar el contenido del RichTextBox en el archivo asociado
                    File.WriteAllText(rtfi.FilePath, rtfi.RichTextBox.Text);
                    filePaths.Add(rtfi.FilePath);
                }
            }

            // Guardar la lista de rutas de archivos en el archivo de configuración
            string json = JsonConvert.SerializeObject(filePaths, Formatting.Indented);
            File.WriteAllText(GetConfigFilePath(), json);
        }
        private void TabControl1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                for (int i = 0; i < TabControl1.TabCount; i++)
                {
                    Rectangle r = TabControl1.GetTabRect(i);
                    if (r.Contains(e.Location))
                    {
                        // Guardar la pestaña bajo el cursor y mostrar el menú contextual
                        tabUnderCursor = TabControl1.TabPages[i];
                        ContextMenuStrip.Show(TabControl1, e.Location);
                        return; // Salir después de encontrar la pestaña correcta
                    }
                }
            }
        }


        private void TabControl1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                for (int i = 0; i < TabControl1.TabCount; i++)
                {
                    Rectangle r = TabControl1.GetTabRect(i);
                    if (r.Contains(e.Location))
                    {
                        // Guardar la pestaña bajo el cursor
                        tabUnderCursor = TabControl1.TabPages[i];
                        break;
                    }
                }
            }
        }

        private void CerrarTabItem_Click()
        {
            if (tabUnderCursor != null)
            {
                timer1.Stop(); // Detener el temporizador para que no se active el evento Tick
                DialogResult result = MessageBox.Show("¿Estás seguro de que quieres cerrar esta pestaña?", "Confirmar cierre", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    // Cerrar la pestaña que está bajo el cursor
                    TabControl1.TabPages.Remove(tabUnderCursor);
                    tabUnderCursor.Dispose();
                    tabUnderCursor = null; // Resetear la referencia
                }
                timer1.Start(); // Reiniciar el temporizador
            }
        }


    }
}
