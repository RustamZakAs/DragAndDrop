using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace RZ
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            // например, получаем иконку для PDF
            // имя файла - от балды, главное это расширение
            //-string fileName = "bla-bla-bla.pdf";
            // вся кухня - здесь
            //-Shell32.SHFILEINFO shfi = new Shell32.SHFILEINFO();
            //-Shell32.SHGetFileInfo(fileName, Shell32.FILE_ATTRIBUTE_NORMAL, ref shfi,
            //-(uint)Marshal.SizeOf(shfi), Shell32.SHGFI_ICON | Shell32.SHGFI_USEFILEATTRIBUTES | Shell32.SHGFI_LARGEICON);
            //-Icon icon = (Icon)Icon.FromHandle(shfi.hIcon).Clone();
            //-User32.DestroyIcon(shfi.hIcon);
            // теперь можем, например, сохранить в bmp
            //-icon.ToBitmap().Save(@"c:\pdf_icon.bmp");

            
        }

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
            {
                e.Effect = DragDropEffects.All;
            }
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            //Take dropped items and store in array
            string[] droppedFiles = (string[])e.Data.GetData(DataFormats.FileDrop);
            //Loop thru all dropped items and display them
            foreach (string file in droppedFiles)
            {
                string filename = getFileInfo(file);
                string fileformat = Path.GetExtension(file);
                //MessageBox.Show(file + " | " + filename);

                try
                {
                    if (fileformat == ".jpg"
                        || fileformat == ".jpeg"
                        || fileformat == ".png"
                        || fileformat == ".ico"

                    )
                    {
                        pictureBox1.Load(filename);
                        return;
                    }
                        


                    Bitmap bmp = default(Bitmap);
                    //bmp = new Bitmap(System.Drawing.Icon.ExtractAssociatedIcon(file).ToBitmap());
                    bmp = new Bitmap(ExtractIconClass.GetIcon(file, false).ToBitmap());
                    pictureBox1.Image = bmp;
                    //bmp.Save("D:/ff.ico");

                    
                }
                catch (Exception)
                {
                    pictureBox1.Load(filename);
                }
                finally
                {
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                }
                
            }
        }

        private string getFileInfo(string path)
        {
            string fileName = Path.GetFileNameWithoutExtension(path);
            string fileFullName = Path.GetFullPath(path);
            string fileFormat = Path.GetExtension(path);
            return fileFullName;
        }





        public class Shell32
        {
            public const int MAX_PATH = 256;
            [StructLayout(LayoutKind.Sequential)]
            public struct SHITEMID
            {
                public ushort cb;
                [MarshalAs(UnmanagedType.LPArray)]
                public byte[] abID;
            }
            [StructLayout(LayoutKind.Sequential)]
            public struct ITEMIDLIST
            {
                public SHITEMID mkid;
            }
            [StructLayout(LayoutKind.Sequential)]
            public struct SHFILEINFO
            {
                public const int NAMESIZE = 80;
                public IntPtr hIcon;
                public int iIcon;
                public uint dwAttributes;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
                public string szDisplayName;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = NAMESIZE)]
                public string szTypeName;
            };
            public const uint SHGFI_ICON = 0x000000100;     // get icon
            public const uint SHGFI_DISPLAYNAME = 0x000000200;     // get display name
            public const uint SHGFI_TYPENAME = 0x000000400;     // get type name
            public const uint SHGFI_ATTRIBUTES = 0x000000800;     // get attributes
            public const uint SHGFI_ICONLOCATION = 0x000001000;     // get icon location
            public const uint SHGFI_EXETYPE = 0x000002000;     // return exe type
            public const uint SHGFI_SYSICONINDEX = 0x000004000;     // get system icon index
            public const uint SHGFI_LINKOVERLAY = 0x000008000;     // put a link overlay on icon
            public const uint SHGFI_SELECTED = 0x000010000;     // show icon in selected state
            public const uint SHGFI_ATTR_SPECIFIED = 0x000020000;     // get only specified attributes
            public const uint SHGFI_LARGEICON = 0x000000000;     // get large icon
            public const uint SHGFI_SMALLICON = 0x000000001;     // get small icon
            public const uint SHGFI_OPENICON = 0x000000002;     // get open icon
            public const uint SHGFI_SHELLICONSIZE = 0x000000004;     // get shell size icon
            public const uint SHGFI_PIDL = 0x000000008;     // pszPath is a pidl
            public const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;     // use passed dwFileAttribute
            public const uint SHGFI_ADDOVERLAYS = 0x000000020;     // apply the appropriate overlays
            public const uint SHGFI_OVERLAYINDEX = 0x000000040;     // Get the index of the overlay
            public const uint FILE_ATTRIBUTE_DIRECTORY = 0x00000010;
            public const uint FILE_ATTRIBUTE_NORMAL = 0x00000080;
            [DllImport("Shell32.dll")]
            public static extern IntPtr SHGetFileInfo(
                string pszPath,
                uint dwFileAttributes,
                ref SHFILEINFO psfi,
                uint cbFileInfo,
                uint uFlags
                );
        }
        public class User32
        {
            [DllImport("User32.dll")]
            public static extern int DestroyIcon(IntPtr hIcon);
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
    }

    class ExtractIconClass
    {
        [DllImport("Kernel32.dll")]
        public static extern int GetModuleHandle(string lpModuleName);
        [DllImport("Shell32.dll")]
        public static extern IntPtr ExtractIcon(int hInst, string FileName, int nIconIndex);
        [DllImport("Shell32.dll")]
        public static extern int DestroyIcon(IntPtr hIcon);
        [DllImport("Shell32.dll")]
        public static extern IntPtr ExtractIconEx(string FileName, int nIconIndex, int[] lgIcon, int[] smIcon, int nIcons);
        [DllImport("Shell32.dll")]
        private static extern int SHGetFileInfo(string pszPath, uint dwFileAttributes, out SHFILEINFO psfi, uint cbfileInfo, SHGFI uFlags);
        [StructLayout(LayoutKind.Sequential)]

        private struct SHFILEINFO
        {
            public SHFILEINFO(bool b)
            {
                hIcon = IntPtr.Zero;
                iIcon = 0;
                dwAttributes = 0;
                szDisplayName = "";
                szTypeName = "";
            }
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.LPStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.LPStr, SizeConst = 80)]
            public string szTypeName;
        };

        private enum SHGFI
        {
            SmallIcon = 0x00000001,
            LargeIcon = 0x00000000,
            Icon = 0x00000100,
            DisplayName = 0x00000200,
            Typename = 0x00000400,
            SysIconIndex = 0x00004000,
            UseFileAttributes = 0x00000010
        }

        public static System.Drawing.Icon GetIcon(string strPath, bool bSmall)
        {
            SHFILEINFO info = new SHFILEINFO(true);
            int cbFileInfo = Marshal.SizeOf(info);
            SHGFI flags;
            if (bSmall)
                flags = SHGFI.Icon | SHGFI.SmallIcon | SHGFI.UseFileAttributes;
            else
                flags = SHGFI.Icon | SHGFI.LargeIcon | SHGFI.UseFileAttributes;

            SHGetFileInfo(strPath, 256, out info, (uint)cbFileInfo, flags);

            return System.Drawing.Icon.FromHandle(info.hIcon);
        }

        public static System.Drawing.Icon GetSysIcon(int icNo)
        {
            IntPtr HIcon = ExtractIcon(GetModuleHandle(string.Empty), "DDORes.dll"/*"Shell32.dll"*/, icNo);
            return System.Drawing.Icon.FromHandle(HIcon);
        }
        public static System.Drawing.Icon GetSysIconFromDll(int icNo, string dll)
        {
            IntPtr HIcon = ExtractIcon(GetModuleHandle(string.Empty), dll + ".dll", icNo);
            return System.Drawing.Icon.FromHandle(HIcon);
        }
        public static System.Drawing.Icon GetIconFromExeDll(int icNo, string dll)
        {
            IntPtr HIcon = ExtractIcon(GetModuleHandle(string.Empty), dll, icNo);

            return System.Drawing.Icon.FromHandle(HIcon);
        }
    }
}
