using Microsoft.Win32;
using SandBoxCore;
using SandBoxCore.DataTransferObjects;
using SandBoxCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace SandBoxUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //UI for .Net framework 
        public MainWindow()
        {
            InitializeComponent();
        }
        private Sandboxer sandboxer = new Sandboxer();
        private HelperClass helper= new HelperClass();

        //private StackPanel innerStack = new StackPanel
        //{
        //    Orientation = Orientation.Vertical
        //};
        private void btnOpenFiles_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var innerlabel = innerstack.Children[0];
                if (innerstack.Children.OfType<TreeViewItem>() != null) innerstack.Children.Clear();
                innerstack.Children.Add(innerlabel);

                Console.WriteLine("i was clicked");
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Multiselect = false;
                openFileDialog.Filter = "Exe files (*.exe)|*.exe|Dll files (*.dll)|*.dll";
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);


                if (openFileDialog.ShowDialog() == true)
                {
                    foreach (string filename in openFileDialog.FileNames)
                    {
                        List<AssemblyEntity> assembliesraw = sandboxer.GetAllAssembliesFromPath(filename);
                        lbFiles.Items.Clear();
                        lbFiles.Items.Add(filename);


                        foreach (var c in assembliesraw)
                        {
                            TreeViewItem ParentItem = new TreeViewItem();
                            ParentItem.Header = c.ShortName;
                            innerstack.Children.Add(ParentItem);
                            //
                            Label typelabel = new Label();
                            typelabel.Content = "Classes";
                            ParentItem.Items.Add(typelabel);
                            foreach (var classinassembly in c.Classes)
                            {
                                TreeViewItem classItem = new TreeViewItem();
                                classItem.Header = classinassembly.FullName;
                                //UIElementCollection collectionHeadandbody = new UIElementCollection();
                                ParentItem.Items.Add(classItem);

                                Label methodlabel = new Label();
                                methodlabel.Content = "Methods";
                                classItem.Items.Add(methodlabel);

                                foreach (var methodinclass in classinassembly.Methods)
                                {
                                    CheckBox methoditem = new CheckBox();
                                    methoditem.Name = methodinclass.FullName;
                                    methoditem.Content = methodinclass.FullName;
                                    //cb.Click += "";
                                    TreeViewItem methodtree = new TreeViewItem();
                                    methodtree.Header = methoditem;
                                    classItem.Items.Add(methodtree);

                                    Label parameterlabel = new Label();
                                    parameterlabel.Content = "Parameters";
                                    methodtree.Items.Add(parameterlabel);

                                    foreach (var param in methodinclass.Parameters)
                                    {
                                        Label paramlabel = new Label();
                                        paramlabel.Content = param.ShortName;

                                        Label paramtype = new Label();
                                        paramtype.Content = param.FullName;

                                        TextBox paramtextbox = new TextBox()
                                        {
                                            Width = 50
                                        };

                                        WrapPanel paramcontainer = new WrapPanel();
                                        paramcontainer.Children.Add(paramtype);
                                        paramcontainer.Children.Add(paramlabel);
                                        paramcontainer.Children.Add(paramtextbox);

                                        methodtree.Items.Add(paramcontainer);

                                    }

                                }
                            }
                            //  
                        }


                        Grid.SetColumn(innerstack,  /*Set the column of your stackPanel, default is 0*/0);
                        Grid.SetRow(innerstack,  /*Set the row of your stackPanel, default is 0*/1);
                        Grid.SetColumnSpan(innerstack,  /*Set the columnSpan of your stackPanel, default is 1*/1);
                        Grid.SetRowSpan(innerstack,  /*Set the rowSpan of your stackPanel, default is 1*/1);

                    }

                }
            }
            catch (Exception error)
            {

                string messageBoxText = "An error occured while running the program" + "\n\n" + error.Message;
                string caption = "Error";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Error;
                MessageBoxResult result;

                result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);
            }

        }

        private void Checkbox_Click_Event_Handler(object sender, RoutedEventArgs e)
        {
            try
            {

                foreach (var childassembly in innerstack.Children.OfType<TreeViewItem>())
                {
                    foreach (var childclass in childassembly.Items.OfType<TreeViewItem>())
                    {
                        foreach (var childmethod in childclass.Items.OfType<TreeViewItem>())
                        {
                            CheckBox methodcheckbox = childmethod.Header as CheckBox;

                            CheckBox clickedcheckbox = sender as CheckBox;
                            //if (methodcheckbox != sender && (bool)(clickedcheckbox.IsChecked)) clickedcheckbox.Checked = false;
                            foreach (var childparam in childmethod.Items.OfType<WrapPanel>())
                            {
                            }
                        }
                    }
                }

            }
            catch (Exception error)
            {
                string messageBoxText = "An error occured while running the program" + "\n\n" + error.Message;

                string caption = "Error";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Error;
                MessageBoxResult result;

                result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);

            }
        }

        private void Execute_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<string> permissionboxes = permissions.Children.OfType<CheckBox>()
                    .Where(x => x.IsChecked == true).Select(y => y.Name).ToList();


                string filepath = lbFiles.Items[0].ToString();
                string assemblyname = "";
                string classname = "";
                string methodname = "";
                List<InvokeParameterInterface> parameters = new List<InvokeParameterInterface>();

                foreach (var childassembly in innerstack.Children.OfType<TreeViewItem>())
                {
                    foreach (var childclass in childassembly.Items.OfType<TreeViewItem>())
                    {
                        foreach (var childmethod in childclass.Items.OfType<TreeViewItem>())
                        {
                            foreach (var childparam in childmethod.Items.OfType<WrapPanel>())
                            {
                                var childparamwrap = childparam as WrapPanel;
                                InvokeParameterInterface paramobj = new InvokeParameter();

                                Label paramtype = childparamwrap.Children[0] as Label;
                                Label paramlabel = childparamwrap.Children[1] as Label;
                                TextBox textcontent = childparamwrap.Children[2] as TextBox;


                                if (paramtype != null)
                                {
                                    paramobj.ParameterName = paramtype.Content.ToString();
                                }

                                if (textcontent != null)
                                {
                                    paramobj.ParameterValue = textcontent.Text.ToString();
                                }

                                assemblyname = childassembly.Header.ToString();
                                classname = childclass.Header.ToString();
                                methodname = (string)((bool)((CheckBox)childmethod.Header).IsChecked ? ((CheckBox)childmethod.Header).Content : "");
                                parameters.Add(paramobj);

                                if (methodname == "") throw new Exception("Please select a method checkbox");
                            }
                        }
                    }
                }

                Object[] objparams = helper.ObjectParameters(parameters);
                sandboxer.SetupSandBox(permissionboxes, filepath, assemblyname, classname, methodname, objparams);

                //var execheckbox = innerstack.Children.OfType<CheckBox>().Where(x => x.IsChecked == true).FirstOrDefault();
                //var execheckboxname = execheckbox.Content;
                //Console.WriteLine(" execheckboxname");
                //Console.WriteLine(execheckboxname);
            }
            catch (Exception error )
            {
                string messageBoxText = "An error occured while running the program" + "\n\n" + error.Message;
                
                string caption = "Error";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Error;
                MessageBoxResult result;

                result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);

            }
        }
    }
}
