using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace GT2ModelToolGUI
{
    public partial class MainForm : Form
    {
        private string gt2ModelToolPath = "GT2ModelTool.exe"; 

        public MainForm()
        {
            InitializeComponent();
            SetupForm();
            LoadSettings();
        }

        private void SetupForm()
        {
            this.Text = "GT2 Model Tool GUI v1.0";
            this.Size = new System.Drawing.Size(850, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = true;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Main TabControl
            TabControl tabControl = new TabControl();
            tabControl.Dock = DockStyle.Fill;

            // Convert Tab
            TabPage convertTab = new TabPage("Convert Models");
            SetupConvertTab(convertTab);
            tabControl.TabPages.Add(convertTab);

            // Batch Process Tab
            TabPage batchTab = new TabPage("Batch Process");
            SetupBatchTab(batchTab);
            tabControl.TabPages.Add(batchTab);

            // Settings Tab
            TabPage settingsTab = new TabPage("Settings");
            SetupSettingsTab(settingsTab);
            tabControl.TabPages.Add(settingsTab);

            this.Controls.Add(tabControl);
            this.ResumeLayout(false);
        }

        private void SetupConvertTab(TabPage tab)
        {
            // Input File Section
            GroupBox inputGroup = new GroupBox();
            inputGroup.Text = "Input Files (.cdo, .cno, .json)";
            inputGroup.Location = new System.Drawing.Point(10, 10);
            inputGroup.Size = new System.Drawing.Size(800, 120);

            ListBox inputFilesList = new ListBox();
            inputFilesList.Name = "inputFilesList";
            inputFilesList.Location = new System.Drawing.Point(10, 25);
            inputFilesList.Size = new System.Drawing.Size(650, 60);
            inputFilesList.SelectionMode = SelectionMode.MultiExtended;
            inputFilesList.AllowDrop = true;
            inputFilesList.DragEnter += InputFilesList_DragEnter;
            inputFilesList.DragDrop += InputFilesList_DragDrop;

            Button browseInputBtn = new Button();
            browseInputBtn.Text = "Add Files...";
            browseInputBtn.Location = new System.Drawing.Point(670, 24);
            browseInputBtn.Size = new System.Drawing.Size(80, 25);
            browseInputBtn.Click += BrowseInputFile_Click;

            Button clearFilesBtn = new Button();
            clearFilesBtn.Text = "Clear All";
            clearFilesBtn.Location = new System.Drawing.Point(670, 54);
            clearFilesBtn.Size = new System.Drawing.Size(80, 25);
            clearFilesBtn.Click += ClearFiles_Click;

            Button removeSelectedBtn = new Button();
            removeSelectedBtn.Text = "Remove";
            removeSelectedBtn.Location = new System.Drawing.Point(670, 84);
            removeSelectedBtn.Size = new System.Drawing.Size(80, 25);
            removeSelectedBtn.Click += RemoveSelected_Click;

            inputGroup.Controls.Add(inputFilesList);
            inputGroup.Controls.Add(browseInputBtn);
            inputGroup.Controls.Add(clearFilesBtn);
            inputGroup.Controls.Add(removeSelectedBtn);

            // Output Type Section
            GroupBox outputTypeGroup = new GroupBox();
            outputTypeGroup.Text = "Output Format";
            outputTypeGroup.Location = new System.Drawing.Point(10, 140);
            outputTypeGroup.Size = new System.Drawing.Size(800, 120);

            RadioButton radioO2 = new RadioButton();
            radioO2.Name = "radioO2";
            radioO2.Text = "Gran Turismo 2 Format (.cdo/.cno) - Convert from editable files back to game format";
            radioO2.Location = new System.Drawing.Point(15, 25);
            radioO2.Size = new System.Drawing.Size(770, 20);

            RadioButton radioOE = new RadioButton();
            radioOE.Name = "radioOE";
            radioOE.Text = "Editable Files (.json, .obj, .mtl) - Convert to editable format for 3D modeling";
            radioOE.Location = new System.Drawing.Point(15, 50);
            radioOE.Size = new System.Drawing.Size(770, 20);
            radioOE.Checked = true; 

            RadioButton radioOES = new RadioButton();
            radioOES.Name = "radioOES";
            radioOES.Text = "Editable Files with Split Overlapping Faces - Better Blender compatibility";
            radioOES.Location = new System.Drawing.Point(15, 75);
            radioOES.Size = new System.Drawing.Size(770, 20);

            outputTypeGroup.Controls.Add(radioO2);
            outputTypeGroup.Controls.Add(radioOE);
            outputTypeGroup.Controls.Add(radioOES);

            // Command Preview Section
            GroupBox commandGroup = new GroupBox();
            commandGroup.Text = "Command Preview";
            commandGroup.Location = new System.Drawing.Point(10, 270);
            commandGroup.Size = new System.Drawing.Size(800, 60);

            TextBox commandPreview = new TextBox();
            commandPreview.Name = "commandPreview";
            commandPreview.Location = new System.Drawing.Point(10, 25);
            commandPreview.Size = new System.Drawing.Size(780, 23);
            commandPreview.ReadOnly = true;
            commandPreview.BackColor = System.Drawing.Color.LightGray;
            commandPreview.Font = new System.Drawing.Font("Consolas", 9);

            commandGroup.Controls.Add(commandPreview);

            // Convert Button
            Button convertBtn = new Button();
            convertBtn.Name = "convertBtn";
            convertBtn.Text = "Convert Selected Files";
            convertBtn.Location = new System.Drawing.Point(325, 340);
            convertBtn.Size = new System.Drawing.Size(150, 35);
            convertBtn.BackColor = System.Drawing.Color.LightGreen;
            convertBtn.Click += ConvertButton_Click;

            // Output Log
            GroupBox logGroup = new GroupBox();
            logGroup.Text = "Output Log";
            logGroup.Location = new System.Drawing.Point(10, 385);
            logGroup.Size = new System.Drawing.Size(800, 150);

            TextBox outputLog = new TextBox();
            outputLog.Name = "outputLog";
            outputLog.Location = new System.Drawing.Point(10, 20);
            outputLog.Size = new System.Drawing.Size(780, 120);
            outputLog.Multiline = true;
            outputLog.ScrollBars = ScrollBars.Vertical;
            outputLog.ReadOnly = true;
            outputLog.BackColor = System.Drawing.Color.Black;
            outputLog.ForeColor = System.Drawing.Color.LightGreen;
            outputLog.Font = new System.Drawing.Font("Consolas", 9);

            logGroup.Controls.Add(outputLog);


            radioO2.CheckedChanged += (s, e) => UpdateCommandPreview();
            radioOE.CheckedChanged += (s, e) => UpdateCommandPreview();
            radioOES.CheckedChanged += (s, e) => UpdateCommandPreview();
            inputFilesList.SelectedIndexChanged += (s, e) => UpdateCommandPreview();

            tab.Controls.Add(inputGroup);
            tab.Controls.Add(outputTypeGroup);
            tab.Controls.Add(commandGroup);
            tab.Controls.Add(convertBtn);
            tab.Controls.Add(logGroup);
        }

        private void SetupBatchTab(TabPage tab)
        {
            // Input Directory for batch processing
            GroupBox batchInputGroup = new GroupBox();
            batchInputGroup.Text = "Input Directory (Search for .cdo, .cno, .json files)";
            batchInputGroup.Location = new System.Drawing.Point(10, 10);
            batchInputGroup.Size = new System.Drawing.Size(800, 80);

            TextBox batchInputText = new TextBox();
            batchInputText.Name = "batchInputText";
            batchInputText.Location = new System.Drawing.Point(10, 25);
            batchInputText.Size = new System.Drawing.Size(650, 23);
            batchInputText.ReadOnly = true;

            Button browseBatchBtn = new Button();
            browseBatchBtn.Text = "Browse...";
            browseBatchBtn.Location = new System.Drawing.Point(670, 24);
            browseBatchBtn.Size = new System.Drawing.Size(80, 25);
            browseBatchBtn.Click += BrowseBatchInput_Click;

            batchInputGroup.Controls.Add(batchInputText);
            batchInputGroup.Controls.Add(browseBatchBtn);

            // Batch Output Type
            GroupBox batchOutputGroup = new GroupBox();
            batchOutputGroup.Text = "Batch Output Format";
            batchOutputGroup.Location = new System.Drawing.Point(10, 100);
            batchOutputGroup.Size = new System.Drawing.Size(800, 100);

            RadioButton batchRadioOE = new RadioButton();
            batchRadioOE.Name = "batchRadioOE";
            batchRadioOE.Text = "Convert all to Editable Files (.json, .obj, .mtl)";
            batchRadioOE.Location = new System.Drawing.Point(15, 25);
            batchRadioOE.Size = new System.Drawing.Size(770, 20);
            batchRadioOE.Checked = true;

            RadioButton batchRadioOES = new RadioButton();
            batchRadioOES.Name = "batchRadioOES";
            batchRadioOES.Text = "Convert all to Editable Files with Split Overlapping Faces";
            batchRadioOES.Location = new System.Drawing.Point(15, 50);
            batchRadioOES.Size = new System.Drawing.Size(770, 20);

            batchOutputGroup.Controls.Add(batchRadioOE);
            batchOutputGroup.Controls.Add(batchRadioOES);

            // Progress Bar
            GroupBox progressGroup = new GroupBox();
            progressGroup.Text = "Progress";
            progressGroup.Location = new System.Drawing.Point(10, 210);
            progressGroup.Size = new System.Drawing.Size(800, 80);

            ProgressBar progressBar = new ProgressBar();
            progressBar.Name = "batchProgressBar";
            progressBar.Location = new System.Drawing.Point(10, 25);
            progressBar.Size = new System.Drawing.Size(780, 25);

            Label progressLabel = new Label();
            progressLabel.Name = "progressLabel";
            progressLabel.Text = "Ready";
            progressLabel.Location = new System.Drawing.Point(10, 55);
            progressLabel.Size = new System.Drawing.Size(780, 20);

            progressGroup.Controls.Add(progressBar);
            progressGroup.Controls.Add(progressLabel);

            // Batch Process Button
            Button batchProcessBtn = new Button();
            batchProcessBtn.Text = "Start Batch Process";
            batchProcessBtn.Location = new System.Drawing.Point(325, 300);
            batchProcessBtn.Size = new System.Drawing.Size(150, 35);
            batchProcessBtn.BackColor = System.Drawing.Color.LightBlue;
            batchProcessBtn.Click += BatchProcessButton_Click;

            tab.Controls.Add(batchInputGroup);
            tab.Controls.Add(batchOutputGroup);
            tab.Controls.Add(progressGroup);
            tab.Controls.Add(batchProcessBtn);
        }

        private void SetupSettingsTab(TabPage tab)
        {
            GroupBox toolPathGroup = new GroupBox();
            toolPathGroup.Text = "GT2ModelTool.exe Path";
            toolPathGroup.Location = new System.Drawing.Point(10, 10);
            toolPathGroup.Size = new System.Drawing.Size(800, 80);

            TextBox toolPathText = new TextBox();
            toolPathText.Name = "toolPathText";
            toolPathText.Text = gt2ModelToolPath;
            toolPathText.Location = new System.Drawing.Point(10, 25);
            toolPathText.Size = new System.Drawing.Size(650, 23);

            Button browseToolBtn = new Button();
            browseToolBtn.Text = "Browse...";
            browseToolBtn.Location = new System.Drawing.Point(670, 24);
            browseToolBtn.Size = new System.Drawing.Size(80, 25);
            browseToolBtn.Click += BrowseToolPath_Click;

            toolPathGroup.Controls.Add(toolPathText);
            toolPathGroup.Controls.Add(browseToolBtn);

            // Info section
            GroupBox infoGroup = new GroupBox();
            infoGroup.Text = "Information";
            infoGroup.Location = new System.Drawing.Point(20, 100);
            infoGroup.Size = new System.Drawing.Size(800, 250);

            LinkLabel infoLabel = new LinkLabel(); // Change from Label to LinkLabel
            infoLabel.Text = "GT2 Model Tool Usage:\n\n" +
                           "• This tool does not use any code from Pez2k's GT2 Model Tool.\n" +
                           " It must be downloaded seperately and placed into the 'Tool' folder.\n" +
                           "• .cdo/.cno → Editable: Converts game models to .json/.obj/.mtl for editing\n" +
                           "• .json → .cdo/.cno: Converts edited models back to game format\n" +
                           "• Split Overlapping Faces: Use when editing in Blender to prevent face deletion\n\n" +
                           "Requirements:\n" +
                           "• .NET 8.0 Desktop Runtime x64\n" +
                           "• GT2ModelTool.exe in the specified path\n\n" +
                           "Note: Always backup your original files before conversion!\n" +
                           "Credit goes to Pez2k for the original tool which can be downloaded from the link below.\n" +
                           "https://github.com/pez2k/gt2tools/releases/tag/GT2ModelTool210";
            infoLabel.Location = new System.Drawing.Point(35, 1);
            infoLabel.Size = new System.Drawing.Size(870, 560);

            // Set up the clickable link
            string linkUrl = "https://github.com/pez2k/gt2tools/releases/tag/GT2ModelTool210";
            int linkStart = infoLabel.Text.IndexOf(linkUrl);
            infoLabel.Links.Add(linkStart, linkUrl.Length, linkUrl);

            // Handle the link click event
            infoLabel.LinkClicked += (sender, e) => {
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = e.Link.LinkData.ToString(),
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Unable to open link: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            infoGroup.Controls.Add(infoLabel);

            Button saveSettingsBtn = new Button();
            saveSettingsBtn.Text = "Save Settings";
            saveSettingsBtn.Location = new System.Drawing.Point(350, 410);
            saveSettingsBtn.Size = new System.Drawing.Size(100, 30);
            saveSettingsBtn.Click += SaveSettings_Click;

            Button testToolBtn = new Button();
            testToolBtn.Text = "Test Tool";
            testToolBtn.Location = new System.Drawing.Point(460, 410);
            testToolBtn.Size = new System.Drawing.Size(100, 30);
            testToolBtn.Click += TestTool_Click;

            tab.Controls.Add(toolPathGroup);
            tab.Controls.Add(infoGroup);
            tab.Controls.Add(saveSettingsBtn);
            tab.Controls.Add(testToolBtn);
        }


        private void InputFilesList_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void InputFilesList_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            var filesList = FindControl<ListBox>(this, "inputFilesList");

            foreach (string file in files)
            {
                string ext = Path.GetExtension(file).ToLower();
                if (ext == ".cdo" || ext == ".cno" || ext == ".json")
                {
                    if (!filesList.Items.Contains(file))
                    {
                        filesList.Items.Add(file);
                    }
                }
            }
            UpdateCommandPreview();
        }

        private void BrowseInputFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "GT2 Model Files (*.cdo;*.cno;*.json)|*.cdo;*.cno;*.json|" +
                               "CDO Files (*.cdo)|*.cdo|" +
                               "CNO Files (*.cno)|*.cno|" +
                               "JSON Files (*.json)|*.json|" +
                               "All Files (*.*)|*.*";
                dialog.Title = "Select GT2 Model Files";
                dialog.Multiselect = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var filesList = FindControl<ListBox>(this, "inputFilesList");
                    foreach (string fileName in dialog.FileNames)
                    {
                        if (!filesList.Items.Contains(fileName))
                        {
                            filesList.Items.Add(fileName);
                        }
                    }
                    UpdateCommandPreview();
                }
            }
        }

        private void ClearFiles_Click(object sender, EventArgs e)
        {
            var filesList = FindControl<ListBox>(this, "inputFilesList");
            filesList.Items.Clear();
            UpdateCommandPreview();
        }

        private void RemoveSelected_Click(object sender, EventArgs e)
        {
            var filesList = FindControl<ListBox>(this, "inputFilesList");
            if (filesList.SelectedItems.Count > 0)
            {
                for (int i = filesList.SelectedIndices.Count - 1; i >= 0; i--)
                {
                    filesList.Items.RemoveAt(filesList.SelectedIndices[i]);
                }
                UpdateCommandPreview();
            }
        }

        private void BrowseBatchInput_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select folder containing GT2 model files";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    FindControl<TextBox>(this, "batchInputText").Text = dialog.SelectedPath;
                }
            }
        }

        private void BrowseToolPath_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "GT2ModelTool (GT2ModelTool.exe)|GT2ModelTool.exe|Executable Files (*.exe)|*.exe|All Files (*.*)|*.*";
                dialog.Title = "Select GT2ModelTool.exe";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    FindControl<TextBox>(this, "toolPathText").Text = dialog.FileName;
                }
            }
        }

        private void UpdateCommandPreview()
        {
            var filesList = FindControl<ListBox>(this, "inputFilesList");
            var commandPreview = FindControl<TextBox>(this, "commandPreview");

            if (filesList.SelectedItem != null)
            {
                string selectedFile = filesList.SelectedItem.ToString();
                string outputType = GetSelectedOutputType();
                string command = $"GT2ModelTool -{outputType} \"{Path.GetFileName(selectedFile)}\"";
                commandPreview.Text = command;
            }
            else if (filesList.Items.Count > 0)
            {
                string outputType = GetSelectedOutputType();
                commandPreview.Text = $"GT2ModelTool -{outputType} [selected_file]";
            }
            else
            {
                commandPreview.Text = "Select files to see command preview";
            }
        }

        private string GetSelectedOutputType()
        {
            if (FindControl<RadioButton>(this, "radioO2")?.Checked == true) return "o2";
            if (FindControl<RadioButton>(this, "radioOES")?.Checked == true) return "oes";
            return "oe";
        }

        private async void ConvertButton_Click(object sender, EventArgs e)
        {
            var filesList = FindControl<ListBox>(this, "inputFilesList");
            var outputLog = FindControl<TextBox>(this, "outputLog");
            var convertBtn = FindControl<Button>(this, "convertBtn");

            if (filesList.Items.Count == 0)
            {
                MessageBox.Show("Please select at least one file to convert.", "No Files Selected",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!File.Exists(gt2ModelToolPath))
            {
                MessageBox.Show($"GT2ModelTool.exe not found at: {gt2ModelToolPath}\n\nPlease check the Settings tab.",
                    "Tool Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string outputFolder = string.Empty;
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select the folder to save converted files:";
                if (folderDialog.ShowDialog() != DialogResult.OK)
                {
                    // User cancelled folder selection
                    return;
                }
                outputFolder = folderDialog.SelectedPath;
            }

            outputLog.Clear();
            outputLog.AppendText($"Starting conversion of {filesList.Items.Count} files...\r\n");
            outputLog.AppendText($"Using tool: {gt2ModelToolPath}\r\n\r\n");

            convertBtn.Enabled = false;
            convertBtn.Text = "Converting...";

            try
            {
                string outputType = GetSelectedOutputType();

                for (int i = 0; i < filesList.Items.Count; i++)
                {
                    string inputFile = filesList.Items[i].ToString();
                    outputLog.AppendText($"--- File {i + 1}/{filesList.Items.Count}: {Path.GetFileName(inputFile)} ---\r\n");

                    await RunToolAsync(outputType, inputFile, outputLog);
                    Application.DoEvents();
                }

                outputLog.AppendText($"\r\n=== CONVERSION COMPLETE ===\r\n");
                MessageBox.Show($"Conversion complete!\nProcessed {filesList.Items.Count} files.",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                outputLog.AppendText($"\r\nFATAL ERROR: {ex.Message}\r\n");
                MessageBox.Show($"Error during conversion: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                convertBtn.Enabled = true;
                convertBtn.Text = "Convert Selected Files";
            }
        }

        private async void BatchProcessButton_Click(object sender, EventArgs e)
        {
            var batchInput = FindControl<TextBox>(this, "batchInputText").Text;
            var progressBar = FindControl<ProgressBar>(this, "batchProgressBar");
            var progressLabel = FindControl<Label>(this, "progressLabel");

            if (string.IsNullOrEmpty(batchInput) || !Directory.Exists(batchInput))
            {
                MessageBox.Show("Please select a valid input directory.", "Invalid Directory",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!File.Exists(gt2ModelToolPath))
            {
                MessageBox.Show($"GT2ModelTool.exe not found at: {gt2ModelToolPath}", "Tool Not Found",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                string[] files = Directory.GetFiles(batchInput, "*.*", SearchOption.AllDirectories)
                    .Where(file => {
                        string ext = Path.GetExtension(file).ToLower();
                        return ext == ".cdo" || ext == ".cno" || ext == ".json";
                    }).ToArray();

                if (files.Length == 0)
                {
                    MessageBox.Show("No GT2 model files found in the selected directory.", "No Files Found",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                progressBar.Maximum = files.Length;
                progressBar.Value = 0;

                string outputType = FindControl<RadioButton>(this, "batchRadioOES")?.Checked == true ? "oes" : "oe";

                for (int i = 0; i < files.Length; i++)
                {
                    string file = files[i];
                    progressLabel.Text = $"Processing {Path.GetFileName(file)} ({i + 1}/{files.Length})";

                    await RunToolAsync(outputType, file, null);

                    progressBar.Value = i + 1;
                    Application.DoEvents();
                }

                progressLabel.Text = $"Batch complete! Processed {files.Length} files.";
                MessageBox.Show($"Successfully processed {files.Length} files!", "Batch Complete",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                progressLabel.Text = "Batch processing failed.";
                MessageBox.Show($"Error during batch processing: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveSettings_Click(object sender, EventArgs e)
        {
            gt2ModelToolPath = FindControl<TextBox>(this, "toolPathText").Text;
            SaveSettings();
            MessageBox.Show("Settings saved!", "Settings", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async void TestTool_Click(object sender, EventArgs e)
        {
            string toolPath = FindControl<TextBox>(this, "toolPathText").Text;

            if (!File.Exists(toolPath))
            {
                MessageBox.Show("GT2ModelTool.exe not found at the specified path.", "Tool Not Found",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = toolPath,
                    Arguments = "", 
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(startInfo))
                {
                    await process.WaitForExitAsync();
                    string output = await process.StandardOutput.ReadToEndAsync();
                    string error = await process.StandardError.ReadToEndAsync();

                    MessageBox.Show($"Tool test successful!\nExit code: {process.ExitCode}\n\nOutput:\n{output}\n\nError:\n{error}",
                        "Tool Test", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to run tool: {ex.Message}", "Tool Test Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task RunToolAsync(string outputType, string inputFile, TextBox outputLog)
        {
            try
            {
                string arguments = $"-{outputType} \"{inputFile}\"";

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = gt2ModelToolPath,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    WorkingDirectory = Path.GetDirectoryName(inputFile)
                };

                if (outputLog != null)
                {
                    outputLog.AppendText($"Running: GT2ModelTool {arguments}\r\n");
                }

                using (Process process = Process.Start(startInfo))
                {
                    Task<string> outputTask = process.StandardOutput.ReadToEndAsync();
                    Task<string> errorTask = process.StandardError.ReadToEndAsync();

                    await process.WaitForExitAsync();

                    string output = await outputTask;
                    string error = await errorTask;

                    if (outputLog != null)
                    {
                        if (!string.IsNullOrEmpty(output))
                            outputLog.AppendText($"Output: {output}\r\n");

                        if (!string.IsNullOrEmpty(error))
                            outputLog.AppendText($"Error: {error}\r\n");

                        outputLog.AppendText($"Exit code: {process.ExitCode}\r\n\r\n");
                    }

                    if (process.ExitCode != 0 && !string.IsNullOrEmpty(error))
                    {
                        throw new Exception($"Tool failed with exit code {process.ExitCode}: {error}");
                    }
                }
            }
            catch (Exception ex)
            {
                if (outputLog != null)
                {
                    outputLog.AppendText($"Error: {ex.Message}\r\n\r\n");
                }
                throw;
            }
        }

        private void LoadSettings()
        {
            string[] possiblePaths = {
                "GT2ModelTool.exe",
                Path.Combine(Application.StartupPath, "GT2ModelTool.exe"),
                Path.Combine(Environment.CurrentDirectory, "GT2ModelTool.exe")
            };

            foreach (string path in possiblePaths)
            {
                if (File.Exists(path))
                {
                    gt2ModelToolPath = path;
                    break;
                }
            }
        }

        private void SaveSettings()
        {

        }

        private T FindControl<T>(Control parent, string name) where T : Control
        {
            foreach (Control control in parent.Controls)
            {
                if (control is T && control.Name == name)
                    return (T)control;

                T found = FindControl<T>(control, name);
                if (found != null)
                    return found;
            }
            return null;
        }
    }


    public static class ProcessExtensions
    {
        public static Task WaitForExitAsync(this Process process)
        {
            var tcs = new TaskCompletionSource<bool>();
            process.EnableRaisingEvents = true;
            process.Exited += (sender, args) => tcs.TrySetResult(true);
            return process.HasExited ? Task.CompletedTask : tcs.Task;
        }
    }
}