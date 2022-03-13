using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Concurrent;

using System.IO;
using System.Diagnostics;

namespace Comox_KML
{
    public partial class Form1 : Form
    {
        public XElement xmlFrag;
        public bool toInjRes;
        public string[] excStyleList;
        public string elNameToInjAfterT;
        public CancellationTokenSource tokenSource ;

        public Form1()
        {
            InitializeComponent();
            toInjRes = false;
        }        

        private void processFile(string filePath)
        {
            //loging(Task.CurrentId.ToString());
            //loging("1");
            XDocument xml = XDocument.Load(filePath, LoadOptions.SetBaseUri);
            
            XNamespace ns = "http://www.opengis.net/kml/2.2";
            foreach (XAttribute xA in xml.Root.Attributes())
            {
                if (xA.Name == "xmlns")
                {
                    ns = xA.Value;
                    XAttribute foo = xA;
                    xA.Remove();
                    xml.Root.Add(foo);
                    break;
                }
            }

            xml.Descendants(ns + "Style")
            .Where(x => !excStyleList.Contains((string)x.Attribute("id")))
            .Remove();
                        
            if (toInjRes)
            {
                foreach (XElement el in xml.Descendants(ns + elNameToInjAfterT))
                {
                    el.AddAfterSelf(xmlFrag);
                }
            }
            //xml.Descendants(ns + "name").First().AddAfterSelf(xmlTree);

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = ("\t");
            settings.OmitXmlDeclaration = false;
            settings.Encoding = new UTF8Encoding(false);
            settings.NamespaceHandling = NamespaceHandling.OmitDuplicates;

            using (XmlWriter w = XmlWriter.Create(filePath + "1", settings))
            {
                xml.Save(w);
            }

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            elNameToInjAfterT = elNameToInjAfter.Text;
            //elNameToInjAfter.Invoke(new MethodInvoker(() => elNameToInjAfterT = elNameToInjAfter.Text));
            excStyleList = richTextBox2.Lines;
            toInjRes = injFrag.Checked;

            if (toInjRes && elNameToInjAfterT=="")
            {
                loging("Имя элемента после, которого необходимо необходимо добавить фрагмент, не может быть пустым.", 2);
                return;
            }

            if (toInjRes && !checkXmlFrag())
                return;  

            //var filePath = string.Empty;
            //using (OpenFileDialog openFileDialog = new OpenFileDialog())
            //{
            //    openFileDialog.Filter = "txt files (*.kml)|*.kml|All files (*.*)|*.*";
            //    openFileDialog.FilterIndex = 2;
            //    openFileDialog.RestoreDirectory = false;

            //    if (openFileDialog.ShowDialog() == DialogResult.OK)
            //        filePath = openFileDialog.FileName;
            //}
            //if (filePath == string.Empty) return;

            //await Task.Run(() => processFile(filePath));

            loging("Подсчет фалов...");
            DirectoryInfo d = new DirectoryInfo(@"C:\Users\akim\Downloads\Новая папка"); //Assuming Test is your Folder
            FileInfo[] Files = d.GetFiles("*.kml");
            loging("Всего файлов - " + Files.Count().ToString());

            tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            //var factory = new TaskFactory();
            var tasks = new List<Task>();
            loging("Начало.");
            foreach (var file in Files)
                //tasks.Add(factory.StartNew(() => p2(file.FullName), token, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default));
                tasks.Add(Task.Run(() => p2(file.FullName), token));

            try
            {
                await Task.WhenAll(tasks.ToArray());
            }
            catch (OperationCanceledException)
            {
                loging("Остановлено по причине " + nameof(OperationCanceledException));
            }
            finally
            {
                tokenSource.Dispose();
            }
            
            loging("Завершено.");

        }

        private void p2(string ss)
        {
            loging(Task.CurrentId.ToString() + " - start");
            Thread.Sleep(1000);
            loging(Task.CurrentId.ToString() + " - stop");
        }

        private bool checkXmlFrag()
        {
            string fragT = "";
            richTextBox3.Invoke(new MethodInvoker(()=>  fragT = richTextBox3.Text ));

            try
            {
                xmlFrag = XElement.Parse(fragT);
            }
            catch (Exception ex)
            {
                loging("Ошибка в XML фрагменте. " + ex.Message,2);
                return false;
            }
            return true;
        }

        private async void  button2_Click(object sender, EventArgs e)
        {
            if (await Task.Run(() => checkXmlFrag()))
                loging("Ошибок не найдено.");
        }

        public void loging(string text, int level = 0)
        {
            if (richTextBox1.InvokeRequired)
            {
                richTextBox1.Invoke(new Action< string, int>(loging), new object[] { text, level });
                return;
            }
            var aColor = Color.Black;
            if (level == 1)
                aColor = Color.Green;
            else if (level == 2)
                aColor = Color.Red;
            string curentTime = DateTime.Now.TimeOfDay.ToString("hh\\:mm\\:ss");
            richTextBox1.AppendText(curentTime + ": " + text + Environment.NewLine, aColor);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            loging("Завершение...");
            tokenSource.Cancel();
        }
    }
}

public static class RichTextBoxExtensions
{
    public static void AppendText(this RichTextBox box, string text, Color color)
    {
        box.SelectionStart = box.TextLength;
        box.SelectionLength = 0;

        box.SelectionColor = color;
        box.AppendText(text);
        box.SelectionColor = box.ForeColor;
        box.SelectionStart = box.Text.Length;
        box.ScrollToCaret();
    }
}
