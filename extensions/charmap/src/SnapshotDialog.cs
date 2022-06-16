using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;
using System.Resources;
using gehtsoft.xce.conio;
using gehtsoft.xce.conio.win;
using gehtsoft.xce.editor.application;

namespace gehtsoft.xce.extension.charmap_impl
{
    internal class SnapshotDialogItem : DialogItem
    {
        static Canvas mShot;
        static Encoding mEncoding = Encoding.GetEncoding(866);


        public SnapshotDialogItem(string resource, int id, int row, int column) : base(id, row, column, 24, 80)
        {
            mShot = new Canvas(24, 80);
            gehtsoft.xce.conio.ConsoleColor clr = new gehtsoft.xce.conio.ConsoleColor(0);
            try
            {
                Stream stream = this.GetType().Assembly.GetManifestResourceStream("gehtsoft.xce.extension.charmap.resource.resources.resources");
                ResourceReader reader = new ResourceReader(stream);

                byte[] data = null;
                string type = "";

                reader.GetResourceData(resource, out type, out data);

                reader.Close();
                stream.Close();

                byte[] _c = new byte[1];
                char[] c = new char[1];

                for (int i = 0; i < 24; i++)
                    for (int j = 0; j < 80; j++)
                    {
                        _c[0] = data[4 + i * 160 + j * 2];
                        byte _a = data[4 + i * 160 + j * 2 + 1];
                        mEncoding.GetChars(_c, 0, 1, c, 0);
                        short a = (short)(((short)_a) & 0xff);
                        clr.PaletteColor = a;
                        mShot.write(i, j, c[0], clr);
                    }
            }
            catch (Exception )
            {
                clr = new gehtsoft.xce.conio.ConsoleColor(0);
                mShot.fill(0, 0, 24, 80, ' ', clr);
            }
        }


        public override bool HasHotKey
        {
            get
            {
                return false;
            }
        }

        public override char HotKey
        {
            get
            {
                throw new InvalidOperationException();
            }
        }

        public override bool IsInputElement
        {
            get
            {
                return false;
            }
        }

        public override bool Enabled
        {
            get
            {
                return true;
            }
        }

        public override void OnPaint(Canvas canvas)
        {
            canvas.paint(0, 0, mShot);
        }
    }

    internal class SnapshotDialog : Dialog
    {
        Application mApplication;

        internal SnapshotDialog(Application application, string shotname) : base(shotname, application.ColorScheme, false, 27, 82)
        {
            mApplication = application;
            DialogItemButton b1;
            AddItem(new SnapshotDialogItem(shotname, 0x1000, 0, 0));
            AddItem(b1 = new DialogItemButton("< Ok >", DialogResultOK, 24, 0));
            CenterButtons(b1);
        }

        internal void DoModal()
        {
            base.DoModal(mApplication.WindowManager);
        }
    }
}
