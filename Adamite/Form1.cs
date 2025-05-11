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

            pnlDragDrop.AllowDrop = true;
            pnlDragDrop.DragEnter += DragDropPanel_DragEnter;
            pnlDragDrop.DragDrop += DragDropPanel_DragDrop;
            pnlDragDrop.DragLeave += DragDropPanel_DragLeave;

            lblInfo.TextAlign = ContentAlignment.MiddleCenter;
            lblInfo.Dock = DockStyle.Fill;
            lblInfo.Padding = new Padding(6);

            // Subscribe to the Resize and TextChanged events
            pnlDragDrop.Resize += Panel1_Resize;
            lblInfo.TextChanged += Label1_TextChanged;

            // Center the label initially
            CenterLabel();
        }

        private void DragDropPanel_DragEnter(object sender, DragEventArgs e)
        {
            var (hasFileDrop, _) = (e.Data?.GetDataPresent(DataFormats.FileDrop) == true, e.Data);

            e.Effect = hasFileDrop ? DragDropEffects.Copy : DragDropEffects.None;

            pnlDragDrop.BackColor = Color.LightBlue;
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
            pnlDragDrop.BackColor = SystemColors.Control;

            if (e.Data?.GetData(DataFormats.FileDrop) is string[] draggedFiles && draggedFiles.Length > 0)
            {
                var filePath = draggedFiles[0];
                var fileHashResult = GetFileHash(filePath);

                if (fileHashResult != _fileHash)
                {
                    DialogResult result =
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
            byte[] fileBuffer = File.ReadAllBytes(filePath);

            var patches = new Dictionary<int, byte[]>
            {
                [0x1A6B1B] = [0xE9, 0xF1, 0x00, 0x00, 0x00] // jump past the new code block
            };

            foreach (var patch in patches)
            {
                int offset = patch.Key;
                byte[] patchBytes = patch.Value;

                if (offset + patchBytes.Length <= fileBuffer.Length)
                {
                    Array.Copy(patchBytes, 0, fileBuffer, offset, patchBytes.Length);
                }
                else
                {
                    lblInfo.Text = $"Patch offset {offset:X} out of bounds!";
                    pnlDragDrop.BackColor = Color.Red;
                    return;
                }
            }

            // Verification before writing
            foreach (var patch in patches)
            {
                int offset = patch.Key;
                byte[] expectedBytes = patch.Value;

                if (!fileBuffer.Skip(offset).Take(expectedBytes.Length).SequenceEqual(expectedBytes))
                {
                    lblInfo.Text = $"Patch verification failed at offset 0x{offset:X}!";
                    pnlDragDrop.BackColor = Color.Red;
                    return;
                }
            }

            string newFilePath = Path.Combine(
                Path.GetDirectoryName(filePath)!,
                Path.GetFileNameWithoutExtension(filePath) + "_nb" + Path.GetExtension(filePath)
            );

            File.WriteAllBytes(newFilePath, fileBuffer);

            string displayPath = newFilePath.Replace(" ", "\u00A0"); // Remove breaking space in the path
            lblInfo.Text = $"File patched and verified successfully! New file: {displayPath}";
            pnlDragDrop.BackColor = Color.LightGreen;
        }

        private void DragDropPanel_DragLeave(object sender, EventArgs e)
            => pnlDragDrop.BackColor = SystemColors.Control;

        private void Panel1_Resize(object sender, EventArgs e)
            => CenterLabel();

        private void Label1_TextChanged(object sender, EventArgs e)
            => CenterLabel();

        private void CenterLabel()
        {
            lblInfo.Left = (pnlDragDrop.ClientSize.Width - lblInfo.Width) / 2;
            lblInfo.Top = (pnlDragDrop.ClientSize.Height - lblInfo.Height) / 2;
        }
    }
}