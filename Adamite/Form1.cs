using System.Drawing.Text;
using System.Security.Cryptography;

namespace Adamite
{
    public partial class Form1 : Form
    {
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
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }

            panel1.BackColor = Color.LightBlue;
        }

        private static string GetFileHash(string filePath)
        {
            using (var sha1 = SHA1.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    var hash = sha1.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToUpperInvariant();
                }
            }
        }

        private void DragDropPanel_DragDrop(object sender, DragEventArgs e)
        {
            panel1.BackColor = SystemColors.Control;

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length > 0)
            {
                var fileHash = "4B19C1FE3266B5EBC4305CD182ED6E864E3A1C4A";
                var filePath = files[0];
                var fileName = Path.GetFileName(filePath);
                var fileHashResult = GetFileHash(filePath);
                if (fileHashResult != fileHash)
                {
                    var result = MessageBox.Show("Hash does not match! Do you want to continue?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.No)
                    {
                        return;
                    }
                }

                PatchFile(filePath);
            }
        }

        private void PatchFile(string filePath)
        {
            byte[] fileBytes = File.ReadAllBytes(filePath);

            Dictionary<int, byte[]> patches = new Dictionary<int, byte[]>
            {
                { 0x1a6b1b, new byte[] { 
                    0xe9, 0xf1, 0x00, 0x00, 0x00 // jump past the new code block
                } }
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

            string newFilePath = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath) + "_nb" + Path.GetExtension(filePath));
            File.WriteAllBytes(newFilePath, fileBytes);

            label1.Text = "File patched successfully! New file created: " + newFilePath;
            panel1.BackColor = Color.LightGreen;
        }

        private void DragDropPanel_DragLeave(object sender, EventArgs e)
        {
            panel1.BackColor = SystemColors.Control;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }
        private void CenterLabel()
        {
            label1.Left = (panel1.ClientSize.Width - label1.Width) / 2;
            label1.Top = (panel1.ClientSize.Height - label1.Height) / 2;
        }
        private void Panel1_Resize(object sender, EventArgs e)
        {
            CenterLabel();
        }
        private void Label1_TextChanged(object sender, EventArgs e)
        {
            CenterLabel();
        }
    }
}
