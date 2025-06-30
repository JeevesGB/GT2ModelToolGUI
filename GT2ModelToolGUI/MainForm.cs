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
        private string gt2ModelToolPath = ""; // Path to the executable

        public MainForm()
        {
            InitializeComponent();
            SetupForm();
        }

        private void SetupForm()
        {
            this.Text = "GT2 Model Tool GUI";
            this.Size = new System.Drawing.Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
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
            inputGroup.Text = "GT2 Car Model Files (.cdp, .cnp, .cdo, .cno)";
            inputGroup.Location = new System.Drawing.Point(10, 10);
            inputGroup.Size = new System.Drawing.Size(750, 120);

            ListBox inputFilesList = new ListBox();
            inputFilesList.Name = "inputFilesList";
            inputFilesList.Location = new System.Drawing.Point(10, 25);
            inputFilesList.Size = new System.Drawing.Size(600, 60);
            inputFilesList.SelectionMode = SelectionMode.MultiExtended;

            Button browseInputBtn = new Button();
            browseInputBtn.Text = "Add Files...";
            browseInputBtn.Location = new System.Drawing.Point(620, 24);
            browseInputBtn.Size = new System.Drawing.Size(80, 25);
            browseInputBtn.Click += BrowseInputFile_Click;

            Button clearFilesBtn = new Button();
            clearFilesBtn.Text = "Clear All";
            clearFilesBtn.Location = new System.Drawing.Point(620, 54);
            clearFilesBtn.Size = new System.Drawing.Size(80, 25);
            clearFilesBtn.Click += ClearFiles_Click;

            Button removeSelectedBtn = new Button();
            removeSelectedBtn.Text = "Remove";
            removeSelectedBtn.Location = new System.Drawing.Point(620, 84);
            removeSelectedBtn.Size = new System.Drawing.Size(80, 25);
            removeSelectedBtn.Click += RemoveSelected_Click;

            inputGroup.Controls.Add(inputFilesList);
            inputGroup.Controls.Add(browseInputBtn);
            inputGroup.Controls.Add(clearFilesBtn);
            inputGroup.Controls.Add(removeSelectedBtn);

            // Output Directory Section
            GroupBox outputGroup = new GroupBox();
            outputGroup.Text = "Output Directory";
            outputGroup.Location = new System.Drawing.Point(10, 140);
            outputGroup.Size = new System.Drawing.Size(750, 80);

            TextBox outputDirText = new TextBox();
            outputDirText.Name = "outputDirText";
            outputDirText.Location = new System.Drawing.Point(10, 25);
            outputDirText.Size = new System.Drawing.Size(600, 23);
            outputDirText.ReadOnly = true;

            Button browseOutputBtn = new Button();
            browseOutputBtn.Text = "Browse...";
            browseOutputBtn.Location = new System.Drawing.Point(620, 24);
            browseOutputBtn.Size = new System.Drawing.Size(80, 25);
            browseOutputBtn.Click += BrowseOutputDir_Click;

            outputGroup.Controls.Add(outputDirText);
            outputGroup.Controls.Add(browseOutputBtn);

            // Options Section
            GroupBox optionsGroup = new GroupBox();
            optionsGroup.Text = "Conversion Options";
            optionsGroup.Location = new System.Drawing.Point(10, 230);
            optionsGroup.Size = new System.Drawing.Size(750, 120);

            CheckBox extractTexturesChk = new CheckBox();
            extractTexturesChk.Name = "extractTexturesChk";
            extractTexturesChk.Text = "Extract Textures";
            extractTexturesChk.Location = new System.Drawing.Point(10, 25);
            extractTexturesChk.Checked = true;

            CheckBox preserveStructureChk = new CheckBox();
            preserveStructureChk.Name = "preserveStructureChk";
            preserveStructureChk.Text = "Preserve Directory Structure";
            preserveStructureChk.Location = new System.Drawing.Point(10, 50);
            preserveStructureChk.Checked = true;

            CheckBox verboseOutputChk = new CheckBox();
            verboseOutputChk.Name = "verboseOutputChk";
            verboseOutputChk.Text = "Verbose Output";
            verboseOutputChk.Location = new System.Drawing.Point(10, 75);

            optionsGroup.Controls.Add(extractTexturesChk);
            optionsGroup.Controls.Add(preserveStructureChk);
            optionsGroup.Controls.Add(verboseOutputChk);

            // Convert Button
            Button convertBtn = new Button();
            convertBtn.Text = "Convert Selected Files";
            convertBtn.Location = new System.Drawing.Point(320, 360);
            convertBtn.Size = new System.Drawing.Size(140, 35);
            convertBtn.BackColor = System.Drawing.Color.LightGreen;
            convertBtn.Click += ConvertButton_Click;

            // Output Log
            GroupBox logGroup = new GroupBox();
            logGroup.Text = "Output Log";
            logGroup.Location = new System.Drawing.Point(10, 405);
            logGroup.Size = new System.Drawing.Size(750, 110);

            TextBox outputLog = new TextBox();
            outputLog.Name = "outputLog";
            outputLog.Location = new System.Drawing.Point(10, 20);
            outputLog.Size = new System.Drawing.Size(730, 80);
            outputLog.Multiline = true;
            outputLog.ScrollBars = ScrollBars.Vertical;
            outputLog.ReadOnly = true;
            outputLog.BackColor = System.Drawing.Color.Black;
            outputLog.ForeColor = System.Drawing.Color.LightGreen;
            outputLog.Font = new System.Drawing.Font("Consolas", 9);

            logGroup.Controls.Add(outputLog);

            tab.Controls.Add(inputGroup);
            tab.Controls.Add(outputGroup);
            tab.Controls.Add(optionsGroup);
            tab.Controls.Add(convertBtn);
            tab.Controls.Add(logGroup);
        }

        private void SetupBatchTab(TabPage tab)
        {
            // Input Directory for batch processing
            GroupBox batchInputGroup = new GroupBox();
            batchInputGroup.Text = "Input Directory (Search for .cdp, .cnp, .cdo, .cno files)";
            batchInputGroup.Location = new System.Drawing.Point(10, 10);
            batchInputGroup.Size = new System.Drawing.Size(750, 80);

            TextBox batchInputText = new TextBox();
            batchInputText.Name = "batchInputText";
            batchInputText.Location = new System.Drawing.Point(10, 25);
            batchInputText.Size = new System.Drawing.Size(600, 23);
            batchInputText.ReadOnly = true;

            Button browseBatchBtn = new Button();
            browseBatchBtn.Text = "Browse...";
            browseBatchBtn.Location = new System.Drawing.Point(620, 24);
            browseBatchBtn.Size = new System.Drawing.Size(80, 25);
            browseBatchBtn.Click += BrowseBatchInput_Click;

            batchInputGroup.Controls.Add(batchInputText);
            batchInputGroup.Controls.Add(browseBatchBtn);

            // Progress Bar
            GroupBox progressGroup = new GroupBox();
            progressGroup.Text = "Progress";
            progressGroup.Location = new System.Drawing.Point(10, 100);
            progressGroup.Size = new System.Drawing.Size(750, 80);

            ProgressBar progressBar = new ProgressBar();
            progressBar.Name = "batchProgressBar";
            progressBar.Location = new System.Drawing.Point(10, 25);
            progressBar.Size = new System.Drawing.Size(730, 25);

            Label progressLabel = new Label();
            progressLabel.Name = "progressLabel";
            progressLabel.Text = "Ready";
            progressLabel.Location = new System.Drawing.Point(10, 55);
            progressLabel.Size = new System.Drawing.Size(730, 20);

            progressGroup.Controls.Add(progressBar);
            progressGroup.Controls.Add(progressLabel);

            // Batch Process Button
            Button batchProcessBtn = new Button();
            batchProcessBtn.Text = "Start Batch Process";
            batchProcessBtn.Location = new System.Drawing.Point(300, 190);
            batchProcessBtn.Size = new System.Drawing.Size(150, 35);
            batchProcessBtn.BackColor = System.Drawing.Color.LightBlue;
            batchProcessBtn.Click += BatchProcessButton_Click;

            tab.Controls.Add(batchInputGroup);
            tab.Controls.Add(progressGroup);
            tab.Controls.Add(batchProcessBtn);
        }

        private void SetupSettingsTab(TabPage tab)
        {
            GroupBox toolPathGroup = new GroupBox();
            toolPathGroup.Text = "GT2ModelTool Executable Path";
            toolPathGroup.Location = new System.Drawing.Point(10, 10);
            toolPathGroup.Size = new System.Drawing.Size(750, 80);

            TextBox toolPathText = new TextBox();
            toolPathText.Name = "toolPathText";
            toolPathText.Text = gt2ModelToolPath;
            toolPathText.Location = new System.Drawing.Point(10, 25);
            toolPathText.Size = new System.Drawing.Size(600, 23);

            Button browseToolBtn = new Button();
            browseToolBtn.Text = "Browse...";
            browseToolBtn.Location = new System.Drawing.Point(620, 24);
            browseToolBtn.Size = new System.Drawing.Size(80, 25);
            browseToolBtn.Click += BrowseToolPath_Click;

            Button saveSettingsBtn = new Button();
            saveSettingsBtn.Text = "Save Settings";
            saveSettingsBtn.Location = new System.Drawing.Point(350, 100);
            saveSettingsBtn.Size = new System.Drawing.Size(100, 30);
            saveSettingsBtn.Click += SaveSettings_Click;

            toolPathGroup.Controls.Add(toolPathText);
            toolPathGroup.Controls.Add(browseToolBtn);

            tab.Controls.Add(toolPathGroup);
            tab.Controls.Add(saveSettingsBtn);
        }

        // Event Handlers
        private void BrowseInputFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "GT2 Model Files (*.cdp;*.cnp;*.cdo;*.cno)|*.cdp;*.cnp;*.cdo;*.cno|" +
                               "CDP Files (*.cdp)|*.cdp|" +
                               "CNP Files (*.cnp)|*.cnp|" +
                               "CDO Files (*.cdo)|*.cdo|" +
                               "CNO Files (*.cno)|*.cno|" +
                               "All Files (*.*)|*.*";
                dialog.Title = "Select GT2 Car Model Files";
                dialog.Multiselect = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var filesList = FindControl<ListBox>(this, "inputFilesList");
                    foreach (string fileName in dialog.FileNames)
                    {
                        // Avoid duplicates
                        if (!filesList.Items.Contains(fileName))
                        {
                            filesList.Items.Add(fileName);
                        }
                    }
                }
            }
        }

        private void ClearFiles_Click(object sender, EventArgs e)
        {
            var filesList = FindControl<ListBox>(this, "inputFilesList");
            filesList.Items.Clear();
        }

        private void RemoveSelected_Click(object sender, EventArgs e)
        {
            var filesList = FindControl<ListBox>(this, "inputFilesList");
            if (filesList.SelectedItems.Count > 0)
            {
                // Remove selected items (go backwards to avoid index issues)
                for (int i = filesList.SelectedIndices.Count - 1; i >= 0; i--)
                {
                    filesList.Items.RemoveAt(filesList.SelectedIndices[i]);
                }
            }
        }

        private void BrowseOutputDir_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    FindControl<TextBox>(this, "outputDirText").Text = dialog.SelectedPath;
                }
            }
        }

        private void BrowseBatchInput_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
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
                dialog.Filter = "Executable Files (*.exe)|*.exe|All Files (*.*)|*.*";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    FindControl<TextBox>(this, "toolPathText").Text = dialog.FileName;
                }
            }
        }

        private async void ConvertButton_Click(object sender, EventArgs e)
        {
            var filesList = FindControl<ListBox>(this, "inputFilesList");
            var outputDir = FindControl<TextBox>(this, "outputDirText").Text;
            var outputLog = FindControl<TextBox>(this, "outputLog");

            if (filesList.Items.Count == 0)
            {
                MessageBox.Show("Please select at least one GT2 model file to convert.", "No Files Selected",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(outputDir))
            {
                MessageBox.Show("Please select an output directory.", "Missing Output Directory",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            outputLog.Clear();
            outputLog.AppendText($"Starting conversion of {filesList.Items.Count} files...\r\n");

            // Disable the convert button during processing
            var convertBtn = sender as Button;
            convertBtn.Enabled = false;
            convertBtn.Text = "Converting...";

            try
            {
                for (int i = 0; i < filesList.Items.Count; i++)
                {
                    string inputFile = filesList.Items[i].ToString();
                    outputLog.AppendText($"\r\n--- Processing file {i + 1}/{filesList.Items.Count}: {Path.GetFileName(inputFile)} ---\r\n");

                    string arguments = BuildArguments(inputFile, outputDir);
                    await RunToolAsync(arguments, outputLog);

                    // Update UI to show progress
                    Application.DoEvents();
                }

                outputLog.AppendText($"\r\n=== CONVERSION COMPLETE ===\r\n");
                outputLog.AppendText($"Successfully processed {filesList.Items.Count} files!\r\n");

                MessageBox.Show($"Conversion complete!\r\nProcessed {filesList.Items.Count} files.",
                    "Conversion Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                outputLog.AppendText($"\r\nFATAL ERROR: {ex.Message}\r\n");
                MessageBox.Show($"An error occurred during conversion: {ex.Message}", "Conversion Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Re-enable the convert button
                convertBtn.Enabled = true;
                convertBtn.Text = "Convert Selected Files";
            }
        }

        private async void BatchProcessButton_Click(object sender, EventArgs e)
        {
            var batchInput = FindControl<TextBox>(this, "batchInputText").Text;

            if (string.IsNullOrEmpty(batchInput))
            {
                MessageBox.Show("Please select input directory for batch processing.", "Missing Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var progressBar = FindControl<ProgressBar>(this, "batchProgressBar");
            var progressLabel = FindControl<Label>(this, "progressLabel");

            try
            {
                // Find all GT2 model files in the directory
                string[] gt2Files = Directory.GetFiles(batchInput, "*.*", SearchOption.AllDirectories)
                    .Where(file => file.ToLower().EndsWith(".cdp") ||
                                   file.ToLower().EndsWith(".cnp") ||
                                   file.ToLower().EndsWith(".cdo") ||
                                   file.ToLower().EndsWith(".cno"))
                    .ToArray();

                if (gt2Files.Length == 0)
                {
                    MessageBox.Show("No GT2 model files (.cdp, .cnp, .cdo, .cno) found in the selected directory.",
                        "No Files Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                progressBar.Maximum = gt2Files.Length;
                progressBar.Value = 0;

                for (int i = 0; i < gt2Files.Length; i++)
                {
                    string file = gt2Files[i];
                    progressLabel.Text = $"Processing {Path.GetFileName(file)} ({i + 1}/{gt2Files.Length})";

                    // Create output directory next to input file
                    string outputDir = Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file) + "_converted");
                    Directory.CreateDirectory(outputDir);

                    // Process the file
                    string arguments = BuildArguments(file, outputDir);
                    await RunToolAsync(arguments, null); // null means don't update log for batch

                    progressBar.Value = i + 1;
                    Application.DoEvents(); // Update UI
                }

                progressLabel.Text = $"Batch processing complete! Processed {gt2Files.Length} files.";
                MessageBox.Show($"Successfully processed {gt2Files.Length} GT2 model files!", "Batch Complete",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                progressLabel.Text = "Batch processing failed.";
                MessageBox.Show($"Error during batch processing: {ex.Message}", "Batch Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveSettings_Click(object sender, EventArgs e)
        {
            gt2ModelToolPath = FindControl<TextBox>(this, "toolPathText").Text;
            MessageBox.Show("Settings saved!", "Settings", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private string BuildArguments(string inputFile, string outputDir)
        {
            var extractTextures = FindControl<CheckBox>(this, "extractTexturesChk").Checked;
            var preserveStructure = FindControl<CheckBox>(this, "preserveStructureChk").Checked;
            var verbose = FindControl<CheckBox>(this, "verboseOutputChk").Checked;

            // Build command line arguments based on tool's actual parameters
            // This is a placeholder - adjust based on GT2ModelTool's actual command line syntax
            string args = $"\"{inputFile}\" \"{outputDir}\"";

            if (extractTextures) args += " -t";
            if (preserveStructure) args += " -p";
            if (verbose) args += " -v";

            return args;
        }

        private async Task RunToolAsync(string arguments, TextBox outputLog)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = gt2ModelToolPath,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(startInfo))
                {
                    // Read output asynchronously
                    Task<string> outputTask = process.StandardOutput.ReadToEndAsync();
                    Task<string> errorTask = process.StandardError.ReadToEndAsync();

                    await process.WaitForExitAsync();

                    string output = await outputTask;
                    string error = await errorTask;

                    // Only update log if outputLog is provided (not null)
                    if (outputLog != null)
                    {
                        if (!string.IsNullOrEmpty(output))
                        {
                            outputLog.AppendText(output + "\r\n");
                        }

                        if (!string.IsNullOrEmpty(error))
                        {
                            outputLog.AppendText("ERROR: " + error + "\r\n");
                        }

                        outputLog.AppendText($"Process completed with exit code: {process.ExitCode}\r\n");
                    }
                }
            }
            catch (Exception ex)
            {
                if (outputLog != null)
                {
                    outputLog.AppendText($"Error running tool: {ex.Message}\r\n");
                }

                // Always show message box for errors, even in batch mode
                MessageBox.Show($"Error running GT2ModelTool: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Helper method to find controls by name
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
}