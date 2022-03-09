using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml;
using System.IO;

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
            XDocument xml = XDocument.Load(filePath);
            
            //LoadOptions asd = new LoadOptions();
            richTextBox1.AppendText((xml.Descendants(ns + "Style")
                                        .Where(x => (string)x.Attribute("id") == "file:ПСП.zip:grn-pushpin.webp1")).First().Value);

            xml.Descendants(ns + "Style")
            .Where(x => (string)x.Attribute("id") == "file:ПСП.zip:grn-pushpin.webp1")
            .Remove();

            //var output = xml.ToString().Replace("kml:", string.Empty).Replace("    ", );

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = ("\t");
            settings.OmitXmlDeclaration = true;

            // Create the XmlWriter object and write some content.
            XmlWriter writer = XmlWriter.Create("data.xml", settings);
            writer.WriteStartElement("book");
            writer.WriteElementString("item", "tesing");
            writer.WriteEndElement();

            writer.Flush()

            //xml.Save(filePath+"1");
            File.WriteAllText(filePath + "1", output);

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
