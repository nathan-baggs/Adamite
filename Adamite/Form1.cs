using System.Security.Cryptography;

namespace Adamite
{
    public partial class Form1 : Form
    {
        private const string _fileHash = "4B19C1FE3266B5EBC4305CD182ED6E864E3A1C4A";

        public Form1()
        {
            InitializeComponent();
            MaximizeBox = false;

            panel1.AllowDrop = true;
            panel1.DragEnter += DragDropPanel_DragEnter;
            panel1.DragDrop += DragDropPanel_DragDrop;
            panel1.DragLeave += DragDropPanel_DragLeave;

            label1.TextAlign = ContentAlignment.MiddleCenter;

            // Subscribe to the Resize and TextChanged events
            panel1.Resize += Panel1_Resize;
            label1.TextChanged += Label1_TextChanged;

            // Center the label initially
            CenterLabel();
        }

        private void DragDropPanel_DragEnter(object sender, DragEventArgs e)
        {
            var (hasFileDrop, _) = (e.Data?.GetDataPresent(DataFormats.FileDrop) == true, e.Data);

            e.Effect = hasFileDrop ? DragDropEffects.Copy : DragDropEffects.None;

            panel1.BackColor = Color.LightBlue;
        }

        private static string GetFileHash(string filePath)
        {
            using SHA1 sha1 = SHA1.Create();
            using Stream stream = File.OpenRead(filePath);
            byte[] hash = sha1.ComputeHash(stream);

            return Convert.ToHexString(hash).ToUpperInvariant();
        }

        private void DragDropPanel_DragDrop(object sender, DragEventArgs e)
        {
            panel1.BackColor = SystemColors.Control;

            if (e.Data?.GetData(DataFormats.FileDrop) is string[] draggedFiles && draggedFiles.Length > 0)
            {
                var fileHash = _fileHash;
                var filePath = draggedFiles[0];
                var fileHashResult = GetFileHash(filePath);

                if (fileHashResult != fileHash)
                {
                    var result =
                        MessageBox.Show(
                            "Hash does not match! Do you want to continue?",
                            "Warning",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning);

                    if (result == DialogResult.No)
                    {
                        return;
                    }
                }

                PatchFile(filePath);
            }
            else
            {
                MessageBox.Show(
                    "No valid files were dropped.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void PatchFile(string filePath)
        {
            byte[] fileBytes = File.ReadAllBytes(filePath);

            Dictionary<int, byte[]> patches = new Dictionary<int, byte[]>
            {
                { 0x1a6b1b, new byte[] {
                    0xe9, 0xf1, 0x00, 0x00, 0x00 // jump past the new code block
                }}
            };

            foreach (var patch in patches)
            {
                int offset = patch.Key;
                byte[] patchBytes = patch.Value;
                if (offset + patchBytes.Length <= fileBytes.Length)
                {
                    Array.Copy(patchBytes, 0, fileBytes, offset, patchBytes.Length);
                }
            }

            string newFilePath =
                Path.Combine(Path.GetDirectoryName(filePath)!,
                Path.GetFileNameWithoutExtension(filePath) + "_nb" + Path.GetExtension(filePath));

            File.WriteAllBytes(newFilePath, fileBytes);

            label1.Text = "File patched successfully! New file created: " + newFilePath;
            panel1.BackColor = Color.LightGreen;
        }

        private void DragDropPanel_DragLeave(object sender, EventArgs e)
            => panel1.BackColor = SystemColors.Control;

        private void Panel1_Resize(object sender, EventArgs e)
            => CenterLabel();

        private void Label1_TextChanged(object sender, EventArgs e)
            => CenterLabel();

        private void CenterLabel()
        {
            label1.Left = (panel1.ClientSize.Width - label1.Width) / 2;
            label1.Top = (panel1.ClientSize.Height - label1.Height) / 2;
        }
    }
}