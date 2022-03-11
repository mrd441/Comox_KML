using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

using System.IO;
using System.Diagnostics;

namespace Comox_KML
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                //openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "txt files (*.kml)|*.kml|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = false;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                    filePath = openFileDialog.FileName;
            }
            if (filePath == string.Empty) return;
            XNamespace ns = "http://www.opengis.net/kml/2.2";
            richTextBox1.AppendText(watch.Elapsed.TotalSeconds.ToString());
            //XmlDocument doc = new XmlDocument();
            XDocument xml = XDocument.Load(filePath);
            //xml.AddBeforeSelf("<?xml version=\"1.0\" encoding=\"UTF - 8\"?>");
            //LoadOptions asd = new LoadOptions();
            richTextBox1.AppendText(watch.Elapsed.TotalSeconds.ToString());
            //try
            //{
            //    richTextBox1.AppendText((xml.Descendants(ns + "Style")
            //                            .Where(x => (string)x.Attribute("id") == "file:ПСП.zip:grn-pushpin.webp")).First().Value);
            //}
            //catch (Exception) { }
            richTextBox1.AppendText(watch.Elapsed.TotalSeconds.ToString());

            xml.Descendants(ns + "Style")
            .Where(x =>  !richTextBox2.Lines.Contains((string)x.Attribute("id")))
            .Remove();
            xml.Declaration = new XDeclaration("1.0", "UTF-8", null);
            //var output = xml.ToString().Replace("kml:", string.Empty).Replace("    ", );

            //XmlDocumentFragment xfrag = doc.CreateDocumentFragment();

            //xfrag.InnerXml = "<Style id =\"file: ПСП\"><IconStyle ><Icon ><href > files /file - ПСП.zip - grn - pushpin.webp </href ></Icon ><hotSpot x = \"0.5\" y = \"0\" xunits = \"fraction\" yunits = \"fraction\" /> </IconStyle ></Style > ";
            //xml.Descendants(ns + "name").First().AddAfterSelf(xfrag);
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = ("\t");
            settings.OmitXmlDeclaration = true;
            settings.Encoding = new UTF8Encoding(false);

            StringBuilder sb = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(sb, settings);
           
            xml.Save(writer);
            writer.Dispose();
            //richTextBox1.AppendText(watch.Elapsed.TotalSeconds.ToString());
            sb.Insert(0,"<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n");
            sb.Replace("kml:", string.Empty);
            //richTextBox1.AppendText(watch.Elapsed.TotalSeconds.ToString());
            File.WriteAllText(filePath + "1", sb.ToString());
            //richTextBox1.AppendText(watch.Elapsed.TotalSeconds.ToString());
            watch.Stop();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                //openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "txt files (*.kml)|*.kml|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = false;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                    filePath = openFileDialog.FileName;
            }
            if (filePath == string.Empty) return;


            
        }
    }
}
