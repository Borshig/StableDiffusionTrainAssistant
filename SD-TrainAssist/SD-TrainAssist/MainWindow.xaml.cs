using HandyControl.Controls;
using HandyControl.Data;
using HandyControl.Tools;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace SD_TrainAssist
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : HandyControl.Controls.BlurWindow
    {
        string workDir = "";
        string txtAlltokensConf = "txtAlltokens.conf";
        int currentElement = -1;
        List<string> imgFiles = new List<string>();
        List<string> txtFiles = new List<string>();
        List<string> tokensAllInMemory = new List<string>();
        FileStream currentOpenedFile;

        public MainWindow()
        {
            InitializeComponent();
        }

        
        private void Button_Prev_Click(object sender, RoutedEventArgs e)
        {
           
            if (currentElement < 0) return;

            SaveTokensIntoTxtFile();

            if (currentElement >= 0)
            {
                int prevElement = currentElement - 1;
                if (prevElement < 0) prevElement = imgFiles.Count - 1;

                imgView.Source = new BitmapImage(new Uri(imgFiles[prevElement]));
                currentElement = prevElement;
            }
                      
           
            ProcessCheckboxes();
        }

        private void Button_Next_Click(object sender, RoutedEventArgs e)
        {
            
            if (currentElement < 0) return;

            SaveTokensIntoTxtFile();

            int nextElement = currentElement + 1;

            if (nextElement > imgFiles.Count - 1) nextElement = 0;

            imgView.Source = new BitmapImage(new Uri(imgFiles[nextElement]));
            currentElement = nextElement;    
            

            ProcessCheckboxes();
        }

        private void Button_ChooseWorkDir_Click(object sender, RoutedEventArgs e)
        {
            workDir = tbWorkDir.Text;
            if (!tbWorkDir.ToString().IsUrl())
            {
                var dialog = new System.Windows.Forms.FolderBrowserDialog();

                // Show open file dialog box
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                // Process open file dialog box results
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    // Open document
                    workDir = dialog.SelectedPath;
                    tbWorkDir.Text = workDir;
                }
            }

            imgFiles.Clear();
            txtFiles.Clear();


            if (!(Directory.Exists(workDir)))
            {
                HandyControl.Controls.MessageBox.Show("Not correct workdir");
                return;
            }


            string[] files = Directory.GetFiles(workDir);

            if(files.Length == 0)
            {
                HandyControl.Controls.MessageBox.Show("Choosen directory is empty");
                return;
            }

            

            foreach(string file in files)
            {
                if ((file.ToLower().EndsWith(".jpg")) 
                   || (file.ToLower().EndsWith(".jpeg"))
                   || (file.ToLower().EndsWith(".png"))
                ){
                    imgFiles.Add(file);
                }
                else if((file.EndsWith(".txt")))
                {
                    txtFiles.Add(file);
                }
            }
            if (imgFiles.Count == 0)
            {
                HandyControl.Controls.MessageBox.Show("There is no images");
                return;
            }
            currentElement = 0;
            imgView.Source = new BitmapImage(new Uri(imgFiles[currentElement]));
            


            PreProccessFiles();
            if (File.Exists(workDir + "\\" + txtAlltokensConf))
            {
                string tokensStringLine = File.ReadAllText(workDir + "\\" + txtAlltokensConf);

                string[] alltokens = tokensStringLine.Split(",");

                foreach (string token in alltokens)
                {
                    if(searchTokensInMemory(token) == 0) AddTokenInMemory(token);
                }
               
            }
            else
            {
                File.Create(workDir + "\\" + txtAlltokensConf).Dispose();
            }


            UpdateTokensCheckBoxes();
            ProcessCheckboxes();
        }

        private void Button_AddToken_Click(object sender, RoutedEventArgs e)
        {
            if(tbTokens.Text == "" || tbTokens.Text == " " || tbTokens.Text == "\t")
            {               
                tbTokens.Clear();
                return;
            }

            string token = tbTokens.Text;
            if (searchTokensInMemory(token) == 0)
            {
                AddTokenInMemory(token);
            }
            UpdateTokensCheckBoxes();
            ProcessCheckboxes();
            tbTokens.Clear();
        }

        private void UpdateTokensCheckBoxes()
        {
            GridCheckboxes.Children.Clear();

            for (int i = 0; i < tokensAllInMemory.Count; i++)
            {
                CheckBox newCheckBox = new CheckBox();
                newCheckBox.Content = tokensAllInMemory[i];
                newCheckBox.Name = "Button" + i;
                newCheckBox.Height = 28;
                newCheckBox.Width = 160;
                newCheckBox.FontSize = 20;
                newCheckBox.VerticalContentAlignment = VerticalAlignment.Center;
                newCheckBox.HorizontalAlignment = HorizontalAlignment.Left;
                newCheckBox.VerticalAlignment = VerticalAlignment.Top;

                double y = newCheckBox.Height * i;
                int j = (int)(y / (GridCheckboxes.ActualHeight - 24));
                y = (double)(y % (GridCheckboxes.ActualHeight - 24));

                newCheckBox.Margin = new Thickness(newCheckBox.Width*j, y, 0.0, 0.0);


                //newBtn.Margin.Left = 0.0;
                //newBtn.Margin.Top = (double)i*15;

                GridCheckboxes.Children.Add(newCheckBox);

            }

            CheckBox chb = new CheckBox();



           

        }

        private void Button_SetForAll_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PreProccessFiles()
        {
            if(txtFiles.Count == 0)
            {
                foreach (string imgfile in imgFiles)
                {
                    string newTxtFile = System.IO.Path.GetFileNameWithoutExtension(imgfile) + ".txt";
                    txtFiles.Add ( newTxtFile  );



                    File.Create(workDir +"\\"+ newTxtFile).Dispose();


                    //TODO: Exceptions for File.Create!

                    //try { File.Create(newTxtFile); }
                    //catch(...) {}

                    //      T:System.UnauthorizedAccessException:
                    //     The caller does not have the required permission. -or- path specified a file
                    //     that is read-only. -or- path specified a file that is hidden.
                    //
                    //   T:System.ArgumentException:
                    //     .NET Framework and .NET Core versions older than 2.1: path is a zero-length string,
                    //     contains only white space, or contains one or more invalid characters. You can
                    //     query for invalid characters by using the System.IO.Path.GetInvalidPathChars
                    //     method.
                    //
                    //   T:System.ArgumentNullException:
                    //     path is null.
                    //
                    //   T:System.IO.PathTooLongException:
                    //     The specified path, file name, or both exceed the system-defined maximum length.
                    //
                    //   T:System.IO.DirectoryNotFoundException:
                    //     The specified path is invalid (for example, it is on an unmapped drive).
                    //
                    //   T:System.IO.IOException:
                    //     An I/O error occurred while creating the file.
                    //
                    //   T:System.NotSupportedException:
                    //     path is in an invalid format.




                }
            }

            else if(txtFiles.Count < imgFiles.Count)
            {
                foreach (string imgfile in imgFiles)
                {
                    string compTxtFile = System.IO.Path.GetFileNameWithoutExtension(imgfile) + ".txt";

                    if (!File.Exists(compTxtFile))
                    {
                        File.Create(workDir + "\\" + compTxtFile).Dispose();
                    }
                    else
                    {
                        //LoadTokensFromFile()
                    }
                    
                    txtFiles.Add(compTxtFile);
                       
                   
                    //TODO: Exceptions for File.Create!

                    //try { File.Create(newTxtFile); }
                    //catch(...) {}

                    //      T:System.UnauthorizedAccessException:
                    //     The caller does not have the required permission. -or- path specified a file
                    //     that is read-only. -or- path specified a file that is hidden.
                    //
                    //   T:System.ArgumentException:
                    //     .NET Framework and .NET Core versions older than 2.1: path is a zero-length string,
                    //     contains only white space, or contains one or more invalid characters. You can
                    //     query for invalid characters by using the System.IO.Path.GetInvalidPathChars
                    //     method.
                    //
                    //   T:System.ArgumentNullException:
                    //     path is null.
                    //
                    //   T:System.IO.PathTooLongException:
                    //     The specified path, file name, or both exceed the system-defined maximum length.
                    //
                    //   T:System.IO.DirectoryNotFoundException:
                    //     The specified path is invalid (for example, it is on an unmapped drive).
                    //
                    //   T:System.IO.IOException:
                    //     An I/O error occurred while creating the file.
                    //
                    //   T:System.NotSupportedException:
                    //     path is in an invalid format.

                }

                if(txtFiles.Count != imgFiles.Count)
                {
                    HandyControl.Controls.MessageBox.Show("Maybe extra .txt without image-pair");
                }


            }// else if




        }

        // return count of this token. should be 0 or 1; 
        private int searchTokensInMemory(string token)
        {
            int sum = 0;
            for (int i = 0; i < tokensAllInMemory.Count; i++)
            {
                if (tokensAllInMemory[i] == token) sum++;
            }
                 

            return sum;
        }


        private void SaveTokensIntoTxtFile()
        {
            List<string> tokens = new List<string>();
            for (int i = 0; i < GridCheckboxes.Children.Count; i++)
            {
                CheckBox chBox = (CheckBox)GridCheckboxes.Children[i];
                if(chBox.IsChecked.Value) tokens.Add(chBox.Content.ToString());
            }


            string fileOutput = "";
            for (int i = 0; i < tokens.Count; i++)
            {   
                fileOutput += tokens[i].ToString() + ",";
            }

            File.WriteAllText(txtFiles[currentElement], fileOutput);
            //////////////////////////////saved txt files//////////////////////////////////


            string tokensStringLine = "";
            foreach (string token in tokensAllInMemory)
            {
                tokensStringLine += token + ",";
            }
            if(tokensStringLine.EndsWith(","))
            {
                int index = tokensStringLine.LastIndexOf(",");
                tokensStringLine.Remove(index);
            }

            File.WriteAllText(workDir + "\\" + txtAlltokensConf, tokensStringLine);
            //////////////////////////////saved conf files//////////////////////////////////

        }

        List<string> readTokensFromFile(string path)
        {
            List<string> result = new List<string>();

            string sReadedSting = File.ReadAllText(path);

            string[] splitedString = sReadedSting.Split(",");

            foreach(string s in splitedString)
            {
                if(s != "" && s != " ")
                {
                    result.Add(s);
                }                      
            }
            return result;
        }

        private void ProcessCheckboxes()
        {

            List<string> tokensForImage = readTokensFromFile(txtFiles[currentElement]);

            if (tokensForImage.Count == 0)
            {
                //// Uncheck all if file was empty
                foreach (CheckBox children in GridCheckboxes.Children)
                {
                    children.IsChecked = false;
                }
            }
            else
            {
                foreach (CheckBox children in GridCheckboxes.Children)
                {
                    children.IsChecked = false;
                }

                foreach (CheckBox children in GridCheckboxes.Children)
                {
                    foreach (string token in tokensForImage)
                    {
                        if (token == children.Content.ToString())
                        {
                            children.IsChecked = true;
                        }
                    }
                }
            }

          

        }

        private void AddTokenInMemory(string token)
        {
            if (token != "" && token != " " && token != "\t")
            {
                tokensAllInMemory.Add(token);
            }
        }


    }
}
