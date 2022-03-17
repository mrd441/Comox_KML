using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.IO;

namespace Comox_KML
{
    public partial class Form1 : Form
    {
        public XElement xmlFrag;
        public bool toInjRes;
        public List<string> excStyleList;
        public string elNameToInjAfterT;
        public CancellationTokenSource tokenSource ;
        public int done;
        public int error;

        public Form1()
        {
            InitializeComponent();
            toInjRes = false;
        }  

        public void updateDoneLable(int value = 0)
        {
            done = done + value;
            labelDone.Invoke(new MethodInvoker(() => labelDone.Text = $"Обработано - {done}"));
        }
        public void updateErorLable(int value = 0 )
        {
            error = error + value;
            labelError.Invoke(new MethodInvoker(() => labelError.Text = $"Ошибок - {error}"));
        }


        private void processFile(FileInfo filePath)
        {
            //loging(Task.CurrentId.ToString());
            //loging("1");
            try
            {
                XDocument xml = XDocument.Load(filePath.FullName);

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

                xml.Descendants()
                .Where(x => ((string)x.Name.LocalName== "Style" || (string)x.Name.LocalName == "StyleMap") && (!compareStyleId((string)x.Attribute("id"))))
                .Remove();
                //xml.Descendants(ns + "Style").Where(x => !compareStyleId((string)x.Attribute("id")))
                //.Remove();
                //xml.Descendants(ns + "StyleMap")
                //.Where(x => !compareStyleId((string)x.Attribute("id")))
                //.Remove();

                //if (toInjRes)
                //{
                //    foreach (XElement el in xml.Descendants(ns + elNameToInjAfterT))
                //    {
                //        el.AddAfterSelf(xmlFrag);
                //    }
                //}
                if (toInjRes)
                    xml.Descendants(ns + elNameToInjAfterT).First().AddAfterSelf(xmlFrag.Elements());

                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = ("\t");
                settings.OmitXmlDeclaration = false;
                settings.Encoding = new UTF8Encoding(false);
                settings.NamespaceHandling = NamespaceHandling.OmitDuplicates;               

                //string destName = filePath.DirectoryName + "\\done\\" + filePath.Name;
                using (XmlWriter w = XmlWriter.Create(filePath.FullName, settings))
                {
                    xml.Save(w);
                }                
                updateDoneLable(1);
            }
            catch(Exception ex)
            {                
                loging($"Ошибка обработки {filePath.Name}. {ex.Message}",2);
                updateErorLable(1);
                //string destName = filePath.DirectoryName + "\\error\\" + filePath.Name;
                //File.Move(filePath.FullName, destName);
            }

        }

        public bool compareStyleId(string idName)
        {
            idName = idName.Split(":").Last().Replace(".webp", "");
            foreach (string foo in excStyleList)
                if (foo.Trim().IndexOf(idName)==0)
                    return true;
            return false;
            //return excStyleList.Contains(idName);
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            
            button1.Enabled = false;
            richTextBox1.Clear();
            elNameToInjAfterT = elNameToInjAfter.Text;
            //elNameToInjAfter.Invoke(new MethodInvoker(() => elNameToInjAfterT = elNameToInjAfter.Text));
            excStyleList = new List<string>();
           
            foreach(string line in richTextBox2.Lines)
                excStyleList.Add(line.Trim());
            //excStyleList.Add("file:ПСП.zip:" + line.Trim() + ".webp");

            toInjRes = injFrag.Checked;

            if (toInjRes && elNameToInjAfterT=="")
            {
                loging("Имя элемента после, которого необходимо необходимо добавить фрагмент, не может быть пустым.", 2);
                return;
            }

            if (toInjRes && !checkXmlFrag())
                return;

            var filePath = string.Empty;
            //using (OpenFileDialog openFileDialog = new OpenFileDialog())
            //{
            //    openFileDialog.Filter = "txt files (*.kml)|*.kml|All files (*.*)|*.*";
            //    openFileDialog.FilterIndex = 2;
            //    openFileDialog.RestoreDirectory = false;

            //    if (openFileDialog.ShowDialog() == DialogResult.OK)
            //        filePath = openFileDialog.FileName;
            //}
            //if (filePath == string.Empty) return;

            using (FolderBrowserDialog openFileDialog = new FolderBrowserDialog())
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                    filePath = openFileDialog.SelectedPath;
            }
            if (filePath == string.Empty) return;

            //await Task.Run(() => processFile(filePath));

            loging("Подсчет фалов...");
            DirectoryInfo d = new DirectoryInfo(filePath); //Assuming Test is your Folder
            FileInfo[] Files = d.GetFiles("*.kml",SearchOption.AllDirectories);
            loging("Всего файлов - " + Files.Count().ToString());

            if (Files==null || Files.Length==0)
            {
                loging("Завершено.");
                return;
            }

            //Directory.CreateDirectory(filePath + "\\done");
            //Directory.CreateDirectory(filePath + "\\error");
            done = 0;
            error = 0;
            labelFileCount.Text = $"Всего файлов {Files.Count()}";
            updateErorLable();
            updateDoneLable();
            labelFileCount.Visible = true;
            labelDone.Visible = true;
            labelError.Visible = true;

            tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            //var factory = new TaskFactory();
            var tasks = new List<Task>();
            loging("Начало.");
            foreach (var file in Files)
                //tasks.Add(factory.StartNew(() => p2(file.FullName), token, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default));
                tasks.Add(Task.Run(() => processFile(file), token));
            
            try
            {
                await Task.WhenAll(tasks.ToArray());
            }
            catch (OperationCanceledException)
            {
                //loging("Операция прервана по причине " + nameof(OperationCanceledException));
            }
            finally
            {
                button1.Enabled = true;
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
                fragT = "<rootadditional>" + fragT + "</rootadditional>";
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
            try
            {
                tokenSource.Cancel();
                loging("Операция прервана...");
            }
            catch (Exception) { }
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
